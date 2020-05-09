using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Globalization;

namespace JA.Engineering
{

    public interface IContainsMeasures
    {
        void ScaleForUnits(ProjectUnits.ChangeEventArgs g);
    }

    public interface IHasUnits
    {
        [XmlAttribute(), Bindable(BindableSupport.No)]
        string UnitSymbols { get; set; }
        [XmlIgnore(), Bindable(BindableSupport.Yes)]
        ProjectUnits Units { get; }
        void SetProjectUnits(ProjectUnits target);
    }

    public class ProjectUnits : IEquatable<ProjectUnits>, ICloneable
    {

        public static readonly ProjectUnitSystem DefaultUnitSystem = ProjectUnitSystem.FeetPoundSecond;
        public static readonly ProjectUnits Default = new ProjectUnits();

        string time_sym, length_sym, mass_sym, force_sym;
        Unit time_unit, length_unit, mass_unit, force_unit;

        public event EventHandler<UnitChangeEventArgs> TimeUnitChanged;
        public event EventHandler<UnitChangeEventArgs> LengthUnitChanged;
        public event EventHandler<UnitChangeEventArgs> MassUnitChanged;
        public event EventHandler<UnitChangeEventArgs> ForceUnitChanged;

        [XmlIgnore(), Bindable(BindableSupport.No)]
        public bool RaiseChangeEvents { get; set; }

        #region Factory
        public ProjectUnits() : this(DefaultUnitSystem) { }
        public ProjectUnits(ProjectUnitSystem system)
        {
            switch (system)
            {
                case ProjectUnitSystem.NewtonMeterSecond:
                    time_sym="s";
                    length_sym="m";
                    mass_sym="kg";
                    force_sym="N";
                    break;
                case ProjectUnitSystem.NewtonMillimeterSecond:
                    time_sym="s";
                    length_sym="mm";
                    mass_sym="kg";
                    force_sym="N";
                    break;
                case ProjectUnitSystem.KiloNetwonMeterSecond:
                    time_sym="s";
                    length_sym="m";
                    mass_sym="kg";
                    force_sym="kN";
                    break;
                case ProjectUnitSystem.InchPoundSecond:
                    time_sym="s";
                    length_sym="in";
                    mass_sym="lbm";
                    force_sym="lbf";
                    break;
                case ProjectUnitSystem.FeetPoundSecond:
                    time_sym="s";
                    length_sym="ft";
                    mass_sym="lbm";
                    force_sym="lbf";
                    break;
                case ProjectUnitSystem.FeetOunceSecond:
                    time_sym="s";
                    length_sym="ft";
                    mass_sym="oz";
                    force_sym="ozf";
                    break;
                case ProjectUnitSystem.InchOunceSecond:
                    time_sym="s";
                    length_sym="in";
                    mass_sym="oz";
                    force_sym="ozf";
                    break;
            }
            this.time_unit=Unit.Parse(time_sym);
            this.length_unit=Unit.Parse(length_sym);
            this.mass_unit=Unit.Parse(mass_sym);
            this.force_unit=Unit.Parse(force_sym);
            this.RaiseChangeEvents=true;
        }
        public ProjectUnits(params string[] units)
        {
            foreach (var sym in units)
            {
                Unit unit = sym;
                if (unit.IsCompatible(Unit.s)) { time_sym=sym; }
                if (unit.IsCompatible(Unit.m)) { length_sym=sym; }
                if (unit.IsCompatible(Unit.kg)) { mass_sym=sym; }
                if (unit.IsCompatible(Unit.N)) { force_sym=sym; }
            }
            this.time_unit=Unit.Parse(time_sym);
            this.length_unit=Unit.Parse(length_sym);
            this.mass_unit=Unit.Parse(mass_sym);
            this.force_unit=Unit.Parse(force_sym);
            this.RaiseChangeEvents=true;
        }
        public ProjectUnits(ProjectUnits other)
        {
            this.time_unit=other.time_unit;
            this.length_unit=other.length_unit;
            this.mass_unit=other.mass_unit;
            this.force_unit=other.force_unit;
            this.RaiseChangeEvents=true;
        }
        public bool IsOk
        {
            get
            {
                return TimeUnit.IsOk&&LengthUnit.IsOk&&MassUnit.IsOk&&ForceUnit.IsOk;
            }
        }
        #endregion

