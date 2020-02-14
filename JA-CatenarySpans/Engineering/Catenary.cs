using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using JA.Gdi;
using System.Drawing;
using System.Xml.Serialization;

namespace JA.Engineering
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Catenary : Span, ICloneable, IFormattable
    {
        public static string DefaultForceFormat="0.###";
        public static double DefaultHorizontalTension=1000;
        public static double DefaultWeight=1;

        //public event EventHandler<CatenaryChangedEventArgs> CatenaryChanged;
        public event EventArgs<Catenary>.Handler CatenaryChanged;

        double w, H;
        Vector2 center;

        #region Factory
        public Catenary()
            : base()
        {
            this.w=DefaultWeight;
            this.H=DefaultHorizontalTension;
            this.center=CatenaryCalculator.CenterPosition(Step, w, H);
            this.SpanChanged+=new EventArgs<Span>.Handler(Catenary_SpanChanged);
            this.CatenaryChanged+=new EventArgs<Catenary>.Handler(Catenary_CatenaryChanged);
        }

        public Catenary(Vector2 origin, double dx, double dy, double weight)
            : this(origin, new Vector2(dx, dy), weight)
        { }
        public Catenary(Vector2 origin, Vector2 span, double weight)
            : base(origin, span)
        {
            this.w=weight;
            this.H=DefaultHorizontalTension;
            this.center=CatenaryCalculator.CenterPosition(Step, w, H);
            this.SpanChanged+=new EventArgs<Span>.Handler(Catenary_SpanChanged);
            this.CatenaryChanged+=new EventArgs<Catenary>.Handler(Catenary_CatenaryChanged);
        }
        public Catenary(ISpan span) : this(span, DefaultWeight) { }
        public Catenary(ISpan span, double weight) : this(span, weight, DefaultHorizontalTension) { }
        public Catenary(ISpan span, double weight, double H)
            : base(span)
        {
            this.w=weight;
            this.H=H;
            this.center=CatenaryCalculator.CenterPosition(Step, w, H);
            this.SpanChanged+=new EventArgs<Span>.Handler(Catenary_SpanChanged);
            this.CatenaryChanged+=new EventArgs<Catenary>.Handler(Catenary_CatenaryChanged);
        }
        public Catenary(Catenary other)
            : base(other)
        {
            this.center=other.center;
            this.w=other.w;
            this.H=other.H;
            this.SpanChanged+=new EventArgs<Span>.Handler(Catenary_SpanChanged);
            this.CatenaryChanged+=new EventArgs<Catenary>.Handler(Catenary_CatenaryChanged);
        }
        #endregion

        #region Properties
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.Yes)]
        public override bool IsOK
        {
            get { return base.IsOK&&H.IsPositive(); }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Vector2 LowestPosition { get { return StartPosition+center; } }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double Weight
        {
            get { return w; }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Weight must be finite and positive");
                }
                if (!w.Equals(value))
                {
                    this.w=value;
                    if (RaisesChangedEvents)
                    {
                        OnCatenaryChanged(new EventArgs<Catenary>(this));
                        OnPropertyChanged(() => Weight);
                    }
                }
            }
        }

        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double CenterX { get { return StartPosition.X+center.x; } }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double CenterY
        {
            get { return StartPosition.Y+center.y; }
        }
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double Clearance
        {
            get { return IsCenterInSpan?StartPosition.y+center.y:Math.Min(StartPosition.y, EndPosition.y); }
            set
            {
                if (value.IsNotFinite()||IsUpliftCondition)
                {
                    throw new ArgumentException("Clerance must be finite and lowest point must be in span.");
                }
                HorizontalTension=CatenaryCalculator.SetClearance(Step, w, StartPosition.y-value, 1e-3);
            }
        }

        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double MaximumSag
        {
            get
            {
                return CatenaryCalculator.MaximumSag(Step, center, w, H);
            }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Sag must be finite and positive.");
                }
                HorizontalTension=CatenaryCalculator.SetMaximumSag(Step, w, value, 1e-3);
            }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double MidSag
        {
            get
            {
                return CatenaryCalculator.MidSag(Step, center, w, H);
            }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Vector2 SagPosition
        {
            get
            {
                double x=CatenaryCalculator.MaximumSagX(Step, center, w, H);
                return StartPosition+CatenaryCalculator.PositionAtX(Step, center, w, H, x);
            }
        }
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double CatenaryConstant
        {
            get { return H/w; }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Catenary constant must be finite and positive.");
                }
                HorizontalTension=w*value;
            }
        }
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double Eta
        {
            get { return w*SpanX/(2*H); }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Eta must be finite and positive.");
                }
                HorizontalTension=w*Step.x/(2*value);
            }
        }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double HorizontalTension
        {
            get { return H; }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Horizontal Tension must be finite and positive.");
                }
                if (!H.Equals(value))
                {
                    this.H=value;
                    if (RaisesChangedEvents)
                    {
                        OnCatenaryChanged(new EventArgs<Catenary>(this));
                        OnPropertyChanged(() => HorizontalTension);
                    }
                }
            }
        }
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double TotalLength
        {
            get
            {
                return CatenaryCalculator.TotalLength(Step, center, w, H);
            }
            set
            {
                if (value.IsNotFinite()||value<=Step.Manitude)
                {
                    throw new ArgumentException("Length must be finite and larger than the span diagonal.");
                }
                HorizontalTension=CatenaryCalculator.SetTotalLength(Step, w, value, 1e-3);
            }
        }

        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double GeometricStrainPct
        {
            get { return 100*(TotalLength/Step.Manitude-1); }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Geometric strain must be finite and positive.");
                }
                TotalLength=SpanX*(1+value/100);
            }
        }
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double AverageTension
        {
            get
            {
                return CatenaryCalculator.AverageTension(Step, center, w, H);
            }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Average Tension must be finite and positive.");
                }
                HorizontalTension=CatenaryCalculator.SetAverageTension(Step, w, value, 1e-3);
            }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 StartTension
        {
            get
            {
                return new Vector2(-H, -CatenaryCalculator.VertricalTensionAtX(Step, center, w, H, 0));
            }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 EndTension
        {
            get
            {
                return new Vector2(H, CatenaryCalculator.VertricalTensionAtX(Step, center, w, H, SpanX));
            }
        }
        /// <summary>
        /// Return the tension from the highest tower
        /// </summary>
        [ReadOnly(true), XmlIgnore()]
        public double MaxTension
        {
            get
            {
                return center.x<=SpanX/2?
                    CatenaryCalculator.TotalTensionAtX(Step, center, w, H, 0):
                    CatenaryCalculator.TotalTensionAtX(Step, center, w, H, SpanX);
            }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double StartVerticalTension
        {
            get { return -CatenaryCalculator.VertricalTensionAtX(Step, center, w, H, 0); }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double EndVerticalTension
        {
            get { return CatenaryCalculator.VertricalTensionAtX(Step, center, w, H, SpanX); }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double StartTotalTension
        {
            get { return CatenaryCalculator.TotalTensionAtX(Step, center, w, H, 0); }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double EndTotalTension
        {
            get { return CatenaryCalculator.TotalTensionAtX(Step, center, w, H, SpanX); }
        }

        [ReadOnly(true), XmlIgnore()]
        public bool IsCenterInSpan
        {
            get
            {
                double L=CatenaryCalculator.LengthSegmentAtX(Step, center, w, H, center.x);
                return L>=0&&L<=CatenaryCalculator.TotalLength(Step, center, w, H);
            }
        }
        /// <summary>
        /// Checks for uplift condition (vertical tension on end is upwards)
        /// </summary>
        [ReadOnly(true), XmlIgnore()]
        public bool IsUpliftCondition { get { return !IsCenterInSpan; } }
        [ReadOnly(true), XmlIgnore()]
        public bool IsStartTowerUplift { get { return StartVerticalTension<0; } }
        [ReadOnly(true), XmlIgnore()]
        public bool IsEndTowerUplift { get { return EndVerticalTension<0; } }
        #endregion

        #region Event Handlers
        protected void Catenary_SpanChanged(object sender, EventArgs<Span> e)
        {
            OnCatenaryChanged(new EventArgs<Catenary>(e.Item as Catenary));
        }
        protected void Catenary_CatenaryChanged(object sender, EventArgs<Catenary> e)
        {
            CalculateCenter();
        }

        /// <summary>
        /// Helper function that calculates the catenary lowest point and triggers property changed notifiers
        /// </summary>
        public void CalculateCenter()
        {
            this.center=CatenaryCalculator.CenterPosition(Step, w, H);
            OnPropertyChanged(() => CenterX);
            OnPropertyChanged(() => CenterY);
        }

        #endregion

        #region Event Triggers
        //protected void OnCatenaryChanged(CatenaryChangedEventArgs e)
        //{
        //    if (this.CatenaryChanged!=null)
        //    {
        //        this.CatenaryChanged(this, e);
        //    }
        //}
        protected void OnCatenaryChanged(EventArgs<Catenary> e)
        {
            this.CatenaryChanged?.Invoke(this, e);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the corner coordinates of a bounding box for the catenary curve.
        /// </summary>
        /// <remarks>It splits the curve into 16 segments and finds the bounds based on they node coordinates</remarks>
        /// <param name="minPosition">The lowest x,y values of the bounding box</param>
        /// <param name="maxPosition">The highest x,y values of the bounding box</param>
        public override void GetBounds(ref Vector2 minPosition, ref Vector2 maxPosition)
        {
            base.GetBounds(ref minPosition, ref maxPosition);
            Func<double, Vector2> f=ParametricCurve;
            int N=16;
            for (int i=0; i<=N; i++)
            {
                double t=(double)(i)/N;
                Vector2 pos=f(t);
                minPosition.x=Math.Min(minPosition.x, pos.x);
                minPosition.y=Math.Min(minPosition.y, pos.y);
                maxPosition.x=Math.Max(maxPosition.x, pos.x);
                maxPosition.y=Math.Max(maxPosition.y, pos.y);
            }
        }

        public void SetClearancePoint(Vector2 point)
        {
            double x=point.x-StartPosition.x;
            double D=StartPosition.y+SpanY/SpanX*x-point.y;
            HorizontalTension=CatenaryCalculator.SetSagAtX(Step, w, D, x, 1e-3);
        }

        #endregion

        #region Functions
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, Vector2> ParametricCurve
        {
            get
            {
                return (t) => StartPosition+CatenaryCalculator.PositionAtT(Step, center, w, H, t);
            }
        }
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, Vector2> ParametricTension
        {
            get
            {
                return (t) => new Vector2(H, CatenaryCalculator.VertricalTensionAtX(Step, center, w, H, CatenaryCalculator.ParameterToX(Step, center, w, H, t)));
            }
        }
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, double> CatenaryFunction
        {
            get
            {
                return (x) => StartPosition.y+CatenaryCalculator.PositionAtX(Step, center, w, H, x-StartPosition.x).y;
            }
        }

        #endregion

        #region ICloneable Members

        public new Catenary Clone() { return new Catenary(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region Formatting
        public override string ToString()
        {
            return ToString(DefaultForceFormat);
        }
        public new string ToString(string format)
        {
            return ToString(format, null);
        }
        public new string ToString(string format, IFormatProvider provider)
        {
            return string.Format(provider, "{0}, H={1:"+format+"}, w={2:"+format+"}",
                base.ToString(), H, w);
        } 
        #endregion
    }

}
