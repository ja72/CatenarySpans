using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Diagnostics;

namespace JA.Engineering
{

    public interface ISpan : IContainsMeasures
    {
        Vector2 StartPosition { get; set; }
        Vector2 EndPosition { get; }
        Vector2 Step { get; }
        bool IsOK { get; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Span : ISpan, INotifyPropertyChanged, ICloneable, IFormattable
    {
        public static readonly string DefaultLengthFormat="0.###";
        public static readonly double DefaultSpanLength=500;
        public static readonly double DefaultTowerHeight=100;
        public static readonly double DefaultSpanRise=50;
        Vector2 step;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventArgs<Span>.Handler SpanChanged;

        [XmlIgnore(), Bindable(BindableSupport.No)]
        public bool RaisesChangedEvents { get; set; }

        #region Factory
        public Span()
            : this(DefaultTowerHeight*Vector2.UnitY, DefaultSpanLength, DefaultSpanRise)
        { }
        public Span(Vector2 origin, double dx, double dy)
            : this(origin, new Vector2(dx, dy))
        { }
        public Span(Vector2 origin, Vector2 span)
        {
            this.StartPosition=origin;
            this.step=span;
            this.RaisesChangedEvents=true;
        }
        public Span(ISpan other)
        {
            this.StartPosition=other.StartPosition;
            this.step=other.Step;
            this.RaisesChangedEvents=true;
        }
        public Span(Span previous, double dx, double dy)
        {
            this.StartPosition = previous.EndPosition;
            this.step = new Vector2(dx, dy);
            this.RaisesChangedEvents = true;
        }
        #endregion

        #region Properties

        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.Yes)]
        public virtual bool IsOK => step.X.IsFinite()&&step.X.IsPositive();
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 StartPosition { get; set; }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 EndPosition => StartPosition+step;
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 StartBase => new Vector2(StartPosition.X, 0);
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 EndBase => new Vector2(StartPosition.X+step.X, 0);
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 Step
        {
            get => step;
            private set
            {
                if (!step.Equals(value))
                {
                    this.step=value;
                    if (RaisesChangedEvents)
                    {
                        OnSpanChanged(new EventArgs<Span>(this));
                        OnPropertyChanged(() => Step);
                    }
                }
            }
        }
        [RefreshProperties(RefreshProperties.None), XmlAttribute()]
        public double StartX
        {
            get => StartPosition.X;
            set =>
                //start.x=value;
                StartPosition = new Vector2(value, StartPosition.Y);
        }
        [RefreshProperties(RefreshProperties.None), XmlAttribute()]
        public double StartY
        {
            get => StartPosition.Y;
            set =>
                //start.y=value;
                StartPosition = new Vector2(StartPosition.X, value);
        }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        public double SpanX
        {
            get => step.X;
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Span must be finite and positive.");
                }
                if (!step.Y.Equals(value))
                {
                    //step.x=value;
                    step = new Vector2(value, step.Y);
                    if (RaisesChangedEvents)
                    {
                        OnSpanChanged(new EventArgs<Span>(this));
                        OnPropertyChanged(() => SpanX);
                    }
                }
            }
        }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        public double SpanY
        {
            get => step.Y;
            set
            {
                if (value.IsNotFinite())
                {
                    throw new ArgumentException("Span height must be finite.");
                }
                if (!step.Y.Equals(value))
                {
                    //step.y=value;
                    step = new Vector2(step.X, value);
                    if (RaisesChangedEvents)
                    {
                        OnSpanChanged(new EventArgs<Span>(this));
                        OnPropertyChanged(() => SpanY);
                    }
                }
            }
        }
        [ReadOnly(true), RefreshProperties(RefreshProperties.None), XmlIgnore()]
        public double SpanLength => step.Manitude;
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, double> DiagonalFunction => (x) => StartPosition.Y+x*step.Y/step.X;
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, Vector2> ParametricDiagonal => (t) => new Vector2(StartPosition.X+step.X*t, StartPosition.Y+step.Y*t);
        #endregion

        #region Event Triggers
        protected void OnSpanChanged(EventArgs<Span> e)
        {
            this.SpanChanged?.Invoke(this, e);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the corner coordinates of a bounding box for the supported span.
        /// </summary>
        /// <param name="minPosition">The lowest x,y values of the bounding box</param>
        /// <param name="maxPosition">The highest x,y values of the bounding box</param>
        public virtual void GetBounds(ref Vector2 minPosition, ref Vector2 maxPosition)
        {
            if (minPosition.IsZero&&maxPosition.IsZero)
            {
                minPosition=StartBase;
                maxPosition=EndBase;
            }
            double minx = minPosition.X, miny = minPosition.Y;
            double maxx = maxPosition.X, maxy = maxPosition.Y;

            miny=Math.Min(miny, StartPosition.Y);
            miny=Math.Min(miny, StartPosition.Y+step.Y);
            maxy=Math.Max(maxy, StartPosition.Y);
            maxy=Math.Max(maxy, StartPosition.Y+step.Y);
            minx=Math.Min(minx, StartPosition.X);
            minx=Math.Min(minx, StartPosition.X+step.X);
            maxx=Math.Max(maxx, StartPosition.X);
            maxx=Math.Max(maxx, StartPosition.X+step.X);

            minPosition = new Vector2(minx, miny);
            maxPosition = new Vector2(maxx, maxy);
        }
        public bool ContainsX(double x)
        {
            return StartPosition.X<=x&&step.X>=(x-StartPosition.X);
        }

        
        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }
        public Span Clone() { return new Span(this); }
        #endregion

        #region Formatting
        public override string ToString()
        {
            return ToString(DefaultLengthFormat);
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("Start={0}, Step={1}",
                StartPosition.ToString(format, provider),
                step.ToString(format, provider));
        }
        #endregion

        #region INotifyPropertyChanged Members

        protected void OnPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            var mex=property.Body as System.Linq.Expressions.MemberExpression;
            OnPropertyChanged(mex.Member.Name);
        }
        protected virtual void OnPropertyChanged(string property)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(property));
        }
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        public virtual void ScaleForUnits(ProjectUnits.ChangeEventArgs g, bool raiseEvents = false)
        {
            var raise = RaisesChangedEvents;
            this.RaisesChangedEvents = raiseEvents;

            this.StartPosition *= g.LengthFactor;
            this.Step *= g.LengthFactor;

            this.RaisesChangedEvents = raise;
        }
        void IContainsMeasures.ScaleForUnits(ProjectUnits.ChangeEventArgs g) => ScaleForUnits(g);

        #endregion

    }

    #region Span List
    public class SpanListBase<T> : ItemList<T> where T : Span, new()
    {

        public SpanListBase()
        {
            this.ItemChanged+=new EventHandler<ItemChangeEventArgs>(SpanList_ItemChanged);
        }

        public SpanListBase(ProjectUnits units, params T[] items)
            : base(units, items)
        {
            this.ItemChanged+=new EventHandler<ItemChangeEventArgs>(SpanList_ItemChanged);
        }

        protected override void ConvertUnits(ProjectUnits target)
        {
            var g = new ProjectUnits.ChangeEventArgs(Units, target);
            for (int i = 0; i < Items.Count; i++)
            {
                this[i].ScaleForUnits(g);
            }
            OnItemChanged(new ItemChangeEventArgs());
        }

        void SpanList_ItemChanged(object sender, ItemChangeEventArgs e)
        {
            UpdateSpanEnds();
        }

        //void SpanListBase_ProjectUnitsChanged(object sender, ProjectUnits.ChangeEventArgs e)
        //{
        //    for (int i=0; i<Items.Count; i++)
        //    {
        //        this[i].RaisesChangedEvents=false;
        //        this[i].StartPosition*=e.LengthFactor;
        //        this[i].Step*=e.LengthFactor;
        //        this[i].HorizontalTension*=e.ForceFactor;
        //        this[i].Weight*=e.ForceFactor/e.LengthFactor;
        //        this[i].RaisesChangedEvents=true;
        //    }
        //    OnItemChanged(new ItemChangeEventArgs());
        //}

        public void UpdateSpanEnds()
        {
            for (int i=1; i<Items.Count; i++)
            {
                this[i].StartPosition=this[i-1].EndPosition;
            }
        }
        public int FindSpanIndexFromX(double x)
        {
            T[] list=ItemArray;
            for (int i=0; i<list.Length; i++)
            {
                if (list[i].ContainsX(x))
                {
                    return i;
                }
            }
            return -1;
        }

        public override T NewItem()
        {
            var last=Last;
            if (last==null)
            {
                return new T();
            }
            else
            {
                return new T()
                {
                    StartPosition=last.EndPosition,
                    SpanX=last.SpanX,
                };
            }
        }
    }
    #endregion

}
