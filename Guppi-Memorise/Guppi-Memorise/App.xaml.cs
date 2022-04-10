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

<<<<<<< HEAD
            //var aboutapp = new NavigationPage(new AboutApp());
            //MainPage = aboutapp;
            var navbar = new NavigationPage(new MainPage());
            MainPage = navbar;
=======
            var aboutapp = new NavigationPage(new MainPage());
            MainPage = aboutapp;
            //var navbar = new NavigationPage(new MainPage());
            //MainPage = navbar;
>>>>>>> a2391196a1637a1adbac78d5a7d4a912bd500b6c
            //navbar.BarBackgroundColor = Color.FromRgb(0, 205, 167);
            //navbar.BarTextColor = Color.FromRgb(1, 37, 69);
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
