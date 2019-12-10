using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Models
{
    public class StudentDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NeptunCode { get; set; }
        public string MessengerName { get; set; }
        public string InstagramName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderType { get; set; }   
        public string ImagePath { get; set; }
        public IEnumerable<SubjectListItemDTO> TutoringSubjects { get; set; }
    }
}
