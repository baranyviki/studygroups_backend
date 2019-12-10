using System;

namespace StudyGroups.WebAPI.Services.Exceptions
{
    public class ParameterException : Exception
    {
        public ParameterException(string message) : base(message)
        {

        }
    }
}
