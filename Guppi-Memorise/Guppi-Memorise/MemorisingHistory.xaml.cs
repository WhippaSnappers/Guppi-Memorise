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

    public partial class MemorisingHistory : ContentPage {

        struct historyElement { //для теста
            public string time { get; set; }
            public string text { get; set; }
        }

        public MemorisingHistory() {
            InitializeComponent();
            BindableLayout.SetItemsSource(layout, new ObservableCollection<historyElement>() {
                new historyElement { time = "00:12:33", text = "Я помню чудное мгновенье:\nПередо мной явилась ты,\nКак мимолетное виденье,\nКак гений чистой красоты.\n\nВ томленьях грусти безнадежной,\nВ тревогах шумной суеты,\nЗвучал мне долго голос нежный\nИ снились милые черты."},
                new historyElement { time = "01:24:48", text = "Я помню чудное мгновенье:\nПередо мной явилась ты,\nКак мимолетное виденье,\nКак гений чистой красоты.\n\nВ томленьях грусти безнадежной,\nВ тревогах шумной суеты,\nЗвучал мне долго голос нежный\nИ снились милые черты."},
                new historyElement { time = "51:53:24", text = "111111111111111111111111\nединчика!!!\n1111111111111111111111111111111111111111111111\n1111111111111111111111111111111111111111111111111\n1111111111111111111111111111111\n1111111111111 111111111111\n11111111111111111111111111\n1111111111111111111111111111\n1111111111111111111111\n1111111111111111111111\n1111111111 111111111111111111111111111\n11111111111111111111111111111111111111111111\n1111111111111111111111111111111111111111111111111111111111\n11111111111111 единичка11"}
            });
        }

        private void Repeat(object sender, EventArgs e) {
            var mp = new MemorisingPage(MemorisingStartPage.ParseUsersText((((sender as Frame).Content as StackLayout).Children[0] as Label).Text));
            mp.Disappearing += (__, ___) => {
                if (MemorisingStartPage.isLearned) {
                    DisplayAlert("Ура", "Вы повторили свой успех! Если необходимо, можете попробовать еще раз.", "Ок");
                    MemorisingStartPage.isLearned = false;
                }
            };
            Navigation.PushAsync(mp);
        }//ура костыль

        private void ClearHistory(object sender, EventArgs e) {

        }
    }
}