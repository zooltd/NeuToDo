using System;
using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Services;
using NUnit.Framework;

namespace NeuToDo.UnitTest.Services
{
    public class AcademicCalendarServiceTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile()
        {
            File.Delete(DbStorageProvider.DbPath);
        }

        [Test]
        public async Task GetCurrentSemester()
        {
            var dbStorageProvider = new DbStorageProvider();
            await dbStorageProvider.CheckInitialization();
            var semesterStorage = dbStorageProvider.GetSemesterStorage();
            var autumnSemester = new Semester
                {SemesterId = 12, SchoolYear = "2019-2020", Season = "秋季", BaseDate = new DateTime(2019, 9, 1)};
            var summerSemester = new Semester
                {SemesterId = 54, SchoolYear = "2019-2020", Season = "夏季", BaseDate = new DateTime(2020, 6, 21)};
            var springSemester = new Semester
                {SemesterId = 31, SchoolYear = "2019-2020", Season = "春季", BaseDate = new DateTime(2020, 2, 16)};
            await semesterStorage.InsertOrReplaceAsync(springSemester);
            await semesterStorage.InsertOrReplaceAsync(summerSemester);
            await semesterStorage.InsertOrReplaceAsync(autumnSemester);
            var dbData = await semesterStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 3);

            var academicCalendarService = new AcademicCalendarService(dbStorageProvider);
            var (currSemester, _) = await academicCalendarService.GetCurrentSemester();
            Assert.AreEqual(summerSemester.SemesterId,currSemester.SemesterId);

            await dbStorageProvider.CloseConnectionAsync();
        }
    }
}