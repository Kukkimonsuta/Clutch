using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics.EntityFramework.Tests.Objects
{
	public class CodeFirstContext : DbContext
	{
		public CodeFirstContext()
			: base()
		{
			this.Configuration.AutoDetectChangesEnabled = true;
			this.Configuration.LazyLoadingEnabled = false;
			this.Configuration.ProxyCreationEnabled = true;
			this.Configuration.ValidateOnSaveEnabled = true;
			
			TestEntity1 = Set<TestEntity1>();
			TestEntity2 = Set<TestEntity2>();
		}

		public DbSet<TestEntity1> TestEntity1 { get; private set; }
		public DbSet<TestEntity2> TestEntity2 { get; private set; }
	}

	public class TestEntity1
	{
		public int ID { get; set; }
	}

	public class TestEntity2
	{
		public int ID { get; set; }
	}
}
