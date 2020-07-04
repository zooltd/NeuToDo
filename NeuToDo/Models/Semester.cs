using System;
using SQLite;

namespace NeuToDo.Models
{
    [Table("Semester")]
    public class Semester
    {
        [PrimaryKey]
        [Column("semester_id")]
        public int SemesterId { get; set; }

        [Column("semester_name")]
        public string SemesterName { get; set; }

        [Column("base_date")]
        public DateTime BaseDate { get; set; }
    }
}