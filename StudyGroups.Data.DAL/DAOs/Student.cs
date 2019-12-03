using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Data.DAL.DAOs
{
    public class Student : User
    {
        public string NeptunCode { get; set; }
        public string MessengerName { get; set; }
        public string InstagramName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImagePath { get; set; }
        public int GenderType { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
