using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Contracts.Repository
{
    public interface IUserRepository
    {
        int getNextUserId();
    }
}
