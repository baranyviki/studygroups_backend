using Neo4j.Driver.V1;
using Neo4jMapper;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Repository;
using System.Collections.Generic;
using System.Linq;

namespace StudyGroups.Data.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDriver _neo4jDriver) : base(_neo4jDriver)
        {

        }

        public override void Delete(User user, string ID)
        {
            using (var session = Neo4jDriver.Session())
            {
                if (ID == null)
                {
                    var param = new Neo4jParameters().WithValue("userName", user.UserName);
                    string queryName = $@"MATCH (node:User) WHERE node.UserName =$userID DETACH DELETE node";
                    session.Run(queryName, param);
                }

                var parameters = new Neo4jParameters().WithValue("userID", ID);
                string query = $@"MATCH (node:User) WHERE node.UserID =$userID DETACH DELETE node";
                session.Run(query, parameters);
                return;

            }
        }

        public User FindUserById(string userID)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userID", userID);
                string query = $@"MATCH (node:User) WHERE node.UserID = $userID RETURN node";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count() != 1)
                    return null;
                return resultList.Single().Map<User>();
            }
        }

        public User FindUserByUserName(string userName)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("username", userName);
                string query = $@"MATCH (node:User) WHERE node.UserName = $username RETURN node";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count() != 1)
                {
                    return null;
                }
                var user = resultList.Single().Map<User>();
                return user;
            }
        }

        //public User FindUserByUserNameAndPassword(string username, string password)
        //{
        //    using (var session = Neo4jDriver.Session())
        //    {
        //        var parameters = new Neo4jParameters().WithValue("username", username)
        //                                              .WithValue("password", password);
        //        string query = $@"MATCH (node:User) WHERE node.UserName = $username AND node.Password= $password RETURN node";
        //        var result = session.Run(query, parameters);
        //        var resultList = result.ToList();
        //        if (resultList.Count() == 0)
        //        {
        //            return null;
        //        }
        //        return resultList.Single().Map<User>();
        //    }
        //}

        public List<string> GetUserLabelsByUserID(string userID)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userID);

                string query = $@"MATCH (n:User)
                                WHERE n.UserID = $userId
                                UNWIND( labels(n)) as labels 
                                RETURN distinct labels";
                var result = session.Run(query, parameters);
                var labels = result.Select(record => record[0].As<string>()).ToList();
                return labels;
            }
        }

        public void UpdateUserDisabledPropertyByUserId(string ID, bool isDisabled)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", ID)
                                                      .WithValue("isDisabled", isDisabled);

                string query = @"MATCH(n: User)
                                 WHERE n.UserID = $userId
                                 SET n.IsDisabled = $isDisabled";
                var result = session.Run(query, parameters);
            }
        }
    }
}
