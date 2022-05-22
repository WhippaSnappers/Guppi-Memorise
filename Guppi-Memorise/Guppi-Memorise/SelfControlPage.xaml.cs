using System;
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

    public partial class SelfControlPage : ContentPage
    {
        private List<Card> cards;
        private List<bool> cardsRated;
        private int selectedIndex;

        public SelfControlPage(ObservableCollection<Card> cards)
        {
            InitializeComponent();
            
            this.cards = new List<Card>(cards);
            cardsRated = Enumerable.Repeat(false, cards.Count).ToList();
            var rand = new Random();
            this.cards = this.cards.OrderBy(x => rand.Next()).ToList();
            selectedIndex = 0;
            SetCard(0);
            ToggleBtns();
        }
        private void PrevBtn(object sender, EventArgs _)
        {
            if (selectedIndex > 0)
            {
                SetCard(--selectedIndex);
                ToggleBtns();
            }
        }
        private void NextBtn(object sender, EventArgs _)
        {
            if (selectedIndex < cards.Count - 1)
            {
                SetCard(++selectedIndex);
                ToggleBtns();
            }
        }
        private void CardTapped(object sender, MR.Gestures.TapEventArgs _)
        {
            var slChildren = ((sender as Frame).Content as StackLayout).Children;
            slChildren[0].IsVisible = !slChildren[0].IsVisible;
            slChildren[1].IsVisible = !slChildren[1].IsVisible;
        }
        private void SetCard(int index)
        {
            var slChildren = (frame.Content as StackLayout).Children;
            (slChildren[0] as Label).Text = cards[index].Title;
            ((slChildren[1] as ScrollView).Content as Label).Text = cards[index].Text;
            ToggleRatingBtns(!cardsRated[index]);
        }
        private void ToggleBtns()
        {
            if (selectedIndex == 0)
            {
                prev.IsEnabled = false;
            }
            if (selectedIndex > 0)
            {
                prev.IsEnabled = true;
            }
            if (selectedIndex == cards.Count - 1 || !cardsRated[selectedIndex])
            {
                next.IsEnabled = false;
            }
            if (selectedIndex < cards.Count - 1 && cardsRated[selectedIndex])
            {
                next.IsEnabled = true;
            }
        }
        private async void RatingPlus(object sender, EventArgs _)
        {
            cards[selectedIndex].Rating++;
            await DB.UpdateCard(cards[selectedIndex]);
            cardsRated[selectedIndex] = true;
            ToggleRatingBtns(false);
            ToggleBtns();
        }
        private async void RatingMinus(object sender, EventArgs _)
        {
            cards[selectedIndex].Rating--;
            await DB.UpdateCard(cards[selectedIndex]);
            cardsRated[selectedIndex] = true;
            ToggleRatingBtns(false);
            ToggleBtns();
        }
        private void ToggleRatingBtns(bool flag)
        {
            ratingMinus.IsEnabled = flag;
            ratingPlus.IsEnabled = flag;
        }
    }
}