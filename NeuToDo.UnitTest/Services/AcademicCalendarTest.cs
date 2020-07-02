using System;
using Moq;
using NeuToDo.Services;
using NUnit.Framework;
using Range = Moq.Range;

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

        [Test]
        public void GetAndSetTest()
        {
            var preferenceStorageProviderMock = new Mock<IPreferenceStorageProvider>();
            var mockPreferenceStorageProvider = preferenceStorageProviderMock.Object;
            preferenceStorageProviderMock.Setup(p => p.Get("Campus", (int) Campus.Hunnan)).Returns((int) Campus.Hunnan);
            preferenceStorageProviderMock.Setup(p => p.Get("BaseDate", DateTime.MinValue)).Returns(DateTime.MinValue);
            preferenceStorageProviderMock.Setup(p => p.Get("Semester", "未知的时间裂缝")).Returns("test");
            var academicCalendar = new AcademicCalendar(mockPreferenceStorageProvider);

            academicCalendar.Campus = Campus.Hunnan;
            preferenceStorageProviderMock.Verify(x => x.Set("Campus", (int) Campus.Hunnan), Times.Once());
            Assert.AreEqual(academicCalendar.Campus,
                (Campus) mockPreferenceStorageProvider.Get("Campus", (int) Campus.Hunnan));

            academicCalendar.BaseDate = DateTime.MinValue;
            preferenceStorageProviderMock.Verify(x => x.Set("BaseDate", DateTime.MinValue));
            Assert.AreEqual(academicCalendar.BaseDate,
                mockPreferenceStorageProvider.Get("BaseDate", DateTime.MinValue));

            academicCalendar.Semester = "test";
            preferenceStorageProviderMock.Verify(x => x.Set("Semester", "test"), Times.Once());
            Assert.AreEqual(academicCalendar.Semester,
                mockPreferenceStorageProvider.Get("Semester", "未知的时间裂缝"));

            academicCalendar.WeekNo = 1;
            Assert.AreEqual(academicCalendar.WeekNo, 1);
        }

        [Test]
        public void ResetTest()
        {
            var preferenceStorageProviderMock = new Mock<IPreferenceStorageProvider>();
            var mockPreferenceStorageProvider = preferenceStorageProviderMock.Object;
            preferenceStorageProviderMock.Setup(p => p.Get("Campus", (int) Campus.Hunnan)).Returns((int) Campus.Hunnan);
            preferenceStorageProviderMock.Setup(p => p.Get("BaseDate", DateTime.MinValue)).Returns(DateTime.MinValue);
            preferenceStorageProviderMock.Setup(p => p.Get("Semester", "未知的时间裂缝")).Returns("test");
            var academicCalendar = new AcademicCalendar(mockPreferenceStorageProvider);

            academicCalendar.Campus = Campus.Hunnan;
            preferenceStorageProviderMock.Verify(x => x.Set("Campus", (int) Campus.Hunnan), Times.Once());
            Assert.AreEqual(academicCalendar.Campus,
                (Campus) mockPreferenceStorageProvider.Get("Campus", (int) Campus.Hunnan));
            preferenceStorageProviderMock.Verify(x => x.Get("Campus", (int) Campus.Hunnan),
                Times.Once);
            academicCalendar.Reset();
            var temp = academicCalendar.Campus;
            preferenceStorageProviderMock.Verify(x => x.Get("Campus", (int) Campus.Hunnan),
                Times.Between(2, 2, Range.Inclusive));
            temp = academicCalendar.Campus;
            preferenceStorageProviderMock.Verify(x => x.Get("Campus", (int) Campus.Hunnan),
                Times.Between(2, 2, Range.Inclusive));

            var t = academicCalendar.WeekNo;
            academicCalendar.BaseDate = academicCalendar.BaseDate.AddDays(7);
            preferenceStorageProviderMock.Setup(p => p.Get("BaseDate", DateTime.MinValue))
                .Returns(DateTime.MinValue.AddDays(7));
            academicCalendar.Reset();
            Assert.AreEqual(academicCalendar.WeekNo, t - 1);
        }
    }
}