using System;

namespace StudyGroups.WebAPI.Services.Exceptions
{
    public class RegistrationException : Exception
    {
        public RegistrationException(string exceptionMessage) : base(exceptionMessage)
        {

        }

    }
}
