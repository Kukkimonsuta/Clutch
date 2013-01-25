using System;
using System.Data.Common;

namespace Clutch.Diagnostics.EntityFramework
{
    public interface IDbTracingListener
    {
        void CommandExecuting(DbTracingContext context);
        void CommandFinished(DbTracingContext context);
        void ReaderFinished(DbTracingContext context);
        void CommandFailed(DbTracingContext context);
        void CommandExecuted(DbTracingContext context);
    }

    public class DbTracingListener : IDbTracingListener
    {
        public virtual void CommandExecuting(DbTracingContext context)
        { }

        public virtual void CommandFinished(DbTracingContext context)
        { }

        public virtual void ReaderFinished(DbTracingContext context)
        { }

        public virtual void CommandFailed(DbTracingContext context)
        { }

        public virtual void CommandExecuted(DbTracingContext context)
        { }
    }

    public class GenericDbTracingListener : IDbTracingListener
    {
        public GenericDbTracingListener()
        { }

        private Action<DbTracingContext> onExecuting;
        private Action<DbTracingContext> onFinished;
        private Action<DbTracingContext> onReaderFinished;
        private Action<DbTracingContext> onFailed;
        private Action<DbTracingContext> onExecuted;

        public void OnExecuting(Action<DbTracingContext> handler)
        {
            onExecuting = handler;
        }

        public void OnFinished(Action<DbTracingContext> handler)
        {
            onFinished = handler;
        }

        public void OnReaderFinished(Action<DbTracingContext> handler)
        {
            onReaderFinished = handler;
        }

        public void OnFailed(Action<DbTracingContext> handler)
        {
            onFailed = handler;
        }

        public void OnExecuted(Action<DbTracingContext> handler)
        {
            onExecuted = handler;
        }

        #region IDbTracingListener

        void IDbTracingListener.CommandExecuting(DbTracingContext context)
        {
            if (onExecuting != null)
                onExecuting(context);
        }

        void IDbTracingListener.CommandFinished(DbTracingContext context)
        {
            if (onFinished != null)
                onFinished(context);
        }

        void IDbTracingListener.ReaderFinished(DbTracingContext context)
        {
            if (onReaderFinished != null)
                onReaderFinished(context);
        }

        void IDbTracingListener.CommandFailed(DbTracingContext context)
        {
            if (onFailed != null)
                onFailed(context);
        }

        void IDbTracingListener.CommandExecuted(DbTracingContext context)
        {
            if (onExecuted != null)
                onExecuted(context);
        }

        #endregion
    }
}
