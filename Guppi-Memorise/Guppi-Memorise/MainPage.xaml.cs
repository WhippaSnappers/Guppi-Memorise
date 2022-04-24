using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;
using SQLite;

namespace Guppi_Memorise
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitializeGestureRecognisers();
        }

        private void InitializeGestureRecognisers()
        {
            // Flashcards
            var gr1 = new TapGestureRecognizer();
            gr1.NumberOfTapsRequired = 1;
            gr1.Tapped += (s, e) => {
                Navigation.PushAsync(new FlashCardsPage());
            };
            flashCards.GestureRecognizers.Add(gr1);
            // About app
            var gr2 = new TapGestureRecognizer();
            gr2.NumberOfTapsRequired = 1;
            gr2.Tapped += (s, e) => {
                Navigation.PushAsync(new AboutApp());
            };
            aboutApp.GestureRecognizers.Add(gr2);
            // About us
            var gr3 = new TapGestureRecognizer();
            gr3.NumberOfTapsRequired = 1;
            gr3.Tapped += (s, e) =>
            {
                Navigation.PushAsync(new AboutUs());
            };
            aboutUs.GestureRecognizers.Add(gr3);
            // Profile
            var gr4 = new TapGestureRecognizer();
            gr4.NumberOfTapsRequired = 1;
            gr4.Tapped += (s, e) =>
            {
                Navigation.PushAsync(new ProfilePage());
            };
            profile.GestureRecognizers.Add(gr4);
            // Statistics
            var gr5 = new TapGestureRecognizer();
            gr5.NumberOfTapsRequired = 1;
            gr5.Tapped += (s, e) =>
            {
                Navigation.PushAsync(new StatisticsPage());
            };
            statistics.GestureRecognizers.Add(gr5);
        }
    }
}
