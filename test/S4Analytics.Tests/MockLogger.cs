using Microsoft.Extensions.Logging;
using System;

namespace S4Analytics.Tests
{
    public class MockScope : IDisposable
    {
        public void Dispose()
        {
            return;
        }
    }

    public class MockLogger<TCategoryName> : ILogger<TCategoryName>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return new MockScope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            return;
        }
    }
}
