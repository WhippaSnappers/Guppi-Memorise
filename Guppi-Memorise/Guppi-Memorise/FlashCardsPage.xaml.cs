using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise {
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class FlashCardsPage : ContentPage {

        public static ObservableCollection<Deck> decks;
        private static bool isRenaming = false;
        private static bool isClicked = false;

        public FlashCardsPage() {
            InitializeComponent();
            BindableLayout.SetItemsSource(layout, decks);

            Task.Run(async () =>
            {
                var decksList = await DB.FetchDecks();
                decks = new ObservableCollection<Deck>(decksList);

            });
        }

        private void RenameCard(object sender) {
            isRenaming = true;
            var sl = (sender as Frame).Content as StackLayout;
            (sl.Children[0] as Label).IsVisible = false;
            (sl.Children[1] as Editor).IsVisible = true;
            (sl.Children[1] as Editor).Focus();
        }

        private void DeckTapped(object sender, MR.Gestures.TapEventArgs e) {
            if (!isRenaming) {
                if (!isClicked) {
                    Deck tappedDeck = decks.Where(i => i.Id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
                    isClicked = true;
                    Navigation.PushAsync(new DeckPage(tappedDeck));
                    isClicked = false;
                }
            }
        }

        private async void AddDeck(object sender, EventArgs e) {
            if (!isRenaming) {
                var newDeck = new Deck();
                await DB.AddDeck(newDeck);
                decks.Add(newDeck);



                var deck = layout.Children.FirstOrDefault(i => Int32.Parse(i.ClassId) == newDeck.Id);
                //RenameCard(newDeck);
            }
        }

        private async void Frame_LongPressed(object sender, MR.Gestures.LongPressEventArgs e) {
            if (!isRenaming) {
                Deck tappedDeck = decks.Where(i => i.Id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
                string res = await DisplayActionSheet("Выберите действие", "Отмена", "", "Удалить", "Переименовать");
                switch (res) {
                    case "Удалить": 
                        decks.Remove(tappedDeck);
                        break;
                    case "Переименовать":
                        RenameCard(sender);
                        break;
                }
            }
        }

        private void Editor_Completed(object sender, EventArgs e) {
            isRenaming = false;
            var sl = (sender as Editor).Parent as StackLayout;
            (sl.Children[0] as Label).IsVisible = true;
            (sl.Children[1] as Editor).IsVisible = false;
        }
    }
}