using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace JA.Engineering
{    
    public sealed class RulingSpan : HasUnitsBase, IContainsMeasures,  IListSource, ICloneable
    {
        public event EventHandler<ItemChangeEventArgs> RulingSpanChanged;

        #region Factory
        public RulingSpan() : this(new ProjectUnits(ProjectUnitSystem.FeetPoundSecond)) { }
        public RulingSpan(ProjectUnits units) : base(units)
        {
            this.Spans = new BindingList<Catenary>
            {
                AllowNew = true,
                AllowRemove = true,
                AllowEdit = true,
                RaiseListChangedEvents = true
            };
            //Span Events
            this.Spans.AddingNew+=new AddingNewEventHandler(spans_AddingNew);
            this.Spans.ListChanged+=new ListChangedEventHandler(spans_ListChanged);            
            
            //Ruling Span Events
            this.RulingSpanChanged+=new EventHandler<ItemChangeEventArgs>(RulingSpan_RulingSpanChanged);
        }
        public RulingSpan(RulingSpan other) : this(other.Units.Clone())
        {
            RaiseListChangedEvents=false;
            for (int i=0; i<other.Spans.Count; i++)
            {
                Spans.Add(other.Spans[i].Clone());
            }
            RaiseListChangedEvents=true;
            OnRulingSpanChange(new ItemChangeEventArgs());
        }
        public RulingSpan(ProjectUnits units, params Catenary[] list) : this(units)
        {
            //Set Units
            this.AddArray(list);
        }
        public RulingSpan(ProjectUnits units, params Span[] list) : this(units)
        {
            AddArray(list);
        }
        public void AddArray(params Span[] list)
        {
            var wt = Catenary.DefaultWeight;
            var H = Catenary.DefaultHorizontalTension;
            Catenary last = null;
            if (Spans.Count>0)
            {
                last = Spans.Last();
                wt = last.Weight;
                H = last.HorizontalTension;
            }
            RaiseListChangedEvents=false;
            for(int i=0; i<list.Length; i++)
            {
                var cat = new Catenary(list[i], wt, H);
                if (last!=null)
                {
                    cat.StartPosition = last.EndPosition;
                }
                Spans.Add(cat);
                last = cat;
            }
            RaiseListChangedEvents=true;

            OnRulingSpanChange(new ItemChangeEventArgs());
        }
        public void AddArray(params Catenary[] list)
        {
            RaiseListChangedEvents=false;
            for (int i = 0; i<list.Length; i++)
            {
                Spans.Add(list[i]);
            }
            RaiseListChangedEvents=true;

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
            return new Catenary(rs, Spans[0].Weight, Spans[0].HorizontalTension);
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
                for (int i=0; i<Spans.Count; i++)
                {
                    double δx=Spans[i].SpanX;
                    double δy=Spans[i].SpanY;
                    rs1+=Math.Pow(δx, 3);
                    double slope=δy/δx;
                    rs2+=δx*Math.Pow(1+slope*slope, 1.5);
                }
                return Math.Round( Math.Sqrt(rs1/rs2), 1);
            }
        }

        public void UpdateAllFromFrom(int span_index)
        {
            if (span_index>=0&&span_index<=Spans.Count)
            {
                HorizontalTension=Spans[span_index].HorizontalTension;
            }
        }

        public void UpdateAllCatenary()
        {
            for(int i=0; i<Spans.Count; i++)
            {
                Spans[i].CalculateCenter();
            }
        }
        public void UpdateSpanEnds()
        {
            for(int i=1; i<Spans.Count; i++)
            {
                Spans[i].StartPosition=this[i-1].EndPosition;
            }            
        }
        public void SetCableWeight(double wt)
        {
            for (int i = 0; i<Spans.Count; i++)
            {
                Spans[i].Weight = wt;
            }
        }
        public void SetHorizontalTension(double H)
        {
            for(int i=0; i<Spans.Count; i++)
            {
                Spans[i].HorizontalTension=H;
            }
            UpdateSpanEnds();
        }
        public void SetHorizontalTensionFrom(int span_index)
        {
            double H=Spans[span_index].HorizontalTension;
            for(int i=0; i<Spans.Count; i++)
            {
                if(i!=span_index)
                {
                    Spans[i].HorizontalTension=H;
                }
            }
            UpdateSpanEnds();
        }
        public Catenary FindCatenaryFromX(double x)
        {
            foreach(var item in Spans)
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
            Catenary catenary=FindCatenaryFromX(point.X);
            if(catenary!=null)
            {
                double dy=catenary.CatenaryFunction(point.X)-point.Y;
                return directional?dy:Math.Abs(dy);
            }
            return 0;
        }
        protected override void ConvertUnits(ProjectUnits target)
        {
            ScaleForUnits(new ProjectUnits.ChangeEventArgs(Units, target), false);
        }
        public void ScaleForUnits(ProjectUnits.ChangeEventArgs g, bool raiseEvents = false)
        {
            var raise = RaiseListChangedEvents;
            this.RaiseListChangedEvents = raiseEvents;
            for (int i = 0; i < Spans.Count; i++)
            {
                Spans[i].ScaleForUnits(g, raise);
            }
            this.HorizontalTension *= g.ForceFactor;
            this.RaiseListChangedEvents = raise;
        }
        void IContainsMeasures.ScaleForUnits(ProjectUnits.ChangeEventArgs g) => ScaleForUnits(g);

        #endregion

        #region Properties
        [XmlIgnore(), Bindable(BindableSupport.No)]
        public bool RaiseListChangedEvents { 
            get => Spans.RaiseListChangedEvents; 
            set => Spans.RaiseListChangedEvents = value; }

        public Catenary this[int index]
        {
            get { return Spans[index]; }
            set
            {
                Spans[index]=value;
            }
        }

        [XmlArray("Spans"), Browsable(true), DisplayName("Spans")]
        public Catenary[] SpanArray
        {
            get { return Spans.ToArray(); }
            set
            {
                Spans.Clear();
                foreach (var item in value)
                {
                    Spans.Add(item);
                }
            }
        }

        [XmlIgnore(), Browsable(false)]
        public BindingList<Catenary> Spans { get; }

        [XmlIgnore()]
        public Catenary First { get { return Spans.Count>0?this[0]:null; } }
        [XmlIgnore()]
        public Catenary Last { get { return Spans.Count>0?this[Spans.Count-1]:null; } }

        [TypeConverter(typeof(System.ComponentModel.DoubleConverter))]        
        [XmlIgnore(), Bindable(BindableSupport.Yes), DisplayName("TotalLength")]
        public string TotalLengthDisplay
        {
            get { return string.Format("{0:0.###}", TotalLength); }
        }
        [XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public double TotalLength
        {
            get { return Spans.Sum((cat) => cat.TotalLength); }
        }
        [XmlIgnore(), Bindable(BindableSupport.Yes), DisplayName("HorizontalTension")]
        public string HorizontalTensionDisplay
        {
            get { return string.Format("{0:0.###} {1}", HorizontalTension, Units.Force); }
            set
            {
                string[] parts = value.Split(' ');
                Unit unit=Units.ForceUnit;
                if(parts.Length>1)
                {
                    unit=parts[1];
                }
                if(!Units.ForceUnit.IsCompatible(unit))
                {
                    return;
                }
                if(double.TryParse(parts[0], out double x))
                {
                    HorizontalTension=unit.FactorTo(Units.ForceUnit)*x;
                }
            }
        }
        [XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public double HorizontalTension
        {
            get { return HasSpans?Spans[0].HorizontalTension:0; }
            set
            {
                if(HasSpans)
                { 
                    Spans[0].HorizontalTension=value;
                    OnRulingSpanChange(new ItemChangeEventArgs(0));
                }
            }
        }
        [XmlIgnore(), Bindable(BindableSupport.No)]
        public bool HasSpans { get { return Spans.Count>0; } }
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

        #region Event Triggers

        void OnRulingSpanChange(ItemChangeEventArgs e)
        {
            this.RulingSpanChanged?.Invoke(this, e);
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
            return Spans;
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
