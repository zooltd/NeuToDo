using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using NeuToDo.Models;
using Newtonsoft.Json;

namespace NeuToDo.Services
{
    public class SyncService : ISyncService
    {
        private readonly IHttpWebDavService _remoteDbStorage;
        private readonly IEventModelStorage<NeuEvent> _neuStorage;
        private readonly IEventModelStorage<MoocEvent> _moocStorage;
        private readonly IEventModelStorage<UserEvent> _userStorage;

        public SyncService(IDbStorageProvider localDbStorageProvider,
            IHttpWebDavService remoteDbStorage)
        {
            _remoteDbStorage = remoteDbStorage;
            _neuStorage = localDbStorageProvider.GetEventModelStorage<NeuEvent>();
            _moocStorage = localDbStorageProvider.GetEventModelStorage<MoocEvent>();
            _userStorage = localDbStorageProvider.GetEventModelStorage<UserEvent>();
        }


        /// <summary>
        /// 同步。
        /// </summary>
        public async Task SyncEventModelsAsync(string remoteFilePath)
        {
            List<NeuEvent> remoteNeuEvents = null;
            List<MoocEvent> remoteMoocEvents = null;
            List<UserEvent> remoteUserEvents = null;

            //获取远程压缩包内数据
            if (await _remoteDbStorage.FileExist(remoteFilePath))
            {
                var fileStream = await _remoteDbStorage.GetFileStreamAsync(remoteFilePath);

                using (var zipInputStream = new ZipInputStream(fileStream))
                {
                    ZipEntry entry;
                    while ((entry = zipInputStream.GetNextEntry()) != null)
                    {
                        using var jsonStream = new MemoryStream();
                        StreamUtils.Copy(zipInputStream, jsonStream, new byte[2048]);
                        jsonStream.Position = 0;
                        using var jsonReader = new StreamReader(jsonStream);
                        switch (entry.Name)
                        {
                            case nameof(NeuEvent) + ".json":
                                remoteNeuEvents = JsonConvert.DeserializeObject<List<NeuEvent>>(
                                    await jsonReader.ReadToEndAsync());
                                break;
                            case nameof(MoocEvent) + ".json":
                                remoteMoocEvents = JsonConvert.DeserializeObject<List<MoocEvent>>(
                                    await jsonReader.ReadToEndAsync());
                                break;
                            case nameof(UserEvent) + ".json":
                                remoteUserEvents = JsonConvert.DeserializeObject<List<UserEvent>>(
                                    await jsonReader.ReadToEndAsync());
                                break;
                        }
                    }
                }

                fileStream.Close();
            }


            remoteNeuEvents ??= new List<NeuEvent>();
            remoteMoocEvents ??= new List<MoocEvent>();
            remoteUserEvents ??= new List<UserEvent>();

            //获取本地数据
            var localNeuEvents = await _neuStorage.GetAllAsync();
            var localMoocEvents = await _moocStorage.GetAllAsync();
            var localUserEvents = await _userStorage.GetAllAsync();

            // 将远程收藏项合并到本地。
            var newLocalNeuEvents = await MergeEventLists(remoteNeuEvents, localNeuEvents);
            foreach (var neuEvent in newLocalNeuEvents)
                await _neuStorage.InsertOrReplaceAsync(neuEvent);
            var newLocalMoocEvents = await MergeEventLists(remoteMoocEvents, localMoocEvents);
            foreach (var moocEvent in newLocalMoocEvents)
                await _moocStorage.InsertOrReplaceAsync(moocEvent);
            var newLocalUserEvents = await MergeEventLists(remoteUserEvents, localUserEvents);
            foreach (var userEvent in newLocalUserEvents)
                await _userStorage.InsertOrReplaceAsync(userEvent);

            // 将本地收藏项合并到远程。
            var newRemoteNeuEvents = await MergeEventLists(localNeuEvents, remoteNeuEvents);
            var neuJson = JsonConvert.SerializeObject(newRemoteNeuEvents);

            var newRemoteMoocEvents = await MergeEventLists(localMoocEvents, remoteMoocEvents);
            var moocJson = JsonConvert.SerializeObject(newRemoteMoocEvents);

            var newRemoteUserEvents = await MergeEventLists(localUserEvents, remoteUserEvents);
            var userJson = JsonConvert.SerializeObject(newRemoteUserEvents);

            //内存中打包
            using var outputStream = new MemoryStream();
            using var zipOutputStream = new ZipOutputStream(outputStream);
            zipOutputStream.SetLevel(3);

            var neuEntry = new ZipEntry(nameof(NeuEvent) + ".json") {DateTime = DateTime.Now};
            zipOutputStream.PutNextEntry(neuEntry);
            using (var newJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(neuJson)))
                await Task.Run(() => StreamUtils.Copy(newJsonStream, zipOutputStream, new byte[2048]));

