using System;
using NeuToDo.Models;

namespace NeuToDo.Utils
{
    public static class Calculator
    {
        public static DateTime CalculateClassTime(DayOfWeek day, int weekNo, int classNo, Campus campus,
            DateTime baseDate)
        {
            var classTimeDict = campus == Campus.Hunnan ? ClassTime.HunnanClassTimeDict : ClassTime.NanhuClassTimeDict;

            return baseDate.AddDays((int) day + weekNo * 7) + classTimeDict[classNo];
        }

        public static int CalculateWeekNo(DateTime baseDate, DateTime date)
            => (int) ((date - baseDate).TotalDays / 7);
    }
}