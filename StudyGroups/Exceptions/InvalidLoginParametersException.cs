using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyGroups.WebAPI.WebSite.Exceptions
{
    /// <summary>
    /// Custom exception for handling invalid logins.
    /// </summary>
    public class InvalidLoginParametersException : Exception
    {

        /// <summary>
        /// base constructor
        /// </summary>
        /// <param name="msg">Exception message</param>
        public InvalidLoginParametersException(string msg) : base(msg)
        {

        }
    }
}
