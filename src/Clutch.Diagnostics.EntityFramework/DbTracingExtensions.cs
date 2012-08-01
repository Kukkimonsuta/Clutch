using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Clutch.Diagnostics.EntityFramework
{
	public static class DbTracingExtensions
	{
		public static string ToTraceString(this DbCommand command)
		{
			var builder = new StringBuilder();
			var parameters = command.Parameters.Cast<SqlParameter>().ToArray();

			foreach (var parameter in parameters)
			{
				var parameterName = parameter.ParameterName.StartsWith("@") ? parameter.ParameterName : "@" + parameter.ParameterName;
				
				builder.AppendFormat("declare {0} {1} = cast(N'{2}' as {1});", parameterName, parameter.SqlDbType, parameter.SqlValue);
				builder.AppendLine();
			}
			if (parameters.Any())
				builder.AppendLine();

			builder.AppendLine(command.CommandText);

			return builder.ToString();
		}
	}
}
