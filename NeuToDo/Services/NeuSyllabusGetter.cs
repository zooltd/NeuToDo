﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class NeuSyllabusGetter : ResourcesManagement
    {
        private readonly string _userName;
        private readonly string _password;
        public static User User { get; set; }
        public static TeachingTime TeachingTime { get; set; }
        public static Dictionary<string, Course> Syllabus { get; set; }

        public NeuSyllabusGetter(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        public async Task WebCrawler()
        {
            var vpnUrl =
                "https://pass-443.webvpn.neu.edu.cn/tpass/login?service=https%3A%2F%2Fwebvpn.neu.edu.cn%2Fusers%2Fauth%2Fcas%2Fcallback%3Furl";
            InitSources(false);
            var formData = await CollectFormData(vpnUrl);
            vpnUrl = vpnUrl.Insert(vpnUrl.IndexOf('?'), ";" + formData["jsessionid"]);
            var deanUri = await LoginWebVpn(vpnUrl, formData);
            ReallocateSources(true);
            await LoginDean(deanUri);
            await GetCourseInfo();
        }

        private async Task<Dictionary<string, string>> CollectFormData(string vpnUrl)
        {
            var response = await Client.GetAsync(vpnUrl);
            response.EnsureSuccessStatusCode(); //TODO: Exception
            var jsessionid =
                response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value.ToArray()[1]
                    .Split(';')[0]; //TODO: Exception
            var responseBody = await response.Content.ReadAsStringAsync();
            const string ltPattern = "name=\"lt\" value=\"(.*)\"";
            const string executionPattern = "name=\"execution\" value=\"(.*)\"";
            const string eventIdPattern = "name=\"_eventId\" value=\"(.*)\"";
            var lt = Regex.Match(responseBody, ltPattern).Groups[1].Value; //TODO: Exception
            var execution = Regex.Match(responseBody, executionPattern).Groups[1].Value;
            var eventId = Regex.Match(responseBody, eventIdPattern).Groups[1].Value;
            var rsa = _userName + _password + lt;
            var formData = new Dictionary<string, string>
            {
                {"rsa", rsa}, {"ul", _userName.Length.ToString()}, {"pl", _password.Length.ToString()}, {"lt", lt},
                {"execution", execution}, {"_eventId", eventId}, {"jsessionid", jsessionid}
            };
            return formData;
        }

        private async Task<Uri> LoginWebVpn(string vpnUrl, IEnumerable<KeyValuePair<string, string>> formData)
        {
            var response = await Client.PostAsync(vpnUrl, new FormUrlEncodedContent(formData));
            // Ensure code == HttpStatusCode.Redirect 
            var redirectUri = response.Headers.Location;
            return redirectUri;
        }

        private static async Task LoginDean(Uri deanUri)
        {
            var response = await Client.GetAsync(deanUri);
            response.EnsureSuccessStatusCode();
            const string deanOfficeUrl = "https://219-216-96-4.webvpn.neu.edu.cn/eams/homeExt.action";
            response = await Client.GetAsync(deanOfficeUrl);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            const string studentInfoPattern = "class=\"personal-name\">[\\s]*(.*)[\\s]*<\\/a>";
            var studentInfoList = Regex.Match(responseBody, studentInfoPattern).Groups[1].Value
                .Split(new char[] {'(', ')'});
            User = new User() {Id = studentInfoList[1], Name = studentInfoList[0]};
            const string teachingTimePattern = "id=\"teach-week\">[\\s]*(.*)[\\s]*<font[\\s\\S]*?>(.*)<\\/font>";
            var teachingTimeGroups = Regex.Match(responseBody, teachingTimePattern).Groups;
            var semester = teachingTimeGroups[1].Value.Replace('第', ',');
            TeachingTime = new TeachingTime()
                {Semester = semester, TeachingWeek = int.Parse(teachingTimeGroups[2].Value)};
        }

        private static async Task GetCourseInfo()
        {
            Syllabus = new Dictionary<string, Course>();

            var res = await Client.GetAsync("https://219-216-96-4.webvpn.neu.edu.cn/eams/courseTableForStd.action?");
            res.EnsureSuccessStatusCode();
            var responseBody = await res.Content.ReadAsStringAsync();
            var id = res.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value.ToArray()[0].Split(';')[0]
                .Split('=')[1];

            const string idsPattern =
                "if\\(jQuery\\(\"#courseTableType\"\\)\\.val\\(\\)==\"std\"\\){[\\s]*bg\\.form.addInput\\(form,\"ids\",\"([\\d]*)\"\\)";
            var ids = Regex.Match(responseBody, idsPattern).Groups[1].Value;

            var formData = new Dictionary<string, string>
            {
                {"ignoreHead", "1"}, {"showPrintAndExport", "1"}, {"setting.kind", "std"},
                {"startWeek", string.Empty}, {"semester.id", id.ToString()}, {"ids", ids}
            };

            res = await Client.PostAsync(
                "https://219-216-96-4.webvpn.neu.edu.cn/eams/courseTableForStd!courseTable.action",
                new FormUrlEncodedContent(formData));
            res.EnsureSuccessStatusCode();
            responseBody = await res.Content.ReadAsStringAsync();
            // await File.WriteAllTextAsync(".\\courseTable.html", responseBody);

            const string textSplitPattern =
                "(var teachers =[\\s\\S]*?;)[\\s\\S]*?TaskActivity\\(actTeacherId.join\\(','\\),actTeacherName.join\\(','\\),\"(.*)\",\"(.*)\",\"(.*)\",\"(.*)\",\"(.*)\",null,null,assistantName,\"\",\"\"\\);((?:\\s*index =\\d+\\*unitCount\\+\\d+;\\s*.*\\s)*)";
            const string teacherInfoPattern = "{id:([\\d]*),name:\\\"([\\s\\S]*?)\\\",lab:([\\w]*)}";
            const string timeTablePattern = "index =(\\d)\\*unitCount\\+([\\d]+);";

            foreach (Match textSegment in Regex.Matches(responseBody, textSplitPattern))
            {
                Course course;
                var courseExist = false;
                var textSegmentGroups = textSegment.Groups;
                string teacherInfo = textSegmentGroups[1].Value;
                string courseId = textSegmentGroups[2].Value.Split(new char[] {'(', ')'})[1];

                if (Syllabus.ContainsKey(courseId))
                {
                    course = Syllabus[courseId];
                    courseExist = true;
                }
                else
                {
                    course = new Course()
                    {
                        CourseId = courseId,
                        CourseName = textSegmentGroups[3].Value.Split(new[] {'(', ')'})[0],
                        RoomId = textSegmentGroups[4].Value,
                        RoomName = textSegmentGroups[5].Value,
                        TeacherList = new List<Teacher>(),
                        Schedule = new Dictionary<DayOfWeek, DaySchedule>()
                    };
                }

                if (!courseExist)
                {
                    foreach (Match teacherInfoSegment in Regex.Matches(teacherInfo, teacherInfoPattern))
                    {
                        var teacherInfoSegmentGroups = teacherInfoSegment.Groups;
                        course.TeacherList.Add(new Teacher()
                        {
                            IsLab = teacherInfoSegmentGroups[3].Value == "true",
                            TeacherId = teacherInfoSegmentGroups[1].Value,
                            TeacherName = teacherInfoSegmentGroups[2].Value
                        });
                    }
                }

                string weeks = textSegmentGroups[6].Value;
                string timeTable = textSegmentGroups[7].Value;
                var lessonOfDay = ClassTime.None;
                var day = DayOfWeek.Sunday; //DayOfWeek.None

                foreach (Match timeTableSegment in Regex.Matches(timeTable, timeTablePattern))
                {
                    var timeTableSegmentGroups = timeTableSegment.Groups;

                    day = (DayOfWeek) (int.Parse(timeTableSegmentGroups[1].Value) + 1);

                    var lessonNo = (ClassTime) (1 << int.Parse(timeTableSegmentGroups[2].Value));
                    lessonOfDay |= lessonNo;
                }

                course.Schedule[day] = new DaySchedule()
                    {ClassTime = lessonOfDay, Weeks = weeks};
                Syllabus[courseId] = course;
            }
        }
    }
}