using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Minder.Exceptions {

    [Serializable]
    public class ManagedException : Exception {

        public ManagedException() {
        }

        public ManagedException(string message) : base(message) {
        }

        public ManagedException(string message, Exception? innerException) : base(message, innerException) {
        }

        protected ManagedException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        [DoesNotReturn]
        public static void Throw(string message) {
            throw new ManagedException(message);
        }

        [DoesNotReturn]
        public static void Throw(string message, Exception? ex) {
            throw new ManagedException(message, ex);
        }

        public static void ThrowIf([DoesNotReturnIf(true)] bool when, ResponseMessage responseMessage) {
            if (when) throw new ManagedException(JsonConvert.SerializeObject(responseMessage));
        }

        public static void ThrowIf([DoesNotReturnIf(true)] bool when, ResponseMessage responseMessage, Action preThrow) {
            if (when) {
                preThrow.Invoke();
                throw new ManagedException(JsonConvert.SerializeObject(responseMessage));
            }
        }
    }
}