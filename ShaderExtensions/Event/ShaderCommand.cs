/*using CustomJSONData;
using System.Collections.Generic;
using UnityEngine;
using static ShaderExtensions.Managers.ShaderEventManager;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;

namespace ShaderExtensions.Event
{
    internal class ShaderCommand
    {

        public string ID { get; private set; }

        public string ReferenceName { get; private set; }

        public bool ClearAfterLastPropIsDone { get; private set; } = false;

        public ShaderPropertiesCommand Properties { get; private set; }

        public ShaderEffectData ShaderEffectData { get; internal set; }

        public Material Material { get; internal set; }

        public ShaderCommand(TreeDict dict)
        {

            ReferenceName = Trees.At(dict, "_ref");
            if (ReferenceName == null)
            {
                ReferenceName = "Error";
            }

            ID = Trees.At(dict, "_id");
            if (ID == null)
            {
                ID = ReferenceName;
            }

            dynamic tmp = Trees.At(dict, "_clearAfterDone");
            if (tmp != null && tmp.GetType() == typeof(bool))
            {
                ClearAfterLastPropIsDone = tmp;
            }

            //Logger.log.Debug("ShaderCommand: _id: " + ID + " _ref:" + Reference);

            object ret = Trees.At(dict, "_props");
            List<object> props;
            if (ret != null)
            {
                props = ret as List<object>;
            }
            else
            {
                props = new List<object>();
            }

            Properties = new ShaderPropertiesCommand(props, this);

            ShaderProperty longest = null;
            Properties.getProps().ForEach(sp => {
                if (sp.Duration > (longest?.Duration ?? 0))
                {
                    longest = sp;
                }
            });
            if (longest != null)
            {
                longest.IsLast = true;
            }

        }

    }
}
*/