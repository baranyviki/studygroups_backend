using System;
using System.Linq;
using System.Linq.Expressions;

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
        /// <returns>Queryable data structure, contains given type records</returns>
        IQueryable<T> FindAll();

        /// <summary>
        /// Find all database node for the given database entity by given condition.
        /// </summary>
        /// <param name="expression">
        /// Condition in lambda expression
        /// </param>
        /// <returns>Queryable data structure, contains given type records</returns>
        IQueryable<T> FindByCondition(string whereExpression);
    }
}
