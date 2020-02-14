using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace JA.Engineering
{    
    public sealed class RulingSpan : IHasUnits, IListSource, ICloneable
    {
        ProjectUnits units;
        readonly BindingList<Catenary> spans;

        public event EventHandler<ItemChangeEventArgs> RulingSpanChanged;
        public event EventHandler<ProjectUnits.SetEventArgs> ProjectUnitsSet;
        public event EventHandler<ProjectUnits.ChangeEventArgs> ProjectUnitsChanged;

        #region Factory
        public RulingSpan()
        {
            this.units=new ProjectUnits(ProjectUnitSystem.FeetPoundSecond);
            this.spans = new BindingList<Catenary>
            {
                AllowNew = true,
                AllowRemove = true,
                AllowEdit = true,
                RaiseListChangedEvents = true
            };
            //Span Events
            this.spans.AddingNew+=new AddingNewEventHandler(spans_AddingNew);
            this.spans.ListChanged+=new ListChangedEventHandler(spans_ListChanged);            
            
            //Ruling Span Events
            this.RulingSpanChanged+=new EventHandler<ItemChangeEventArgs>(RulingSpan_RulingSpanChanged);
            this.ProjectUnitsChanged+=new EventHandler<ProjectUnits.ChangeEventArgs>(RulingSpan_ProjectUnitsChanged);
        }
        public RulingSpan(RulingSpan other) : this()
        {
            this.units=other.units.Clone();
            spans.RaiseListChangedEvents=false;
            for (int i=0; i<other.spans.Count; i++)
            {
                spans.Add(other.spans[i].Clone());
            }
            spans.RaiseListChangedEvents=true;
            OnRulingSpanChange(new ItemChangeEventArgs());
        }
        public RulingSpan(ProjectUnits units, params Catenary[] list) : this()
        {
            //Set Units
            this.units=units;            
            this.AddArray(list);
        }
        public void AddArray(params Catenary[] list)
        {
            spans.RaiseListChangedEvents=false;
            for(int i=0; i<list.Length; i++)
            {
                spans.Add(list[i]);
            }
            spans.RaiseListChangedEvents=true;

            OnRulingSpanChange(new ItemChangeEventArgs());
        }

        public Catenary NewCatenary()
        {
            var last=Last;
            if(last==null)
            {
                return new Catenary();
            }
            return new Catenary(last.EndPosition, last.SpanX, 0, last.Weight)
            {
                HorizontalTension=last.HorizontalTension
            };
        }
        #endregion

        #region Methods

        public Catenary GetRulingSpanCatenary()
        {
            var rs= new Span(Vector2.Origin, RulingSpanLength ,0);
            return new Catenary(rs, spans[0].Weight, spans[0].HorizontalTension);
        }

        /// <summary>
        /// Ruling span length calculation for uneven spans
        /// </summary>
        public double RulingSpanLength
        {
            get
            {
                var rs1=0.0;
                var rs2=0.0;
                for (int i=0; i<spans.Count; i++)
                {
                    double δx=spans[i].SpanX;
                    double δy=spans[i].SpanY;
                    rs1+=Math.Pow(δx, 3);
                    double slope=δy/δx;
                    rs2+=δx*Math.Pow(1+slope*slope, 1.5);
                }
                return Math.Round( Math.Sqrt(rs1/rs2), 1);
            }
        }

        public void UpdateAllFromFrom(int span_index)
        {
            if (span_index>=0&&span_index<=spans.Count)
            {
                HorizontalTension=spans[span_index].HorizontalTension;
            }
        }

        public void UpdateAllCatenary()
        {
            for(int i=0; i<spans.Count; i++)
            {
                this[i].CalculateCenter();
            }
        }
        public void UpdateSpanEnds()
        {
            for(int i=1; i<spans.Count; i++)
            {
                this[i].StartPosition=this[i-1].EndPosition;
            }            
        }
        public void SetHorizontalTension(double H)
        {
            for(int i=0; i<spans.Count; i++)
            {
                this[i].HorizontalTension=H;
            }
            UpdateSpanEnds();
        }
        public void SetHorizontalTensionFrom(int span_index)
        {
            double H=this[span_index].HorizontalTension;
            for(int i=0; i<spans.Count; i++)
            {
                if(i!=span_index)
                {
                    this[i].HorizontalTension=H;
                }
            }
            UpdateSpanEnds();
        }
        public Catenary FindCatenaryFromX(double x)
        {
            foreach(var item in spans)
            {
                if(item.ContainsX(x))
                {
                    return item;
                }
            }
            return null;
        }

        public double ClearanceTo(Vector2 point, bool directional)
        {
            Catenary catenary=FindCatenaryFromX(point.x);
            if(catenary!=null)
            {
                double dy=catenary.CatenaryFunction(point.x)-point.y;
                return directional?dy:Math.Abs(dy);
            }
            return 0;
        }

        #endregion

        #region Properties
        public Catenary this[int index]
        {
            get { return spans[index]; }
            set
            {
                spans[index]=value;
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
                if(units==null)
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

        [XmlArray("Spans"), Browsable(true), DisplayName("Spans")]
        public Catenary[] SpanArray
        {
            get { return spans.ToArray(); }
            set
            {
                spans.Clear();
                foreach (var item in value)
                {
                    spans.Add(item);
                }
            }
        }

        [XmlIgnore(), Browsable(false)]
        public BindingList<Catenary> Spans
        {
            get { return spans; }
        }

        [XmlIgnore()]
        public Catenary First { get { return spans.Count>0?this[0]:null; } }
        [XmlIgnore()]
        public Catenary Last { get { return spans.Count>0?this[spans.Count-1]:null; } }

        [TypeConverter(typeof(System.ComponentModel.DoubleConverter))]        
        [XmlIgnore(), Bindable(BindableSupport.Yes), DisplayName("TotalLength")]
        public string TotalLengthDisplay
        {
            get { return string.Format("{0:0.###}", TotalLength); }
        }
        [XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public double TotalLength
        {
            get { return spans.Sum((cat) => cat.TotalLength); }
        }
        [XmlIgnore(), Bindable(BindableSupport.Yes), DisplayName("HorizontalTension")]
        public string HorizontalTensionDisplay
        {
            get { return string.Format("{0:0.###} {1}", HorizontalTension, units.Force); }
            set
            {
                string[] parts = value.Split(' ');
                Unit unit=units.ForceUnit;
                if(parts.Length>1)
                {
                    unit=parts[1];
                }
                if(!units.ForceUnit.IsCompatible(unit))
                {
                    return;
                }
                if(double.TryParse(parts[0], out double x))
                {
                    HorizontalTension=unit.FactorTo(units.ForceUnit)*x;
                }
            }
        }
        [XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public double HorizontalTension
        {
            get { return HasSpans?spans[0].HorizontalTension:0; }
            set
            {
                if(HasSpans)
                { 
                    spans[0].HorizontalTension=value;
                    OnRulingSpanChange(new ItemChangeEventArgs(0));
                }
            }
        }
        [XmlIgnore(), Bindable(BindableSupport.No)]
        public bool HasSpans { get { return spans.Count>0; } }
        #endregion

        #region Span Event Handlers
        void RulingSpan_RulingSpanChanged(object sender, ItemChangeEventArgs e)
        {

            if(!e.AllItems)
            {
                SetHorizontalTensionFrom(e.ChangedItemIndex);
            }
            else
            {
                UpdateAllCatenary();
                UpdateSpanEnds();
            }
        }

        void spans_ListChanged(object sender, ListChangedEventArgs e)
        {
            if(e.ListChangedType==ListChangedType.PropertyDescriptorAdded
                ||e.ListChangedType==ListChangedType.PropertyDescriptorDeleted)
            {
                return;
            }
            if(e.ListChangedType==ListChangedType.ItemDeleted
                ||e.ListChangedType==ListChangedType.ItemMoved)
            {
                OnRulingSpanChange(new ItemChangeEventArgs());
            }
            else
            {
                OnRulingSpanChange(new ItemChangeEventArgs(e.NewIndex));
            }
        }

        void spans_AddingNew(object sender, AddingNewEventArgs e)
        {
            e.NewObject=NewCatenary();
        }

        #endregion

        #region Unit Event Handlers
        void RulingSpan_ProjectUnitsChanged(object sender, ProjectUnits.ChangeEventArgs e)
        {
            for(int i=0; i<spans.Count; i++)
            {
                this[i].RaisesChangedEvents=false;
                this[i].StartPosition*=e.LengthFactor;
                this[i].Step*=e.LengthFactor;
                this[i].HorizontalTension*=e.ForceFactor;
                this[i].Weight*=e.ForceFactor/e.LengthFactor;
                this[i].RaisesChangedEvents=true;
            }
            OnRulingSpanChange(new ItemChangeEventArgs());
        }

        #endregion

        #region Event Triggers

        void OnRulingSpanChange(ItemChangeEventArgs e)
        {
            this.RulingSpanChanged?.Invoke(this, e);
        }

        void OnProjectUnitsSet(ProjectUnits.SetEventArgs e)
        {
            this.ProjectUnitsSet?.Invoke(this, e);
        }
        void OnProjectUnitsChange(ProjectUnits.ChangeEventArgs e)
        {
            this.ProjectUnitsChanged?.Invoke(this, e);
        }
        #endregion

        #region File I/O
        public static RulingSpan OpenFile(string path)
        {
            XmlSerializer xs=new XmlSerializer(typeof(RulingSpan));
            var fs=System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            var rs=xs.Deserialize(fs) as RulingSpan;
            fs.Close();
            return rs;
        }

        public void SaveFile(string path)
        {
            XmlSerializer xs=new XmlSerializer(typeof(RulingSpan));
            var fs=System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            xs.Serialize(fs, this);
            fs.Close();
        }
        
        #endregion

        #region Event Args
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
        #endregion

        #region IListSource Members

        bool IListSource.ContainsListCollection
        {
            get { return false; }
        }

        System.Collections.IList IListSource.GetList()
        {
            return spans;
        }

        #endregion

        #region ICloneable Members

        public RulingSpan Clone() { return new RulingSpan(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

    }

}
