using CustomJSONData;
using System.Collections.Generic;
using UnityEngine;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;

namespace ShaderExtensions.Event
{
    class ShaderCommand
    {

        public string ID { get; private set; }

        public string Reference { get; private set; }

        public bool Clear { get; private set; } = false;

        public ShaderPropertiesCommand Properties { get; private set; }

        public Material material;

        public ShaderCommand(TreeDict dict) {

            Reference = Trees.At(dict, "_ref");
            if (Reference == null) {
                Reference = "Error";
            }

            ID = Trees.At(dict, "_id");
            if (ID == null) {
                ID = Reference;
            }

            dynamic tmp = Trees.At(dict, "_clear");
            if (tmp != null && tmp.GetType() == typeof(bool)) {
                Clear = tmp;
            }

            Logger.log.Info("ShaderCommand: _id: " + ID + " _ref:" + Reference);

            object ret = Trees.At(dict, "_props");
            List<object> props;
            if (ret != null) {
                props = ret as List<object>;
            } else {
                props = new List<object>();
            }

            Properties = new ShaderPropertiesCommand(props, this);

        }

    }
}
