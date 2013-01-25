using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Clutch.Diagnostics.EntityFramework
{
    public class DbTracingContext
    {
        internal DbTracingContext(DbTracingType type, DbConnection connection, DbCommand command)
        {
            Type = type;
            Connection = connection;
            Command = command;
        }

        public DbTracingType Type { get; private set; }

        public DbConnection Connection { get; private set; }
        public DbCommand Command { get; private set; }

        public object Result { get; private set; }

        public DateTime? StartTime { get; private set; }
        public DateTime? FinishTime { get; private set; }
        public DateTime? ReaderFinishTime { get; private set; }

        public TimeSpan? Duration { get { return FinishTime - StartTime; } }
        public TimeSpan? ReaderDuration { get { return ReaderFinishTime - FinishTime; } }
        public TimeSpan? TotalDuration { get { return ReaderFinishTime - StartTime; } }

        internal void OnStarted()
        {
            if (StartTime != null)
                throw new InvalidOperationException("Timing has already started");

            StartTime = DateTime.UtcNow;
        }

        internal void OnFinished(object result)
        {
            if (FinishTime != null)
                throw new InvalidOperationException("Timing has already finished");

            FinishTime = DateTime.UtcNow;
            Result = result;
        }

        internal void OnReaderFinished()
        {
            if (ReaderFinishTime != null)
                throw new InvalidOperationException("Reader timing has already finished");

            ReaderFinishTime = DateTime.UtcNow;
        }

        internal void OnFailed(Exception ex)
        {
            if (ReaderFinishTime == null)
                ReaderFinishTime = DateTime.UtcNow;

            Result = ex;
        }
    }

    public enum DbTracingType
    { 
        Reader,
        NonQuery,
        Scalar
    }
}
