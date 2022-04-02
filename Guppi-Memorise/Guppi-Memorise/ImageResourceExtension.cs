using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

namespace Guppi_Memorise {
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension {
        public string Source { get; set; }

        public object ProvideValue (IServiceProvider sp) {
            if (Source == null) {
                return null;
            }
            var img = ImageSource.FromResource(Source);
            return img;
        }
    }
}
