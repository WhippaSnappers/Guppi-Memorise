using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

namespace Guppi_Memorise
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutUs : ContentPage
    {
        public AboutUs()
        {
            InitializeComponent();

            Byte taps = 0;
            var gr1 = new TapGestureRecognizer();
            gr1.NumberOfTapsRequired = 1;
            gr1.Tapped += (s, e) =>
            {
                taps++;
                if (taps == 5)
                {
                    easterEgg.Opacity = 1;
                    aboutUsLabel.GestureRecognizers.Remove(gr1);
                }
            };
            aboutUsLabel.GestureRecognizers.Add(gr1);
        }
    }
}