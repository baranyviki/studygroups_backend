﻿using Neo4j.Driver.V1;
using Neo4jMapper;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudyGroups.Data.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDriver _neo4jDriver) : base(_neo4jDriver)
        {

        }

        public User FindUserById(Guid userID)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userID", userID.ToString());
                string query = $@"MATCH (node:User) WHERE node.UserID = $userID RETURN node";
                var result = session.Run(query, parameters);
                return result.Single().Map<User>();
            }
        }

        public User FindUserByUserName(string userName)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("username", userName);
                string query = $@"MATCH (node:User) WHERE node.UserName = $username RETURN node";
                var result = session.Run(query, parameters);
                return result.Single().Map<User>();
            }
        }

        public User FindUserByUserNameAndPassword(string username, string password)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("username", username)
                                                      .WithValue("password", password);
                string query = $@"MATCH (node:User) WHERE node.UserName = $username AND node.Password= $password RETURN node";
                var result = session.Run(query, parameters);
                return result.Single().Map<User>();
            }
        }
    }
}