        #region Properties

        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Unit TimeUnit { get { return time_unit; } }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        public string Time
        {
            get { return time_sym; }
            set
            {
                string old = time_sym;
                if (!Unit.AreEqual(old, value))
                {
                    time_sym=value;
                    time_unit = Unit.Parse(value);
                    if (RaiseChangeEvents)
                        OnTimeUnitChanged(new UnitChangeEventArgs(old, value));
                }
            }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Unit LengthUnit { get { return length_unit; } }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        public string Length
        {
            get { return length_sym; }
            set
            {
                string old = length_sym;
                if (!Unit.AreEqual(old, value))
                {
                    length_sym=value;
                    length_unit = Unit.Parse(value);
                    if (RaiseChangeEvents)
                        OnLengthUnitChanged(new UnitChangeEventArgs(old, value));
                }
            }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Unit MassUnit { get { return mass_unit; } }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        public string Mass
        {
            get { return mass_sym; }
            set
            {
                string old = mass_sym;
                if (!Unit.AreEqual(old, value))
                {
                    mass_sym=value;
                    mass_unit = Unit.Parse(value);
                    if (RaiseChangeEvents)
                        OnMassUnitChanged(new UnitChangeEventArgs(old, value));
                }
            }
        }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Unit ForceUnit { get { return force_unit; } }
        [RefreshProperties(RefreshProperties.All), XmlAttribute()]
        public string Force
        {
            get { return force_sym; }
            set
            {
                string old = force_sym;
                if (!Unit.AreEqual(old, value))
                {
                    force_sym=value;
                    force_unit = Unit.Parse(value);
                    if (RaiseChangeEvents)
                        OnForceUnitChanged(new UnitChangeEventArgs(old, value));
                }
            }
        }

        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.Yes)]
        public string LinearWeight { get { return string.Format("{0}/{1}", force_sym, length_sym); } }
        [ReadOnly(true), XmlIgnore(), Bindable(BindableSupport.No)]
        public Unit LinearWeightUnit { get { return force_unit/length_unit; } }
        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", mass_sym, force_sym, length_sym, time_sym);
        }
        public static ProjectUnits Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            value=value.TrimStart('(').TrimEnd(')');
            string[] unit_sym = value.Split(',');
            return new ProjectUnits(unit_sym);
        }
        public static ProjectUnits[] PredefinedUnits
        {
            get
            {
                var vals = new ProjectUnitSystem[] 
                {
                    ProjectUnitSystem.FeetPoundSecond,
                    ProjectUnitSystem.NewtonMeterSecond,
                    ProjectUnitSystem.InchOunceSecond,
                }; 
                //vals = (ProjectUnitSystem[])Enum.GetValues(typeof(ProjectUnitSystem));
                return vals.Select((en) => new ProjectUnits(en)).ToArray();
            }
        }

        public System.Windows.Forms.ToolStripMenuItem ToMenuItem(System.Drawing.Image icon,
            EventHandler<SetEventArgs> select_units)
        {
            void click(object tsi, EventArgs ev)
            {
                select_units?.Invoke(tsi, new SetEventArgs(this));
            }
            return new System.Windows.Forms.ToolStripMenuItem(this.ToString(), icon, click) { Tag=this };
        }


        #endregion

        #region Event Triggers
        protected void OnTimeUnitChanged(UnitChangeEventArgs e)
        {
            this.TimeUnitChanged?.Invoke(this, e);
        }
        protected void OnLengthUnitChanged(UnitChangeEventArgs e)
        {
            this.LengthUnitChanged?.Invoke(this, e);
        }
        protected void OnMassUnitChanged(UnitChangeEventArgs e)
        {
            this.MassUnitChanged?.Invoke(this, e);
        }
        protected void OnForceUnitChanged(UnitChangeEventArgs e)
        {
            this.ForceUnitChanged?.Invoke(this, e);
        }
        #endregion

        #region IEquatable Members

        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(ProjectUnits)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is ProjectUnits units)
            {
                return Equals(units);
            }
            return false;
        }

        /// <summary>
        /// Checks for equality among <see cref="ProjectUnits"/> classes
        /// </summary>
        /// <param name="other">The other <see cref="ProjectUnits"/> to compare it to</param>
        /// <returns>True if equal</returns>
        public bool Equals(ProjectUnits other)
        {
            return time_sym.Equals(other.time_sym)
                &&length_sym.Equals(other.length_sym)
                &&mass_sym.Equals(other.mass_sym)
                &&force_sym.Equals(other.force_sym);
        }

#pragma warning disable S2328 // "GetHashCode" should not reference mutable fields
        /// <summary>
        /// Calculates the hash code for the <see cref="ProjectUnits"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
