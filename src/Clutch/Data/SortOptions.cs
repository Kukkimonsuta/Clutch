using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Clutch.Data
{
	/// <summary>
	/// Holds information about sorting
	/// </summary>
	public abstract class SortOptions<TEntity>
	{
		internal SortOptions()
		{ }

		internal abstract IOrderedQueryable<TEntity> Execute(IQueryable<TEntity> query);
		internal abstract IOrderedEnumerable<TEntity> Execute(IEnumerable<TEntity> query);

		public SortOptions<TEntity> ThenBy<TKey>(Expression<Func<TEntity, TKey>> expression, bool descending = false)
		{
			return new SortOptions<TEntity, TKey>(this, expression, descending);
		}

		#region Static members

		public static SortOptions<TEntity> Create<TKey>(Expression<Func<TEntity, TKey>> expression, bool descending = false)
		{
			return new SortOptions<TEntity, TKey>(expression, descending);
		}

		#endregion
	}

	/// <summary>
	/// Holds information about sorting
	/// </summary>
	internal sealed class SortOptions<TEntity, TKey> : SortOptions<TEntity>
	{
		internal SortOptions(Expression<Func<TEntity, TKey>> expression, bool descending = false)
		{
			this.expression = expression;
			this.descending = descending;
		}
		internal SortOptions(SortOptions<TEntity> parent, Expression<Func<TEntity, TKey>> expression, bool descending = false)
		{
			this.parent = parent;
			this.expression = expression;
			this.descending = descending;
		}

		private SortOptions<TEntity> parent;
		private Expression<Func<TEntity, TKey>> expression;
		private bool descending;

		internal override IOrderedQueryable<TEntity> Execute(IQueryable<TEntity> query)
		{
			if (parent != null)
			{
				var result = parent.Execute(query);

				return descending ? result.ThenByDescending(expression) : result.ThenBy(expression);
			}
			else
				return descending ? query.OrderByDescending(expression) : query.OrderBy(expression);
		}

		internal override IOrderedEnumerable<TEntity> Execute(IEnumerable<TEntity> query)
		{
			var compiled = expression.Compile();

			if (parent != null)
			{
				var result = parent.Execute(query);

				return descending ? result.ThenByDescending(compiled) : result.ThenBy(compiled);
			}
			else
				return descending ? query.OrderByDescending(compiled) : query.OrderBy(compiled);
		}
	}
}
