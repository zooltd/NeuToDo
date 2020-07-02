using System;
using System.Collections.Generic;
using NeuToDo.Services;

namespace NeuToDo.Utils
{
    public class AcademicCalendar : IAcademicCalendar
    {
        private readonly IPreferenceStorageProvider _preferenceStorageProvider;

        public AcademicCalendar(IPreferenceStorageProvider preferenceStorageProvider)
        {
            _preferenceStorageProvider = preferenceStorageProvider;
        }
        //TODO 关联东北大学后更新

        private Campus? _campus;

        public Campus Campus
        {
            get => _campus ??= (Campus) _preferenceStorageProvider.Get(nameof(Campus), (int) Campus.Hunnan);
            set
            {
                _campus = value;
                _preferenceStorageProvider.Set(nameof(Campus), (int) value);
            }
        }

        private DateTime? _baseDate;

        /// <summary>
        /// 本学期第0周周日
        /// </summary>
        public DateTime BaseDate
        {
            get => _baseDate ??= _preferenceStorageProvider.Get(nameof(BaseDate), DateTime.MinValue);
            set
            {
                _baseDate = value;
                _preferenceStorageProvider.Set(nameof(BaseDate), value);
            }
        }


        private int? _weekNo;

        public int WeekNo
        {
            get => _weekNo ??= (int) ((DateTime.Today - BaseDate).TotalDays / 7);
            set => _weekNo = value;
        }


        private string _semester;

        public string Semester
        {
            get => _semester ??= _preferenceStorageProvider.Get(nameof(Semester), "未知的时间裂缝");
            set
            {
                _semester = value;
                _preferenceStorageProvider.Set(nameof(Semester), value);
            }
        }

        public DateTime GetClassDateTime(DayOfWeek day, int weekNo, int classNo)
        {
            var classTimeDict = Campus == Campus.Hunnan ? ClassTime.HunnanClassTimeDict : ClassTime.NanhuClassTimeDict;

            return BaseDate.AddDays((int) day + weekNo * 7) + classTimeDict[classNo];
        }
    }

    public enum Campus
    {
        Hunnan,
        Nanhu
    }
}