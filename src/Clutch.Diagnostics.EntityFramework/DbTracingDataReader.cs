using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Clutch.Diagnostics.EntityFramework
{
    [DebuggerStepThrough]
    public class DbTracingDataReader : DbDataReader
    {
        public DbTracingDataReader(DbDataReader reader, DbTracingContext context)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (context == null)
                throw new ArgumentNullException("context");
            if (reader is DbTracingDataReader)
                throw new InvalidOperationException("Reader is already wrapped");

            this.reader = reader;
            this.context = context;
        }

        private DbDataReader reader;
        private DbTracingContext context;

        public DbDataReader UnderlyingReader
        {
            get
            {
                return reader;
            }
        }

        #region DbDataReader

        public override int Depth
        {
            get { return reader.Depth; }
        }

        public override int FieldCount
        {
            get { return reader.FieldCount; }
        }

        public override bool HasRows
        {
            get { return reader.HasRows; }
        }

        public override bool IsClosed
        {
            get { return reader.IsClosed; }
        }

        public override int RecordsAffected
        {
            get { return reader.RecordsAffected; }
        }

        public override object this[string name]
        {
            get { return reader[name]; }
        }

        public override object this[int ordinal]
        {
            get { return reader[ordinal]; }
        }

        public override void Close()
        {
            reader.Close();
            context.OnReaderFinished();

            DbTracing.FireReaderFinished(context);
        }

        public override bool GetBoolean(int ordinal)
        {
            return reader.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return reader.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return reader.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return reader.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return reader.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return reader.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return reader.GetDouble(ordinal);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.IEnumerable)reader).GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return reader.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return reader.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return reader.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return reader.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return reader.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return reader.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return reader.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return reader.GetOrdinal(name);
        }

        public override DataTable GetSchemaTable()
        {
            return reader.GetSchemaTable();
        }

        public override string GetString(int ordinal)
        {
            return reader.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return reader.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return reader.GetValues(values);
        }

        public override bool IsDBNull(int ordinal)
        {
            return reader.IsDBNull(ordinal);
        }

        public override bool NextResult()
        {
            return reader.NextResult();
        }

        public override bool Read()
        {
            return reader.Read();
        }

        #endregion
    }
}
