using System;

namespace StudyGroups.DataAccessLayer
{
    public enum GenderType {
        Female,Male,Other
    }

    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }       
        public string MessengerName { get; set; }
        public string InstagramName { get; set; }
        public string ImagePath { get; set; }
        public GenderType GenderType { get; set; }
        public DateTime Birthday { get; set; }

    }
}
