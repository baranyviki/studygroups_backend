using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;

namespace StudyGroups.WebAPI.Services.Mapping
{
    public static class MapUser
    {
        public static UserManageListItem MapUserToUserManageListItem(User userDBModel)
        {
            if (userDBModel == null)
                return null;
            else
                return new UserManageListItem
                {
                    ID = userDBModel.UserID,
                    UserName = userDBModel.UserName,
                    Email = userDBModel.Email,
                    IsDisabled = userDBModel.IsDisabled
                };

        }


    }
}
