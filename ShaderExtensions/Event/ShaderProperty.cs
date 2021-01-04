using NoodleExtensions.Animation;

namespace ShaderExtensions.Event
{
    internal class ShaderProperty : Property
    {
        public string Property { get; private set; }

        public float Duration { get; private set; }

        public PointDefinition Points { get; private set; }

        public Functions Easing { get; private set; }

        public ShaderCommand ParentCommand { get; private set; }

        public ShaderProperty(string property, float duration, dynamic value, Functions easing, ShaderCommand parent) : base(PropertyType.Linear) {

            this.Property = property;
            this.Duration = duration;
            this.Easing = easing;
            this.ParentCommand = parent;
            if (value is string) {
                this.Value = value;
            }
            Points = PointDefinition.DynamicToPointData(value);
            Logger.log.Info("ShaderProperty: Points: " + Points);
        }

        public void SetValue(float val) => ParentCommand.material.SetFloat(Property, val);

    }
}