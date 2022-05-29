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
        public static bool isLearned = false;

        public MemorisingStartPage()
        {
            isLearned = false;
            InitializeComponent();

            editor.Text = "Я помню чудное мгновенье:\nПередо мной явилась ты,\nКак мимолетное виденье,\nКак гений чистой красоты.\n\nВ томленьях грусти безнадежной,\nВ тревогах шумной суеты,\nЗвучал мне долго голос нежный\nИ снились милые черты.";
        }
        private void ReadyButtonClicked(object sender, EventArgs _)
        {
            var mp = new MemorisingPage(ParseUsersText(editor.Text));
            mp.Disappearing += (__, ___) =>
            {
                if (isLearned) {
                    DisplayAlert("Ура", "Вы выучили этот текст! Если не можете его вспомнить, советуем запустить заучивание еще раз.", "Ок");
                    isLearned = false;
                }   
            };
            Navigation.PushAsync(mp);
        }
        public static List<List<string>> ParseUsersText(string text)
        {
            List<string> t = text.Split('\n').Where(i => i != "").ToList();

            if (isPoem(t)) {
                //t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 4).Select(x => x.Select(v => v.value).ToList());
            
                return t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 4).Select(x => x.Select(v => v.value).ToList()).ToList();
            }
            else {
                t = text.Split(' ', '\n').Where(i => i != "").ToList();
                List<List<string>> result = t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 20).Select(x => x.Select(v => v.value).ToList()).ToList();
                result.ForEach(i => {
                    List<List<string>> temp = i.Select((x, inx) => new { index = inx, value = x }).GroupBy(x => x.index / 5).Select(x => x.Select(v => v.value).ToList()).ToList();
                    i.Clear();
                    for (int j = 0; j < temp.Count; j++) {
                        i.Add(String.Join(" ", temp[j]));
                    }
                });
                if (result.Last().Count == 1 && result.Count >= 2) {
                    result[result.Count - 2].Add(result.Last()[0]);
                    result.Remove(result.Last());
                }

                return result;
            }
        }

        private static bool isPoem(List<string> text) {
            bool isPoem = true;
            foreach (var line in text) {
                if (line.Length >= 50) {
                    isPoem = false;
                    break;
                }
            }
            return isPoem;
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

        private void OpenHistory(object sender, EventArgs _) {
            Navigation.PushAsync(new MemorisingHistory());
        }
    }
}