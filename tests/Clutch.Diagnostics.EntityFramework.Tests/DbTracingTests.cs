using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using System.Data.Common;
using System.Data.SqlClient;
using Clutch.Diagnostics.EntityFramework.Tests.Objects;
using Clutch.Runtime;

namespace Clutch.Diagnostics.EntityFramework.Tests
{
	public class DbTracingTests
	{
		static DbTracingTests()
		{
			Bootstrap.Startup();
			DbTracing.Enable();
		}

		private Mock<IDbTracingListener> MockListener()
		{
			var mock = new Mock<IDbTracingListener>();

			mock
				.Setup(l => l.CommandExecuting(It.IsAny<DbConnection>(), It.IsAny<DbCommand>()))
				.Callback((DbConnection connection, DbCommand command) =>
				{
					Assert.NotNull(connection);
					Assert.NotNull(command);
				})
				.Verifiable();
			mock
				.Setup(l => l.CommandFinished(It.IsAny<DbConnection>(), It.IsAny<DbCommand>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
				.Callback((DbConnection connection, DbCommand command, object result, TimeSpan duration) =>
				{
					Assert.NotNull(connection);
					Assert.NotNull(command);
				})
				.Verifiable();
			mock
				.Verify(l => l.CommandFailed(It.IsAny<DbConnection>(), It.IsAny<DbCommand>(), It.IsAny<Exception>(), It.IsAny<TimeSpan>()), Times.Never());
			mock
				.Setup(l => l.CommandExecuted(It.IsAny<DbConnection>(), It.IsAny<DbCommand>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
				.Callback((DbConnection connection, DbCommand command, object result, TimeSpan duration) =>
				{
					Assert.NotNull(connection);
					Assert.NotNull(command);
				})
				.Verifiable();

			return mock;
		}

		[Fact]
		public void Can_catch_entity_framework_command()
		{
			var mock = MockListener();

			DbTracing.AddListener(mock.Object);
			try
			{
				using (var context = new CodeFirstContext())
				{
					context.TestEntity1.Add(new TestEntity1());
					context.TestEntity2.Add(new TestEntity2());

					context.SaveChanges();
				}
			}
			finally
			{
				DbTracing.RemoveListener(mock.Object);
			}

			mock.Verify();
		}

		[Fact]
		public void Can_catch_execute_sql_command()
		{
			var mock = MockListener();

			DbTracing.AddListener(mock.Object);
			try
			{
				using (var context = new CodeFirstContext())
				{
					context.Database.ExecuteSqlCommand("select 1; select @date", new SqlParameter("date", DateTime.Now));

					context.SaveChanges();
				}
			}
			finally
			{
				DbTracing.RemoveListener(mock.Object);
			}

			mock.Verify();
		}
	}
}
