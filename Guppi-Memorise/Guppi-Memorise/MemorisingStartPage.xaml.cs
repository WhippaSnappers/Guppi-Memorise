using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class MemorisingStartPage : ContentPage
    {
        public static bool isLearned = false;

        public MemorisingStartPage()
        {
            isLearned = false;
            InitializeComponent();

            editor.Text = "Я помню чудное мгновенье:\nПередо мной явилась ты,\nКак мимолетное виденье,\nКак гений чистой красоты.\n\nВ томленьях грусти безнадежной,\nВ тревогах шумной суеты,\nЗвучал мне долго голос нежный\nИ снились милые черты.";
        }
        private void ReadyButtonClicked(object sender, EventArgs _)
        {
            Text userText = new Text { Body = editor.Text, Time = "00:00:00" };
            Task.Run(async () =>
            {
                await DB.AddText(userText);
                var mp = new MemorisingPage(userText);
                mp.Disappearing += (__, ___) =>
                {
                    if (isLearned)
                    {
                        DisplayAlert("Ура", "Вы выучили этот текст! Если не можете его вспомнить, советуем запустить заучивание еще раз.", "Ок");
                        isLearned = false;
                    }
                };
                Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(mp));
            });

            /*var mp = new MemorisingPage(TextUtils.ParseUsersText(editor.Text));
            mp.Disappearing += (__, ___) =>
            {
                if (isLearned) {
                    DisplayAlert("Ура", "Вы выучили этот текст! Если не можете его вспомнить, советуем запустить заучивание еще раз.", "Ок");
                    isLearned = false;
                }   
            };
            Navigation.PushAsync(mp);
            */
        }
        private void TextEditorCompleted(object sender, EventArgs _)
        {
            if ((sender as Editor).Text.Length > 0)
            {
                btn.IsEnabled = true;
            }
            else
            {
                btn.IsEnabled = false;
            }
        }
        private void OpenHistory(object sender, EventArgs _)
        {
            Navigation.PushAsync(new MemorisingHistory());
        }
    }
}