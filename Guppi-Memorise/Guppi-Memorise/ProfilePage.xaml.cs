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
    public partial class ProfilePage : ContentPage
    {
        private int createDecks = 0;
        private int createFlashCards = 0;
        private int downloadText = 0;
        private int shareDecks = 0;
        public ProfilePage()
        {
            InitializeComponent();

           // loginEntry = new Entry;
           // loginEntry.TextChanged+=loginEntry_TextChanged;
        }
    }
}