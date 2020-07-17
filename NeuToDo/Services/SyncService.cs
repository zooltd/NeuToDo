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

            var localNeuEvents = await _neuStorage.GetAllAsync();
            var localMoocEvents = await _moocStorage.GetAllAsync();
            var localUserEvents = await _userStorage.GetAllAsync();


            // 将远程收藏项合并到本地。
            var newLocalNeuEvents = await GetEventList(remoteNeuEvents, localNeuEvents);
            foreach (var neuEvent in newLocalNeuEvents)
                await _neuStorage.InsertOrReplaceAsync(neuEvent);
            var newLocalMoocEvents = await GetEventList(remoteMoocEvents, localMoocEvents);
            foreach (var moocEvent in newLocalMoocEvents)
                await _moocStorage.InsertOrReplaceAsync(moocEvent);
            var newLocalUserEvents = await GetEventList(remoteUserEvents, localUserEvents);
            foreach (var userEvent in newLocalUserEvents)
                await _userStorage.InsertOrReplaceAsync(userEvent);

            // 将本地收藏项合并到远程。
            var newRemoteNeuEvents = await GetEventList(localNeuEvents, remoteNeuEvents);
            var neuJson = JsonConvert.SerializeObject(newRemoteNeuEvents);

            var newRemoteMoocEvents = await GetEventList(localMoocEvents, remoteMoocEvents);
            var moocJson = JsonConvert.SerializeObject(newRemoteMoocEvents);

            var newRemoteUserEvents = await GetEventList(localUserEvents, remoteUserEvents);
            var userJson = JsonConvert.SerializeObject(newRemoteUserEvents);

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
            await _remoteDbStorage.UploadFileAsync(remoteFilePath, outputStream);
        }


        private async Task<List<NeuEvent>> GetEventList(List<NeuEvent> sourceList, List<NeuEvent> destList)
        {
            var localDictionary = new Dictionary<string, NeuEvent>();

            await Task.Run((() =>
            {
                localDictionary = destList.ToDictionary(x => x.Uuid, x => x);
                foreach (var remoteItem in sourceList)
                {
                    //local item exists
                    if (localDictionary.TryGetValue(remoteItem.Uuid, out var localItem))
                    {
                        if (remoteItem.LastModified <= localItem.LastModified) continue;
                        localItem.Title = remoteItem.Title;
                        localItem.Detail = remoteItem.Detail;
                        localItem.Time = remoteItem.Time;
                        localItem.IsDone = remoteItem.IsDone;
                        localItem.IsDeleted = remoteItem.IsDeleted;
                        localItem.Day = remoteItem.Day;
                        localItem.Week = remoteItem.Week;
                        localItem.ClassNo = remoteItem.ClassNo;
                    }
                    else //local item not exists
                    {
                        localDictionary[remoteItem.Uuid] = remoteItem;
                    }
                }
            }));
            var neuEvents = localDictionary.Values.ToList();
            neuEvents.ForEach(x => x.LastModified = DateTime.Now);
            return neuEvents;
        }


        private async Task<List<MoocEvent>> GetEventList(List<MoocEvent> sourceList, List<MoocEvent> destList)
        {
            var localDictionary = new Dictionary<string, MoocEvent>();

            await Task.Run((() =>
            {
                localDictionary = destList.ToDictionary(x => x.Uuid, x => x);
                foreach (var remoteItem in sourceList)
                {
                    //local item exists
                    if (localDictionary.TryGetValue(remoteItem.Uuid, out var localItem))
                    {
                        if (remoteItem.LastModified <= localItem.LastModified) continue;
                        localItem.Title = remoteItem.Title;
                        localItem.Detail = remoteItem.Detail;
                        localItem.Time = remoteItem.Time;
                        localItem.IsDone = remoteItem.IsDone;
                        localItem.IsDeleted = remoteItem.IsDeleted;
                    }
                    else //local item not exists
                    {
                        localDictionary[remoteItem.Uuid] = remoteItem;
                    }
                }
            }));

            var moocEvents = localDictionary.Values.ToList();
            moocEvents.ForEach(x => x.LastModified = DateTime.Now);
            return moocEvents;
        }


        private async Task<List<UserEvent>> GetEventList(List<UserEvent> sourceList, List<UserEvent> destList)
        {
            var localDictionary = new Dictionary<string, UserEvent>();

            await Task.Run((() =>
            {
                localDictionary = destList.ToDictionary(x => x.Uuid, x => x);
                foreach (var remoteItem in sourceList)
                {
                    //local item exists
                    if (localDictionary.TryGetValue(remoteItem.Uuid, out var localItem))
                    {
                        if (remoteItem.LastModified <= localItem.LastModified) continue;
                        localItem.Title = remoteItem.Title;
                        localItem.Detail = remoteItem.Detail;
                        localItem.Time = remoteItem.Time;
                        localItem.IsDone = remoteItem.IsDone;
                        localItem.IsDeleted = remoteItem.IsDeleted;
                        localItem.StartDate = remoteItem.StartDate;
                        localItem.EndDate = remoteItem.EndDate;
                        localItem.DaySpan = remoteItem.DaySpan;
                        localItem.TimeOfDay = remoteItem.TimeOfDay;
                        localItem.IsRepeat = remoteItem.IsRepeat;
                    }
                    else //local item not exists
                    {
                        localDictionary[remoteItem.Uuid] = remoteItem;
                    }
                }
            }));

            var userEvents = localDictionary.Values.ToList();
            userEvents.ForEach(x => x.LastModified = DateTime.Now);
            return userEvents;
        }
    }
}