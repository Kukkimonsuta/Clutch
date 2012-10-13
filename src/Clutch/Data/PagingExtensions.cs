using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Data
{
	public static class PagingExtensions
	{
		/// <summary>
		/// Selects only part of a collection.
		/// </summary>
		public static CollectionPage<TEntity> ToCollectionPage<TEntity>(this IQueryable<TEntity> query, PagingOptions<TEntity> pagingOptions)
		{
			return pagingOptions.Execute(query);
		}

		/// <summary>
		/// Selects only part of a collection.
		/// </summary>
		public static CollectionPage<TEntity> ToCollectionPage<TEntity>(this IEnumerable<TEntity> resource, PagingOptions<TEntity> pageOptions)
		{
			return pageOptions.Execute(resource);
		}
	}
}
