using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class MemorisingHistory : ContentPage
    {
        private ObservableCollection<Text> texts;
        public MemorisingHistory()
        {
            InitializeComponent();
            RefreshTextsCollection();
        }
        private void RefreshTextsCollection()
        {
            Task.Run(async () =>
            {
                // await DB.AddDummyTexts();
                var texts = await DB.FetchTexts();
                this.texts = new ObservableCollection<Text>(texts);
                Device.BeginInvokeOnMainThread(() => BindableLayout.SetItemsSource(layout, this.texts));
            });
        }
        private void Repeat(object sender, EventArgs _)
        {
            int textId = int.Parse((sender as Frame).ClassId);
            Text selectedText = texts.Single(t => t.Id == textId);
            var mp = new MemorisingPage(selectedText);
            mp.Disappearing += (__, ___) =>
            {
                if (MemorisingStartPage.isLearned)
                {
                    DisplayAlert("Ура", "Вы повторили свой успех! Если необходимо, можете попробовать еще раз.", "Ок");
                    MemorisingStartPage.isLearned = false;
                }
                RefreshTextsCollection();
            };
            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(mp));
        }//ура костыль (really?)

        private void ClearHistory(object sender, EventArgs _)
        {
            Task.Run(async () =>
            {
                await DB.PurgeTexts();
                Device.BeginInvokeOnMainThread(() => texts.Clear());
            });
        }
    }
}