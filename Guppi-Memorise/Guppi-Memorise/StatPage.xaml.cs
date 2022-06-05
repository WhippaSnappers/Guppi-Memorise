using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

namespace Guppi_Memorise
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatPage : ContentPage
    {
        private int _decksCreated;
        private int _cardsCreated;
        private int _textsEntered;
        private int _textsLearned;
        private string _minimalLearningTime;

        public StatPage()
        {
            InitializeComponent();
            InitStats();
        }
        private void InitStats()
        {
            Task.Run(async () =>
            {
                _decksCreated = await DB.CountDecksTotal();
                _cardsCreated = await DB.CountCardsTotal();
                _textsEntered = await DB.FetchNumberOfTextsEntered();
                _textsLearned = await DB.CountTextsTotal();
                _minimalLearningTime = await DB.FetchMinimalLearningTime();
                Device.BeginInvokeOnMainThread(() =>
                {
                    decksCreated.Text = _decksCreated.ToString();
                    cardsCreated.Text = _cardsCreated.ToString();
                    textsEntered.Text = _textsEntered.ToString();
                    textsLearned.Text = _textsLearned.ToString();
                    minimalLearningTime.Text = _minimalLearningTime;
                });
            });
        }

        private async void ClearStats(object _, EventArgs __)
        {
            bool res = await DisplayAlert("Вы уверены?", "Будут удалены все ваши колоды и карточки, а также очищена история текстов.", "Ок", "Отмена");
            if (res) {
                await DB.PurgeUserData();
                Device.BeginInvokeOnMainThread(() =>
                {
                    decksCreated.Text = "0";
                    cardsCreated.Text = "0";
                    textsEntered.Text = "0";
                    textsLearned.Text = "0";
                    minimalLearningTime.Text = "--:--:--";
                });
            }
        }
    }
}