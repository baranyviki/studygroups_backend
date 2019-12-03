using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Models
{
    public class GradeBookExportModel
    {
        [Index(0)]
        public string SubjectCode { get; set; }
        [Index(1)]
        public string Name { get; set; }
        [Index(2)]
        public int Credits { get; set; }
        [Index(3)]
        public string Semester { get; set; }
        [Index(4)]
        public string Kovetelmenyek { get; set; }
        [Index(5)]
        public string WeeklyClass { get; set; }
        [Index(6)]
        public string SemesterlyClass { get; set; }
        [Index(7)]
        public string Sign { get; set; }
        [Index(8)]
        public string Grade { get; set; }
        [Index(9)]
        public string Comment { get; set; }
        [Index(10)]
        public string WaitingList { get; set; }
        [Index(11)]
        public string Completed { get; set; }

    }
}
