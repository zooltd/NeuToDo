using NeuToDo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NeuToDo.Utils;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace NeuToDo.Services
{
    //TODO 存储Semester
    public class NeuSyllabusGetter
    {
        private static int CurrWeekIndex { get; set; }

        private static HttpClient _initClient;
        private static HttpClient _reallocateClient;
        private Semester _semester;
        private List<NeuEvent> _neuCourses;

        public NeuSyllabusGetter(IHttpClientFactory httpClientFactory)
        {
            _initClient = httpClientFactory.NeuInitClient();
            _reallocateClient = httpClientFactory.NeuReallocateClient();
            _semester = new Semester();
            _neuCourses = new List<NeuEvent>();
        }

        public async Task<(Semester semester, List<NeuEvent> neuCourses)> LoginAndFetchData(string userId, string password)
        {
            var vpnUrl =
                "https://pass-443.webvpn.neu.edu.cn/tpass/login?service=https%3A%2F%2Fwebvpn.neu.edu.cn%2Fusers%2Fauth%2Fcas%2Fcallback%3Furl";
            // InitSources(false);
            var authFormData = await GetAuthFormData(vpnUrl, userId, password);

            vpnUrl = vpnUrl.Insert(vpnUrl.IndexOf('?'), ";" + authFormData["jsessionid"]);

            var deanUri = await LoginWebVpn(vpnUrl, authFormData);
            // ReallocateSources(true);
            await LoginDean(deanUri);

            var semesterFormData = await GetSemesterFormData();

            var semestersResponseBody = await GetSemesterInfoResponseBody(semesterFormData);

            _semester = ParseSemesters(semestersResponseBody);

            var coursesResponseBody = await GetCourseInfoResponseBody(semesterFormData);

            _neuCourses = ParseCourses(coursesResponseBody, _semester);

            return (_semester, _neuCourses);
        }

        private async Task<Dictionary<string, string>> GetAuthFormData(
            string vpnUrl, string userId, string password)
        {
            var response = await _initClient.GetAsync(vpnUrl);
            response.EnsureSuccessStatusCode(); //TODO: Exception
            var jsessionid =
                response.Headers
                    .SingleOrDefault(header => header.Key == "Set-Cookie").Value
                    .ToArray()[1].Split(';')[0]; //TODO: Exception
            var responseBody = await response.Content.ReadAsStringAsync();
            const string ltPattern = "name=\"lt\" value=\"(.*)\"";
            const string executionPattern = "name=\"execution\" value=\"(.*)\"";
            const string eventIdPattern = "name=\"_eventId\" value=\"(.*)\"";
            var lt = Regex.Match(responseBody, ltPattern).Groups[1]
                .Value; //TODO: Exception
            var execution = Regex.Match(responseBody, executionPattern)
                .Groups[1].Value;
            var eventId = Regex.Match(responseBody, eventIdPattern).Groups[1]
                .Value;
            var rsa = userId + password + lt;
            var formData = new Dictionary<string, string>
            {
                {"rsa", rsa},
                {"ul", userId.Length.ToString()},
                {"pl", password.Length.ToString()},
                {"lt", lt},
                {"execution", execution},
                {"_eventId", eventId},
                {"jsessionid", jsessionid}
            };
            return formData;
        }

        private async Task<Uri> LoginWebVpn(string vpnUrl,
            IEnumerable<KeyValuePair<string, string>> formData)
        {
            var response = await _initClient.PostAsync(vpnUrl,
                new FormUrlEncodedContent(formData));
            // Ensure code == HttpStatusCode.Redirect 
            var redirectUri = response.Headers.Location;
            return redirectUri;
        }

        private async Task LoginDean(Uri deanUri)
        {
            var response = await _reallocateClient.GetAsync(deanUri);
            response.EnsureSuccessStatusCode();
            const string deanOfficeUrl =
                "https://219-216-96-4.webvpn.neu.edu.cn/eams/homeExt.action";
            response = await _reallocateClient.GetAsync(deanOfficeUrl);
            response.EnsureSuccessStatusCode();
            // var responseBody = await response.Content.ReadAsStringAsync();

            // const string studentInfoPattern =
            //     "class=\"personal-name\">[\\s]*(.*)[\\s]*<\\/a>";
            // var studentInfoList = Regex.Match(responseBody, studentInfoPattern)
            //     .Groups[1].Value.Split('(', ')');

            // User = new User() { Id = studentInfoList[1], Title = studentInfoList[0] };

            // string stuName = studentInfoList[0];
            // string stuId = studentInfoList[1];

            // const string teachingTimePattern =
            //     "id=\"teach-week\">[\\s]*(.*)[\\s]*<font[\\s\\S]*?>(.*)<\\/font>";
            // var teachingTimeGroups =
            //     Regex.Match(responseBody, teachingTimePattern).Groups;
            //
            // var semesterName = teachingTimeGroups[1].Value.Replace("第", ", ");
            // int weekNo = int.TryParse(teachingTimeGroups[2].Value, out weekNo) ? weekNo : 0;

            // CurrWeekIndex = weekNo;
            // var baseDate = DateTime.Today.AddDays(-(int) DateTime.Today.DayOfWeek - weekNo * 7);

            //TODO 正则 查看是否符合标准
            // _semester.SemesterName = semesterName;
            // _semester.BaseDate = baseDate;
        }

        private async Task<Dictionary<string, string>> GetSemesterFormData()
        {
            var res = await _reallocateClient.GetAsync(
                "https://219-216-96-4.webvpn.neu.edu.cn/eams/courseTableForStd.action?");
            res.EnsureSuccessStatusCode();
            var responseBody = await res.Content.ReadAsStringAsync();


            var cookieString = res.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value.ToList()[0];
            var semesterId = Regex.Match(cookieString, @"semester\.id=(\d+);").Groups[1].Value;

            _semester.SemesterId = int.TryParse(semesterId, out var temp) ? temp : 0;

            const string idsPattern =
                "if\\(jQuery\\(\"#courseTableType\"\\)\\.val\\(\\)==\"std\"\\){[\\s]*bg\\.form.addInput\\(form,\"ids\",\"([\\d]*)\"\\)";
            var ids = Regex.Match(responseBody, idsPattern).Groups[1].Value;

            return new Dictionary<string, string>
            {
                {"ignoreHead", "1"},
                {"showPrintAndExport", "1"},
                {"setting.kind", "std"},
                {"startWeek", string.Empty},
                {"semester.id", semesterId},
                {"ids", ids}
            };
        }

        private async Task<String> GetSemesterInfoResponseBody(Dictionary<string, string> formData)
        {
            var dict = new Dictionary<string, string>
            {
                {"dataType", "semesterCalendar"}, {"value", formData["semester.id"]}, {"empty", "false"}
            };
            var res = await _reallocateClient.PostAsync("https://219-216-96-4.webvpn.neu.edu.cn/eams/dataQuery.action",
                new FormUrlEncodedContent(dict));
            return await res.Content.ReadAsStringAsync();
        }

        private async Task<string> GetCourseInfoResponseBody(IEnumerable<KeyValuePair<string, string>> formData)
        {
            var res = await _reallocateClient.PostAsync(
                "https://219-216-96-4.webvpn.neu.edu.cn/eams/courseTableForStd!courseTable.action",
                new FormUrlEncodedContent(formData));
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync();
        }

        public Semester ParseSemesters(string responseBody)
        {
            dynamic deserializeObject = JsonConvert.DeserializeObject(responseBody);
            int yearIndex = deserializeObject.yearIndex;
            int termIndex = deserializeObject.termIndex;
            int semesterId = deserializeObject.semesterId;
            var semesters = deserializeObject.semesters.ToString();
            Dictionary<string, List<SemesterInfo>> dict =
                JsonConvert.DeserializeObject<Dictionary<string, List<SemesterInfo>>>(semesters);
            var currSemester = dict["y" + yearIndex].Find(x => x.id == semesterId);
            return new Semester
            {
                BaseDate = DateTime.Today.AddDays(-(int) DateTime.Today.DayOfWeek - termIndex * 7),
                SemesterId = semesterId,
                SchoolYear = currSemester.schoolYear,
                Season = currSemester.name
            };
        }

        public List<NeuEvent> ParseCourses(string responseBody, Semester semester)
        {
            var eventList = new List<NeuEvent>();

            const string textSplitPattern =
                "(var teachers =[\\s\\S]*?;)[\\s\\S]*?TaskActivity\\(actTeacherId.join\\(','\\),actTeacherName.join\\(','\\),\"(.*)\",\"(.*)\",\"(.*)\",\"(.*)\",\"(.*)\",null,null,assistantName,\"\",\"\"\\);((?:\\s*index =\\d+\\*unitCount\\+\\d+;\\s*.*\\s)*)";

            // var currDate = DateTime.Today;

            foreach (Match textSegment in Regex.Matches(responseBody,
                textSplitPattern))
            {
                var textSegmentGroups = textSegment.Groups;
                string teacherInfo = textSegmentGroups[1].Value;
                string courseId = textSegmentGroups[2].Value.Split('(', ')')[1];
                string courseName =
                    textSegmentGroups[3].Value.Split('(', ')')[0];
                string roomName = textSegmentGroups[5].Value;

                string campusName = roomName.Split('(', ')')[1];

                Campus campus = (campusName == "浑南校区") ? Campus.Hunnan : Campus.Nanhu;

                string teacherName = GetTeacherName(teacherInfo);
                string weeks = textSegmentGroups[6].Value;
                string timeTable = textSegmentGroups[7].Value;
                var weekIndexes = FindAllIndexes(weeks, '1');
                var classTime = GetClassTime(timeTable);
                var day = classTime.day;
                var firstClass = classTime.firstClass;
                var classTimeStr = classTime.classTimeStr;
                string eventDetail = classTimeStr + ", " + teacherName + ", " + roomName;

                eventList.AddRange(weekIndexes.Select(weekIndex => new NeuEvent
                {
                    Title = courseName,
                    Detail = eventDetail,
                    Code = courseId,
                    Time = Calculator.CalculateClassTime(day, weekIndex, firstClass, campus, semester.BaseDate),
                    IsDone = false,
                    Day = (int) day,
                    Week = weekIndex,
                    ClassNo = firstClass,
                    SemesterId = semester.SemesterId
                }));
            }

            return eventList;
        }

        private static string GetTeacherName(string teacherInfo)
        {
            string teacherName = string.Empty;
            const string teacherInfoPattern =
                "{id:([\\d]*),name:\\\"([\\s\\S]*?)\\\",lab:([\\w]*)}";
            foreach (Match teacherInfoSegment in Regex.Matches(teacherInfo,
                teacherInfoPattern))
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

        private static (DayOfWeek day, int firstClass, string classTimeStr)
            GetClassTime(string timeTable)
        {
            const string timeTablePattern =
                "index =(\\d)\\*unitCount\\+([\\d]+);";
            var segments = Regex.Matches(timeTable, timeTablePattern);
            DayOfWeek day =
                (DayOfWeek) ((int.Parse(segments[0].Groups[1].Value) + 1) % 7);
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