using StudyGroups.WebAPI.Models;
using System.Collections.Generic;

namespace StudyGroups.Contracts.Logic
{
    public interface IUserService
    {
        IEnumerable<UserManageListItem> GetUsers();
        UserManageListItem GetUserByID(string userID);
        void UpdateUserDisabled(UserPatchDTO user);
    }
}
