using Microsoft.AspNetCore.Http;

namespace StudyGroups.WebAPI.Models
{
    public class StudentRegistrationDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string MessengerName { get; set; }
        public string InstagramName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NeptunCode { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile GradeBook { get; set; }
        public IFormFile Courses { get; set; }
    }
}
