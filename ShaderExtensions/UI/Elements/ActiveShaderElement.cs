using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderExtensions.UI.Elements
{
    internal class ActiveShaderElement : CustomListElement
    {
        public string ID { get; private set; }

        internal ActiveShaderElement(string referenceName, string id) {
            ReferenceName = referenceName;
            ID = id;
        }

        private string _referenceName;
        [UIValue("reference-name")]
        public string ReferenceName {
            get => _referenceName;
            set {
                _referenceName = value;
                NotifyPropertyChanged(nameof(ReferenceName));
            }
        }

    }
}
