using StudyGroups.Data.DAL.DAOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Contracts.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User FindUserByUserName(string userName);
        User FindUserById(Guid userID);
        User FindUserByUserNameAndPassword(string username, string password);

    }
}
