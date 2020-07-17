using NeuToDo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class MoocEventsGetter
    {
        private const string LoginUrl =
            "https://www.icourse163.org/passport/reg/icourseLogin.do";

        private const string GetTokenUrl = "https://www.icourse163.org/";

        private const string GetOnGoingCoursesUrl =
            "https://www.icourse163.org/web/j/learnerCourseRpcBean.getMyLearnedCoursePanelList.rpc";

        private const string CourseDetailUrl =
            "https://www.icourse163.org/learn/";

        private const string GetCourseTestInfoUrl =
            "https://www.icourse163.org/dwr/call/plaincall/CourseBean.getLastLearnedMocTermDto.dwr";

        private static HttpClient _client;

        private static string _token;

        private static List<MoocEvent> _eventList;

        private static List<Course> _courseList;

        public MoocEventsGetter()
        {
            _token = string.Empty;
            _client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });
            _client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58");
            _eventList = new List<MoocEvent>();
            _courseList = new List<Course>();
        }


        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task Login(string userName, string password)
        {
            var postParams = new Dictionary<string, string>
            {
                {"saveLogin", "true"},
                {"oauthType", ""},
                {"username", userName},
                {"passwd", password}
            };

            var response = await _client.PostAsync(LoginUrl,
                new FormUrlEncodedContent(postParams));
            response.EnsureSuccessStatusCode();

            response = await _client.GetAsync(GetTokenUrl);
            var token = GetToken(response);
            _token = token;
            // var doc = new HtmlDocument();
            // doc.LoadHtml(response.Content.ToString());
            // var node = doc.DocumentNode.SelectSingleNode(
            //     "//div[@class='m-slideTop-personFunc']//span['f-thide']");
            // Console.WriteLine(node.InnerText);
        }

        /// <summary>
        /// 获取token。
        /// </summary>
        /// <param name="response">HttpResponseMessage。</param>
        /// <returns></returns>
        private static string GetToken(HttpResponseMessage response)
        {
            var token = response.Headers
                .SingleOrDefault(header => header.Key == "Set-Cookie").Value
                .ToArray()[0].Split('=', ';')[1];
            return token;
        }

        /// <summary>
        /// 获取慕课日程列表。
        /// </summary>
        /// <returns></returns>
        public async Task<(List<Course> courseList, List<MoocEvent> eventList)> GetEventList()
        {
            await GetOnGoingCourses(_token);

            foreach (var course in _courseList)
            {
                await GetTestInfo(course);
            }

            _client.Dispose();

            return (_courseList, _eventList);
        }

        /// <summary>
        /// 获取正在进行中的课程。
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static async Task GetOnGoingCourses(string token)
        {
            var postParams = new Dictionary<string, string>
            {
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

            if (root.result?.result != null)
                foreach (var course in root.result.result)
                {
                    // courses.Add(course.termPanel.courseId.ToString(), course.name);
                    _courseList.Add(new Course
                    {
                        Code = course.termPanel.courseId.ToString(),
                        TermId = course.termPanel.id.ToString(),
                        ImgUrl = course.imgUrl,
                        IsSelected = false,
                        Name = course.name,
                        School = course.schoolPanel.name
                    });
                }
        }

        /// <summary>
        /// 获取测试信息。
        /// </summary>
        /// <param name="course">课程。</param>
        /// <returns></returns>
        private static async Task GetTestInfo(Course course)
        {
            var postParams = new Dictionary<string, string>
            {
                {"callCount", "1"},
                {"scriptSessionId", "${scriptSessionId}190"},
                {"httpSessionId", _token},
                {"c0-scriptName", "CourseBean"},
                {"c0-methodName", "getLastLearnedMocTermDto"},
                {"c0-id", "0"},
                {"c0-param0", course.TermId},
                {"batchId", "0"}
            };

            var response = await _client.PostAsync(GetCourseTestInfoUrl,
                new FormUrlEncodedContent(postParams));
            response.EnsureSuccessStatusCode();
            var dwr = await response.Content.ReadAsStringAsync();

            var quizPattern = @"\w\d+.contentType=2";
            var quizNumList = new List<string>();
            foreach (Match match in Regex.Matches(dwr, quizPattern))
            {
                var str = match.Value.Split('.')[0];
                quizNumList.Add(str);
            }

            var quizName = new List<string>();
            var removeQuizNum = new List<string>();
            foreach (var num in quizNumList)
            {
                quizPattern = num + @".name="".*""";
                var match = Regex.Match(dwr, quizPattern).Value.Split('\"')[1];
                var name = Regex.Unescape(match);
                if (name.Contains("测验") || name.Contains("测试"))
                {
                    quizName.Add(name);
                    Console.WriteLine(name);
                }
                else
                {
                    removeQuizNum.Add(num);
                }
            }

            foreach (string item in removeQuizNum)
            {
                quizNumList.Remove(item);
            }

            var quizDeadline = new List<DateTime>();

            foreach (string num in quizNumList)
            {
                try
                {
                    quizPattern = "s" + (int.Parse(num.Substring(1)) + 1) +
                                  @".deadline=\d*";
                    var unixTime = long.Parse(Regex.Match(dwr, quizPattern)
                        .Value.Split('=')[1]);
                    var time = TimeZoneInfo
                        .ConvertTimeFromUtc(new DateTime(1970, 1, 1),
                            TimeZoneInfo.Local).AddMilliseconds(unixTime);
                    quizDeadline.Add(time);
                }
                catch (Exception e)
                {
                    quizName.RemoveAt(quizName.Count - 1);
                }
            }


            var homeworkPattern = @"\w\d+.contentType=3";
            var homeworkNumList = new List<string>();
            foreach (Match match in Regex.Matches(dwr, homeworkPattern))
            {
                var str = match.Value.Split('.')[0];
                homeworkNumList.Add(str);
            }

            var homeworkName = new List<string>();
            var removeHomeworkNum = new List<string>();
            foreach (string num in homeworkNumList)
            {
                homeworkPattern = num + @".name="".*""";
                var match =
                    Regex.Match(dwr, homeworkPattern).Value.Split('\"')[1];
                var name = Regex.Unescape(match);
                if (name.Contains("作业"))
                {
                    homeworkName.Add(name);
                    Console.WriteLine(name);
                }
                else
                {
                    removeHomeworkNum.Add(num);
                }
            }

            foreach (string item in removeHomeworkNum)
            {
                homeworkNumList.Remove(item);
            }

            var homeworkDeadline = new List<DateTime>();

            foreach (string num in homeworkNumList)
            {
                try
                {
                    homeworkPattern = "s" + (int.Parse(num.Substring(1)) + 1) +
                                      @".deadline=\d*";
                    var unixTime = long.Parse(Regex.Match(dwr, homeworkPattern)
                        .Value.Split('=')[1]);
                    var time = TimeZoneInfo
                        .ConvertTimeFromUtc(new DateTime(1970, 1, 1),
                            TimeZoneInfo.Local).AddMilliseconds(unixTime);
                    homeworkDeadline.Add(time);
                }
                catch (Exception e)
                {
                    homeworkName.RemoveAt(homeworkName.Count - 1);
                }
            }

            //向EventList赋值
            for (var i = 0; i < quizName.Count; i++)
            {
                _eventList.Add(new MoocEvent
                {
                    Title = "Mooc " + course.Name,
                    Detail = quizName[i],
                    Code = course.Code,
                    Time = quizDeadline[i],
                    IsDone = false,
                    Uuid = course.Code + "_" + quizNumList[i],
                    IsDeleted = false,
                    LastModified = DateTime.Now
                });
            }

            for (var i = 0; i < homeworkName.Count; i++)
            {
                _eventList.Add(new MoocEvent
                {
                    Title = "Mooc " + course.Name,
                    Detail = homeworkName[i],
                    Code = course.Code,
                    Time = homeworkDeadline[i],
                    IsDone = false,
                    Uuid = course.Code + "_" + homeworkNumList[i],
                    IsDeleted = false,
                    LastModified = DateTime.Now
                });
            }
        }
    }
}