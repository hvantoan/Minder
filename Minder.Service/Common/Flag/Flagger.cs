using System;
using System.Collections.Concurrent;

namespace Minder.Services.Common.Flag {
    public class Flagger : IDisposable {
        private static readonly ConcurrentDictionary<string, string> flags = new();

        private readonly string flagKey;

        public Flagger(string key) {
            this.flagKey = $"Flag:{key}";
        }

        public bool Flagged() {
            return flags.TryAdd(this.flagKey, this.flagKey);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    flags.TryRemove(this.flagKey, out _);
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}