#pragma warning restore S2328 // "GetHashCode" should not reference mutable fields
        {
            unchecked
            {
                return (((17*23
                    +time_sym.GetHashCode())*23
                    +length_sym.GetHashCode())*23
                    +mass_sym.GetHashCode())*23
                    +force_sym.GetHashCode();
            }
        }

        #endregion

        #region Type Converter

        public class UnitsTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType==typeof(string)||base.CanConvertFrom(context, sourceType);
            }
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string) return ProjectUnits.Parse(value as string);
                return base.ConvertFrom(context, culture, value);
            }
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;
            }
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(
                    ProjectUnits.PredefinedUnits.Select((pu) => pu.ToString()).ToArray());
            }
        }

        #endregion

        #region Events
        public class SetEventArgs : EventArgs
        {
            readonly ProjectUnits new_units;

            public SetEventArgs(ProjectUnits new_units)
            {
                this.new_units=new_units;
            }
            public ProjectUnits NewProjectUnits { get { return new_units; } }
        }
        public class ChangeEventArgs : EventArgs
        {
            readonly ProjectUnits old_units, new_units;
            readonly double time_factor, length_factor, mass_factor, force_factor;

            public ChangeEventArgs(ProjectUnits old_units, ProjectUnits new_units)
            {
                this.old_units=new_units;
                this.new_units=new_units;
                this.time_factor=1;
                this.length_factor=1;
                this.mass_factor=1;
                this.force_factor=1;
                if (old_units.TimeUnit.IsOk&&new_units.TimeUnit.IsOk)
                {
                    this.time_factor=old_units.TimeUnit.Scale/new_units.TimeUnit.Scale;
                }
                if (old_units.LengthUnit.IsOk&&new_units.LengthUnit.IsOk)
                {
                    this.length_factor=old_units.LengthUnit.Scale/new_units.LengthUnit.Scale;
                }
                if (old_units.MassUnit.IsOk&&new_units.MassUnit.IsOk)
                {
                    this.mass_factor=old_units.MassUnit.Scale/new_units.MassUnit.Scale;
                }
                if (old_units.ForceUnit.IsOk&&new_units.ForceUnit.IsOk)
                {
                    this.force_factor=old_units.ForceUnit.Scale/new_units.ForceUnit.Scale;
                }
            }
            public ProjectUnits OldProjectUnits { get { return old_units; } }
            public ProjectUnits NewProjectUnits { get { return new_units; } }

            public double TimeFactor { get { return time_factor; } }
            public double LengthFactor { get { return length_factor; } }
            public double MassFactor { get { return mass_factor; } }
            public double ForceFactor { get { return force_factor; } }
        }

        #endregion

        #region ICloneable Members

        public ProjectUnits Clone() { return new ProjectUnits(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion        

    }

    #region HasUnitsBase

    public abstract class HasUnitsBase : IHasUnits
    {
        public void SetProjectUnits(ProjectUnits target)
        {
            if (Units == null)
            {
                OnProjectUnitsSet(new ProjectUnits.SetEventArgs(target));
                this.Units = target;
            }
            else if (!Units.Equals(target))
            {
                OnProjectUnitsChange(new ProjectUnits.ChangeEventArgs(Units, target));
                ConvertUnits(target);
                this.Units = target;
            }
        }
        protected abstract void ConvertUnits(ProjectUnits target);

        #region Factory
        protected HasUnitsBase()
        {
            Units = new ProjectUnits();
        }
        protected HasUnitsBase(ProjectUnits units)
        {
            this.Units = units;
        }
        #endregion

        #region Events
        public event EventHandler<ProjectUnits.SetEventArgs> ProjectUnitsSet;
        public event EventHandler<ProjectUnits.ChangeEventArgs> ProjectUnitsChanged;
        protected void OnProjectUnitsSet(ProjectUnits.SetEventArgs e)
        {
            this.ProjectUnitsSet?.Invoke(this, e);
        }
        protected void OnProjectUnitsChange(ProjectUnits.ChangeEventArgs e)
        {
            this.ProjectUnitsChanged?.Invoke(this, e);
        }

        #endregion

        #region Properties
        [XmlAttribute(), Bindable(BindableSupport.No), Browsable(false)]
        public string UnitSymbols
        {
            get { return Units.ToString(); }
            set { SetProjectUnits(ProjectUnits.Parse(value)); }
        }

        [XmlIgnore(), TypeConverter(typeof(ProjectUnits.UnitsTypeConverter))]
        public ProjectUnits Units { get; private set; }

        #endregion
    }
    #endregion
}
