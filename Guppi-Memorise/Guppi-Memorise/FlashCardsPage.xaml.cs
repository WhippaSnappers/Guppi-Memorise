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

        public static ObservableCollection<Deck> decks;

        public FlashCardsPage() {
            InitializeComponent();

            decks = new ObservableCollection<Deck>();
            BindableLayout.SetItemsSource(layout, decks);
        }

        private void DeckTapped(object sender, EventArgs e) {
            Deck tappedDeck = decks.Where(i => i.id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
            Navigation.PushAsync(new DeckPage(ref tappedDeck));
        }

        private void AddDeck(object sender, EventArgs e) {
            decks.Add(new Deck());
        }
    }
}