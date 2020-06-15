using NeuToDo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NeuToDo.Services
{
    public class NeuSyllabusGetter : ResourcesManagement
    {
        private readonly string _userName;

        private readonly string _password;

        private static int CurrWeekIndex { get; set; }

        public static List<NeuEvent> EventList;

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

            // User = new User() { Id = studentInfoList[1], Title = studentInfoList[0] };

            string stuName = studentInfoList[0];
            string stuId = studentInfoList[1];

            const string teachingTimePattern = "id=\"teach-week\">[\\s]*(.*)[\\s]*<font[\\s\\S]*?>(.*)<\\/font>";
            var teachingTimeGroups = Regex.Match(responseBody, teachingTimePattern).Groups;

            var semester = teachingTimeGroups[1].Value.Replace('第', ',');
            int weekIndex = int.Parse(teachingTimeGroups[2].Value);

            // TeachingTime = new TeachingTime()
            //     {Semester = semester, TeachingWeek = int.Parse(teachingTimeGroups[2].Value)};

            CurrWeekIndex = weekIndex;

            //TODO 正则 查看是否符合标准
            Preferences.Set("stuName", stuName);
            Preferences.Set("stuId", stuId);
            Preferences.Set("weekIndex", weekIndex);
            Preferences.Set("semester", semester);
        }

        private static async Task GetCourseInfo()
        {
            // Syllabus = new Dictionary<string, Course>();
            EventList = new List<NeuEvent>();

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

            var currDate = DateTime.Today;

            foreach (Match textSegment in Regex.Matches(responseBody, textSplitPattern))
            {
                var textSegmentGroups = textSegment.Groups;
                string teacherInfo = textSegmentGroups[1].Value;
                string courseId = textSegmentGroups[2].Value.Split('(', ')')[1];
                string courseName = textSegmentGroups[3].Value.Split('(', ')')[0];
                string roomName = textSegmentGroups[5].Value;
                string teacherName = GetTeacherName(teacherInfo);
                string weeks = textSegmentGroups[6].Value;
                string timeTable = textSegmentGroups[7].Value;
                var weekIndexes = FindAllIndexes(weeks, '1');
                var classTime = GetClassTime(timeTable);
                var day = classTime.day;
                var firstClass = classTime.firstClass;
                var classTimeStr = classTime.classTimeStr;
                string eventDetail = classTimeStr + ", " + teacherName + ", " + roomName;

                var baseDate = currDate.AddDays((int) day - (int) currDate.DayOfWeek); //本周星期day的日期

                foreach (var weekIndex in weekIndexes)
                {
                    var offset = GetOffsetMinutes(firstClass);
                    var localTime = baseDate.AddDays(7 * (weekIndex - CurrWeekIndex)).AddMinutes(offset);
                    EventList.Add(new NeuEvent
                    {
                        Title = courseName,
                        Detail = eventDetail,
                        Code = courseId,
                        Time = localTime,
                        IsDone = false
                    });
                }
            }
        }
        //new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime))
        private static double GetOffsetMinutes(int firstClass)
        {
            return firstClass switch
            {
                1 => 60 * 8.5,
                3 => 10 * 60 + 40,
                5 => 14 * 60,
                7 => 16 * 60 + 10,
                9 => 18.5 * 60,
                11 => 21.5 * 60,
                _ => 0
            };
        }

        private static string GetTeacherName(string teacherInfo)
        {
            string teacherName = string.Empty;
            const string teacherInfoPattern = "{id:([\\d]*),name:\\\"([\\s\\S]*?)\\\",lab:([\\w]*)}";
            foreach (Match teacherInfoSegment in Regex.Matches(teacherInfo, teacherInfoPattern))
            {
                var teacherInfoSegmentGroups = teacherInfoSegment.Groups;
                teacherName += (teacherInfoSegmentGroups[2].Value + ",");
            }

            teacherName = teacherName.TrimEnd(',');
            return teacherName;
        }

        private static IList<int> FindAllIndexes(string source, char key)
        {
            var i = source.IndexOf(key);
            var indexes = new List<int>();
            while (i != -1)
            {
                indexes.Add(i);
                i = source.IndexOf(key, i + 1);
            }

            return indexes;
        }

        private static (DayOfWeek day, int firstClass, string classTimeStr) GetClassTime(string timeTable)
        {
            const string timeTablePattern = "index =(\\d)\\*unitCount\\+([\\d]+);";
            var segments = Regex.Matches(timeTable, timeTablePattern);
            DayOfWeek day = (DayOfWeek) (int.Parse(segments[0].Groups[1].Value) + 1);
            int firstClass = int.Parse(segments[0].Groups[2].Value) + 1;
            string classTimeStr = firstClass + "-";
            int lastClassIndex = firstClass;
            for (int i = 1; i < segments.Count; i++)
            {
                var segmentGroups = segments[i].Groups;
                int classIndex = int.Parse(segmentGroups[2].Value) + 1;

                if (lastClassIndex + 1 == classIndex)
                {
                    lastClassIndex = classIndex;
                }
                else
                {
                    classTimeStr += (lastClassIndex + ", " + classIndex + "-");
                    lastClassIndex = classIndex;
                }
            }

            classTimeStr += lastClassIndex;
            return (day, firstClass, classTimeStr);
        }
    }
}