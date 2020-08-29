using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Globalization;

namespace JA.Engineering
{
    public sealed class RulingSpan : HasUnitsBase, IContainsMeasures, IListSource, ICloneable, IEquatable<RulingSpan>, IFormattable
    {
        public event EventHandler<ItemChangeEventArgs> RulingSpanChanged;

        #region Factory
        public RulingSpan() : this(ProjectUnits.Default()) { }
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
        public RulingSpan(RulingSpan other)
            : this(other.Units.Clone())
        {
            RaiseListChangedEvents=false;
            for (int i = 0; i<other.Spans.Count; i++)
            {
                Spans.Add(other.Spans[i].Clone());
            }
            RaiseListChangedEvents=true;
            OnRulingSpanChange(new ItemChangeEventArgs());
        }
        public RulingSpan(params Catenary[] list) : this(ProjectUnits.Default(), list) { }
        public RulingSpan(ProjectUnits units, params Catenary[] list) : this(units)
        {
            //Set Units
            this.AddSpans(list);
        }
        public RulingSpan(Span[] list, double weight, double H) : this(ProjectUnits.Default(), list, weight, H) { }
        public RulingSpan(ProjectUnits units, Span[] list, double weight, double H) : this(units)
        {
            AddSpans(list, weight, H);
        }
        public void AddSpans(Span[] list, double weight, double H)
        {
            Catenary last = null;
            if (Spans.Count>0)
            {
                last = Spans.Last();
                weight = last.CableWeight;
                H = last.HorizontalTension;
            }
            RaiseListChangedEvents=false;
            for (int i = 0; i<list.Length; i++)
            {
                var cat = new Catenary(list[i], weight, H);
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
        public void AddSpans(params Catenary[] list)
        {
            RaiseListChangedEvents=false;
            for (int i = 0; i<list.Length; i++)
            {
                Spans.Add(list[i]);
            }
            RaiseListChangedEvents=true;

            OnRulingSpanChange(new ItemChangeEventArgs());
        }
        /// <summary>
        /// Define a new catenary using defaults
        /// </summary>
        /// <returns></returns>
        public Catenary NewCatenary()
        {
            var last = Last;
            if (last==null)
            {
                return Catenary.Default(ProjectUnits.Default());
            }
            return new Catenary(last.EndPosition, last.StepX, 0.0, last.CableWeight, last.HorizontalTension);
        }
        #endregion

        #region Methods

        public Catenary GetRulingSpanCatenary()
        {
            var rs = new Span(
                HasSpans ? Spans[0].StartPosition : Span.Default(Units).StartPosition,
                RulingSpanLength,
                0);
            return new Catenary(rs, CableWeight, HorizontalTension);
        }

        /// <summary>
        /// Ruling span length calculation for uneven spans
        /// </summary>
        public double RulingSpanLength
        {
            get
            {
                var rs1 = 0.0;
                var rs2 = 0.0;
                for (int i = 0; i<Spans.Count; i++)
                {
                    double δx = Spans[i].StepX;
                    double δy = Spans[i].StepY;
                    rs1+=Math.Pow(δx, 3);
                    double slope = δy/δx;
                    rs2+=δx*Math.Pow(1+slope*slope, 1.5);
                }
                return Math.Round(Math.Sqrt(rs1/rs2), 1);
            }
        }
        /// <summary>
        /// Update all spans from tension on one span.
        /// </summary>
        /// <param name="spanIndex">The span index whose tension to use.</param>
        public void UpdateAllFromFrom(int spanIndex)
        {
            if (spanIndex>=0&&spanIndex<=Spans.Count)
            {
                HorizontalTension=Spans[spanIndex].HorizontalTension;
            }
        }

        public void UpdateAllCatenary()
        {
            for (int i = 0; i<Spans.Count; i++)
            {
                Spans[i].CalculateCenter();
            }
        }
        public void UpdateSpanEnds()
        {
            for (int i = 1; i<Spans.Count; i++)
            {
                Spans[i].StartPosition=this[i-1].EndPosition;
            }
        }
        public void SetCableWeight(double wt)
        {
            for (int i = 0; i<Spans.Count; i++)
            {
                Spans[i].CableWeight = wt;
            }
        }
        public void SetHorizontalTension(double H)
        {
            for (int i = 0; i<Spans.Count; i++)
            {
                Spans[i].HorizontalTension=H;
            }
            UpdateSpanEnds();
        }
        public void SetHorizontalTensionFrom(int span_index)
        {
            double H = Spans[span_index].HorizontalTension;
            for (int i = 0; i<Spans.Count; i++)
            {
                if (i!=span_index)
                {
                    Spans[i].HorizontalTension=H;
                }
            }
            UpdateSpanEnds();
        }
        public Catenary FindCatenaryFromX(double x)
        {
            foreach (var item in Spans)
            {
                if (item.ContainsX(x))
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// Find vertical separation between catenary and point.
        /// </summary>
        /// <param name="point">The target point</param>
        /// <param name="directional">Return absolute value if false.</param>
        /// <returns></returns>
        public double ClearanceTo(Vector2 point, bool directional)
        {
            Catenary catenary = FindCatenaryFromX(point.X);
            if (catenary!=null)
            {
                double dy = catenary.CatenaryFunction(point.X)-point.Y;
                return directional ? dy : Math.Abs(dy);
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
        [XmlIgnore()]
        public bool IsOk => HasSpans && Spans.All((s) => s.IsOK);

        [XmlIgnore(), Bindable(BindableSupport.No)]
        public bool RaiseListChangedEvents
        {
            get => Spans.RaiseListChangedEvents;
            set => Spans.RaiseListChangedEvents = value;
        }

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
        public Catenary First { get { return Spans.Count>0 ? this[0] : null; } }
        [XmlIgnore()]
        public Catenary Last { get { return Spans.Count>0 ? this[Spans.Count-1] : null; } }

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
                Unit unit = Units.ForceUnit;
                if (parts.Length>1)
                {
                    unit=parts[1];
                }
                if (!Units.ForceUnit.IsCompatible(unit))
                {
                    return;
                }
                if (double.TryParse(parts[0], out double x))
                {
                    HorizontalTension=unit.FactorTo(Units.ForceUnit)*x;
                }
            }
        }
        [XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public double CableWeight
        {
            get { return HasSpans ? Spans[Spans.Count-1].CableWeight : 0; }
            set
            {
                SetCableWeight(value);
            }
        }
        [XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public double HorizontalTension
        {
            get { return HasSpans ? Spans[Spans.Count-1].HorizontalTension : 0; }
            set
            {
                SetHorizontalTension(value);
            }
        }
        [XmlIgnore(), Bindable(BindableSupport.No)]
        public bool HasSpans { get { return Spans.Count>0; } }
        #endregion

        #region Span Event Handlers
        void RulingSpan_RulingSpanChanged(object sender, ItemChangeEventArgs e)
        {

            if (!e.AllItems)
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
            if (e.ListChangedType==ListChangedType.PropertyDescriptorAdded
                ||e.ListChangedType==ListChangedType.PropertyDescriptorDeleted)
            {
                return;
            }
            if (e.ListChangedType==ListChangedType.ItemDeleted
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
            XmlSerializer xs = new XmlSerializer(typeof(RulingSpan));
            var fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            var rs = xs.Deserialize(fs) as RulingSpan;
            fs.Close();
            return rs;
        }

        public void SaveFile(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(RulingSpan));
            var fs = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            xs.Serialize(fs, this);
            fs.Close();
        }

        #endregion

        #region Event Arguments
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

        #region Equatable

        public bool Equals(RulingSpan other)
        {
            return Units.Equals(other.Units)
                && Spans.AllEqual(other.Spans);
        }

        public override bool Equals(object obj)
        {
            if (obj is RulingSpan rs)
            {
                return Equals(rs);
            }
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = 17;
                foreach (var item in Spans)
                {
                    hc = 23*hc + item.GetHashCode();
                }
                return hc;
            }
        }
        #endregion

        #region Formatting
        public override string ToString() => ToString("0.###", CultureInfo.CurrentCulture.NumberFormat);
        public string ToString(string formatting, IFormatProvider formatProvider)
        {
            return $"RS({string.Join(", ", Spans.Select((s) => s.StepX.ToString(formatting, formatProvider)))}), Weight={CableWeight.ToString(formatting, formatProvider)}, H={HorizontalTension.ToString(formatting, formatProvider)}";
        }
        #endregion
    }
}
