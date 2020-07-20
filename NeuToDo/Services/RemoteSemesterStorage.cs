using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NeuToDo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeuToDo.Services
{
    public class RemoteSemesterStorage : IRemoteSemesterStorage
    {
        private static HttpClient _client;
        private readonly IPreferenceStorageProvider _preferenceStorageProvider;

        public const string RemoteUri =
            "https://gitee.com/siberia9527/codes/u58w7vf210t96qyipeskj100/raw?blob_name=Semester.json";

        public RemoteSemesterStorage(IPreferenceStorageProvider preferenceStorageProvider)
        {
            _preferenceStorageProvider = preferenceStorageProvider;
            _client = new HttpClient();
        }

        public async Task<List<Semester>> GetSemesterListAsync()
        {
            string responseBody;
            try
            {
                responseBody = await _client.GetStringAsync(RemoteUri);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new List<Semester>();
            }

            var jObj = JObject.Parse(responseBody);

            var remoteLastModifiedTime = jObj["lastModifiedTime"]?.ToObject<DateTime>();
            var localModifiedTime = _preferenceStorageProvider.Get("LocalSemesterModifiedTime", DateTime.MinValue);
            if (remoteLastModifiedTime < localModifiedTime) return new List<Semester>();

            var objList = jObj["semesters"]?.Children().ToList();
            var semesterList = objList?.ConvertAll(x => x.ToObject<Semester>());
            _preferenceStorageProvider.Set("LocalSemesterModifiedTime", DateTime.Now);
            return semesterList;
        }
    }
}