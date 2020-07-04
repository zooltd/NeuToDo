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

        public static int CalculateCurrentWeekNo(DateTime baseDate)
            => (int) ((DateTime.Today - baseDate).TotalDays / 7);
    }
}