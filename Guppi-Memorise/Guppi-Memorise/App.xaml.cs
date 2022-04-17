using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var navbar = new NavigationPage(new ProfilePage());
            MainPage = navbar;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
