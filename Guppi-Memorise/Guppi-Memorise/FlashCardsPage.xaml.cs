using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise {
	[XamlCompilation(XamlCompilationOptions.Compile)]


    public partial class FlashCardsPage : ContentPage {

        public ObservableCollection<Deck> decks;

        public FlashCardsPage() {
            InitializeComponent();
            decks = new ObservableCollection<Deck>() {
                new Deck("new deck"), new Deck("new deck 2")
            };
            BindableLayout.SetItemsSource(layout, decks);
            Console.WriteLine("11");
        }
    }
}