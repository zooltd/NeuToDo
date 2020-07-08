using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Utils;

namespace NeuToDo.Services
{
    public class AcademicCalendarService : IAcademicCalendarService
    {
        private readonly ISemesterStorage _semesterStorage;

        private static readonly Semester EmptySemester = new Semester
            {SchoolYear = "未知的时间裂缝", Season = "请关联教务处", SemesterId = 0};

        public AcademicCalendarService(IDbStorageProvider dbStorageProvider)
        {
            _semesterStorage = dbStorageProvider.GetSemesterStorage();
        }

        private LinkedList<Semester> _semesters;

        private async Task<LinkedList<Semester>> GetOrderedSemesters()
        {
            if (_semesters != null) return _semesters;
            var semesters = new LinkedList<Semester>(await _semesterStorage.GetAllOrderedByBaseDateAsync());
            semesters.AddLast(EmptySemester);
            _semesters = semesters;
            return _semesters;
        }

        private async Task<LinkedListNode<Semester>> GetCurrentSemesterNode()
        {
            var semesters = await GetOrderedSemesters();
            return semesters.First;
        }

        public async Task<(Semester semester, int weekNo)> GetCurrentSemester()
        {
            var semesterNode = await GetCurrentSemesterNode();
            var semester = semesterNode.Value;
            var weekNo = semester.SemesterId == 0 ? 0 : Calculator.CalculateWeekNo(semester.BaseDate, DateTime.Today);
            return (semester, weekNo);
        }


        private LinkedListNode<Semester> _thisSemesterNode;

        public async Task<(Semester semester, int weekNo)> ToLastWeekSemester(
            int thisWeekNo, DateTime lastSunday)
        {
            int weekNo;
            Semester semester;

            _thisSemesterNode ??= await GetCurrentSemesterNode();

            if (thisWeekNo > 0)
            {
                weekNo = --thisWeekNo;
                semester = _thisSemesterNode.Value;
            }
            else
            {
                _thisSemesterNode = _thisSemesterNode.Next ?? _thisSemesterNode;
                semester = _thisSemesterNode.Value;
                weekNo = semester.SemesterId == 0 ? 0 : Calculator.CalculateWeekNo(semester.BaseDate, lastSunday);
            }

            return (semester, weekNo);
        }

        public async Task<(Semester semester, int weekNo)> ToNextWeekSemester(
            int thisWeekNo, DateTime nextSunday)
        {
            int weekNo;
            Semester semester;

            _thisSemesterNode ??= await GetCurrentSemesterNode();

            var threshold = _thisSemesterNode.Previous?.Value.BaseDate ?? DateTime.MaxValue;
            if (nextSunday < threshold)
            {
                semester = _thisSemesterNode.Value;
                weekNo = semester.SemesterId == 0 ? 0 : ++thisWeekNo;
            }
            else
            {
                _thisSemesterNode = _thisSemesterNode.Previous;
                semester = _thisSemesterNode.Value;
                weekNo = 0;
            }

            return (semester, weekNo);
        }
    }
}