using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Models;
using NeuToDo.Services;
using NUnit.Framework;

namespace NeuToDo.UnitTest.Services
{
    public class SyncServiceTest
    {
        [Test]
        public async Task MergeNeuEventListsTest()
        {
            var localNeuList = new List<NeuEvent>();
            var remoteNeuList = new List<NeuEvent>();

            var dbStorageProviderMock = new Mock<IDbStorageProvider>();
            var httpWebDavServiceMock = new Mock<IHttpWebDavService>();
            var syncService = new SyncService(dbStorageProviderMock.Object, httpWebDavServiceMock.Object);

            var resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(0, resList.Count);

            for (int i = 0; i < 5; i++)
                remoteNeuList.Add(new NeuEvent {Uuid = Guid.NewGuid().ToString(), LastModified = DateTime.Now});
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(5, resList.Count);

            for (int i = 0; i < 5; i++)
                localNeuList.Add(new NeuEvent {Uuid = Guid.NewGuid().ToString(), LastModified = DateTime.Now});
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(10, resList.Count);


            var theEvent = new NeuEvent(remoteNeuList.First()) {LastModified = DateTime.Now, Detail = "Modified"};
            Assert.IsFalse(resList.Exists(x => x.Uuid == theEvent.Uuid && x.Detail == "Modified"));
            localNeuList.Add(theEvent);
            Assert.AreEqual(6, localNeuList.Count);
            Assert.AreEqual(5, remoteNeuList.Count);
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(10, resList.Count);
            Assert.IsTrue(resList.Exists(x => x.Uuid == theEvent.Uuid && x.Detail == "Modified"));
        }

        [Test]
        public async Task MergeMoocEventListsTest()
        {
            var localNeuList = new List<MoocEvent>();
            var remoteNeuList = new List<MoocEvent>();

            var dbStorageProviderMock = new Mock<IDbStorageProvider>();
            var httpWebDavServiceMock = new Mock<IHttpWebDavService>();
            var syncService = new SyncService(dbStorageProviderMock.Object, httpWebDavServiceMock.Object);

            var resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(0, resList.Count);

            for (int i = 0; i < 5; i++)
                remoteNeuList.Add(new MoocEvent {Uuid = Guid.NewGuid().ToString(), LastModified = DateTime.Now});
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(5, resList.Count);

            for (int i = 0; i < 5; i++)
                localNeuList.Add(new MoocEvent {Uuid = Guid.NewGuid().ToString(), LastModified = DateTime.Now});
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(10, resList.Count);


            var theEvent = new MoocEvent(remoteNeuList.First()) {LastModified = DateTime.Now, Detail = "Modified"};
            Assert.IsFalse(resList.Exists(x => x.Uuid == theEvent.Uuid && x.Detail == "Modified"));
            localNeuList.Add(theEvent);
            Assert.AreEqual(6, localNeuList.Count);
            Assert.AreEqual(5, remoteNeuList.Count);
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(10, resList.Count);
            Assert.IsTrue(resList.Exists(x => x.Uuid == theEvent.Uuid && x.Detail == "Modified"));
        }

        [Test]
        public async Task MergeUserEventListsTest()
        {
            var localNeuList = new List<UserEvent>();
            var remoteNeuList = new List<UserEvent>();

            var dbStorageProviderMock = new Mock<IDbStorageProvider>();
            var httpWebDavServiceMock = new Mock<IHttpWebDavService>();
            var syncService = new SyncService(dbStorageProviderMock.Object, httpWebDavServiceMock.Object);

            var resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(0, resList.Count);

            for (int i = 0; i < 5; i++)
                remoteNeuList.Add(new UserEvent {Uuid = Guid.NewGuid().ToString(), LastModified = DateTime.Now});
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(5, resList.Count);

            for (int i = 0; i < 5; i++)
                localNeuList.Add(new UserEvent {Uuid = Guid.NewGuid().ToString(), LastModified = DateTime.Now});
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(10, resList.Count);


            var theEvent = new UserEvent(remoteNeuList.First()) {LastModified = DateTime.Now, Detail = "Modified"};
            Assert.IsFalse(resList.Exists(x => x.Uuid == theEvent.Uuid && x.Detail == "Modified"));
            localNeuList.Add(theEvent);
            Assert.AreEqual(6, localNeuList.Count);
            Assert.AreEqual(5, remoteNeuList.Count);
            resList = await syncService.MergeEventLists(remoteNeuList, localNeuList);
            Assert.AreEqual(10, resList.Count);
            Assert.IsTrue(resList.Exists(x => x.Uuid == theEvent.Uuid && x.Detail == "Modified"));
        }
    }
}