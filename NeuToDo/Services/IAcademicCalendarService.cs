using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface IAcademicCalendarService
    {
        Task<(Semester semester, int weekNo)> GetCurrentSemester();

        Task<(Semester semester, int weekNo)> ToLastWeekSemester(int thisWeekNo, DateTime lastSunday);

        Task<(Semester semester, int weekNo)> ToNextWeekSemester(int thisWeekNo, DateTime nextSunday);
    }
}