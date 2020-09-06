using CustomJSONData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;

namespace ShaderExtensions.Event
{
    class ShaderCommand
    {

        public string id { get; private set; }

        public string reference { get; private set; }

        public bool clear { get; private set; } = false;

    public ShaderPropertiesCommand properties { get; private set; }

        public Material mat;

        public ShaderCommand(TreeDict dict) {

            reference = Trees.At(dict, "_ref");
            if (reference == null) {
                reference = "Error";
            }

            id = Trees.At(dict, "_id");
            if (id == null) {
                id = reference;
            }

            dynamic tmp = Trees.At(dict, "_clear");
            if (tmp != null && tmp.GetType() == typeof(bool)) {
                clear = tmp;
            }

            Logger.log.Info("ShaderCommand: _id: "+id+" _ref:"+reference);

            object ret = Trees.At(dict, "_props");
            List<object> props;
            if(ret != null) {
                props = ret as List<object>;
            } else {
                props = new List<object>();
            }

            properties = new ShaderPropertiesCommand(props, this);

        }

    }
}
