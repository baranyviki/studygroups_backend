using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Models
{
    public class StudentRegistrationDTO
    {
        public string FirstName { get; set;}
        public string LastName { get; set;}
        public DateTime Birthday { get; set;}
        public int GenderType { get; set;}
        public string Email { get; set; }
        public string MessengerName { get; set; }
        public string InstagramName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NeptunCode { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile GradeBookExport { get; set; }
        public IFormFile CoursesExport { get; set; }
    }
}
