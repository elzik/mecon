using System;
using Microsoft.Extensions.Logging;

namespace Elzik.Mecon.Framework.Tests.Unit.Application
{
    // This class is necessary because mocking an ILogger will not work; Further explanation here:
    // https://github.com/nsubstitute/NSubstitute/issues/597#issuecomment-653555567

    public abstract class MockLogger<T> : ILogger<T>
    {
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => Log(logLevel, formatter(state, exception));

        public abstract void Log(LogLevel logLevel, string message);

        public virtual bool IsEnabled(LogLevel logLevel) => true;

        public abstract IDisposable BeginScope<TState>(TState state);
    }
}
