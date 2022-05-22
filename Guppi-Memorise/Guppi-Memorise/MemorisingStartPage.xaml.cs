using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class MemorisingStartPage : ContentPage
    {
        public MemorisingStartPage()
        {
            InitializeComponent();

            editor.Text = "Я помню чудное мгновенье:\nПередо мной явилась ты,\nКак мимолетное виденье,\nКак гений чистой красоты.\n\nВ томленьях грусти безнадежной,\nВ тревогах шумной суеты,\nЗвучал мне долго голос нежный\nИ снились милые черты.";
        }
        private void ReadyButtonClicked(object sender, EventArgs _)
        {
            var mp = new MemorisingPage(ParseUsersText(editor.Text));
            mp.Disappearing += (__, ___) =>
            {
                DisplayAlert("Ура", "Вы выучили этот текст! Если не можете его вспомнить, советуем запустить заучивание еще раз.", "Ок");
            };
            Navigation.PushAsync(mp);
        }
        private List<List<string>> ParseUsersText(string text)
        {
            List<string> t = text.Split('\n').ToList();
            t = t.Where(i => i != "").ToList();
            t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 4).Select(x => x.Select(v => v.value).ToList());
            
            return t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 4).Select(x => x.Select(v => v.value).ToList()).ToList();
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
    }
}