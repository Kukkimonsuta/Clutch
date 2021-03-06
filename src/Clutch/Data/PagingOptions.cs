﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Clutch.Data
{
	/// <summary>
	/// Holds information about requested paging
	/// </summary>
	public sealed class PagingOptions
	{
		private PagingOptions(int pageIndex, int pageSize, bool totalNeeded = true, int offset = 0, int? maxPageCount = null)
		{
			if (pageIndex < 0)
				throw new ArgumentOutOfRangeException("pageIndex", "PageIndex must be more or equal to zero");
			if (pageSize <= 0)
				throw new ArgumentOutOfRangeException("pageSize", "PageSize must be more than zero");
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", "Offset must be more or than zero");
			if (maxPageCount != null && maxPageCount <= 0)
				throw new ArgumentOutOfRangeException("pageSize", "MaxPageCount must be more than zero or null");

			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalNeeded = totalNeeded;
			Offset = offset;
			MaxPageCount = maxPageCount;
		}

		private int pageIndex;
		private int pageSize;
		private int offset;
		private int? maxPageCount;

		public int PageIndex
		{
			get { return pageIndex; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "PageIndex must be more or equal to zero");

				pageIndex = value;
			}
		}
		public int PageSize
		{
			get { return pageSize; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value", "PageSize must be more than zero");

				pageSize = value;
			}
		}
		public bool TotalNeeded { get; set; }
		public int Offset
		{
			get { return offset; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "Offset must be more or than zero");

				offset = value;
			}
		}
		public int? MaxPageCount
		{
			get { return maxPageCount; }
			set
			{
				if (value != null && value <= 0)
					throw new ArgumentOutOfRangeException("value", "MaxPageCount must be more than zero or null");

				maxPageCount = value;
			}
		}

		public int ToSkip
		{
			get { return this.PageIndex * this.PageSize - this.Offset; }
		}

		public int OffsetOnPage
		{
			get { return Math.Max(0, this.Offset - this.PageIndex * this.PageSize); }
		}

		internal CollectionPage<TEntity> Execute<TEntity>(IQueryable<TEntity> query)
		{
			var result = query;

			if (this.ToSkip > 0)
				result = result.Skip(this.ToSkip);

			var data = this.OffsetOnPage >= this.PageSize ? new TEntity[0] : result.Take(this.PageSize - this.OffsetOnPage).ToArray();

			var total = -1;
			if (this.TotalNeeded)
			{
				if (this.PageIndex == 0 && data.Length + this.OffsetOnPage < this.PageSize)
					total = data.Length + this.OffsetOnPage;
				else if (this.MaxPageCount != null)
					total = (query.Take(this.MaxPageCount.Value * this.PageSize - this.Offset)).Count();
				else
					total = query.Count();
			}

			return new CollectionPage<TEntity>(
				data,
				this.PageIndex,
				this.PageSize,
				total,
				this.Offset,
				this.MaxPageCount
			);
		}
		internal CollectionPage<TEntity> Execute<TEntity>(IEnumerable<TEntity> resource)
		{
			var result = resource;

			if (ToSkip > 0)
				result = result.Skip(ToSkip);

			var data = this.OffsetOnPage >= this.PageSize ? new TEntity[0] : result.Take(this.PageSize - this.OffsetOnPage).ToArray();

			var total = -1;
			if (this.TotalNeeded)
			{
				if (this.PageIndex == 0 && data.Length + this.OffsetOnPage < this.PageSize)
					total = data.Length + this.OffsetOnPage;
				else if (this.MaxPageCount != null)
					total = (resource.Take(this.MaxPageCount.Value * this.PageSize - this.Offset)).Count();
				else
					total = resource.Count();
			}

			return new CollectionPage<TEntity>(
				data,
				this.PageIndex,
				this.PageSize,
				total,
				this.Offset,
				maxPageCount: this.MaxPageCount
			);
		}

		#region Static members

		public static PagingOptions Create(int pageIndex, int pageSize, bool totalNeeded = true, int offset = 0, int? maxPageCount = null)
		{
			return new PagingOptions(pageIndex, pageSize, totalNeeded: totalNeeded, offset: offset, maxPageCount: maxPageCount);
		}

		#endregion
	}
}
