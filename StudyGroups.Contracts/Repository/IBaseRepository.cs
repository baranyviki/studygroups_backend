namespace StudyGroups.Contracts.Repository
{
    public interface IBaseRepository<T> : IQueryableRepository<T> where T : class
    {
        /// <summary>
        /// Create method
        /// </summary>
        /// <param name="node">node for create</param>
        T Create(T node);

        /// <summary>
        /// Delete a node
        /// </summary>
        /// <param name="node">node to be deleted</param>
        /// <param name="ID">node's ID</param>
        void Delete(T node, string ID);

    }
}
