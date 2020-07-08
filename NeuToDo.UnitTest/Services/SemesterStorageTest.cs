using NeuToDo.Models;
using NeuToDo.Services;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NeuToDo.UnitTest.Services
{
    public class SemesterStorageTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile()
        {
            File.Delete(DbStorageProvider.DbPath);
        }

        [Test]
        public async Task CrudTest()
        {
            var storageProvider = new DbStorageProvider();
            await storageProvider.CheckInitialization();
            var semesterStorage = storageProvider.GetSemesterStorage();
            var dbData = await semesterStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 0);
            var autumnSemester = new Semester
            { SemesterId = 12, SchoolYear = "2019-2020", Season = "秋季", BaseDate = new DateTime(2019, 9, 1) };
            var summerSemester = new Semester
            { SemesterId = 54, SchoolYear = "2019-2020", Season = "夏季", BaseDate = new DateTime(2020, 6, 21) };
            var springSemester = new Semester
            { SemesterId = 31, SchoolYear = "2019-2020", Season = "春季", BaseDate = new DateTime(2020, 2, 16) };
            await semesterStorage.InsertOrReplaceAsync(springSemester);
            await semesterStorage.InsertOrReplaceAsync(summerSemester);
            await semesterStorage.InsertOrReplaceAsync(autumnSemester);
            dbData = await semesterStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 3);
            dbData = await semesterStorage.GetAllOrderedByBaseDateAsync();
            Assert.AreEqual(summerSemester.SemesterId, dbData[0].SemesterId);
            Assert.AreEqual(springSemester.SemesterId, dbData[1].SemesterId);
            Assert.AreEqual(autumnSemester.SemesterId, dbData[2].SemesterId);
            dbData = await semesterStorage.GetAllAsync(x => x.BaseDate > autumnSemester.BaseDate);
            Assert.AreEqual(dbData.Count, 2);
            var querySemester = await semesterStorage.GetAsync(31);
            Assert.AreEqual(querySemester.Season, "春季");
            await storageProvider.CloseConnectionAsync();
        }
    }
}