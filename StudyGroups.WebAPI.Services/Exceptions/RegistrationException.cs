using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Services.Exceptions
{
    public class RegistrationException : Exception
    {
        public RegistrationException(string exceptionMessage) : base(exceptionMessage)
        {

        }

    }
}
