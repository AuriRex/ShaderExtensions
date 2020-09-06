using NoodleExtensions.Animation;
using System.Collections.Generic;
using System.Linq;

namespace ShaderExtensions.Event
{
    internal class ShaderProperty : Property
    {
        public string property { get; private set; }

        public float duration { get; private set; }

        public PointDefinition points { get; private set; }

        public Functions easing { get; private set; }

        public ShaderCommand parent { get; private set; }

        public ShaderProperty(string property, float duration, dynamic value, Functions easing, ShaderCommand parent) : base(PropertyType.Linear) {
            
            this.property = property;
            this.duration = duration;
            this.easing = easing;
            this.parent = parent;
            if(value is string) {
                this.Value = value;
            }
            points = PointDefinition.DynamicToPointData(value);
            Logger.log.Info("ShaderProperty: Points: "+points);
        }

        public void SetValue(float val) {

            parent.mat.SetFloat(property, val);

        }

    }
}