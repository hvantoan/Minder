using System;
using System.Runtime.Serialization;

namespace Minder.Exceptions {
    [Serializable]
    public class ManagedException : Exception {

        public ManagedException() {
        }

        public ManagedException(string message) : base(message) {
        }

        public ManagedException(string message, Exception innerException) : base(message, innerException) {
        }

        protected ManagedException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}