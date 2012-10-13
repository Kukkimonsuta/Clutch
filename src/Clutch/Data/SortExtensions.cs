using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Data
{
	public static class SortExtensions
	{
		/// <summary>
		/// Sorts collection using given sort options
		/// </summary>
		public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, SortOptions<TEntity> sortOptions)
		{
			return sortOptions.Execute(query);
		}

		/// <summary>
		/// Sorts collection using given sort options
		/// </summary>
		public static IEnumerable<TEntity> OrderBy<TEntity>(this IEnumerable<TEntity> query, SortOptions<TEntity> sortOptions)
		{
			return sortOptions.Execute(query);
		}
	}
}
