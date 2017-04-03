using System;

namespace BleLab.Commands
{
    public interface ICommandFormatter
    {
        object OnEnqueued(CommandBase command);
        object OnDispatched(CommandBase command);
        object OnExecuted(CommandBase command);
        Type CommandType { get; }
    }

    public interface ICommandFormatter<in T> : ICommandFormatter where T : CommandBase
    {
        object OnEnqueued(T command);
        object OnDispatched(T command);
        object OnExecuted(T command);
    }

    public abstract class CommandFormatterBase<T> : ICommandFormatter<T> where T : CommandBase
    {
        public object OnEnqueued(CommandBase command) => OnEnqueued((T)command);

        public object OnDispatched(CommandBase command) => OnDispatched((T)command);

        public object OnExecuted(CommandBase command) => OnExecuted((T)command);

        public abstract object OnEnqueued(T command);
        public abstract object OnDispatched(T command);
        public abstract object OnExecuted(T command);

        public Type CommandType => typeof(T);
    }
}