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
        private DateTime _thisSunday;
        private int _thisWeekNo;

        public async Task<(Semester semester, int weekNo, DateTime sunday)> ToLastWeekSemester()
        {
            if (_thisSemesterNode == null)
            {
                _thisSemesterNode = await GetCurrentSemesterNode();
                _thisSunday = DateTime.Today.AddDays(-(int) DateTime.Today.DayOfWeek);
                _thisWeekNo = Calculator.CalculateWeekNo(_thisSemesterNode.Value.BaseDate, DateTime.Today);
            }

            _thisSunday = _thisSunday.AddDays(-7);

            if (_thisWeekNo > 0)
            {
                --_thisWeekNo;
            }
            else
            {
                _thisSemesterNode = _thisSemesterNode.Next ?? _thisSemesterNode;
                _thisWeekNo = _thisSemesterNode.Value.SemesterId == 0
                    ? 0
                    : Calculator.CalculateWeekNo(_thisSemesterNode.Value.BaseDate, _thisSunday);
            }

            return (_thisSemesterNode.Value, _thisWeekNo, _thisSunday);
        }

        public async Task<(Semester semester, int weekNo, DateTime sunday)> ToNextWeekSemester()
        {
            if (_thisSemesterNode == null)
            {
                _thisSemesterNode = await GetCurrentSemesterNode();
                _thisSunday = DateTime.Today.AddDays(-(int) DateTime.Today.DayOfWeek);
                _thisWeekNo = Calculator.CalculateWeekNo(_thisSemesterNode.Value.BaseDate, DateTime.Today);
            }

            _thisSunday = _thisSunday.AddDays(7);

            var threshold = _thisSemesterNode.Previous?.Value.BaseDate ?? DateTime.MaxValue;
            if (_thisSunday < threshold)
            {
                _thisWeekNo = _thisSemesterNode.Value.SemesterId == 0 ? 0 : _thisWeekNo + 1;
            }
            else
            {
                _thisSemesterNode = _thisSemesterNode.Previous;
                _thisWeekNo = 0;
            }

            return (_thisSemesterNode?.Value, _thisWeekNo, _thisSunday);
        }
    }
}