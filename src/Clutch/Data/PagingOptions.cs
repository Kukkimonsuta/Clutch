using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Clutch.Data
{
	/// <summary>
	/// Holds information about requested paging
	/// </summary>
	public sealed class PagingOptions<TEntity>
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

		internal CollectionPage<TEntity> Execute(IQueryable<TEntity> query)
		{
			var result = query;

			var actualOffset = Math.Max(0, this.Offset - this.PageIndex * this.PageSize);

			var toSkip = this.PageIndex * this.PageSize - this.Offset;
			if (toSkip > 0)
				result = result.Skip(toSkip);

			var data = actualOffset >= this.PageSize ? new TEntity[0] : result.Take(this.PageSize - actualOffset).ToArray();

			var total = -1;
			if (this.TotalNeeded)
			{
				if (this.PageIndex == 0 && data.Length + actualOffset < this.PageSize)
					total = data.Length + actualOffset;
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
		internal CollectionPage<TEntity> Execute(IEnumerable<TEntity> resource)
		{
			var result = resource;

			var actualOffset = Math.Max(0, this.Offset - this.PageIndex * this.PageSize);

			var toSkip = this.PageIndex * this.PageSize - this.Offset;
			if (toSkip > 0)
				result = result.Skip(toSkip);

			var data = actualOffset >= this.PageSize ? new TEntity[0] : result.Take(this.PageSize - actualOffset).ToArray();

			var total = -1;
			if (this.TotalNeeded)
			{
				if (this.PageIndex == 0 && data.Length + actualOffset < this.PageSize)
					total = data.Length + actualOffset;
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

		public static PagingOptions<TEntity> Create(int pageIndex, int pageSize, bool totalNeeded = true, int offset = 0, int? maxPageCount = null)
		{
			return new PagingOptions<TEntity>(pageIndex, pageSize, totalNeeded, offset);
		}

		#endregion
	}
}
