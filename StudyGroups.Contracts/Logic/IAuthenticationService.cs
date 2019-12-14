using StudyGroups.WebAPI.Models;

namespace StudyGroups.Contracts.Logic
{
    public interface IAuthenticationService
    {
        void RegisterUser(StudentRegistrationDTO userRegistrationDTO);
        string Login(LoginDTO user);
    }
}
