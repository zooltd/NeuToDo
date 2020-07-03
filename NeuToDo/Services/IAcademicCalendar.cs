﻿using System;

namespace NeuToDo.Services
{
    public interface IAcademicCalendar
    {
        Campus Campus { get; set; }
        DateTime BaseDate { get; set; }
        int WeekNo { get; set; }
        string SemesterName { get; set; }
        int SemesterId { get; set; }
        DateTime GetClassDateTime(DayOfWeek day, int weekNo, int classNo);
        void Reset();
    }
}