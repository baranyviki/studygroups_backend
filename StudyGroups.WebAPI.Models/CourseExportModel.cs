using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Models
{

    public class CourseExportModel
    {
        // Tárgy kódja Tárgy neve  Kurzus kódja    Kurzus típusa   Óraszám:	Órarend infó    Oktatók Várólista
        [Index(0)]
        public string SubjectCode { get; set; }
        [Index(1)]
        public string SubjectName { get; set; }
        [Index(2)]
        public string CourseCode { get; set; }
        [Index(3)]
        public string CourseType { get; set; }
        [Index(4)]
        public string ClassHours { get; set; }
        [Index(5)]
        public string ClassSchedule { get; set; }
        [Index(6)]
        public string TeacherName { get; set; }
        [Index(7)]
        public string WaitingList { get; set; }
    }
}

