using Moq;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using StudyGroups.WebAPI.Services.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTestProject
{
    public class UserServiceTest
    {
        /*
          public UserManageListItem GetUserByID(string userID)
        {
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
         */

        private readonly Mock<IUserRepository> _userRepository;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userRepository = new Mock<IUserRepository>();
            _userService = new UserService(_userRepository.Object);
        }

        [Fact]
        public void GetUserByID_WhenCalledWithNullID_ThrowsParameterException()
        {
            _userRepository.Setup(x => x.FindUserById(It.IsAny<string>()));

            Assert.Throws<ParameterException>(() => _userService.GetUserByID(null));

        }

        [Fact]
        public void GetUsers_WhenCalled_ReturnsExactlyAll()
        {
            var users = new List<User> { new User { }, new User { }, new User { } }.AsQueryable();
            _userRepository.Setup(x => x.FindAll()).Returns(users);

            var result = _userService.GetUsers();
            Assert.Equal(users.Count(), result.Count());
        }

        [Fact]
        public void UpdateUserDisabled_WhenCalledWithNullID_ThrowsParameterException()
        {
            string id = null;
            var usrDTO = new UserPatchDTO { ID = id, Disabled = true };

            Assert.Throws<ParameterException>(() => _userService.UpdateUserDisabled(usrDTO));

        }

        [Fact]
        public void UpdateUserDisabled_WhenCalledWithNullObject_ThrowsParameterException()
        {
            UserPatchDTO usrDTO = null;

            Assert.Throws<ParameterException>(() => _userService.UpdateUserDisabled(usrDTO));

        }
    }
}
