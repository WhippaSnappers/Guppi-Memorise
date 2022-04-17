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
        private int selectedIndex;

        public SelfControlPage(Deck deck) {
            this.deck = deck;
            ShuffleDeck(ref this.deck);
            InitializeComponent();
            selectedIndex = 0;
            setCard(0);
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
                setCard(--selectedIndex);
                toggleBtns();
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e) {
            if (selectedIndex < deck.cards.Count - 1) {
                setCard(++selectedIndex);
                toggleBtns();
            }
        }

        private void CardTapped(object sender, EventArgs e) {
            var slChildren = ((sender as Frame).Content as StackLayout).Children;
            slChildren[0].IsVisible = !slChildren[0].IsVisible;
            slChildren[1].IsVisible = !slChildren[1].IsVisible;
        }

        private void setCard(int index) {
            ((frame.Content as StackLayout).Children[0] as Label).Text = deck.cards[index].title;
            (((frame.Content as StackLayout).Children[1] as ScrollView).Content as Label).Text = deck.cards[index].text;
        }

        private void toggleBtns() {
            if (selectedIndex == 0) {
                prev.IsEnabled = false;
            }
            if (selectedIndex == deck.cards.Count - 1) {
                next.IsEnabled = false;
            }
            if (selectedIndex > 0) {
                prev.IsEnabled = true;
            }
            if (selectedIndex < deck.cards.Count - 1) {
                next.IsEnabled = true;
            }
        }
    }
}