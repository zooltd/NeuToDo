using System;
using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Services;
using NUnit.Framework;

namespace NeuToDo.UnitTest.Services
{
    public class SemesterStorageTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile()
        {
            File.Delete(StorageProvider.DbPath);
        }

        [Test]
        public async Task CrudTest()
        {
            var storageProvider = new StorageProvider();
            var semesterStorage = await storageProvider.GetSemesterStorage();
            var dbData = await semesterStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 0);
            var autumnSemester = new Semester
                {SemesterId = 12, SchoolYear = "2019-2020", Season = "秋季", BaseDate = new DateTime(2019, 9, 1)};
            var summerSemester = new Semester
                {SemesterId = 54, SchoolYear = "2019-2020", Season = "夏季", BaseDate = new DateTime(2020, 6, 21)};
            var springSemester = new Semester
                {SemesterId = 31, SchoolYear = "2019-2020", Season = "春季", BaseDate = new DateTime(2020, 2, 16)};
            await semesterStorage.InsertAsync(springSemester);
            await semesterStorage.InsertAsync(summerSemester);
            await semesterStorage.InsertAsync(autumnSemester);
            dbData = await semesterStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 3);
            var currentSemester = await semesterStorage.GetSemesterByMaxBaseDateAsync();
            Assert.AreEqual(summerSemester.SemesterId, currentSemester.SemesterId);
            var querySemesterList = await semesterStorage.GetAllAsync(x => x.BaseDate > autumnSemester.BaseDate);
            Assert.AreEqual(querySemesterList.Count, 2);
            await storageProvider.CloseConnectionAsync();
        }
    }
}