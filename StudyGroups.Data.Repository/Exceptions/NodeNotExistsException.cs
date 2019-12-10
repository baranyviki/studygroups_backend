using System;
using System.Runtime.Serialization;

namespace StudyGroups.Data.Repository
{
    [Serializable]
    public class NodeNotExistsException : Exception
    {
        public NodeNotExistsException()
        {
        }

        public NodeNotExistsException(string message) : base(message)
        {
        }

        public NodeNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NodeNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}