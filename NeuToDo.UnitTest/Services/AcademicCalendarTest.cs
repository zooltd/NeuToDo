using System;
using Moq;
using NeuToDo.Services;
using NeuToDo.Utils;
using NUnit.Framework;

namespace NeuToDo.UnitTest.Services
{
    public class AcademicCalendarTest
    {
        [Test]
        public void GetDateTimeTest()
        {
            var preferenceStorageProviderMock = new Mock<IPreferenceStorageProvider>();
            var mockPreferenceStorageProvider = preferenceStorageProviderMock.Object;
            preferenceStorageProviderMock.Setup(p => p.Get("Campus", (int) Campus.Hunnan)).Returns((int) Campus.Nanhu);
            preferenceStorageProviderMock.Setup(p => p.Get("BaseDate", DateTime.MinValue))
                .Returns(DateTime.Today.AddDays(-(int) DateTime.Today.DayOfWeek));
            var academicCalendar = new AcademicCalendar(mockPreferenceStorageProvider);
            Assert.AreEqual(academicCalendar.BaseDate,
                DateTime.Today.AddDays(-(int) DateTime.Today.DayOfWeek));
            var classTime = academicCalendar.GetClassDateTime(DayOfWeek.Monday, 1, 1);
            Assert.AreEqual(classTime, academicCalendar.BaseDate.AddDays(1 + 7).AddMinutes(8 * 60));
        }
    }
}