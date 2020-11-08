using CustomJSONData;
using System;
using System.Collections.Generic;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;

namespace ShaderExtensions.Event
{
    class ShaderPropertiesCommand
    {

        private List<ShaderProperty> propList;

        public ShaderPropertiesCommand(List<object> list, ShaderCommand parent) {
            propList = new List<ShaderProperty>();

            foreach (TreeDict d in list) {

                string property = "" + Trees.At(d, "_prop");

                float duration;

                dynamic value = Trees.At(d, "_value");

                dynamic tmp = Trees.At(d, "_duration");

                try {
                    duration = (float) tmp;
                } catch (Exception) {
                    duration = -1;
                }


                Functions easing = Functions.easeLinear;

                string easingString = Trees.At(d, "_easing");
                if (easingString != null) {
                    easing = (Functions) Enum.Parse(typeof(Functions), easingString);
                }

                Logger.log.Info("ShaderPropertiesCommand: p:" + property + " d: " + duration + " e: " + easing + " v:" + value);

                propList.Add(new ShaderProperty(property, duration, value, easing, parent));

            }

        }

        public List<ShaderProperty> getProps() => propList;

    }
}
