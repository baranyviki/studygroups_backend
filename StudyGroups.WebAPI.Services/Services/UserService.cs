using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using StudyGroups.WebAPI.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudyGroups.WebAPI.Services.Services
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserManageListItem GetUserByID(string userID)
        {
            if (userID == null || !Guid.TryParse(userID, out Guid guid))
                throw new ParameterException("userID is invalid");
            var usr = _userRepository.FindUserById(userID);
            return MapUser.MapUserToUserManageListItem(usr);
        }

        public IEnumerable<UserManageListItem> GetUsers()
        {
            var users = _userRepository.FindAll();
            var listitems = users.Select(x => MapUser.MapUserToUserManageListItem(x));
            return listitems;
        }

        public void UpdateUserDisabled(UserPatchDTO user)
        {
            if (user == null)
                throw new ParameterException("Patch object cannot be null.");
            if (user.ID == null || !Guid.TryParse(user.ID, out Guid res))
                throw new ParameterException("User patch properties is in bad format.");
            _userRepository.UpdateUserDisabledPropertyByUserId(user.ID, user.Disabled);
        }
    }
}
