using System;
using SQLite;

namespace NeuToDo.Models
{
    [Table(nameof(Semester))]
    public class Semester
    {
        [PrimaryKey]
        [Column(nameof(SemesterId))]
        public int SemesterId { get; set; }

        [Column(nameof(SemesterName))] public string SemesterName { get; set; }

        [Column(nameof(BaseDate))] public DateTime BaseDate { get; set; }
    }
}