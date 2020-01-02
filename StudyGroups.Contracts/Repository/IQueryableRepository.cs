using System.Linq;

namespace StudyGroups.Contracts
{
    /// <summary>
    /// Repository parent for generic handling of query type database operations
    /// </summary>
    /// <typeparam name="T">Database entity class</typeparam>
    public interface IQueryableRepository<T>
    {
        /// <summary>
        /// Find all database node for the given database set.
        /// </summary>
        /// <returns>Enumerable data structure, contains given type records</returns>
        IQueryable<T> FindAll();
    }
}
