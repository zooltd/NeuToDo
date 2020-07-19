using System;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface IAcademicCalendarService
    {
        void Reset();

        Task<(Semester semester, int weekNo)> GetCurrentSemester();

        Task<(Semester semester, int weekNo, DateTime sunday)> ToLastWeekSemester();

        Task<(Semester semester, int weekNo, DateTime sunday)> ToNextWeekSemester();
    }
}