using System;
using Xamarin.Forms;

namespace Guppi_Memorise
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public async void FlashCardsButton(object _, EventArgs __)
        {
            await Navigation.PushAsync(new FlashCardsPage());
        }
        public async void LearnTextButton(object _, EventArgs __)
        {
            await Navigation.PushAsync(new MemorisingStartPage());
        }
        public async void StatisticsButton(object _, EventArgs __)
        {
            await Navigation.PushAsync(new StatPage());
        }
        public async void ProfileButton(object _, EventArgs __)
        {
            await Navigation.PushAsync(new StatPage());
        }
        public async void AboutAppButton(object _, EventArgs __)
        {
            await Navigation.PushAsync(new AboutApp());
        }
        public async void AboutUsButton(object _, EventArgs __)
        {
            await Navigation.PushAsync(new AboutUs());
        }
    }
}
