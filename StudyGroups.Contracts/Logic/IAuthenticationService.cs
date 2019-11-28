using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StudyGroups.Contracts.Logic
{
    public interface IAuthenticationService
    {
        Task RegisterUserAsync(StudentRegistrationDTO userRegistrationDTO);
        
    }
}
