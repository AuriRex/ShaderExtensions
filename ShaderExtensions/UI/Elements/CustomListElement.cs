﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

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
