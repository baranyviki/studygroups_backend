using StudyGroups.WebAPI.Models;
using System.Threading.Tasks;

namespace StudyGroups.Contracts.Logic
{
    public interface IAuthenticationService
    {
        void RegisterUser(StudentRegistrationDTO userRegistrationDTO);
        string Login(LoginDTO user);
    }
}
