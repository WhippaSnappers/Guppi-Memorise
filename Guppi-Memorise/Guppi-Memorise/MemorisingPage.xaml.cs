using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class MemorisingPage : ContentPage
    {
        public List<List<string>> startText { get; set; }
        public List<List<string>> boundText;
        private List<List<string>> standardText;
        private List<string> boundTextWords; 
        private List<string> missedWords; 
        private List<int> missedWordsIndexes;
        private bool isEnded = false;
        private int currentPart = 0;
        private int currentExtract = 0;
        private int mistakes = 0;
        private int done = 0;
        private int level = 1;
        private List<TimeSpan> timeArray;
        private DateTime startTime;
        private TimeSpan memorisingTime;
        private DateTime memorisingTimeStart;
        private Text userText;

        public MemorisingPage(Text userText)
        {
            InitializeComponent();
            this.userText = userText;
            var parsedText = TextUtils.ParseUsersText(userText.Body);
            startText = parsedText;
            boundText = parsedText;
            missedWords = new List<string>();
            boundTextWords = JoinText(boundText);
            timeArray = new List<TimeSpan>(6);
            BindExtract(startText[currentExtract]);
            memorisingTimeStart = DateTime.Now;
        }
        private async void OpenInfo(object sender, EventArgs _)
        {
            switch (level) {
                case 1:
                    await DisplayAlert("Инфо", "На данном этапе текст разбивается на фрагменты. Ваша задача - собрать их в правильном порядке, в конце концов получив полный текст.", "Ок");
                    break;
                case 2:
                    await DisplayAlert("Инфо", "На этом этапе ваша задача усложняется: отрывки становятся в два раза больше, и вам также нужно их восстановить из фрагментов.", "Ок");
                    break;
                case 3:
                    await DisplayAlert("Инфо", "Ваша задача не изменилась, однако программа запомнила среднее время, за которое вы собирали отрывки, и теперь вам нужно снова собрать текст, уложившись в это время, таймер вы можете увидеть в верхней части экрана.", "Ок");
                    break;
                case 4:
                    await DisplayAlert("Инфо", "Задача усложнилась! Теперь в вашем тексте пропали случайные слова, и вам необходимо их вернуть на свои места.", "Ок");
                    break;
                case 5:
                    await DisplayAlert("Инфо", "Как и во втором уровне, здесь отрывки стали в два раза больше, а это означает, что вам нужно вставить больше слов!", "Ок");
                    break;
                case 6:
                    await DisplayAlert("Инфо", "Это последний уровень, так что он не из простых. Здесь вам так же нужно вернуть пропавшие слова, однако теперь вам еще и придется уложиться в таймер! На этом этапе, если вы не успеете за данное вам время, мы увеличим его на 3 секунды. Идея в том, чтобы вы научились не только быстро вспоминать нужные слова, но и повторять это несколько раз в связи с недостатком времени.", "Ок");
                    break;
            }
        }
        private async void PartTapped(object sender, EventArgs _)
        {

            if ((sender as Frame).Opacity != 0)
            {
                if (currentPart == 0 && currentExtract == 0)
                {
                    startTime = DateTime.Now;

                    isEnded = false;
                    if (level == 3 || level == 6)
                    {
                        int time = (timeArray[level - 3].Seconds + timeArray[level - 2].Seconds) / 2; //
                        if (level == 6)
                        {
                            time /= 2;
                        }

                        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (!isEnded)
                                {
                                    time--;
                                    timer.Text = String.Format("{0:00}:{1:00}", time / 60, time % 60);
                                }
                            });
                            if (time == 0 && !isEnded)
                            {
                                if (level == 6)
                                {
                                    timeArray[level - 3] += TimeSpan.FromSeconds(6);
                                    timeArray[level - 2] += TimeSpan.FromSeconds(6);
                                    Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Ой-ой", "Кажется, вы не успели :( Пожалуйста, попробуйте еще раз. Чтобы вам было чуть проще, мы добавили к таймеру 3 секунды.", "Ок"));
                                }
                                else
                                {
                                    Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Ой-ой", "Кажется, вы не успели :( Пожалуйста, попробуйте еще раз.", "Ок"));
                                }
                                Reset();
                                return false;
                            }
                            return !isEnded;
                        });
                    }
                } 

                foreach (var item in answers.Children)
                {
                    item.BackgroundColor = Color.White;
                }

                string text = ((sender as Frame).Content as Label).Text;

                if (level <= 3 && text == (window.Children[currentPart] as Label).Text || level > 3 && text == standardText[currentExtract][missedWordsIndexes[currentPart]])
                {
                    if (level <= 3 && text == (window.Children[currentPart] as Label).Text)
                    {
                        window.Children[currentPart++].Opacity = 1;
                    }
                    else
                    {
                        (window.Children[0] as Label).Text = new Regex("______").Replace((window.Children[0] as Label).Text, text, 1);
                        currentPart++;
                    }
                    (sender as Frame).Opacity = 0;
                    // (sender as Frame).IsVisible = false;

                    if (level <= 3 & currentPart == boundText[currentExtract].Count || level > 3 & currentPart == missedWords.Count & currentPart > 0)
                    {
                        if (currentPart == boundText[currentExtract].Count)
                        {
                            foreach (var item in window.Children)
                            {
                                item.Opacity = 0;
                                // item.IsVisible = false;
                            }
                        }
                        foreach (var item in answers.Children)
                        {
                            item.Opacity = 1;
                            // item.IsVisible = true;
                        }
                        currentPart = 0;
                        ++currentExtract;
                        if (currentExtract < boundText.Count)
                        {
                            if (level <= 3)
                            {
                                BindExtract(boundText[currentExtract]);
                            }
                            else
                            {
                                BindWords(boundTextWords[currentExtract]);
                            }
                        }

                    }

                    if (currentExtract == boundText.Count)
                    {
                        if (mistakes == 0)
                        {
                            isEnded = true;
                            if (done == 0)
                            {
                                timeArray.Add(DateTime.Now.Subtract(startTime));

                                await DisplayAlert("Так держать!", "Вы справились с этим упражнением! Чтобы перейти на следующий уровень, повторите свой успех!", "Ок");

                                currentExtract = 0;

                                if (level <= 3)
                                {
                                    BindExtract(boundText[currentExtract]);
                                }
                                else
                                {
                                    BindWords(boundTextWords[currentExtract]);
                                }


                                doneCounter.Text = $"Верно: {++done} / 2";
                                again.IsEnabled = false;

                                if (level == 3 || level == 6)
                                {
                                    int average = (timeArray[level - 3].Seconds + timeArray[level - 2].Seconds) / 2;
                                    if (level == 6) {
                                        average = average / 2;
                                    }
                                    timer.Text = String.Format("{0:00}:{1:00}", average / 60, average % 60);
                                }
                            }
                            else
                            {
                                doneCounter.Text = $"Верно: {++done} / 2";
                                if (level < 6)
                                {
                                    timeArray[level - 1] = new TimeSpan((timeArray[level - 1] + (DateTime.Now.Subtract(startTime))).Ticks / 2);

                                    await DisplayAlert("Ура!", "Вы закончили этот этап и переходите на следующий!", "Ок");
                                    NextLevel();
                                    doneCounter.Text = $"Верно: {done} / 2";
                                }
                                else
                                {
                                    // Exit Point
                                    MemorisingStartPage.isLearned = true;
                                    memorisingTime = DateTime.Now - memorisingTimeStart;
                                    var timeString = string.Format("{0:00}:{1:00}:{2:00}", memorisingTime.Hours, memorisingTime.Minutes, memorisingTime.Seconds);
                                    userText.Time = timeString;
                                    await DB.AddText(userText);
                                    Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
                                }
                            }

                        }
                        else
                        {
                            await DisplayAlert("Ой-ой", "Кажется, вы допустили одну или несколько ошибок! Чтобы перейти к следующему этапу, пройдите этот этап дважды без ошибок!", "Ок");
                            Reset();
                        }
                    }
                }
                else
                {
                    mistakesCounter.Text = $"Ошибок: {++mistakes}";
                    (sender as Frame).BackgroundColor = Color.Red;
                    again.IsEnabled = true;
                }
            }
        }
        private void Again(object sender, EventArgs _)
        {
            Reset();
        }
        private void Reset()
        {
            currentExtract = 0;
            currentPart = 0;
            mistakes = 0;
            done = 0;
            isEnded = true;

            if (timeArray.Count >= level)
            {
                timeArray.RemoveAt(level - 1);
            }

            doneCounter.Text = "Верно: 0 / 2";
            mistakesCounter.Text = "Ошибок: 0";
            again.IsEnabled = false;

            if (level <= 3)
            {
                BindExtract(boundText[currentExtract]);
                foreach (var item in window.Children)
                {
                    item.Opacity = 0;
                }
            }
            else
            {
                BindWords(boundTextWords[currentExtract]);
            }

            if (level == 3 || level == 6) 
            {
                int average = (timeArray[level - 3].Seconds + timeArray[level - 2].Seconds) / 2;
                if (level == 6) 
                {
                    average /= 2;
                }
                timer.Text = String.Format("{0:00}:{1:00}", average / 60, average % 60);
            }

            foreach (var item in answers.Children) 
            {
                item.Opacity = 1;
            }
        }
        private List<string> ShuffleParts(List<string> text) 
        {
            var rand = new Random();
            return text.OrderBy(i => rand.Next()).ToList();
        }
        private List<List<string>> JoinExtracts (List<List<string>> lines) 
        {
            List<List<string>> result = new List<List<string>>();
            for (int i = 0; i < lines.Count; i += 2)
            {
                if (i + 1 != lines.Count)
                {
                    result.Add(lines[i].Concat(lines[i + 1]).ToList());
                }
                else 
                {
                    result.Add(lines[i]);
                }
            }
            return result;
        }
        private void NextLevel() 
        {
            currentExtract = 0;
            currentPart = 0;
            done = 0;
            title.Text = $"Уровень {++level} / 6";
            switch (level)
            {
                case 2:
                    boundText = JoinExtracts(startText);
                    BindExtract(boundText[currentExtract]);
                    break;
                case 3:
                    boundText = startText;
                    BindExtract(boundText[currentExtract]);
                    int averageLines = (timeArray[0].Seconds + timeArray[1].Seconds) / 2;
                    timer.Text = String.Format("{0:00}:{1:00}", averageLines / 60, averageLines % 60);
                    timer.IsVisible = true;
                    break;
                case 4:
                    timer.IsVisible = false;

                    BindableLayout.SetItemTemplate(answers, (DataTemplate)Resources["wordTemplate"]);
                    answers.Margin = new Thickness(-5, 0);

                    standardText = SplitText(boundText); 

                    BindWords(boundTextWords[0]);
                    break;
                case 5:
                    boundText = JoinExtracts(startText);
                    standardText = SplitText(boundText);
                    boundTextWords = JoinText(boundText);
                    BindWords(boundTextWords[0]);
                    break;
                case 6:
                    boundText = startText;
                    standardText = SplitText(boundText);
                    boundTextWords = JoinText(boundText);
                    BindWords(boundTextWords[0]);
                    int averageWords = (timeArray[3].Seconds + timeArray[4].Seconds) / 4;
                    timer.Text = String.Format("{0:00}:{1:00}", averageWords / 60, averageWords % 60);
                    timer.IsVisible = true;
                    break;
            }
        }
        private List<int> GenerateListOfRandomNumbers(int wordsCount)
        {
            int size = new Random().Next(wordsCount / 5 + 1, wordsCount / 4 * 3 + 1);
            List<int> list = new List<int>();
            Random rand = new Random();

            while (list.Count < size)
            {
                int n = rand.Next(wordsCount);
                if (!list.Contains(n))
                {
                    list.Add(n);
                }
            }
            list.Sort();
            return list;
        }
        private List<string> GenerateMissingWords(string extract)
        {
            List<string> splittedExtract = extract.Split(' ', '\n').ToList();
            List<string> words = new List<string>();
            missedWordsIndexes = GenerateListOfRandomNumbers(splittedExtract.Count);
            for (int i = 0; i < missedWordsIndexes.Count; ++i)
            {
                words.Add(splittedExtract[missedWordsIndexes[i]]);
            }
            return words;
        }
        private void BindExtract(List<string> extract)
        {
            BindableLayout.SetItemsSource(window, extract);
            BindableLayout.SetItemsSource(answers, ShuffleParts(extract));
        }
        private void BindWords(string extract) 
        {
            window.Children.Clear();
            window.Children.Add(new Label()
            {
                Text = ReplaceMissingWords(extract),
                FontFamily = "Exo",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontSize = 17
            });
            BindableLayout.SetItemsSource(answers, ShuffleParts(missedWords));
        }
        private string ReplaceMissingWords(string extract)
        {
            List<string> words = extract.Split(' ', '\n').ToList();
            missedWords = GenerateMissingWords(extract);
            for (int i = 0; i < missedWords.Count; ++i) 
            {
                words[missedWordsIndexes[i]] = "______";
            }

            List<List<string>> splitted = new List<List<string>>();
            foreach (string line in boundText[currentExtract])
            {
                splitted.Add(line.Split(' ').ToList());
            }

            int c = 0;
            for (int i = 0; i < splitted.Count; ++i)
            {
                for (int j = 0; j < splitted[i].Count; ++j)
                {
                    if (splitted[i][j] != words[c])
                    {
                        splitted[i][j] = words[c];
                    }
                    c++;
                }
            }

            List<string> result = new List<string>();
            foreach (List<string> line in splitted)
            {
                result.Add(String.Join(" ", line));
            }
            return String.Join("\n", result);
        }
        private List<List<string>> SplitText(List<List<string>> text)
        {
            List<List<string>> result = new List<List<string>>();
            foreach (var item in text)
            {
                result.Add(string.Join(" ", item).Split(' ').ToList());
            }
            return result;
        }
        private List<string> JoinText(List<List<string>> text)
        {
            List<string> result = new List<string>();
            foreach (var item in text)
            {
                result.Add(string.Join("\n", item));
            }
            return result;
        }
    }
}