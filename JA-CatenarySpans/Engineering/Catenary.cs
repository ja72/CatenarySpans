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
    public class Catenary : Span, IEquatable<Catenary>, ICloneable, IFormattable
    {
        public static readonly string DefaultForceFormat="0.###";
        public static readonly double DefaultHorizontalTension=1000;
        public static readonly double DefaultWeight=1;

        public new static Catenary Default(ProjectUnits projectUnits)
        {
            var f_length = Unit.ft.FactorTo(projectUnits.LengthUnit);
            var f_force = Unit.lbf.FactorTo(projectUnits.ForceUnit);
            return new Catenary(Span.Default(projectUnits), (f_force/f_length)*DefaultWeight, f_force*DefaultHorizontalTension);
        }

        public event EventArgs<Catenary>.Handler CatenaryChanged;

        double weight, horizontalTension;

        #region Factory
        /// <summary>
        /// Create a catenary using the default values defined in <see cref="Catenary"/>.
        /// </summary>
        public Catenary() : this(Default(ProjectUnits.Default()))
        { }
        public Catenary(Vector2 origin, double dx, double dy, double weight, double H)
            : this(origin, new Vector2(dx,dy), weight, H)
        {
        }
        public Catenary(Vector2 origin, Vector2 span, double weight, double H)
            : base(origin, span)
        {
            this.weight=weight;
            this.horizontalTension=H;
            this.RelativeCenter=CatenaryCalculator.CenterPosition(Step, this.weight, horizontalTension);
            this.SpanChanged+=new EventArgs<Span>.Handler(Catenary_SpanChanged);
            this.CatenaryChanged+=new EventArgs<Catenary>.Handler(Catenary_CatenaryChanged);
        }
        public Catenary(ISpan span, double weight, double H)
            : base(span)
        {
            this.weight=weight;
            this.horizontalTension=H;
            this.RelativeCenter=CatenaryCalculator.CenterPosition(Step, this.weight, H);
            this.SpanChanged+=new EventArgs<Span>.Handler(Catenary_SpanChanged);
            this.CatenaryChanged+=new EventArgs<Catenary>.Handler(Catenary_CatenaryChanged);
        }
        public Catenary(Catenary other)
            : base(other)
        {
            this.RelativeCenter=other.RelativeCenter;
            this.weight=other.weight;
            this.horizontalTension=other.horizontalTension;
            this.SpanChanged+=new EventArgs<Span>.Handler(Catenary_SpanChanged);
            this.CatenaryChanged+=new EventArgs<Catenary>.Handler(Catenary_CatenaryChanged);
        }
        #endregion

        #region Properties
        /// <summary>
        /// True if weight and tension are defined for a valid span.
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.Yes)]
        public override bool IsOK
        {
            get { return base.IsOK && horizontalTension>0 && weight>0; }
        }

        /// <summary>
        /// Cable weight per unit length [Force/Length]
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double CableWeight
        {
            get { return weight; }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Weight must be finite and positive");
                }
                if (!weight.Equals(value))
                {
                    this.weight=value;
                    if (RaisesChangedEvents)
                    {
                        OnCatenaryChanged(new EventArgs<Catenary>(this));
                        OnPropertyChanged(() => CableWeight);
                    }
                }
            }
        }
        /// <summary>
        /// Lowest point [Length]
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Vector2 LowestPosition { get { return StartPosition+RelativeCenter; } }
        /// <summary>
        /// Sag point location relative to start point [Length]
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        protected internal Vector2 RelativeCenter { get; private set; }
        /// <summary>
        /// Sag point location [Length]
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Vector2 SagPoint => StartPosition + RelativeCenter;
        /// <summary>
        /// Location of the sag point [Length]
        /// </summary>
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double CenterX { get { return StartPosition.X+RelativeCenter.X; } }

        /// <summary>
        /// Height of sag point [Length]
        /// </summary>
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double CenterY
        {
            get { return StartPosition.Y+RelativeCenter.Y; }
        }
        /// <summary>
        /// Clearance is distance from ground to sag point [Length]        
        /// </summary>
        /// <remarks>If sag point is outside the span, then clearance is to the lowest support point.</remarks>
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double Clearance
        {
            get { return IsCenterInSpan?StartPosition.Y+RelativeCenter.Y:Math.Min(StartPosition.Y, EndPosition.Y); }
            set
            {
                if (value.IsNotFinite()||IsUpliftCondition)
                {
                    throw new ArgumentException("Clearance must be finite and lowest point must be in span.");
                }
                HorizontalTension=CatenaryCalculator.SetClearance(Step, weight, StartPosition.Y-value, 1e-3);
            }
        }
        /// <summary>
        /// Maximum sag (deviation from diagonal) [Length]
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double MaximumSag
        {
            get
            {
                return CatenaryCalculator.MaximumSag(Step, RelativeCenter, weight, horizontalTension);
            }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Sag must be finite and positive.");
                }
                HorizontalTension=CatenaryCalculator.SetMaximumSag(Step, weight, value, 1e-3);
            }
        }
        /// <summary>
        /// Sag in the middle of the span [Length]
        /// </summary>
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double MidSag
        {
            get
            {
                return CatenaryCalculator.MidSag(Step, RelativeCenter, weight, horizontalTension);
            }
        }
        /// <summary>
        /// The location of maximum sag [Length]
        /// </summary>
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Vector2 SagPosition
        {
            get
            {
                double x=CatenaryCalculator.MaximumSagX(Step, RelativeCenter, weight, horizontalTension);
                return StartPosition+CatenaryCalculator.PositionAtX(Step, RelativeCenter, weight, horizontalTension, x);
            }
        }
        /// <summary>
        /// The catenary constant <c>a = H/w</c> [Length]
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double CatenaryConstant
        {
            get { return horizontalTension/weight; }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Catenary constant must be finite and positive.");
                }
                HorizontalTension=weight*value;
            }
        }
        /// <summary>
        /// Tension parameter <c>η = w*S/(2*H)</c> [Dimensionless]
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double Eta
        {
            get { return weight*StepX/(2*horizontalTension); }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Eta must be finite and positive.");
                }
                HorizontalTension=weight*Step.X/(2*value);
            }
        }
        /// <summary>
        /// The minimum tension in the cable [Force]
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double HorizontalTension
        {
            get { return horizontalTension; }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Horizontal Tension must be finite and positive.");
                }
                if (!horizontalTension.Equals(value))
                {
                    this.horizontalTension=value;
                    if (RaisesChangedEvents)
                    {
                        OnCatenaryChanged(new EventArgs<Catenary>(this));
                        OnPropertyChanged(() => HorizontalTension);
                    }
                }
            }
        }
        /// <summary>
        /// The total length of the cable [Length]
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double TotalLength
        {
            get
            {
                return CatenaryCalculator.TotalLength(Step, RelativeCenter, weight, horizontalTension);
            }
            set
            {
                if (value.IsNotFinite()||value<=Step.Manitude)
                {
                    throw new ArgumentException("Length must be finite and larger than the span diagonal.");
                }
                HorizontalTension=CatenaryCalculator.SetTotalLength(Step, weight, value, 1e-3);
            }
        }
        
        /// <summary>
        /// The total gravitation weight of the cable [Force]
        /// </summary>
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double TotalWeight => CableWeight * TotalLength;

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
                TotalLength=StepX*(1+value/100);
            }
        }
        [RefreshProperties(RefreshProperties.All), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double AverageTension
        {
            get
            {
                return CatenaryCalculator.AverageTension(Step, RelativeCenter, weight, horizontalTension);
            }
            set
            {
                if (value.IsNotFinite()||value.IsNegativeOrZero())
                {
                    throw new ArgumentException("Average Tension must be finite and positive.");
                }
                HorizontalTension=CatenaryCalculator.SetAverageTension(Step, weight, value, 1e-3);
            }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 StartTension
        {
            get
            {
                return new Vector2(-horizontalTension, -CatenaryCalculator.VertricalTensionAtX(Step, RelativeCenter, weight, horizontalTension, 0));
            }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No), Browsable(false)]
        public Vector2 EndTension
        {
            get
            {
                return new Vector2(horizontalTension, CatenaryCalculator.VertricalTensionAtX(Step, RelativeCenter, weight, horizontalTension, StepX));
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
                return RelativeCenter.X<=StepX/2?
                    CatenaryCalculator.TotalTensionAtX(Step, RelativeCenter, weight, horizontalTension, 0):
                    CatenaryCalculator.TotalTensionAtX(Step, RelativeCenter, weight, horizontalTension, StepX);
            }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double StartVerticalTension
        {
            get { return -CatenaryCalculator.VertricalTensionAtX(Step, RelativeCenter, weight, horizontalTension, 0); }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double EndVerticalTension
        {
            get { return CatenaryCalculator.VertricalTensionAtX(Step, RelativeCenter, weight, horizontalTension, StepX); }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double StartTotalTension
        {
            get { return CatenaryCalculator.TotalTensionAtX(Step, RelativeCenter, weight, horizontalTension, 0); }
        }
        [ReadOnly(true), XmlIgnore()]
        [TypeConverter(typeof(NiceTypeConverter))]
        public double EndTotalTension
        {
            get { return CatenaryCalculator.TotalTensionAtX(Step, RelativeCenter, weight, horizontalTension, StepX); }
        }

        [ReadOnly(true), XmlIgnore()]
        public bool IsCenterInSpan
        {
            get
            {
                double L=CatenaryCalculator.LengthSegmentAtX(Step, RelativeCenter, weight, horizontalTension, RelativeCenter.X);
                return L>=0&&L<=CatenaryCalculator.TotalLength(Step, RelativeCenter, weight, horizontalTension);
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
        protected internal void CalculateCenter()
        {
            this.RelativeCenter=CatenaryCalculator.CenterPosition(Step, weight, horizontalTension);
            OnPropertyChanged(() => CenterX);
            OnPropertyChanged(() => CenterY);
        }

        #endregion

        #region Event Triggers
        protected void OnCatenaryChanged(EventArgs<Catenary> e)
        {
            this.CatenaryChanged?.Invoke(this, e);
        }

        #endregion

        #region Methods

        public override void ScaleForUnits(ProjectUnits.ChangeEventArgs g, bool raiseEvents = false)
        {
            base.ScaleForUnits(g, raiseEvents);

            var raise = RaisesChangedEvents;
            this.RaisesChangedEvents = raiseEvents;
            this.HorizontalTension *= g.ForceFactor;
            this.CableWeight *= g.ForceFactor/g.LengthFactor;
            this.RelativeCenter *= g.LengthFactor;
            this.RaisesChangedEvents = raise;
        }

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
                double minx = minPosition.X, miny = minPosition.Y;
                double maxx = maxPosition.X, maxy = maxPosition.Y;

                minx=Math.Min(minx, pos.X);
                miny=Math.Min(miny, pos.Y);
                maxx=Math.Max(maxx, pos.X);
                maxy=Math.Max(maxy, pos.Y);

                minPosition = new Vector2(minx, miny);
                maxPosition = new Vector2(maxx, maxy);

            }
        }
        /// <summary>
        /// Find the tension needed for the catenary to pass through a specific point in space.
        /// </summary>
        /// <param name="point"></param>
        public void SetClearancePoint(Vector2 point)
        {
            if (ContainsX(point.X))
            {
                double x = point.X-StartPosition.X;
                double D = StartPosition.Y+StepY/StepX*x-point.Y;
                HorizontalTension=CatenaryCalculator.SetSagAtX(Step, weight, D, x, 1e-3);
            }
        }

        #endregion

        #region Functions
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, Vector2> ParametricCurve
        {
            get
            {
                return (t) => StartPosition+CatenaryCalculator.PositionAtT(Step, RelativeCenter, weight, horizontalTension, t);
            }
        }
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, Vector2> ParametricTension
        {
            get
            {
                return (t) => new Vector2(horizontalTension, CatenaryCalculator.VertricalTensionAtX(Step, RelativeCenter, weight, horizontalTension, CatenaryCalculator.ParameterToX(Step, RelativeCenter, weight, horizontalTension, t)));
            }
        }
        [XmlIgnore(), Browsable(false), Bindable(BindableSupport.No)]
        public Func<double, double> CatenaryFunction
        {
            get
            {
                return (x) => StartPosition.Y+CatenaryCalculator.PositionAtX(Step, RelativeCenter, weight, horizontalTension, x-StartPosition.X).Y;
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
                base.ToString(), horizontalTension, weight);
        }
        #endregion

        #region Equating
        public bool Equals(Catenary other)
        {
            return base.Equals(other)
                && HorizontalTension == other.horizontalTension
                && CableWeight == other.CableWeight;
        }
        public override bool Equals(object obj)
        {
            if (obj is Catenary other)
            {
                return Equals(other);
            }
            if (obj is ISpan span)
            {
                return Equals(span);
            }
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = 23*17 + base.GetHashCode();
                hc = 23*hc + HorizontalTension.GetHashCode();
                hc = 23*hc + CableWeight.GetHashCode();
                return hc;
            }
        }
        #endregion
    }

}
