using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            var stkLayout = (sender as Frame).Content as StackLayout;
            (stkLayout.Children[0] as Label).IsVisible = false;
            (stkLayout.Children[1] as Editor).IsVisible = true;
            (stkLayout.Children[1] as Editor).Placeholder = (stkLayout.Children[0] as Label).Text;
            (stkLayout.Children[1] as Editor).Text = "";
            (stkLayout.Children[1] as Editor).Focus();
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
                    isClicked = true;
                    Deck tappedDeck = decks.Where(i => i.Id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
                    Navigation.PushAsync(new DeckPage(tappedDeck));
                    isClicked = false;
                }
            }
        }
        private void AddDeck(object sender, EventArgs _)
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
                        //var children = ((deck as Frame).Content as StackLayout).Children;
                        //(children[0] as Label).SetBinding(Label.TextProperty, new Binding { Source = children[1] as Editor, Path = "Text"});
                        RenameDeck(deck);
                    });
                });
            }
        }
        private async void DeckDoubleTapped(object sender, EventArgs _)
        {
            if (!isRenaming) {
                Deck tappedDeck = decks.Where(i => i.Id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
                string res = await DisplayActionSheet("Выберите действие", "Отмена", "", "Переименовать", "Удалить");
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
        private void NameEditorCompleted(object sender, EventArgs _)
        {
            var stkLayout = (sender as Editor).Parent as StackLayout;
            string idRaw = (sender as Editor).ClassId;
            int id = Int32.Parse(idRaw);
            Deck curDeck = decks.Where(i => i.Id == id).FirstOrDefault();
            int curDeckIndex = decks.IndexOf(curDeck);
            string newName = (sender as Editor).Text;

            if (string.IsNullOrWhiteSpace((sender as Editor).Text) || (sender as Editor).Text.Contains("\n")) {
                //if ((stkLayout.Children[0] as Label).Text == "") {
                //    newName = "Новая колода";
                //}
                //else {
                    newName = (stkLayout.Children[0] as Label).Text;
                //}
            }

            Device.BeginInvokeOnMainThread(() => {
                decks[curDeckIndex].Name = newName;
                curDeck.Name = newName;
                Task.Run(async () => await DB.UpdateDeck(curDeck));
                (stkLayout.Children[0] as Label).Text = newName;
                (stkLayout.Children[0] as Label).IsVisible = true;
                (stkLayout.Children[1] as Editor).IsVisible = false;
                isRenaming = false;
            });
        }
    }
}