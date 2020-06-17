using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FunctionTests.Helpers
{
    public class ListLogger : ILogger
    {
        public IList<string> Logs;

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => false;

        public ListLogger()
        {
            Logs = new List<string>();
        }

        public void Log<TState>(
            LogLevel logLevel, 
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            Logs.Add(message);
        }
    }
}