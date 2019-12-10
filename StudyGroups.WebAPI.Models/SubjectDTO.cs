using StudyGroups.Data.DAL.DAOs;

namespace StudyGroups.WebAPI.Models
{
    public class SubjectDTO
    {
        public string SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int SuggestedSemester { get; set; }
        public SubjectType SubjectType { get; set; }
    }
}
