// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.SortableBindingListComparer`1
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SDRSharp.Tetra
{
  public class SortableBindingListComparer<T> : IComparer<T>
  {
    private PropertyInfo _sortProperty;
    private ListSortDirection _sortDirection;

    public SortableBindingListComparer(string sortProperty, ListSortDirection sortDirection)
    {
      this._sortProperty = typeof (T).GetProperty(sortProperty);
      this._sortDirection = sortDirection;
    }

    public int Compare(T x, T y)
    {
      IComparable comparable1 = (IComparable) this._sortProperty.GetValue((object) x, (object[]) null);
      IComparable comparable2 = (IComparable) this._sortProperty.GetValue((object) y, (object[]) null);
      return this._sortDirection == ListSortDirection.Ascending ? comparable1.CompareTo((object) comparable2) : comparable2.CompareTo((object) comparable1);
    }
  }
}
