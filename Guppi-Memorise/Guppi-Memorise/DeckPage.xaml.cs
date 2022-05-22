﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class DeckPage : ContentPage
    {

        private Deck deck;
        private bool isRenaming = false;
        private bool isClicked = false;
        private ObservableCollection<Card> cards;

        public DeckPage(Deck deck)
        {
            InitializeComponent();
            this.deck = deck;
            title.SetBinding(Label.TextProperty, new Binding { Source = deck, Path = "Name"});
            Task.Run(async () => await resetCollection());
        }
        private async Task resetCollection()
        {
            var cardsList = await DB.FetchCards(deck);
            cardsList.Reverse();
            cards = new ObservableCollection<Card>(cardsList);
            Device.BeginInvokeOnMainThread(() =>
            {
                BindableLayout.SetItemsSource(layout, cards);
                var cardsCount = cardsList.Count;
                if (cardsCount > 1)
                {
                    selfControl.IsEnabled = true;
                    sort.IsEnabled = true;
                }
            });
        }
        private void CardTapped(object sender, MR.Gestures.TapEventArgs _)
        {
            if (!isRenaming)
            {
                var cardFrame = sender as Frame;
                var absLayout = cardFrame.Content as AbsoluteLayout;
                var stkLayout = absLayout.Children[1] as StackLayout;
                var titleLabel = stkLayout.Children[0] as Label;
                var textLabelSV = stkLayout.Children[2] as ScrollView;
                
                titleLabel.IsVisible = !titleLabel.IsVisible;
                textLabelSV.IsVisible = !textLabelSV.IsVisible;
            }
        }
        private async void CardLongPress(object sender, MR.Gestures.LongPressEventArgs _)
        {
            var cardFrame = sender as Frame;
            var absLayout = cardFrame.Content as AbsoluteLayout;
            var stkLayout = absLayout.Children[1] as StackLayout;
            var titleLabel = stkLayout.Children[0] as Label;
            if (!isRenaming)
            {
                if (titleLabel.IsVisible)
                {
                    Card tappedCard = cards.Where(i => i.Id == Int32.Parse(cardFrame.ClassId)).FirstOrDefault();
                    string res = await DisplayActionSheet("Выберите действие", "Отмена", "", "Удалить", "Переименовать");
                    switch (res)
                    {
                        case "Удалить":
                            await DB.RemoveCard(tappedCard);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                cards.Remove(tappedCard);
                                if (cards.Count < 2)
                                {
                                    selfControl.IsEnabled = false;
                                    sort.IsEnabled = false;
                                }
                            });
                            break;
                        case "Переименовать":
                            isRenaming = true;
                            var cardView = sender as Frame;
                            RenameCardToggle(cardView);
                            break;
                    }
                }
                else
                {
                    isRenaming = true;
                    ChangeTextToggle(cardFrame);
                }
            }
        }
        private void AddCard(object sender, EventArgs _)
        {
            if (!isRenaming)
            {
                isRenaming = true;
                // Improve threading (probably done)
                var newCard = new Card();
                Task.Run(async () =>
                {
                    await resetCollection();
                    await DB.AddCard(deck, newCard);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        cards.Insert(0, newCard);
                        var card = layout.Children[0] as Frame;
                        if (cards.Count > 1)
                        {
                            selfControl.IsEnabled = true;
                            sort.IsEnabled = true;
                        }
                        RenameCardToggle(card);
                    });
                });
            }
        }
        private void RenameCardToggle(Frame cardFrame)
        {
            var absLayout = cardFrame.Content as AbsoluteLayout;
            // var ratingLabel = absLayout.Children[0] as Label;
            var stkLayout = absLayout.Children[1] as StackLayout;
            var titleLabel = stkLayout.Children[0] as Label;
            var titleEditor = stkLayout.Children[1] as Editor;
            // var textLabel = (stkLayout.Children[2] as ScrollView).Content as Label;
            // var textEditor = (stkLayout.Children[3] as ScrollView).Content as Editor;
            titleLabel.IsVisible = !titleLabel.IsVisible;
            titleEditor.IsVisible = !titleEditor.IsVisible;
            if (titleEditor.IsVisible)
            {
                titleEditor.Focus();
            }
        }
        private void TitleEditorCompleted(object sender, EventArgs _)
        {
            var cardFrame = (((sender as Editor).Parent as StackLayout).Parent as AbsoluteLayout).Parent as Frame;
            var idRaw = cardFrame.ClassId;
            int id = int.Parse(idRaw);
            Card curCard = cards.Where(i => i.Id == id).FirstOrDefault();
            int cardIndex = cards.IndexOf(curCard);
            var newTitle = (sender as Editor).Text;
            cards[cardIndex].Title = newTitle;
            curCard.Title = newTitle;
            Task.Run(async () => await DB.UpdateCard(curCard));
            var absLayout = cardFrame.Content as AbsoluteLayout;
            var stkLayout = absLayout.Children[1] as StackLayout;
            var titleLabel = stkLayout.Children[0] as Label;
            titleLabel.Text = newTitle;
            RenameCardToggle(cardFrame);
            isRenaming = false;
        }
        private void TextEditorCompleted(object sender, EventArgs _)
        {
            var cardFrame = ((((sender as Editor).Parent as ScrollView).Parent as StackLayout).Parent as AbsoluteLayout).Parent as Frame;
            var absLayout = cardFrame.Content as AbsoluteLayout;
            var stkLayout = absLayout.Children[1] as StackLayout;
            var textLabel = (stkLayout.Children[2] as ScrollView).Content as Label;
            var textEditor = (stkLayout.Children[3] as ScrollView).Content as Editor;
            var idRaw = cardFrame.ClassId;
            int id = int.Parse(idRaw);
            Card curCard = cards.Where(i => i.Id == id).FirstOrDefault();
            int cardIndex = cards.IndexOf(curCard);
            var newText = textEditor.Text;
            cards[cardIndex].Text = newText;
            curCard.Text = newText;
            Task.Run(async () => await DB.UpdateCard(curCard));
            textLabel.Text = newText;
            ChangeTextToggle(cardFrame);
            isRenaming = false;
        }
        private void ChangeTextToggle(Frame cardFrame)
        {
            var absLayout = cardFrame.Content as AbsoluteLayout;
            var stkLayout = absLayout.Children[1] as StackLayout;
            var textLabelSV = stkLayout.Children[2] as ScrollView;
            var textEditorSV = stkLayout.Children[3] as ScrollView;

            textLabelSV.IsVisible = !textLabelSV.IsVisible;
            textEditorSV.IsVisible = !textEditorSV.IsVisible;

            if (textEditorSV.IsVisible)
            {
                (textEditorSV.Content as Editor).Focus();
            }
        }
        private void SelfControlButton(object sender, EventArgs _)
        {
            if (!isClicked)
            {
                isClicked = true;
                Navigation.PushAsync(new SelfControlPage(cards));
                isClicked = false;
            }
        }
        private async void SortButton(object sender, EventArgs _)
        {
            var result = await DisplayActionSheet("Отсортировать", "Отмена", "", "По рейтингу запоминания (по возрастанию)", "По рейтингу запоминания (по убыванию)", "По алфавиту (по возрастанию)", "По алфавиту (по убыванию)", "В порядке добавления");
            switch (result)
            {
                case "По рейтингу запоминания (по убыванию)":
                    cards = new ObservableCollection<Card>(cards.OrderByDescending(i => i.Rating));
                    Device.BeginInvokeOnMainThread(() => BindableLayout.SetItemsSource(layout, cards));
                    break;
                case "По рейтингу запоминания (по возрастанию)":
                    cards = new ObservableCollection<Card>(cards.OrderBy(i => i.Rating));
                    Device.BeginInvokeOnMainThread(() => BindableLayout.SetItemsSource(layout, cards));
                    break;
                case "По алфавиту (по возрастанию)":
                    cards = new ObservableCollection<Card>(cards.OrderBy(i => i.Title));
                    Device.BeginInvokeOnMainThread(() => BindableLayout.SetItemsSource(layout, cards));
                    break;
                case "По алфавиту (по убыванию)":
                    cards = new ObservableCollection<Card>(cards.OrderByDescending(i => i.Title));
                    Device.BeginInvokeOnMainThread(() => BindableLayout.SetItemsSource(layout, cards));
                    break;
                case "В порядке добавления":
                    await resetCollection();
                    break;
            }
        }
    }
}