using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace NeuToDo.Services
{
    /// <summary>
    /// BB时间可能时间
    /// </summary>
    public class BbInfoGetter
    {
        private static HttpClient _client;
        private readonly string _baseUri;
        private string _bbUri;
        private List<BbCourse> _toSelectCourses;

        public BbInfoGetter()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });
            _client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58");
            _baseUri = "https://webvpn.neu.edu.cn";
            _toSelectCourses = new List<BbCourse>();
        }

        public async Task LoginAndFetchData(string userName, string password)
        {
            var bbResponse = await LoginBb(userName, password);
            var toSelectCourses = ParseCourses(bbResponse);
            var theCourseUri = toSelectCourses[31].CourseUri;
            await GoToCourse(theCourseUri);
        }


        private async Task<string> LoginBb(string userName, string password)
        {
            //TODO
            userName = "20175330";
            password = "Wuyouhan1999616";

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

            const string bbUriPattern = @"data-redirect=""(.*)""[\s]*data-name=""教务BB系统""";
            _bbUri = _baseUri + Regex.Match(vpnResponseBody, bbUriPattern).Groups[1].Value;

            var bbResponse = await _client.GetAsync(_bbUri);
            bbResponse.EnsureSuccessStatusCode();
            return await bbResponse.Content.ReadAsStringAsync();
        }

        public List<BbCourse> ParseCourses(string responseBody)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseBody);
            htmlDoc.LoadHtml(responseBody);
            var ul = htmlDoc.DocumentNode.SelectSingleNode(
                "//ul[@class='portletList-img courseListing coursefakeclass ']");
            var liList = ul.Descendants("li").ToList();

            foreach (var courseInfo in liList)
            {
                var t = courseInfo.Descendants("a").FirstOrDefault();
                var uri = t?.ChildAttributes("href").FirstOrDefault()?.Value;
                var name = t.InnerText;
                _toSelectCourses.Add(new BbCourse {CourseName = name, CourseUri = uri});
            }

            return _toSelectCourses;
        }

        private async Task GoToCourse(string theCourseUri)
        {
            var theUri = _baseUri + theCourseUri;
            var targetUri = HttpUtility.HtmlDecode(theUri);
            var res = await _client.GetAsync(targetUri);
            res.EnsureSuccessStatusCode();
            var responseBody = res.Content.ReadAsStringAsync();
        }
    }

    public class BbCourse
    {
        public string CourseName { get; set; }
        public string CourseUri { get; set; }
    }
}