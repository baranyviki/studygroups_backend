using StudyGroups.Data.DAL.DAOs;
using System;
using System.Collections.Generic;

namespace StudyGroups.Contracts.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User FindUserByUserName(string userName);
        User FindUserById(Guid userID);
        User FindUserByUserNameAndPassword(string username, string password);
        List<string> GetUserLabelsByUserID(string userID);
        new void Delete(User user, string ID);
    }
}
