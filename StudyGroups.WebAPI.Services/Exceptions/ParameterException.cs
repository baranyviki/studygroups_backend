using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Services.Exceptions
{
    public class ParameterException : Exception
    {
        public ParameterException(string message) : base(message)
        {

        }
    }
}
