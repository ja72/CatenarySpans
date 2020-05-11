using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Diagnostics;

namespace JA.Engineering
{
    /// <summary>
    /// Interface definition of span
    /// </summary>
    public interface ISpan : IContainsMeasures, IEquatable<ISpan>
    {
        Vector2 StartPosition { get; set; }
        Vector2 EndPosition { get; }
        Vector2 Step { get; }
        bool IsOK { get; }
    }
    /// <summary>
    /// A span defines two points in space
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Span : ISpan, IEquatable<Span>, INotifyPropertyChanged, ICloneable, IFormattable
    {
        public static readonly string DefaultLengthFormat="0.###";
        public static readonly double DefaultSpanLength=700;
        public static readonly double DefaultTowerHeight=100;
        public static readonly double DefaultSpanRise=0;

        public static Span Default(ProjectUnits projectUnits)
        {
            var f_length = Unit.ft.FactorTo(projectUnits.LengthUnit);
            return new Span(f_length*Vector2.UnitY*DefaultTowerHeight, f_length*DefaultSpanLength, f_length*DefaultSpanRise);
        }

        Vector2 step;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventArgs<Span>.Handler SpanChanged;

        /// <summary>
        /// Changes raise events if true
        /// </summary>
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

        /// <summary>
        /// Check of the span is well defined
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.Yes)]
        public virtual bool IsOK => step.X.IsFinite()&&step.X.IsPositive();
        /// <summary>
        /// The span starting position
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]        
        public Vector2 StartPosition { get; set; }
        /// <summary>
        /// The span ending position
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 EndPosition => StartPosition+step;
        /// <summary>
        /// The ground at the starting position (y=0)
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 StartBase => new Vector2(StartPosition.X, 0);
        /// <summary>
        /// The ground at the ending position (y=0)
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 EndBase => new Vector2(StartPosition.X+step.X, 0);
        /// <summary>
        /// The span
        /// </summary>
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
        /// <summary>
        /// The x-coordinate of the start point
        /// </summary>
        [RefreshProperties(RefreshProperties.None), XmlAttribute()]
        public double StartX
        {
            get => StartPosition.X;
            set =>
                //start.x=value;
                StartPosition = new Vector2(value, StartPosition.Y);
        }
        /// <summary>
        /// The y-coordinate of the start point
        /// </summary>
        [RefreshProperties(RefreshProperties.None), XmlAttribute()]
        public double StartY
        {
            get => StartPosition.Y;
            set =>
                //start.y=value;
                StartPosition = new Vector2(StartPosition.X, value);
        }
        /// <summary>
        /// The horizontal span
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlAttribute("SpanX")]
        public double StepX
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
                        OnPropertyChanged(() => StepX);
                    }
                }
            }
        }
        /// <summary>
        /// The vertical rise
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlAttribute("SpanY")]
        public double StepY
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
                        OnPropertyChanged(() => StepY);
                    }
                }
            }
        }
        /// <summary>
        /// The hypotenuse of the span
        /// </summary>
        [ReadOnly(true), RefreshProperties(RefreshProperties.None), XmlIgnore()]
        public double DiagonalLength => step.Manitude;
        /// <summary>
        /// A function of the <c>y=f(x)</c> form for the diagonal
        /// </summary>
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, double> DiagonalFunction => (x) => StartPosition.Y+x*step.Y/step.X;
        /// <summary>
        /// A parametric line of the diagonal in the form of <c>pos=f(t=0..1)</c>
        /// </summary>
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

        #region Equatable
        public bool Equals(Span other)
        {
            return StartPosition.Equals(other.StartPosition)
                && Step.Equals(other.Step);
        }
        public bool Equals(ISpan other)
        {
            return StartPosition.Equals(other.StartPosition)
                && Step.Equals(other.Step);
        }
        public override bool Equals(object obj)
        {
            if (obj is ISpan span)
            {
                return Equals(span);
            }
            if (obj is Span span2)
            {
                return Equals(span2);
            }
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = 17*23;
                hc = 23*hc + StartPosition.GetHashCode();
                hc = 23*hc + Step.GetHashCode();
                return hc;
            }
        }
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
                    StepX=last.StepX,
                };
            }
        }
    }
    #endregion

}
