/*using CustomJSONData;
using Heck.Animation;
using System;
using System.Collections.Generic;
using static ShaderExtensions.Managers.ShaderEventManager;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;

namespace ShaderExtensions.Event
{
    internal class ShaderPropertiesCommand
    {

        private List<ShaderProperty> _propList;

        public ShaderPropertiesCommand(List<object> list, ShaderCommand parent)
        {
            _propList = new List<ShaderProperty>();

            foreach (TreeDict d in list)
            {

                string property = "" + Trees.At(d, "_prop");

                float duration;

                dynamic value = Trees.At(d, "_value");

                dynamic tmp = Trees.At(d, "_duration");

                try
                {
                    duration = (float) tmp;
                }
                catch (Exception)
                {
                    duration = -1;
                }


                Functions easing = Functions.easeLinear;

                string easingString = Trees.At(d, "_easing");
                if (easingString != null)
                {
                    easing = (Functions) Enum.Parse(typeof(Functions), easingString);
                }

                //Logger.log.Debug("ShaderPropertiesCommand: p:" + property + " d: " + duration + " e: " + easing + " v:" + value);

                _propList.Add(new ShaderProperty(property, duration, value, easing, parent));

            }

        }

        public List<ShaderProperty> getProps() => _propList;

    }
}
*/