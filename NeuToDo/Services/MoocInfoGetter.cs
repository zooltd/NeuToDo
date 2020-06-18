using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NeuToDo.Models;
using Newtonsoft.Json;

namespace NeuToDo.Services {
    public class MoocInfoGetter {
        private const string LoginUrl =
            "/passport/reg/icourseLogin.do";

        private const string GetTokenUrl = "/";

        private const string GetOnGoingCoursesUrl =
            "/web/j/learnerCourseRpcBean.getMyLearnedCoursePanelList.rpc";

        private const string CourseDetailUrl =
            "/learn/";

        private const string GetCourseTestInfoUrl =
            "/dwr/call/plaincall/CourseBean.getLastLearnedMocTermDto.dwr";

        private static HttpClient _client;
        private static string _token = "";

        public static List<MoocEvent> EventList;

        /// <summary>
        /// 获取token。
        /// </summary>
        /// <param name="response">HttpResponseMessage。</param>
        /// <returns></returns>
        private static string GetToken(HttpResponseMessage response) =>
            response.Headers
                .SingleOrDefault(header => header.Key == "Set-Cookie").Value
                .ToArray()[0].Split('=', ';')[1];

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private static async Task Login(string userName, string password) {
            try {
                Dictionary<string, string> postParams =
                    new Dictionary<string, string>();
                postParams.Add("saveLogin", "true");
                postParams.Add("oauthType", "");
                postParams.Add("username", "1013913478@qq.com");
                postParams.Add("passwd", "QQzzp0847");

                var response = await _client.PostAsync(LoginUrl,
                    new FormUrlEncodedContent(postParams));
                response.EnsureSuccessStatusCode();

                response = await _client.GetAsync(GetTokenUrl);
                string token = GetToken(response);
                _token = token;
                // var doc = new HtmlDocument();
                // doc.LoadHtml(response.Content.ToString());
                // var node = doc.DocumentNode.SelectSingleNode(
                //     "//div[@class='m-slideTop-personFunc']//span['f-thide']");
                // Console.WriteLine(node.InnerText);
            } catch (WebException we) {
                string msg = we.Message;
                Console.WriteLine(msg);
            }
        }

        private static async Task<Dictionary<string, string>> GetOnGoingCourses(
            string token) {
            var postParams = new Dictionary<string, string> {
                {"csrfKey", token},
                {"type", "10"},
                {"p", "1"},
                {"psize", "8"},
                {"courseType", "1"}
            };

            var response = await _client.PostAsync(GetOnGoingCoursesUrl,
                new FormUrlEncodedContent(postParams));
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();

            Root root = JsonConvert.DeserializeObject<Root>(json);

            Console.WriteLine(root.result.result[1].name);

            var length = root.result.result.Count;

            var courses = new Dictionary<string, string>();
            for (int i = 0; i < length; i++) {
                courses.Add(root.result.result[i].termPanel.id.ToString(),
                    root.result.result[i].name);
            }

            // var courses = new string[length];
            // for (int i = 0; i < length; i++) {
            //     courses[i] = CourseDetailUrl +
            //         root.result.result[i].schoolPanel.shortName + "-" +
            //         root.result.result[i].termPanel.courseId + "?tid=" +
            //         root.result.result[i].termPanel.id;
            // }

            return courses;
        }

