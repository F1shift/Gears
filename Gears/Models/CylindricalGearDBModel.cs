using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Gears.Utility;
using static System.Math;
using static Gears.Utility.Math;
using static Gears.Utility.EnumerableExtentions;
using SQLite;

namespace Gears.Models
{
    [Table(nameof(CylindricalGearDBModel))]
    internal class CylindricalGearDBModel : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string Name { get; set; } = "Cylindical Gear";
        public string Discription { get {
                return $"Module : {mn}, Teeth Number : [{z1}, {z2}]";
            } }
        public string Created { get; set; } = DateTime.Now.ToLocalTime() + " created.";
        public string LastUsed { get; set; } = DateTime.Now.ToLocalTime() + " updated.";
        [PropertyMapping()]
        public double mn { get; set; }
        [PropertyMapping()]
        public double αn { get; set; }
        [PropertyMapping()]
        public double β { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.z))]
        public int z1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.z))]
        public int z2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.xn))]
        public double xn1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.xn))]
        public double xn2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.ρ_c))]
        public double ρ_c1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.ρ_c))]
        public double ρ_c2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.b_c))]
        public double b_c1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.b_c))]
        public double b_c2 { get; set; }
        [PropertyMapping()]
        public double ha_c { get; set; }
        [PropertyMapping()]
        public double hf_c { get; set; }
        [PropertyMapping()]
        public double mt { get; set; }
        [PropertyMapping()]
        public double αt { get; set; }
        [PropertyMapping()]
        public double αwt { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.L))]
        public double L1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.L))]
        public double L2 { get; set; }
        [PropertyMapping()]
        public double y { get; set; }
        [PropertyMapping()]
        public double a { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.d))]
        public double d1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.d))]
        public double d2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.db))]
        public double db1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.db))]
        public double db2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.dw))]
        public double dw1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.dw))]
        public double dw2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.ha))]
        public double ha1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.ha))]
        public double ha2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.hf))]
        public double hf1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.hf))]
        public double hf2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.h))]
        public double h1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.h))]
        public double h2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.ρ))]
        public double ρ1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.ρ))]
        public double ρ2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.da))]
        public double da1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.da))]
        public double da2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.df))]
        public double df1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.df))]
        public double df2 { get; set; }
        [PropertyMapping(0, nameof(CylindricalGearBase.b))]
        public double b1 { get; set; }
        [PropertyMapping(1, nameof(CylindricalGearBase.b))]
        public double b2 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void CopyFrom(CylindricalGearBase source)
        {
            var sourceType = source.GetType();
            var thisType = this.GetType();
            foreach (var thisProperty in thisType.GetProperties())
            {
                if (Attribute.IsDefined(thisProperty, typeof(PropertyMappingAttribute)))
                {
                    var mappingAttribute = (PropertyMappingAttribute)Attribute.GetCustomAttribute(thisProperty, typeof(PropertyMappingAttribute));
                    if (mappingAttribute.PropertyName == null)
                        mappingAttribute.PropertyName = thisProperty.Name;
                    var sourceProperty = sourceType.GetProperty(mappingAttribute.PropertyName);
                    if (sourceProperty.PropertyType.IsArray)
                    {
                        var value = (sourceProperty.GetValue(source) as Array).GetValue(mappingAttribute.ArrayIndex);
                        thisProperty.SetValue(this, value);
                    }
                    else
                    {
                        thisProperty.SetValue(this, sourceProperty.GetValue(source));
                    }
                }
            } 
        }

        public void CopyTo(CylindricalGearBase target)
        {
            var targetType = target.GetType();
            var thisType = this.GetType();
            foreach (var thisProperty in thisType.GetProperties())
            {
                if (Attribute.IsDefined(thisProperty, typeof(PropertyMappingAttribute)))
                {
                    var mappingAttribute = (PropertyMappingAttribute)Attribute.GetCustomAttribute(thisProperty, typeof(PropertyMappingAttribute));
                    if (mappingAttribute.PropertyName == null)
                        mappingAttribute.PropertyName = thisProperty.Name;
                    var targetProperty = targetType.GetProperty(mappingAttribute.PropertyName);
                    if (targetProperty.PropertyType.IsArray)
                    {
                        var value = thisProperty.GetValue(this);
                        (targetProperty.GetValue(target) as Array).SetValue(value, mappingAttribute.ArrayIndex);
                    }
                    else
                    {
                        targetProperty.SetValue(target, thisProperty.GetValue(this));
                    }
                }
            }
        }

        [AttributeUsage(AttributeTargets.Property)]
        class PropertyMappingAttribute : Attribute
        {
            public string PropertyName { get; set; }
            public int ArrayIndex { get; set; }
            public PropertyMappingAttribute(int arrayIndex = -1, string propertyName = null)
            {
                PropertyName = propertyName;
                ArrayIndex = arrayIndex;
            }
        }
    }
}
