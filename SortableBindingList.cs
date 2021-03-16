// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.SortableBindingList`1
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace SDRSharp.Tetra
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private PropertyDescriptor _sortProperty;
        private ListSortDirection _sortDirection;

        protected override bool SupportsSortingCore => true;

        protected override ListSortDirection SortDirectionCore => this._sortDirection;

        protected override PropertyDescriptor SortPropertyCore => this._sortProperty;

        protected override bool IsSortedCore => this._isSorted;

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> items = (List<T>)this.Items;
            if (items != null)
            {
                SortableBindingListComparer<T> bindingListComparer = new SortableBindingListComparer<T>(property.Name, direction);
                items.Sort((IComparer<T>)bindingListComparer);
                this._isSorted = true;
            }
            else
                this._isSorted = false;
            this._sortProperty = property;
            this._sortDirection = direction;
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }
}
