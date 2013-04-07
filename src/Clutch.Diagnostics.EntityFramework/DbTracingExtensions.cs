using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Clutch.Diagnostics.EntityFramework
{
    public static class DbTracingExtensions
    {
        private static string FormatValue(DbType type, object value)
        {
            if (value == null)
                return "null";

            return string.Format(CultureInfo.InvariantCulture, "cast(N'{1}' as {0})", type, value);
        }

        public static string ToTraceString(this DbCommand command)
        {
            var builder = new StringBuilder();
            var parameters = command.Parameters.Cast<DbParameter>().ToArray();

            foreach (var parameter in parameters)
            {
                var parameterName = parameter.ParameterName.StartsWith("@") ? parameter.ParameterName : "@" + parameter.ParameterName;

                builder.AppendFormat(CultureInfo.InvariantCulture, "declare {0} {1} = {2};", parameterName, parameter.DbType, FormatValue(parameter.DbType, parameter.Value));
                builder.AppendLine();
            }
            if (parameters.Any())
                builder.AppendLine();

            builder.AppendLine(command.CommandText);

            return builder.ToString();
        }
    }
}
