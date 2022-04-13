using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise {
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class DeckPage : ContentPage {

        private Deck deck;

        public DeckPage(ref Deck deck) {
            InitializeComponent();
            this.deck = deck;
            BindableLayout.SetItemsSource(layout, deck.cards);
            title.SetBinding(Label.TextProperty, new Binding { Source = deck, Path = "name"}); ;
        }

        private void CardTapped(object sender, EventArgs e) {

        }

        private void AddCard(object sender, EventArgs e) {
            deck.cards.Add(new Card());
        }
    }
}