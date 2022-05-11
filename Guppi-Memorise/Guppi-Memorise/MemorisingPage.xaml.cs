using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise {
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class MemorisingPage : ContentPage {
        public List<List<string>> startText { get; set; }
        public List<List<string>> boundText;

        private bool isEnded = false;

        private int currentLine = 0;
        private int currentExtract = 0;
        private int mistakes = 0;
        private int done = 0;
        private int level = 1;

        private List<TimeSpan> timeArray;
        private DateTime startTime;

        public MemorisingPage(List<List<string>> str) {
            InitializeComponent();
            startText = str;
            boundText = str;
            timeArray = new List<TimeSpan>(6);
            BindableLayout.SetItemsSource(window, startText[currentExtract]);
            BindableLayout.SetItemsSource(answers, ShuffleLines(startText[currentExtract]));
        }

        private void OpenInfo(object sender, EventArgs e) {
            switch (level) {
                case 1:
                    DisplayAlert("Инфо", "На данном этапе текст разбивается на фрагменты. Ваша задача - собрать их в правильном порядке, в конце концов получив полный текст.", "Ок");
                    break;
                case 2:
                    DisplayAlert("Инфо", "На этом этапе ваша задача усложняется: отрывки становятся в два раза больше, и вам также нужно их восстановить из фрагментов.", "Ок");
                    break;
                case 3:
                    DisplayAlert("Инфо", "Ваша задача не изменилась, однако программа запомнила среднее время, за которое вы собирали отрывки, и теперь вам нужно снова собрать текст, уложившись в это время, таймер вы можете увидеть в верхней части экрана.", "Ок");
                    break;
                case 4:
                    DisplayAlert("Инфо", "Lorem ipsum dolor sit amet", "Ок");
                    break;
                case 5:
                    DisplayAlert("Инфо", "Lorem ipsum dolor sit amet", "Ок");
                    break;
                case 6:
                    DisplayAlert("Инфо", "Lorem ipsum dolor sit amet", "Ок");
                    break;
            }
        }

        private void LineTapped(object sender, EventArgs e) {
            if (level < 4) {
                LinesLevel(sender, boundText);
            }
        }

        private void Again(object sender, EventArgs e) {
            Reset();
        }

        private void Reset() {
            currentExtract = 0;
            currentLine = 0;
            mistakes = 0;
            done = 0;
            isEnded = true;

            if (timeArray.Count >= level) {
                timeArray.RemoveAt(level - 1);
            }

            doneCounter.Text = "Верно: 0 / 2";
            mistakesCounter.Text = "Ошибок: 0";
            again.IsEnabled = false;

            BindableLayout.SetItemsSource(window, boundText[currentExtract]);
            BindableLayout.SetItemsSource(answers, ShuffleLines(boundText[currentExtract]));

            if (level == 3 || level == 6) {
                int average = (timeArray[level - 3].Seconds + timeArray[level - 2].Seconds) / 2;
                timer.Text = String.Format("{0:00}:{1:00}", average / 60, average % 60);
            }

            foreach (var item in window.Children) {
                item.Opacity = 0;
            }
            foreach (var item in answers.Children) {
                item.Opacity = 1;
            }
        }

        private List<string> ShuffleLines(List<string> text) {
            string[] lines = new string[text.Count];
            text.CopyTo(lines);
            Random rnd = new Random();
            for (int i = lines.Length - 1; i >= 1; i--) {
                int index = rnd.Next(i + 1);
                var temp = lines[index];
                lines[index] = lines[i];
                lines[i] = temp;
            }
            return lines.ToList();
        }

        private List<List<string>> JoinLines (List<List<string>> lines) {
            List<List<string>> result = new List<List<string>>();
            for (int i = 0; i < lines.Count; i += 2) {
                if (lines[i + 1] != null) {
                    result.Add(lines[i].Concat(lines[i + 1]).ToList());
                }
            }
            return result;
        }

        private void LinesLevel(object sender, List<List<string>> lines) {

            if ((sender as Frame).Opacity != 0) {

                if (currentLine == 0 && currentExtract == 0) {
                    startTime = DateTime.Now;
                    
                    isEnded = false;
                    if (level == 3 || level == 6) {
                        int time = timeArray[level == 3 ? 0 : 3].Seconds;

                        Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                            Device.BeginInvokeOnMainThread(() => {
                                if (!isEnded) {
                                    timer.Text = String.Format("{0:00}:{1:00}", time / 60, time % 60);
                                }
                            });
                            time--;
                            if (time == 0) {
                                DisplayAlert("Ой-ой", "Кажется, вы не успели :( Пожалуйста, попробуйте еще раз.", "Ок");
                                Reset();
                                return false;
                            }
                            return !isEnded;
                        });
                    }
                }

                foreach (var item in answers.Children) {
                    item.BackgroundColor = Color.White;
                }

                string text = ((sender as Frame).Content as Label).Text;
                Label correctLine = window.Children[currentLine] as Label;

                if (text == correctLine.Text) {
                    window.Children[currentLine++].Opacity = 1;
                    (sender as Frame).Opacity = 0;

                    if (currentLine == lines[currentExtract].Count) {
                        currentLine = 0;
                        foreach (var item in window.Children) {
                            item.Opacity = 0;
                        }
                        foreach (var item in answers.Children) {
                            item.Opacity = 1;
                        }
                        ++currentExtract;
                        if (currentExtract < lines.Count) {
                            BindableLayout.SetItemsSource(window, lines[currentExtract]);
                            BindableLayout.SetItemsSource(answers, ShuffleLines(lines[currentExtract]));
                        }

                    }

                    if (currentExtract == lines.Count) {
                        if (mistakes == 0) {
                            isEnded = true;
                            if (done == 0) {
                                timeArray.Add(DateTime.Now.Subtract(startTime));

                                DisplayAlert("Так держать!", "Вы справились с этим упражнением! Чтобы перейти на следующий уровень, повторите свой успех!", "Ок");

                                currentExtract = 0;

                                BindableLayout.SetItemsSource(window, lines[currentExtract]);
                                BindableLayout.SetItemsSource(answers, ShuffleLines(lines[currentExtract]));


                                doneCounter.Text = $"Верно: {++done} / 2";
                                again.IsEnabled = false;

                                if (level == 3 || level == 6) {
                                    int average = (timeArray[level - 3].Seconds + timeArray[level - 2].Seconds) / 2;
                                    timer.Text = String.Format("{0:00}:{1:00}", average / 60, average % 60);
                                }
                            }
                            else {
                                timeArray[level - 1] = new TimeSpan((timeArray[level - 1] + (DateTime.Now.Subtract(startTime))).Ticks / 2);

                                doneCounter.Text = $"Верно: {++done} / 2";
                                DisplayAlert("Ура!", "Вы закончили этот этап и переходите на следующий!", "Ок");
                                NextLevel();
                                doneCounter.Text = $"Верно: {done} / 2";
                            }

                        }
                        else {
                            DisplayAlert("Ой-ой", "Кажется, вы допустили одну или несколько ошибок! Чтобы перейти к следующему этапу, пройдите этот этап дважды без ошибок!", "Ок");
                            Reset();
                        }
                    }
                }
                else {
                    mistakesCounter.Text = $"Ошибок: {++mistakes}";
                    (sender as Frame).BackgroundColor = Color.Red;
                    again.IsEnabled = true;
                }
            }
        }

        private void NextLevel() {
            currentExtract = 0;
            currentLine = 0;
            done = 0;
            title.Text = $"Уровень {++level} / 6";
            switch (level) {
                case 2:
                    boundText = JoinLines(startText);
                    BindableLayout.SetItemsSource(window, boundText[currentExtract]);
                    BindableLayout.SetItemsSource(answers, ShuffleLines(boundText[currentExtract]));
                    break;
                case 3:
                    boundText = startText;
                    BindableLayout.SetItemsSource(window, boundText[currentExtract]);
                    BindableLayout.SetItemsSource(answers, ShuffleLines(boundText[currentExtract]));
                    int average = (timeArray[0].Seconds + timeArray[1].Seconds) / 2;
                    timer.Text = String.Format("{0:00}:{1:00}", average / 60, average % 60);
                    timer.IsVisible = true;
                    break;
                case 4:
                    timer.IsVisible = false;
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }
        }
    }
}