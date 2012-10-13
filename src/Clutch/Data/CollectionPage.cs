using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Data
{
	/// <summary>
	/// Holds information about a part of collection.
	/// </summary>
	public abstract class CollectionPage
	{
		internal CollectionPage(int pageIndex, int pageSize, int totalSize, int offset, int? maxPageCount)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalSize = totalSize;
			Offset = offset;
			MaxPageCount = maxPageCount;
		}

		public int Offset { get; protected set; }
		public int TotalSize { get; protected set; }
		public int PageIndex { get; protected set; }
		public int PageSize { get; protected set; }
		public int? MaxPageCount { get; protected set; }

		public int PageCount
		{
			get { return Math.Min(TotalSize != -1 ? (int)Math.Ceiling((TotalSize + Offset) / (double)PageSize) : -1, MaxPageCount ?? int.MaxValue); }
		}

		public bool HasPrevious
		{
			get { return PageIndex > 0; }
		}

		public bool HasNext
		{
			get { return PageIndex + 1 < PageCount; }
		}

		public bool HasFirst
		{
			get { return PageIndex > 0; }
		}

		public bool HasLast
		{
			get { return (PageIndex + 1 < PageCount); }
		}

		#region Static members

		public static CollectionPage<T> Empty<T>()
		{
			return new CollectionPage<T>(Enumerable.Empty<T>(), 0, 0, 0, 0, null);
		}

		#endregion
	}

	/// <summary>
	/// Holds information about a part of collection.
	/// </summary>
	public class CollectionPage<T> : CollectionPage, IEnumerable<T>
	{
		internal CollectionPage(IEnumerable<T> data, int pageIndex, int pageSize, int totalSize, int offset, int? maxPageCount)
			: base(pageIndex, pageSize, totalSize, offset, maxPageCount)
		{
			this.data = data;
		}

		private IEnumerable<T> data;

		#region IEnumerable

		public IEnumerator<T> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		#endregion
	}
}
