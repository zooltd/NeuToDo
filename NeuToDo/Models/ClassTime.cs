using System;
using System.Collections.Generic;

namespace NeuToDo.Models
{
    public static class ClassTime
    {
        public static readonly Dictionary<int, TimeSpan> NanhuClassTimeDict = new Dictionary<int, TimeSpan>
        {
            {1, new TimeSpan(8, 0, 0)},
            {2, new TimeSpan(9, 0, 0)},
            {3, new TimeSpan(10, 10, 0)},
            {4, new TimeSpan(11, 10, 0)},
            {5, new TimeSpan(14, 0, 0)},
            {6, new TimeSpan(15, 0, 0)},
            {7, new TimeSpan(16, 10, 0)},
            {8, new TimeSpan(17, 10, 0)},
            {9, new TimeSpan(18, 30, 0)},
            {10, new TimeSpan(19, 30, 0)},
            {11, new TimeSpan(20, 30, 0)},
            {12, new TimeSpan(21, 30, 0)}
        };

        public static readonly Dictionary<int, TimeSpan> HunnanClassTimeDict = new Dictionary<int, TimeSpan>
        {
            {1, new TimeSpan(8, 30, 0)},
            {2, new TimeSpan(9, 30, 0)},
            {3, new TimeSpan(10, 40, 0)},
            {4, new TimeSpan(11, 40, 0)},
            {5, new TimeSpan(14, 0, 0)},
            {6, new TimeSpan(15, 0, 0)},
            {7, new TimeSpan(16, 10, 0)},
            {8, new TimeSpan(17, 10, 0)},
            {9, new TimeSpan(18, 30, 0)},
            {10, new TimeSpan(19, 30, 0)},
            {11, new TimeSpan(20, 30, 0)},
            {12, new TimeSpan(21, 30, 0)}
        };
    }
}