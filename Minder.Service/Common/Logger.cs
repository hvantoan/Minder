using Newtonsoft.Json;
using System;
using System.Text;

namespace TuanVu.Services.Common {

    public interface ILogger {

        void Debug(string message, params object?[] properties);

        void Information(string message, params object?[] properties);

        void Warning(string message, params object?[] properties);

        void Error(string message, Exception? exception, params object?[] properties);
    }

    public class Logger : ILogger {
        private readonly Serilog.ILogger logger;
        private readonly JsonSerializerSettings settings;

        public static ILogger System()
            => Machine("System");

        public static ILogger Machine(string name)
            => Create("Machine", name);

        public static ILogger Create(string name, string userId) {
            return new Logger(name, userId);
        }

        private Logger(string name, string userId) {
            string key = "";
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(userId))
                key = $"{name}:{userId}";

            this.logger = Serilog.Log.ForContext("key", key);
            this.settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
        }

        public void Debug(string message, params object?[] properties)
            => this.Write(Serilog.Events.LogEventLevel.Debug, message, null, properties);

        public void Information(string message, params object?[] properties)
            => this.Write(Serilog.Events.LogEventLevel.Information, message, null, properties);

        public void Warning(string message, params object?[] properties)
            => this.Write(Serilog.Events.LogEventLevel.Warning, message, null, properties);

        public void Error(string message, Exception? exception, params object?[] properties)
            => this.Write(Serilog.Events.LogEventLevel.Error, message, exception, properties);

        private void Write(Serilog.Events.LogEventLevel level, string message, Exception? exception, params object?[] properties) {
            StringBuilder sb = new();

            sb.Append(message);

            if (properties != null) {
                foreach (var property in properties) {
                    sb.Append(" " + JsonConvert.SerializeObject(property, this.settings));
                }
            }

            if (exception != null) {
                sb.Append($" Exception: {JsonConvert.SerializeObject(exception)}");
            }

            this.logger.Write(level, sb.ToString());
        }
    }
}