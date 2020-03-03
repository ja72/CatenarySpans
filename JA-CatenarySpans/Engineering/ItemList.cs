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

        public event EventHandler<ItemChangeEventArgs> ItemChanged;
        public event EventHandler<ProjectUnits.SetEventArgs> ProjectUnitsSet;
        public event EventHandler<ProjectUnits.ChangeEventArgs> ProjectUnitsChanged;

        #region Factory

        public ItemList()
        {
            this.units=new ProjectUnits();
            this.Items = new BindingList<T>
            {
                AllowNew = true,
                AllowRemove = true,
                AllowEdit = true
            };

            //Item Events
            this.Items.AddingNew+=new AddingNewEventHandler(items_AddingNew);
            this.Items.ListChanged+=new ListChangedEventHandler(items_ListChanged);
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
            Items.RaiseListChangedEvents=false;
            for (int i=0; i<list.Length; i++)
            {
                Items.Add(list[i]);
            }
            Items.RaiseListChangedEvents=true;

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
            get { return Items[index]; }
            set
            {
                Items[index]=value;
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
            get { return Items.ToArray(); }
            set
            {
                Items.Clear();
                foreach (var item in value)
                {
                    Items.Add(item);
                }
            }
        }

        [XmlIgnore(), Browsable(false)]
        public BindingList<T> Items { get; }

        [XmlIgnore()]
        public T First
        {
            get { return Items.Count>0 
                    ? this[0] 
                    : default; }
        }
        [XmlIgnore()]
        public T Last
        {
            get { return Items.Count>0 
                    ? this[Items.Count-1] 
                    : default; }
        }
        public bool HasItems { get { return Items.Count>0; } }
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
