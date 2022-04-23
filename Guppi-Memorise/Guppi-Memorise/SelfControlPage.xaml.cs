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

    public partial class SelfControlPage : ContentPage {

        private Card[] cards;
        private int selectedIndex;

        public SelfControlPage(ObservableCollection<Card> cards) {
            this.cards = new Card[cards.Count];
            cards.CopyTo(this.cards, 0);
            ShuffleDeck(this.cards);
            InitializeComponent();
            selectedIndex = 0;
            setCard(0);
        }

        private void ShuffleDeck(Card[] cards) {
            Random rnd = new Random();
            for (int i = cards.Length - 1; i >= 1; i--) {
                int index = rnd.Next(i + 1);
                var temp = cards[index];
                cards[index] = cards[i];
                cards[i] = temp;
            }
        }

        private void Button_Clicked(object sender, EventArgs e) {
            if (selectedIndex > 0) {
                setCard(--selectedIndex);
                toggleBtns();
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e) {
            if (selectedIndex < cards.Length - 1) {
                setCard(++selectedIndex);
                toggleBtns();
            }
        }

        private void CardTapped(object sender, MR.Gestures.TapEventArgs e) {
            var slChildren = ((sender as Frame).Content as StackLayout).Children;
            slChildren[0].IsVisible = !slChildren[0].IsVisible;
            slChildren[1].IsVisible = !slChildren[1].IsVisible;
        }

        private void setCard(int index) {
            ((frame.Content as StackLayout).Children[0] as Label).Text = cards[index].title;
            (((frame.Content as StackLayout).Children[1] as ScrollView).Content as Label).Text = cards[index].text;
        }

        private void toggleBtns() {
            if (selectedIndex == 0) {
                prev.IsEnabled = false;
            }
            if (selectedIndex == cards.Length - 1) {
                next.IsEnabled = false;
            }
            if (selectedIndex > 0) {
                prev.IsEnabled = true;
            }
            if (selectedIndex < cards.Length - 1) {
                next.IsEnabled = true;
            }
        }
    }
}