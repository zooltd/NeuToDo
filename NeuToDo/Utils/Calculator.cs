using NeuToDo.Models;
using System;

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

        public static string CalculateUniqueNeuEventCode()
            => "C" + DateTime.Now.ToString("yyMMddHHmmssff");

        public static string CalculateUniqueUserEventCode() => DateTime.Now.ToString("yyMMddHHmmssff");
    }
}