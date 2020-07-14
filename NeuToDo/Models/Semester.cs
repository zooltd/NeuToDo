using SQLite;
using System;

namespace NeuToDo.Models
{
    [Table(nameof(Semester))]
    public class Semester
    {
        [PrimaryKey]
        [Column(nameof(SemesterId))]
        public int SemesterId { get; set; }

        [Column(nameof(SchoolYear))] public string SchoolYear { get; set; }

        [Column(nameof(Season))] public string Season { get; set; }

        [Column(nameof(BaseDate))] public DateTime BaseDate { get; set; }

        [Column(nameof(Uuid))] public string Uuid { get; set; }

        [Column(nameof(IsDeleted))] public bool IsDeleted { get; set; }

        [Column(nameof(LastModified))] public DateTime LastModified { get; set; }

        public override string ToString()
        {
            return SchoolYear + ", " + Season;
        }
    }
}