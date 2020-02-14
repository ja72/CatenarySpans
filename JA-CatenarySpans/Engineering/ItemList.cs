using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace JA.Engineering
{
    public class ItemList<T> : IHasUnits
        where T : new()
    {
        ProjectUnits units;
        readonly BindingList<T> items;

        public event EventHandler<ItemChangeEventArgs> ItemChanged;
        public event EventHandler<ProjectUnits.SetEventArgs> ProjectUnitsSet;
        public event EventHandler<ProjectUnits.ChangeEventArgs> ProjectUnitsChanged;

        #region Factory

        public ItemList()
        {
            this.units=new ProjectUnits();
            this.items = new BindingList<T>
            {
                AllowNew = true,
                AllowRemove = true,
                AllowEdit = true
            };

            //Item Events
            this.items.AddingNew+=new AddingNewEventHandler(items_AddingNew);
            this.items.ListChanged+=new ListChangedEventHandler(items_ListChanged);
            //Units Events
            this.ProjectUnitsChanged+=new EventHandler<ProjectUnits.ChangeEventArgs>(BindableList_ProjectUnitsChanged);

        }
        public ItemList(ProjectUnits units, params T[] items) : this()
        {
            this.units=new ProjectUnits();
            this.AddArray(items);
        }

        public void AddArray(params T[] list)
        {
            items.RaiseListChangedEvents=false;
            for (int i=0; i<list.Length; i++)
            {
                items.Add(list[i]);
            }
            items.RaiseListChangedEvents=true;

            OnItemChanged(new ItemChangeEventArgs());
        }


        public virtual T NewItem()
        {
            return new T();
        }

        #endregion
        #region Properties
        public T this[int index]
        {
            get { return items[index]; }
            set
            {
                items[index]=value;
            }
        }

        [XmlAttribute(), Bindable(BindableSupport.No), Browsable(false)]
        public string UnitSymbols
        {
            get { return units.ToString(); }
            set { Units=ProjectUnits.Parse(value); }
        }

        [XmlIgnore(), TypeConverter(typeof(ProjectUnits.UnitsTypeConverter))]
        public ProjectUnits Units
        {
            get { return units; }
            set
            {
                if (units==null)
                {
                    // Project Units Set
                    this.units=value;
                    OnProjectUnitsSet(new ProjectUnits.SetEventArgs(value));
                }
                else
                {
                    // Project Units Changed
                    ProjectUnits old_units=units;
                    this.units=value;
                    OnProjectUnitsChange(new ProjectUnits.ChangeEventArgs(old_units, value));
                }
            }
        }

        [XmlArray("Items"), Browsable(true), DisplayName("Items")]
        public T[] ItemArray
        {
            get { return items.ToArray(); }
            set
            {
                items.Clear();
                foreach (var item in value)
                {
                    items.Add(item);
                }
            }
        }

        [XmlIgnore(), Browsable(false)]
        public BindingList<T> Items
        {
            get { return items; }
        }

        [XmlIgnore()]
        public T First { get { return items.Count>0?this[0]:default(T); } }
        [XmlIgnore()]
        public T Last { get { return items.Count>0?this[items.Count-1]:default(T); } }
        public bool HasItems { get { return items.Count>0; } }
        #endregion
        #region Internal Event Handlers
        void BindableList_ProjectUnitsChanged(object sender, ProjectUnits.ChangeEventArgs e)
        {
            OnItemChanged(new ItemChangeEventArgs());
        }

        void items_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType==ListChangedType.PropertyDescriptorAdded
                ||e.ListChangedType==ListChangedType.PropertyDescriptorDeleted)
            {
                return;
            }
            if (e.ListChangedType==ListChangedType.ItemDeleted
                ||e.ListChangedType==ListChangedType.ItemMoved)
            {
                OnItemChanged(new ItemChangeEventArgs());
            }
            else
            {
                OnItemChanged(new ItemChangeEventArgs(e.NewIndex));
            }
        }

        void items_AddingNew(object sender, AddingNewEventArgs e)
        {
            e.NewObject=NewItem();
        }

        #endregion
        #region Event Triggers

        protected void OnItemChanged(ItemChangeEventArgs e)
        {
            this.ItemChanged?.Invoke(this, e);
        }

        protected void OnProjectUnitsSet(ProjectUnits.SetEventArgs e)
        {
            this.ProjectUnitsSet?.Invoke(this, e);
        }
        protected void OnProjectUnitsChange(ProjectUnits.ChangeEventArgs e)
        {
            this.ProjectUnitsChanged?.Invoke(this, e);
        }
        #endregion
        public class ItemChangeEventArgs : EventArgs
        {
            readonly int index;

            public ItemChangeEventArgs() : this(-1) { }
            public ItemChangeEventArgs(int changed_item_index)
            {
                this.index=changed_item_index;
            }
            public bool AllItems { get { return index==-1; } }
            public int ChangedItemIndex { get { return index; } }
        }
    }

}
