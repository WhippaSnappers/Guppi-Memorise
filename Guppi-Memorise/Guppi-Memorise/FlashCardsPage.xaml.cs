using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;

namespace Guppi_Memorise
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class FlashCardsPage : ContentPage
    {
        public static ObservableCollection<Deck> decks;
        private static bool isRenaming = false;
        private static bool isClicked = false;

        public FlashCardsPage()
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                var decksList = await DB.FetchDecks();
                decksList.Reverse();
                decks = new ObservableCollection<Deck>(decksList);
                Device.BeginInvokeOnMainThread(() => BindableLayout.SetItemsSource(layout, decks));
            });
        }
        private void RenameDeck(object sender)
        {
            isRenaming = true;
            var sl = (sender as Frame).Content as StackLayout;
            (sl.Children[0] as Label).IsVisible = false;
            (sl.Children[1] as Editor).IsVisible = true;
            (sl.Children[1] as Editor).Focus();
        }
        private void RemoveDeck(Deck deck)
        {
            decks.Remove(deck);
            Task.Run(async () => await DB.RemoveDeck(deck));
        }
        private void DeckTapped(object sender, EventArgs _)
        {
            if (!isRenaming) {
                if (!isClicked) {
                    Deck tappedDeck = decks.Where(i => i.Id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
                    isClicked = true;
                    Navigation.PushAsync(new DeckPage(tappedDeck));
                    isClicked = false;
                }
            }
        }
        private void AddDeck(object sender, EventArgs e)
        {
            if (!isRenaming) {
                var newDeck = new Deck();
                Task.Run(async () =>
                {
                    await DB.AddDeck(newDeck);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        decks.Insert(0, newDeck);
                        var deck = layout.Children[0];
                        RenameDeck(deck);
                    });
                });
            }
        }
        private async void Frame_LongPressed(object sender, MR.Gestures.LongPressEventArgs e)
        {
            if (!isRenaming) {
                Deck tappedDeck = decks.Where(i => i.Id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
                string res = await DisplayActionSheet("Выберите действие", "Отмена", "", "Удалить", "Переименовать");
                switch (res) {
                    case "Удалить": 
                        RemoveDeck(tappedDeck);
                        break;
                    case "Переименовать":
                        RenameDeck(sender);
                        break;
                }
            }
        }
        private void Editor_Completed(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                string idRaw = (sender as Editor).ClassId;
                int id = Int32.Parse(idRaw);
                string newName = (sender as Editor).Text;
                Deck curDeck = decks.Where(i => i.Id == id).FirstOrDefault();
                int curDeckIndex = decks.IndexOf(curDeck);
                decks[curDeckIndex].Name = newName;
                curDeck.Name = newName;
                Task.Run(async () => await DB.UpdateDeck(curDeck));
                var sl = (sender as Editor).Parent as StackLayout;
                (sl.Children[0] as Label).Text = newName;
                (sl.Children[0] as Label).IsVisible = true;
                (sl.Children[1] as Editor).IsVisible = false;
                isRenaming = false;
            });
        }
    }
}