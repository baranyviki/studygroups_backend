using System;

namespace StudyGroups.DataAccessLayer
{
    public enum GenderType {
        Female,Male
    }

    public class UserDBModel
    {
        public int UserId { get; set; }
        public string NeptunCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public GenderType GenderType { get; set; }

    }
}
