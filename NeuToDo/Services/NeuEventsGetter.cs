using NeuToDo.Models;
using NeuToDo.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace NeuToDo.Services
{
    public class NeuEventsGetter
    {
        private static HttpClient _client;
        private Semester _semester;
        private List<NeuEvent> _neuCourses;
        private readonly string _baseUri;
        private string _deanUri;
        private int _semesterId;
        private int _ids;

        public NeuEventsGetter()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });
            _client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58");
            _semester = new Semester();
            _neuCourses = new List<NeuEvent>();
            _baseUri = "https://webvpn.neu.edu.cn";
        }

        public async Task<(Semester semester, List<NeuEvent> neuCourses)> LoginAndFetchData(string userName,
            string password)
        {
            await Login(userName, password);

            await GetQueryData();

            var semestersResponseBody = await GetSemesterInfoResponseBody();

            _semester = ParseSemesters(semestersResponseBody);

            var coursesResponseBody = await GetCourseInfoResponseBody();

            _neuCourses = ParseCourses(coursesResponseBody, _semester);

            return (_semester, _neuCourses);
        }


        /// <summary>
        /// 登陆教务处
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task Login(string userName, string password)
        {
            var res = await _client.GetAsync(_baseUri);
            res.EnsureSuccessStatusCode();
            var responseBody = await res.Content.ReadAsStringAsync();

            var uriPattern = @"<form id=""loginForm""[\s]*action=""(.*)""[\s]*method=""post"">";
            var ltPattern = @"name=""lt"" value=""(.*)""";
            var executionPattern = @"name=""execution"" value=""(.*)""";
            var eventIdPattern = @"name=""_eventId"" value=""(.*)""";

            var loginUri = _baseUri + Regex.Match(responseBody, uriPattern).Groups[1].Value;
            var lt = Regex.Match(responseBody, ltPattern).Groups[1].Value;
            var execution = Regex.Match(responseBody, executionPattern).Groups[1].Value;
            var eventId = Regex.Match(responseBody, eventIdPattern).Groups[1].Value;

            var loginFormData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("rsa", userName + password + lt),
                new KeyValuePair<string, string>("ul", userName.Length.ToString()),
                new KeyValuePair<string, string>("pl", password.Length.ToString()),
                new KeyValuePair<string, string>("lt", lt),
                new KeyValuePair<string, string>("execution", execution),
                new KeyValuePair<string, string>("_eventId", eventId)
            });

            var vpnResponse = await _client.PostAsync(loginUri, loginFormData);
            vpnResponse.EnsureSuccessStatusCode();
            var vpnResponseBody = await vpnResponse.Content.ReadAsStringAsync();

            const string deanUriPattern = @"data-redirect=""(.*)""[\s]*data-name=""教务系统\(新\)""";
            _deanUri = _baseUri + Regex.Match(vpnResponseBody, deanUriPattern).Groups[1].Value;

            var deanResponse = await _client.GetAsync(_deanUri);
            deanResponse.EnsureSuccessStatusCode();
        }

        public async Task<List<NeuEvent>> GetEventList()
        {
            await GetQueryData();

            var semestersResponseBody = await GetSemesterInfoResponseBody();

            _semester = ParseSemesters(semestersResponseBody);

            var coursesResponseBody = await GetCourseInfoResponseBody();

            _neuCourses = ParseCourses(coursesResponseBody, _semester);

            return _neuCourses;
        }

        private async Task GetQueryData()
        {
            var res = await _client.GetAsync(_deanUri + "courseTableForStd.action?");
            res.EnsureSuccessStatusCode();
            var responseBody = await res.Content.ReadAsStringAsync();
            string idPattern =
                @"semesterCalendar\({empty:""false"",onChange:"""",value:""(.*)""},""searchTable\(\)""\);";
            _semesterId = int.Parse(Regex.Match(responseBody, idPattern).Groups[1].Value);
            string idsPattern =
                @"if\(jQuery\(""#courseTableType""\).val\(\)==""std""\){[\s]*bg.form.addInput\(form,""ids"",""(.*)""\);";
            _ids = int.Parse(Regex.Match(responseBody, idsPattern).Groups[1].Value);
        }

        private async Task<string> GetSemesterInfoResponseBody()
        {
            var formData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("dataType", "semesterCalendar"),
                new KeyValuePair<string, string>("value", _semesterId.ToString()),
                new KeyValuePair<string, string>("empty", "false")
            });

            var queryResponse =
                await _client.PostAsync(_deanUri + "/dataQuery.action?vpn-12-o1-219.216.96.4", formData);
            queryResponse.EnsureSuccessStatusCode();
            return await queryResponse.Content.ReadAsStringAsync();
        }


        private async Task<string> GetCourseInfoResponseBody()
        {
            var formData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ignoreHead", "1"),
                new KeyValuePair<string, string>("showPrintAndExport", "1"),
                new KeyValuePair<string, string>("setting.kind", "std"),
                new KeyValuePair<string, string>("startWeek", string.Empty),
                new KeyValuePair<string, string>("semester.id", _semesterId.ToString()),
                new KeyValuePair<string, string>("ids", _ids.ToString()),
            });

            var res = await _client.PostAsync(_deanUri + "/courseTableForStd!courseTable.action", formData);
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
                Season = currSemester.name,
            };
        }

        public List<NeuEvent> ParseCourses(string responseBody, Semester semester)
        {
            var eventList = new List<NeuEvent>();

            const string textSplitPattern =
                "(var teachers =[\\s\\S]*?;)[\\s\\S]*?TaskActivity\\(actTeacherId.join\\(','\\),actTeacherName.join\\(','\\),\"(.*)\",\"(.*)\",\"(.*)\",\"(.*)\",\"(.*)\",null,null,assistantName,\"\",\"\"\\);((?:\\s*index =\\d+\\*unitCount\\+\\d+;\\s*.*\\s)*)";

            // var currDate = DateTime.Today;
            var courseIdToPeriodIdDict = new Dictionary<string, int>();

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

                var campus = (campusName == "浑南校区") ? Campus.浑南 : Campus.南湖; //TODO

                string teacherName = GetTeacherName(teacherInfo);
                string weeks = textSegmentGroups[6].Value;
                string timeTable = textSegmentGroups[7].Value;
                var weekIndexes = FindAllIndexes(weeks, '1');
                var classTime = GetClassTime(timeTable);
                var day = classTime.day;
                var firstClass = classTime.firstClass;
                var classTimeStr = classTime.classTimeStr;
                string eventDetail = classTimeStr + ", " + teacherName + ", " + roomName;

                var periodId = courseIdToPeriodIdDict.ContainsKey(courseId) ? ++courseIdToPeriodIdDict[courseId] : 0;
                courseIdToPeriodIdDict[courseId] = periodId;


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
                    SemesterId = semester.SemesterId,
                    IsUserGenerated = false,
                    Uuid = courseId + "_" + semester.SemesterId + "_" + weekIndex + "_" + (int) day + "_" + firstClass,
                    IsDeleted = false,
                    LastModified = DateTime.Now,
                    PeriodId = periodId
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