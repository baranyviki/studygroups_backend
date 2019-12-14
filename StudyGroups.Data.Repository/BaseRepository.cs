using Neo4j.Driver.V1;
using Neo4jMapper;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using System.Linq;

namespace StudyGroups.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected IDriver Neo4jDriver { get; set; }

        public BaseRepository(IDriver neo4jDriver)
        {
            this.Neo4jDriver = neo4jDriver;
        }

        public T Create(T node)
        {
            using (var session = Neo4jDriver.Session())
            {
                string classType = typeof(T).Name;
                var parameters = new Neo4jParameters().WithEntity("newNode", node);
                string query = $@"CREATE (node:{classType} $newNode ) RETURN node";
                var result = session.Run(query, parameters);
                return result.Single().Map<T>();
            }
        }

        public virtual void Delete(T node, string ID)
        {
            using (var session = Neo4jDriver.Session())
            {
                string query;
                string classType = typeof(T).Name;
                if (node is User)
                {
                    classType = "User";
                    query = $@"MATCH (node:" + classType + ") WHERE node." + classType + "ID ='" + ID + "' DETACH DELETE node";
                }

                query = $@"MATCH (node:" + classType + ") WHERE node." + classType + "ID ='" + ID + "' DETACH DELETE node";

                session.Run(query);
                return;
            }
        }

        public IQueryable<T> FindAll()
        {
            using (var session = Neo4jDriver.Session())
            {
                string classType = typeof(T).Name;
                string query = $@"Match (node:{classType}) RETURN node";
                var result = session.Run(query);
                var results = result.Map<T>().AsQueryable();
                return results;
            }
        }

    }
}
