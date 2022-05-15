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

    public partial class DeckPage : ContentPage {

        private Deck deck;
        private bool isRenaming = false;
        private bool isClicked = false;
        private ObservableCollection<Card> cards;

        public DeckPage(Deck deck) {
            InitializeComponent();
            this.deck = deck;
            BindableLayout.SetItemsSource(layout, cards);
            title.SetBinding(Label.TextProperty, new Binding { Source = deck, Path = "Name"});
            Task.Run(async () =>
            {
                var cardsList = await DB.FetchCards(deck);
                cards = new ObservableCollection<Card>(cardsList);
                var cardsCount = cardsList.Count;
                if (cardsCount > 1)
                {
                    selfControl.IsEnabled = true;
                    sort.IsEnabled = true;
                }
            });
        }
        private void CardTapped(object sender, MR.Gestures.TapEventArgs e) {
            if (!isRenaming) {
                var slChildren = (((sender as Frame).Content as AbsoluteLayout).Children[1] as StackLayout).Children;
                slChildren[0].IsVisible = !slChildren[0].IsVisible;
                slChildren[2].IsVisible = !slChildren[2].IsVisible;
            }
        }
        private async void AddCard(object sender, EventArgs e) {
            if (!isRenaming) {
                isRenaming = true;
                var newCard = new Card() { DeckId = deck.Id};
                await DB.AddCard(deck, newCard);
                cards.Add(newCard);
                var card = layout.Children.Where(i => Int32.Parse(i.ClassId) == newCard.Id).FirstOrDefault();
                var slChildren = (((card as Frame).Content as AbsoluteLayout).Children[1] as StackLayout).Children;

                if (cards.Count > 1) {
                    selfControl.IsEnabled = true;
                    sort.IsEnabled = true;
                }

                RenameCardToggle(slChildren);
            }
        }
        private async void Frame_LongPressed(object sender, MR.Gestures.LongPressEventArgs e) {
            if (!isRenaming) {
                var slChildren = (((sender as Frame).Content as AbsoluteLayout).Children[1] as StackLayout).Children;
                if (slChildren[0].IsVisible) {
                    Card tappedCard = cards.Where(i => i.Id == Int32.Parse((sender as Frame).ClassId)).FirstOrDefault();
                    string res = await DisplayActionSheet("Выберите действие", "Отмена", "", "Удалить", "Переименовать");
                    switch (res) {
                        case "Удалить":
                            cards.Remove(tappedCard);
                            await DB.RemoveCard(tappedCard);
                            if (cards.Count < 2) {
                                selfControl.IsEnabled = false;
                            }
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
        private void Button_Clicked(object sender, EventArgs e) {
            if (!isClicked) {
                isClicked = true;
                Navigation.PushAsync(new SelfControlPage(cards));
                isClicked = false;
            }
        }
        private async void SortBtn(object sender, EventArgs e) {
            var result = await DisplayActionSheet("Отсортировать", "Отмена", "", "По рейтингу запоминания (по возрастанию)", "По рейтингу запоминания (по убыванию)", "По алфавиту (по возрастанию)", "По алфавиту (по убыванию)", "В порядке добавления");
            switch (result) {
                case "По рейтингу запоминания (по убыванию)":
                    cards = new ObservableCollection<Card>(cards.OrderByDescending(i => i.Rating));
                    BindableLayout.SetItemsSource(layout, cards);
                    BindableLayout.SetItemsSource(layout, cards);
                    break;
                case "По рейтингу запоминания (по возрастанию)":
                    cards = new ObservableCollection<Card>(cards.OrderBy(i => i.Rating));
                    BindableLayout.SetItemsSource(layout, cards);
                    break;
                case "По алфавиту (по возрастанию)":
                    cards = new ObservableCollection<Card>(cards.OrderBy(i => i.Title));
                    BindableLayout.SetItemsSource(layout, cards);
                    break;
                case "По алфавиту (по убыванию)":
                    cards = new ObservableCollection<Card>(cards.OrderByDescending(i => i.Title));
                    BindableLayout.SetItemsSource(layout, cards);
                    break;
                case "В порядке добавления":
                    cards = new ObservableCollection<Card>(cards.OrderByDescending(i => i.Id));
                    BindableLayout.SetItemsSource(layout, cards);
                    break;
            }
        }
    }
}