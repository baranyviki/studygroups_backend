using StudyGroups.Data.DAL.DAOs;
using System.Collections.Generic;

namespace StudyGroups.Contracts.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User FindUserByUserName(string userName);
        User FindUserById(string userID);
        //User FindUserByUserNameAndPassword(string username, string password);
        List<string> GetUserLabelsByUserID(string userID);
        new void Delete(User user, string ID);
        void UpdateUserDisabledPropertyByUserId(string ID, bool disabled);
    }
}
