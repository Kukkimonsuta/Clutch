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

        private Mock<IDbTracingListener> MockListener(bool fail = false, bool reader = false)
        {
            var mock = new Mock<IDbTracingListener>();

            mock
                .Setup(l => l.CommandExecuting(It.IsAny<DbTracingContext>()))
                .Callback((DbTracingContext context) =>
                {
                    Assert.NotNull(context);
                    Assert.NotNull(context.Command);
                })
                .Verifiable();
            mock
                .Setup(l => l.CommandFinished(It.IsAny<DbTracingContext>()))
                .Callback((DbTracingContext context) =>
                {
                    Assert.NotNull(context);
                    Assert.NotNull(context.Command);
                    Assert.NotNull(context.Duration);
                })
                .Verifiable();
            if (fail)
            {
                mock
                    .Verify(l => l.CommandFailed(It.IsAny<DbTracingContext>()));
            }
            if (reader)
            {
                mock
                    .Setup(l => l.ReaderFinished(It.IsAny<DbTracingContext>()))
                    .Callback((DbTracingContext context) =>
                    {
                        Assert.NotNull(context);
                        Assert.NotNull(context.Command);
                        Assert.NotNull(context.ReaderDuration);
                    })
                    .Verifiable();
            }
            mock
                .Setup(l => l.CommandExecuted(It.IsAny<DbTracingContext>()))
                .Callback((DbTracingContext context) =>
                {
                    Assert.NotNull(context);
                    Assert.NotNull(context.Command);
                })
                .Verifiable();

            return mock;
        }

        [Fact]
        public void Can_catch_entity_framework_command()
        {
            var mock = MockListener(reader: true);

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
