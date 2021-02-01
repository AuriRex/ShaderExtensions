using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShaderExtensions.UI.Elements
{
    internal class CustomListElement : INotifyPropertyChanged
    {
#nullable enable annotations
        public event PropertyChangedEventHandler? PropertyChanged;
#nullable restore annotations

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            try {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            } catch { }
        }
    }
}
