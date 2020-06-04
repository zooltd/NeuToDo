using System;
using System.Collections.Generic;

namespace NeuToDo.Models
{
    public class Course
    {
        /// <summary>
        /// 课程序号/课程代码 eg.A016090 用于课表
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// 课程编号 eg.C0801006041 用于培养计划、考试
        /// </summary>
        // public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string RoomId { get; set; }

        public string RoomName { get; set; }

        public List<Teacher> TeacherList;

        /// <summary>
        /// 课表, key: 星期x; value: 星期x的计划
        /// </summary>
        public Dictionary<DayOfWeek, DaySchedule> Schedule;

    }

    public class DaySchedule
    {
        public ClassTime ClassTime;

        public string Weeks;

        public int StartingClassTime;
    }

    /// <summary>
    /// 授课教师
    /// </summary>
    public class Teacher
    {
        /// <summary>
        /// 没懂
        /// </summary>
        public bool IsLab { get; set; }

        public string TeacherId { get; set; }

        public string TeacherName { get; set; }
    }

    /// <summary>
    /// 上课时间
    /// </summary>
    [Flags]
    public enum ClassTime
    {
        /// <summary>
        /// 无课
        /// </summary>
        None = 0b_0000_0000_0000,

        /// <summary>
        /// 第一节
        /// </summary>
        First = 0b_0000_0000_0001,

        Second = 0b_0000_0000_0010,

        Third = 0b_0000_0000_0100,

        Fourth = 0b_0000_0000_1000,

        Fifth = 0b_0000_0001_0000,

        Sixth = 0b_0000_0010_0000,

        Seventh = 0b_0000_0100_0000,

        Eighth = 0b_0000_1000_0000,

        Ninth = 0b_0001_0000_0000,

        Tenth = 0b_0010_0000_0000,

        Eleventh = 0b_0100_0000_0000,

        Twelfth = 0b_1000_0000_0000
    }
}