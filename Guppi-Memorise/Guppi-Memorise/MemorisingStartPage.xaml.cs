using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise {
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class MemorisingStartPage : ContentPage {

        public MemorisingStartPage() {
            InitializeComponent();
            editor.Text = "Я помню чудное мгновенье:\nПередо мной явилась ты,\nКак мимолетное виденье,\nКак гений чистой красоты.\n\nВ томленьях грусти безнадежной,\nВ тревогах шумной суеты,\nЗвучал мне долго голос нежный\nИ снились милые черты.";
        }

        private void Button_Clicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MemorisingPage(parseUsersText(editor.Text)));
        }

        private List<List<string>> parseUsersText(string text) {
            List<string> t = text.Split('\n').ToList();
            t = t.Where(i => i != "").ToList();
            t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 4).Select(x => x.Select(v => v.value).ToList());
            
            return t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 4).Select(x => x.Select(v => v.value).ToList()).ToList();
        }
    }
}