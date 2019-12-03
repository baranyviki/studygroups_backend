using System;

namespace StudyGroups.Data.DAL.DAOs
{
    public enum GenderType {
        Female,Male,Other
    }

    public class User
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }  

    }
}
