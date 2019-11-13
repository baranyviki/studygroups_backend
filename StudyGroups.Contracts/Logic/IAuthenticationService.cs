using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Contracts.Logic
{
    public interface IAuthenticationService
    {
        void RegisterUser(StudentRegistrationDTO userRegistrationDTO);

    }
}
