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
        }

        private void FlashcardsClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new FlashCardsPage());
        }

        private void AboutAppClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new AboutApp());
        }

        private void AboutUsClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new AboutUs());
        }

        private void ProfileClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new ProfilePage());
        }

        private void StatClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new StatisticsPage());
        }

        private void MemorisingClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MemorisingStartPage());
        }
    }
}
