using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;
using SQLite;
using System.Windows.Input;
using System.Diagnostics;

namespace Guppi_Memorise
{
    public partial class MainPage : ContentPage
    {
        public async void FlashCardsButton(object s, EventArgs a)
        {
            await Navigation.PushAsync(new FlashCardsPage());
        }
        public async void LearnTextButton(object s, EventArgs a)
        {
            await Navigation.PushAsync(new MemorisingStartPage());
        }
        public async void StatisticsButton(object s, EventArgs a)
        {
            await Navigation.PushAsync(new StatisticsPage());
        }
        public async void ProfileButton(object s, EventArgs a)
        {
            await Navigation.PushAsync(new ProfilePage());
        }
        public async void AboutAppButton(object s, EventArgs a)
        {
            await Navigation.PushAsync(new AboutApp());
        }
        public async void AboutUsButton(object s, EventArgs a)
        {
            await Navigation.PushAsync(new AboutUs());
        }
        public MainPage()
        {
            InitializeComponent();

        }
    }
}
