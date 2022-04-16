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
        private bool isRenaming = false;

        public DeckPage(ref Deck deck) {
            InitializeComponent();
            this.deck = deck;
            BindableLayout.SetItemsSource(layout, deck.cards);
            title.SetBinding(Label.TextProperty, new Binding { Source = deck, Path = "name"}); ;
        }

        private void CardTapped(object sender, MR.Gestures.TapEventArgs e) {
            if (!isRenaming) {
                var slChildren = ((sender as Frame).Content as StackLayout).Children;
                slChildren[0].IsVisible = !slChildren[0].IsVisible;
                slChildren[2].IsVisible = !slChildren[2].IsVisible;
            }
        }

        private void AddCard(object sender, EventArgs e) {
            if (!isRenaming) {
                isRenaming = true;
                var newCard = new Card();
                deck.cards.Add(newCard);
                var card = layout.Children.Where(i => Int32.Parse(i.ClassId) == newCard.id).FirstOrDefault();
                var slChildren = ((card as Frame).Content as StackLayout).Children;
                RenameCardToggle(slChildren);
            }
        }

        private async void Frame_LongPressed(object sender, MR.Gestures.LongPressEventArgs e) {
            if (!isRenaming) {
                var slChildren = ((sender as Frame).Content as StackLayout).Children;
                if (slChildren[0].IsVisible) {
                    Card tappedCard = deck.cards.Where(i => i.id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
                    string res = await DisplayActionSheet("Выберите действие", "Отмена", "", "Удалить", "Переименовать");
                    switch (res) {
                        case "Удалить":
                            deck.cards.Remove(tappedCard);
                            break;
                        case "Переименовать":
                            isRenaming = true;
                            RenameCardToggle(slChildren);
                            break;
                    }
                }
                else {
                    isRenaming = true;
                    ChangeTextToggle(slChildren);
                }
            }
        }

        private void RenameCardToggle(IList<View> slChildren) {
            slChildren[0].IsVisible = !slChildren[0].IsVisible;
            slChildren[1].IsVisible = !slChildren[1].IsVisible;
            if (slChildren[1].IsVisible) {
                (slChildren[1] as Editor).Focus();
            }
        }

        private void ChangeTextToggle(IList<View> slChildren) {
            slChildren[2].IsVisible = !slChildren[2].IsVisible;
            slChildren[3].IsVisible = !slChildren[3].IsVisible;
            if (slChildren[3].IsVisible) {
                ((slChildren[3] as ScrollView).Content as Editor).Focus();
            }
        }

        private void Editor_Completed(object sender, EventArgs e) {
            var slChildren = ((sender as Editor).Parent as StackLayout).Children;
            RenameCardToggle(slChildren);
            isRenaming = false;
        }

        private void Editor_Completed1(object sender, EventArgs e) {
            var slChildren = (((sender as Editor).Parent as ScrollView).Parent as StackLayout).Children;
            ChangeTextToggle(slChildren);
            isRenaming = false;
        }
    }
}