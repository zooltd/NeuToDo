using System.Collections.Generic;

namespace NeuToDo.Models
{
    /// <summary>
    /// 反序列化类。
    /// </summary>
    public class Root
    {
        public Result result { get; set; }
    }

    public class TermPanel
    {
        public int id { get; set; }

        public int courseId { get; set; }
    }

    public class SchoolPanel
    {
        public int id { get; set; }

        /// <summary>
        /// 学校名。
        /// </summary>
        public string name { get; set; }

        public string shortName { get; set; }
    }

    public class ResultItem
    {
        public int id { get; set; }

        public TermPanel termPanel { get; set; }

        public SchoolPanel schoolPanel { get; set; }

        /// <summary>
        /// 课程名。
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 课程头图。
        /// </summary>
        public string imgUrl { get; set; }
    }

    public class Result
    {
        public List<ResultItem> result { get; set; }
    }
}