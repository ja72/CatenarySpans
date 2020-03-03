using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Diagnostics;

namespace JA.Engineering
{

    public interface ISpan
    {
        Vector2 StartPosition { get; set; }
        Vector2 EndPosition { get; }
        Vector2 Step { get; set; }
        bool IsOK { get; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Span : ISpan, INotifyPropertyChanged, ICloneable, IFormattable
    {
        public static string DefaultLengthFormat="0.###";
        public static double DefaultSpanLength=500;
        public static double DefaultTowerHeight=100;
        public static double DefaultSpanRise=50;

        Vector2 start;
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
            this.start=origin;
            this.step=span;
            this.RaisesChangedEvents=true;
        }
        public Span(ISpan other)
        {
            this.start=other.StartPosition;
            this.step=other.Step;
            this.RaisesChangedEvents=true;
        }
        public Span(Span previous, double dx, double dy)
        {
            this.start = previous.EndPosition;
            this.step = new Vector2(dx, dy);
            this.RaisesChangedEvents = true;
        }
        #endregion

        #region Properties

        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.Yes)]
        public virtual bool IsOK
        {
            get { return step.X.IsFinite()&&step.X.IsPositive(); }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 StartPosition { get { return start; } set { start=value; } }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 EndPosition { get { return start+step; } }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 StartBase { get { return new Vector2(start.X, 0); } }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 EndBase { get { return new Vector2(start.X+step.X, 0); } }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 Step
        {
            get { return step; }
            set
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
            get { return start.X; }
            set
            {
                //start.x=value;
                start = new Vector2(value, start.Y);
            }
        }
        [RefreshProperties(RefreshProperties.None), XmlAttribute()]
        public double StartY
        {
            get { return start.Y; }
            set
            {
                //start.y=value;
                start = new Vector2(start.X, value);
            }
        }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        public double SpanX
        {
            get { return step.X; }
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
            get { return step.Y; }
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
        public double SpanLength
        {
            get
            {
                return step.Manitude;
            }
        }
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, double> DiagonalFunction
        {
            get
            {
                return (x) => start.Y+x*step.Y/step.X;
            }
        }
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, Vector2> ParametricDiagonal
        {
            get
            {
                return (t) => new Vector2(start.X+step.X*t, start.Y+step.Y*t);
            }
        }
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

            miny=Math.Min(miny, start.Y);
            miny=Math.Min(miny, start.Y+step.Y);
            maxy=Math.Max(maxy, start.Y);
            maxy=Math.Max(maxy, start.Y+step.Y);
            minx=Math.Min(minx, start.X);
            minx=Math.Min(minx, start.X+step.X);
            maxx=Math.Max(maxx, start.X);
            maxx=Math.Max(maxx, start.X+step.X);

            minPosition = new Vector2(minx, miny);
            maxPosition = new Vector2(maxx, maxy);
        }
        public bool ContainsX(double x)
        {
            return start.X<=x&&step.X>=(x-start.X);
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
                start.ToString(format, provider),
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

        #endregion

    }

    #region Span List
    public class SpanListBase<T> : ItemList<T> where T : Span, new()
    {

        public SpanListBase()
        {
            this.ItemChanged+=new EventHandler<ItemChangeEventArgs>(SpanList_ItemChanged);
            this.ProjectUnitsChanged+=new EventHandler<ProjectUnits.ChangeEventArgs>(SpanListBase_ProjectUnitsChanged);
        }

        public SpanListBase(ProjectUnits units, params T[] items)
            : base(units, items)
        {
            this.ItemChanged+=new EventHandler<ItemChangeEventArgs>(SpanList_ItemChanged);
            this.ProjectUnitsChanged+=new EventHandler<ProjectUnits.ChangeEventArgs>(SpanListBase_ProjectUnitsChanged);
        }

        void SpanList_ItemChanged(object sender, ItemChangeEventArgs e)
        {
            UpdateSpanEnds();
        }

        void SpanListBase_ProjectUnitsChanged(object sender, ProjectUnits.ChangeEventArgs e)
        {
            for (int i=0; i<Items.Count; i++)
            {
                this[i].RaisesChangedEvents=false;
                this[i].StartPosition*=e.LengthFactor;
                this[i].Step*=e.LengthFactor;
                //this[i].HorizontalTension*=e.ForceFactor;
                //this[i].Weight*=e.ForceFactor/e.LengthFactor;
                this[i].RaisesChangedEvents=true;
            }
            OnItemChanged(new ItemChangeEventArgs());
        }

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
