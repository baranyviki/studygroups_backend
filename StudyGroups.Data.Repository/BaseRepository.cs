﻿using Neo4j.Driver.V1;
using Neo4jMapper;
using StudyGroups.Contracts.Repository;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace StudyGroups.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected IDriver Neo4jDriver { get; set; }

        public BaseRepository(IDriver neo4jDriver)
        {
            this.Neo4jDriver = neo4jDriver;
        }

        public void Create(T node)
        {
            throw new NotImplementedException();
        }

        public void Delete(T node)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindAll()
        {
            using (var session = Neo4jDriver.Session())
            {
                string classType = typeof(T).Name;
                //int classTypeIdx = typeof(T).ToString().LastIndexOf(".");
                //string classType = typeof(T).ToString().Substring(classTypeIdx+1);
                string query = $@"Match (sub:{classType}) RETURN sub";
                var result = session.Run(query);
                var results = result.Map<T>().AsQueryable();
                return results;
            }
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void Update(T node)
        {
            throw new NotImplementedException();
        }
    }
}
