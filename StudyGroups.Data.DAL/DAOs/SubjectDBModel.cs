using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.DataAccessLayer.DAOs
{
    public class SubjectDBModel
    {
        public int SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int SuggestedSemester { get; set; }
    }
}
