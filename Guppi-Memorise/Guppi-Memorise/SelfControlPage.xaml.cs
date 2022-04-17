using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class SelfControlPage : ContentPage {

        private Deck deck;
        private Card selectedCard;
        private int selectedIndex;

        public SelfControlPage(Deck deck) {
            this.deck = deck;
            ShuffleDeck(ref this.deck);
            InitializeComponent();
            selectedIndex = 0;
            selectedCard = deck.cards[selectedIndex];
            frame.BindingContext = selectedCard;
        }

        private void ShuffleDeck(ref Deck deck) {
            Random rnd = new Random();
            for (int i = deck.cards.Count - 1; i >= 1; i--) {
                int index = rnd.Next(i + 1);
                var temp = deck.cards[index];
                deck.cards[index] = deck.cards[i];
                deck.cards[i] = temp;
            }
        }

        private void Button_Clicked(object sender, EventArgs e) {
            if (selectedIndex > 0) {
                selectedCard = deck.cards[--selectedIndex];
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e) {
            if (selectedIndex < deck.cards.Count) {
                selectedCard = deck.cards[++selectedIndex];
            }
        }

        private void CardTapped(object sender, EventArgs e) {
            var slChildren = ((sender as Frame).Content as StackLayout).Children;
            slChildren[0].IsVisible = !slChildren[0].IsVisible;
            slChildren[1].IsVisible = !slChildren[1].IsVisible;
        }
    }
}