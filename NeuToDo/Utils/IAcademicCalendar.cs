using System;

namespace NeuToDo.Utils
{
    public interface IAcademicCalendar
    {
        Campus Campus { get; set; }
        DateTime BaseDate { get; set; }
        int WeekNo { get; set; }
        string Semester { get; set; }
        DateTime GetClassDateTime(DayOfWeek day, int weekNo, int classNo);
        void Reset();
    }
}