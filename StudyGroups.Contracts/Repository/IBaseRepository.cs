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
        /// Update method
        /// </summary>
        /// <param name="node">node for Update</param>
        void Delete(T node, string ID);

    }
}
