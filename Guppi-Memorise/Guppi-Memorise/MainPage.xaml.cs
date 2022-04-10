using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;

namespace Guppi_Memorise
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var gr = new TapGestureRecognizer();
            gr.NumberOfTapsRequired = 1;
            gr.Tapped += (s, e) => {
                Navigation.PushAsync(new FlashCardsPage());
            };
            flashCards.GestureRecognizers.Add(gr);
        }
    }
}