        private static async Task GetTestInfo(
            KeyValuePair<string, string> course) {
            var postParams = new Dictionary<string, string> {
                {"callCount", "1"},
                {"scriptSessionId", "${scriptSessionId}190"},
                {"httpSessionId", _token},
                {"c0-scriptName", "CourseBean"},
                {"c0-methodName", "getLastLearnedMocTermDto"},
                {"c0-id", "0"},
                {"c0-param0", course.Key},
                {"batchId", "0"}
            };

            var response = await _client.PostAsync(GetCourseTestInfoUrl,
                new FormUrlEncodedContent(postParams));
            response.EnsureSuccessStatusCode();
            var dwr = await response.Content.ReadAsStringAsync();

            var quizPattern = @"\w\d+.contentType=2";
            var quizNumList = new List<string>();
            foreach (Match match in Regex.Matches(dwr, quizPattern)) {
                var str = match.Value.Split('.')[0];
                quizNumList.Add(str);
            }

            var quizName = new List<string>();
            var removeQuizNum = new List<string>();
            foreach (var num in quizNumList) {
                quizPattern = num + @".name="".*""";
                var match = Regex.Match(dwr, quizPattern).Value.Split('\"')[1];
                var name = Regex.Unescape(match);
                if (name.Contains("测验")) {
                    quizName.Add(name);
                    Console.WriteLine(name);
                } else {
                    removeQuizNum.Add(num);
                }
            }

            foreach (string item in removeQuizNum) {
                quizNumList.Remove(item);
            }

            var quizDeadline = new List<DateTime>();

            foreach (string num in quizNumList) {
                try {
                    quizPattern = "s" + (int.Parse(num.Substring(1)) + 1) +
                        @".deadline=\d*";
                    var unixTime = long.Parse(Regex.Match(dwr, quizPattern)
                        .Value.Split('=')[1]);
                    var time = TimeZoneInfo
                        .ConvertTimeFromUtc(new DateTime(1970, 1, 1),
                            TimeZoneInfo.Local).AddMilliseconds(unixTime);
                    quizDeadline.Add(time);
                    Console.WriteLine(time);
                } catch (Exception e) {
                    quizName.RemoveAt(quizName.Count - 1);
                }
            }


            var homeworkPattern = @"\w\d+.contentType=3";
            var homeworkNumList = new List<string>();
            foreach (Match match in Regex.Matches(dwr, homeworkPattern)) {
                var str = match.Value.Split('.')[0];
                homeworkNumList.Add(str);
            }

            var homeworkName = new List<string>();
            var removeHomeworkNum = new List<string>();
            foreach (string num in homeworkNumList) {
                homeworkPattern = num + @".name="".*""";
                var match =
                    Regex.Match(dwr, homeworkPattern).Value.Split('\"')[1];
                var name = Regex.Unescape(match);
                if (name.Contains("作业")) {
                    homeworkName.Add(name);
                    Console.WriteLine(name);
                } else {
                    removeHomeworkNum.Add(num);
                }
            }

            foreach (string item in removeHomeworkNum) {
                homeworkNumList.Remove(item);
            }

            var homeworkDeadline = new List<DateTime>();

            foreach (string num in homeworkNumList) {
                try {
                    homeworkPattern = "s" + (int.Parse(num.Substring(1)) + 1) +
                        @".deadline=\d*";
                    var unixTime = long.Parse(Regex.Match(dwr, homeworkPattern)
                        .Value.Split('=')[1]);
                    var time = TimeZoneInfo
                        .ConvertTimeFromUtc(new DateTime(1970, 1, 1),
                            TimeZoneInfo.Local).AddMilliseconds(unixTime);
                    homeworkDeadline.Add(time);
                    Console.WriteLine(time);
                } catch (Exception e) {
                    homeworkName.RemoveAt(homeworkName.Count - 1);
                }
            }

            //向EventList赋值
            for (var i = 0; i < quizName.Count; i++) {
                EventList.Add(new MoocEvent {
                    Title = "Mooc " + course.Value,
                    Detail = quizName[i],
                    Code = course.Key,
                    Time = (DateTime) quizDeadline[i],
                    IsDone = false
                });
            }

            for (var i = 0; i < homeworkName.Count; i++) {
                EventList.Add(new MoocEvent {
                    Title = "Mooc " + course.Value,
                    Detail = (string) homeworkName[i],
                    Code = course.Key,
                    Time = (DateTime) homeworkDeadline[i],
                    IsDone = false
                });
            }
        }

        public async Task WebCrawler(string userName, string password) {
            await Login(userName, password);
            var courses = await GetOnGoingCourses(_token);
            EventList = new List<MoocEvent>();
            foreach (var course in courses)
            {
                await GetTestInfo(course);
            }
        }

        public MoocInfoGetter(IHttpClientFactory httpClientFactory) {
            _client = httpClientFactory.CreateClient("mooc");
            EventList = new List<MoocEvent>();
        }
    }

    public class TermPanel {
        public int id { get; set; }

        public int courseId { get; set; }
    }

    public class SchoolPanel {
        public int id { get; set; }

        /// <summary>
        /// 学校名。
        /// </summary>
        public string name { get; set; }

        public string shortName { get; set; }
    }

    public class ResultItem {
        public int id { get; set; }

        public TermPanel termPanel { get; set; }

        public SchoolPanel schoolPanel { get; set; }

        /// <summary>
        /// 课程名。
        /// </summary>
        public string name { get; set; }
    }

    public class Result {
        public List<ResultItem> result { get; set; }
    }

    public class Root {
        public Result result { get; set; }
    }
}