            var moocEntry = new ZipEntry(nameof(MoocEvent) + ".json") {DateTime = DateTime.Now};
            zipOutputStream.PutNextEntry(moocEntry);
            using (var newJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(moocJson)))
                await Task.Run(() => StreamUtils.Copy(newJsonStream, zipOutputStream, new byte[2048]));

            var userEntry = new ZipEntry(nameof(UserEvent) + ".json") {DateTime = DateTime.Now};
            zipOutputStream.PutNextEntry(userEntry);
            using (var newJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(userJson)))
                await Task.Run(() => StreamUtils.Copy(newJsonStream, zipOutputStream, new byte[2048]));

            zipOutputStream.CloseEntry();
            zipOutputStream.IsStreamOwner = false;
            zipOutputStream.Close();

            outputStream.Position = 0;
            //上传压缩包
            await _remoteDbStorage.UploadFileAsync(remoteFilePath, outputStream);
        }

        /// <summary>
        /// 根据更新时间，合并两个eventList
        /// </summary>
        /// <param name="sourceList"></param>
        /// <param name="destList"></param>
        /// <returns></returns>
        public async Task<List<NeuEvent>> MergeEventLists(IList<NeuEvent> sourceList, IList<NeuEvent> destList)
        {
            var destDict = new Dictionary<string, NeuEvent>();

            await Task.Run((() =>
            {
                destDict = destList.ToDictionary(x => x.Uuid, x => x);
                foreach (var sourceItem in sourceList)
                {
                    //source item exists
                    if (destDict.TryGetValue(sourceItem.Uuid, out var destItem))
                    {
                        if (sourceItem.LastModified <= destItem.LastModified) continue;
                        destItem.Title = sourceItem.Title;
                        destItem.Detail = sourceItem.Detail;
                        destItem.Time = sourceItem.Time;
                        destItem.IsDone = sourceItem.IsDone;
                        destItem.IsDeleted = sourceItem.IsDeleted;
                        destItem.Day = sourceItem.Day;
                        destItem.Week = sourceItem.Week;
                        destItem.ClassNo = sourceItem.ClassNo;
                    }
                    else //source item not exists
                    {
                        destDict[sourceItem.Uuid] = sourceItem;
                    }
                }
            }));
            var neuEvents = destDict.Values.ToList();
            neuEvents.ForEach(x => x.LastModified = DateTime.Now);
            return neuEvents;
        }


        public async Task<List<MoocEvent>> MergeEventLists(IList<MoocEvent> sourceList, IList<MoocEvent> destList)
        {
            var destDict = new Dictionary<string, MoocEvent>();

            await Task.Run((() =>
            {
                destDict = destList.ToDictionary(x => x.Uuid, x => x);
                foreach (var sourceItem in sourceList)
                {
                    //local item exists
                    if (destDict.TryGetValue(sourceItem.Uuid, out var destItem))
                    {
                        if (sourceItem.LastModified <= destItem.LastModified) continue;
                        destItem.Title = sourceItem.Title;
                        destItem.Detail = sourceItem.Detail;
                        destItem.Time = sourceItem.Time;
                        destItem.IsDone = sourceItem.IsDone;
                        destItem.IsDeleted = sourceItem.IsDeleted;
                    }
                    else //local item not exists
                    {
                        destDict[sourceItem.Uuid] = sourceItem;
                    }
                }
            }));

            var moocEvents = destDict.Values.ToList();
            moocEvents.ForEach(x => x.LastModified = DateTime.Now);
            return moocEvents;
        }


        public async Task<List<UserEvent>> MergeEventLists(IList<UserEvent> sourceList, IList<UserEvent> destList)
        {
            var destDict = new Dictionary<string, UserEvent>();

            await Task.Run((() =>
            {
                destDict = destList.ToDictionary(x => x.Uuid, x => x);
                foreach (var sourceItem in sourceList)
                {
                    //local item exists
                    if (destDict.TryGetValue(sourceItem.Uuid, out var destItem))
                    {
                        if (sourceItem.LastModified <= destItem.LastModified) continue;
                        destItem.Title = sourceItem.Title;
                        destItem.Detail = sourceItem.Detail;
                        destItem.Time = sourceItem.Time;
                        destItem.IsDone = sourceItem.IsDone;
                        destItem.IsDeleted = sourceItem.IsDeleted;
                        destItem.StartDate = sourceItem.StartDate;
                        destItem.EndDate = sourceItem.EndDate;
                        destItem.DaySpan = sourceItem.DaySpan;
                        destItem.TimeOfDay = sourceItem.TimeOfDay;
                        destItem.IsRepeat = sourceItem.IsRepeat;
                    }
                    else //local item not exists
                    {
                        destDict[sourceItem.Uuid] = sourceItem;
                    }
                }
            }));

            var userEvents = destDict.Values.ToList();
            userEvents.ForEach(x => x.LastModified = DateTime.Now);
            return userEvents;
        }
    }
}