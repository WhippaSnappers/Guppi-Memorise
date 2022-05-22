using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guppi_Memorise
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class MemorisingPage : ContentPage
    {
        public List<List<string>> startText { get; set; } //оригинал переданного текста со страницы ввода
        public List<List<string>> boundText; //список, который привязывается к лэйаутам на уровнях 1-3

        private List<List<string>> standardText; //список-образец, по нему сравнивается кликнутое слово со словом, на месте которого стоит пропуск
        private List<string> boundTextWords; //список, который привязывается к лэйауту текста с пропусками на уровнях 4-6
        private List<string> missedWords; //список с пропавшими словами
        private List<int> missedWordsIndexes; //список с индексами пропавших слов относительно начала строфы

        private bool isEnded = false; //флаг для определения того, закончил игрок этап или еще нет, используется для мгновенной остановки таймера

        private int currentPart = 0; //номер слова или строки, которая сейчас ожидается от игрока, считается с начала строфы
        private int currentExtract = 0; //номер строфы, которая сейчас отображается
        private int mistakes = 0; //количество ошибок
        private int done = 0; //количество успехов на одном левеле
        private int level = 1; //удивительно, но номер уровня

        private List<TimeSpan> timeArray; //список, в котором лежит среднее время прохождения двух этапов на каждом уровне
        private DateTime startTime; //время начала прохождения этапа, используется для вычисления времени прохождения

        public MemorisingPage(List<List<string>> str)
        {
            InitializeComponent();
            startText = str;
            boundText = str;
            missedWords = new List<string>();
            boundTextWords = JoinText(boundText);
            timeArray = new List<TimeSpan>(6);
            bindExtract(startText[currentExtract]);

        }

        private void OpenInfo(object sender, EventArgs _)
        {
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
        }//кнопка инфо на каждом левеле

        private void PartTapped(object sender, EventArgs _)
        { //функция клика по строке или слову тут не логика а пизда какая-то

            if ((sender as Frame).Opacity != 0)
            { //после верного клика элементы скрываются, это условие не дает засчитать клик по невидимому элементу

                if (currentPart == 0 && currentExtract == 0)
                { //проверка на первый клик на этапе
                    startTime = DateTime.Now; //запоминание времени начала после первого клика

                    isEnded = false; //начался уровень, так что флаг ставим на фолс
                    if (level == 3 || level == 6)
                    { //проверка на левелы с таймером
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
                                    timer.Text = String.Format("{0:00}:{1:00}", time / 60, time % 60);
                                    time--;
                                }
                            });
                            if (time == 0 && !isEnded)
                            {
                                if (level == 6)
                                {
                                    timeArray[level - 3] += TimeSpan.FromSeconds(6);
                                    timeArray[level - 2] += TimeSpan.FromSeconds(6);
                                    DisplayAlert("Ой-ой", "Кажется, вы не успели :( Пожалуйста, попробуйте еще раз. Чтобы вам было чуть проще, мы добавили к таймеру 3 секунды.", "Ок");
                                }
                                else
                                {
                                    DisplayAlert("Ой-ой", "Кажется, вы не успели :( Пожалуйста, попробуйте еще раз.", "Ок");
                                }
                                Reset();
                                return false;
                            }
                            return !isEnded;
                        });
                    }
                } 

                foreach (var item in answers.Children)
                { //после каждого клика предыдущий неверный элемент возвращает свой цвет фона
                    item.BackgroundColor = Color.White;
                }

                string text = ((sender as Frame).Content as Label).Text;

                if (level <= 3 && text == (window.Children[currentPart] as Label).Text || level > 3 && text == standardText[currentExtract][missedWordsIndexes[currentPart]])
                { //условие верного клика
                    if (level <= 3 && text == (window.Children[currentPart] as Label).Text)
                    { //условие на проверку уровня: если уровень на строчки, то меняем опасити, если нет, то ищем 6 подчеркиваний и заменяем на кликнутое слово (здесь могут быть проблемы если юзер решит написать текст с 6 подчеркиваниями подряд, и здесь все поедет)
                        window.Children[currentPart++].Opacity = 1;
                    }
                    else
                    {
                        (window.Children[0] as Label).Text = new Regex("______").Replace((window.Children[0] as Label).Text, text, 1);
                        currentPart++;
                    }
                    (sender as Frame).Opacity = 0;

                    if (level <= 3 & currentPart == boundText[currentExtract].Count || level > 3 & currentPart == missedWords.Count & currentPart > 0)
                    { //проверка на случай, когда юзер закончит с этой строфой
                        if (currentPart == boundText[currentExtract].Count)
                        {
                            foreach (var item in window.Children)
                            {
                                item.Opacity = 0;
                            }
                        }
                        foreach (var item in answers.Children)
                        {
                            item.Opacity = 1;
                        }
                        currentPart = 0;
                        ++currentExtract; //здесь были логические проблемы с определением конца текста, поэтому пришлось сделать такой костыль: сначала прибавлять единицу к текущему отрывку, а только потом проверять его, иначе там ломается хуйня
                        if (currentExtract < boundText.Count)
                        {
                            if (level <= 3)
                            {
                                bindExtract(boundText[currentExtract]);
                            }
                            else
                            {
                                bindWords(boundTextWords[currentExtract]);
                            }
                        }

                    }

                    if (currentExtract == boundText.Count)
                    { //проверка на конец текста
                        if (mistakes == 0)
                        { //проверка на отсутствие ошибок
                            isEnded = true; //меняем флаг, чтобы быстро остановить таймер и он нам не поднасрал
                            if (done == 0)
                            { //если это первое прохождение левела, то начинаем заново и добавляем 1 к счетчику прохождений
                                timeArray.Add(DateTime.Now.Subtract(startTime)); //запоминаем время прохождения

                                DisplayAlert("Так держать!", "Вы справились с этим упражнением! Чтобы перейти на следующий уровень, повторите свой успех!", "Ок");

                                currentExtract = 0;

                                if (level <= 3)
                                {
                                    bindExtract(boundText[currentExtract]);
                                }
                                else
                                {
                                    bindWords(boundTextWords[currentExtract]);
                                }


                                doneCounter.Text = $"Верно: {++done} / 2";
                                again.IsEnabled = false;

                                if (level == 3 || level == 6)
                                { //обнуление таймера
                                    int average = (timeArray[level - 3].Seconds + timeArray[level - 2].Seconds) / 2;
                                    timer.Text = String.Format("{0:00}:{1:00}", average / 60, average % 60);
                                }
                            }
                            else
                            { //если это было второе прохождение, то переходим на следующий левел
                                doneCounter.Text = $"Верно: {++done} / 2";
                                if (level < 6)
                                {
                                    timeArray[level - 1] = new TimeSpan((timeArray[level - 1] + (DateTime.Now.Subtract(startTime))).Ticks / 2);

                                    DisplayAlert("Ура!", "Вы закончили этот этап и переходите на следующий!", "Ок");
                                    NextLevel();
                                    doneCounter.Text = $"Верно: {done} / 2";
                                }
                                else
                                { //если последний левел, то выкидываем юзера со страницы, там через disappearing вызывается информационное окно с похвалой
                                    Navigation.PopAsync();
                                }
                            }

                        }
                        else
                        { //если есть ошибки, пиздим юзера, ебем его в жопу и выкидываем в канаву. ну и конечно ресетим левел
                            DisplayAlert("Ой-ой", "Кажется, вы допустили одну или несколько ошибок! Чтобы перейти к следующему этапу, пройдите этот этап дважды без ошибок!", "Ок");
                            Reset();
                        }
                    }
                }
                else
                { //если клик ошибочный, помечаем красным цветом, чтобы унизить юзера и меняем счетчик. теперь это клеймо на всю жизнь
                    mistakesCounter.Text = $"Ошибок: {++mistakes}";
                    (sender as Frame).BackgroundColor = Color.Red;
                    again.IsEnabled = true;
                }
            }
        }

        private void Again(object sender, EventArgs _)
        { //функция для кнопки нанова
            Reset();
        }

        private void Reset()
        { //обнуляет прогресс уровня
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
                bindExtract(boundText[currentExtract]);
                foreach (var item in window.Children)
                {
                    item.Opacity = 0;
                }
            }
            else
            {
                bindWords(boundTextWords[currentExtract]);
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
        { //перемешивает элементы списка
            string[] lines = new string[text.Count];
            text.CopyTo(lines);
            Random rnd = new Random();
            for (int i = lines.Length - 1; i >= 1; i--) 
            {
                int index = rnd.Next(i + 1);
                var temp = lines[index];
                lines[index] = lines[i];
                lines[i] = temp;
            }
            return lines.ToList();
        }

        private List<List<string>> JoinExtracts (List<List<string>> lines) 
        { //соединяет по две строфы в одну для 2 или 5 уровней
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
        { //осуществляет переход на следующий уровень, изменяя окно соответствующим образом на основе переменной levelи обнуляя все переменные прогресса уровня
            currentExtract = 0;
            currentPart = 0;
            done = 0;
            title.Text = $"Уровень {++level} / 6";
            switch (level)
            {
                case 2:
                    boundText = JoinExtracts(startText);
                    bindExtract(boundText[currentExtract]);
                    break;
                case 3:
                    boundText = startText;
                    bindExtract(boundText[currentExtract]);
                    int averageLines = (timeArray[0].Seconds + timeArray[1].Seconds) / 2;
                    timer.Text = String.Format("{0:00}:{1:00}", averageLines / 60, averageLines % 60);
                    timer.IsVisible = true;
                    break;
                case 4:
                    timer.IsVisible = false;

                    BindableLayout.SetItemTemplate(answers, (DataTemplate)Resources["wordTemplate"]);
                    answers.Margin = new Thickness(-5, 0);

                    standardText = SplitText(boundText); 

                    bindWords(boundTextWords[0]);
                    break;
                case 5:
                    boundText = JoinExtracts(startText);
                    standardText = SplitText(boundText);
                    boundTextWords = JoinText(boundText);
                    bindWords(boundTextWords[0]);
                    break;
                case 6:
                    boundText = startText;
                    standardText = SplitText(boundText);
                    boundTextWords = JoinText(boundText);
                    bindWords(boundTextWords[0]);
                    int averageWords = (timeArray[3].Seconds + timeArray[4].Seconds) / 4;
                    timer.Text = String.Format("{0:00}:{1:00}", averageWords / 60, averageWords % 60);
                    timer.IsVisible = true;
                    break;
            }
        }

        private List<int> generateListOfRandomNumbers(int wordsCount)
        { //возвращает список рандомных уникальных чисел от 0 до размера строфы - это параметр функции, размер этого списка - рандомное число от 20% размера строфы до 75%
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

        private List<string> generateMissingWords(string extract)
        { //на основе списка рандомных чисел и переданной строфы генерирует список пропавших слов
            List<string> splittedExtract = extract.Split(' ', '\n').ToList();
            List<string> words = new List<string>();
            missedWordsIndexes = generateListOfRandomNumbers(splittedExtract.Count);
            for (int i = 0; i < missedWordsIndexes.Count; ++i)
            {
                words.Add(splittedExtract[missedWordsIndexes[i]]);
            }
            return words;
        }

        private void bindExtract(List<string> extract)
        { //просто функция для перепривязки списков со строками к зонам ответов и правильных ответов
            BindableLayout.SetItemsSource(window, extract);
            BindableLayout.SetItemsSource(answers, ShuffleParts(extract));
        }

        private void bindWords(string extract) 
        { //вместо привязки проще добавить готовую строку с пропавшими словами в виде лабеля, к зоне ответов привязываем список с пропавшими словами
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
        { //лучший костыль времен и народов, у тебя наверняка появятся вопросы, почему я так сделал, так что пиши
            List<string> words = extract.Split(' ', '\n').ToList(); //ничего лучше не придумал: здесь передается в функцию полная строфа в виде цельной строки, она сплитится на слова
            missedWords = generateMissingWords(extract); //генерация пропавших слов
            for (int i = 0; i < missedWords.Count; ++i) 
            {
                words[missedWordsIndexes[i]] = "______"; //заменяем слова на подчеркивания
            }

            List<List<string>> splitted = new List<List<string>>(); //здесь создаем список списков со словами, чтобы знать где у нас переносы строк, чтобы потом по этим границам заджойнить
            foreach (string line in boundText[currentExtract])
            {
                splitted.Add(line.Split(' ').ToList());
            }

            int c = 0;
            for (int i = 0; i < splitted.Count; ++i)
            { //костыль для костыля: бежим по двум нашим спискам и проверяем, где не совпадают слова, если они не совпадают, значит так подчеркивания, заменяем во втором массиве
                for (int j = 0; j < splitted[i].Count; ++j)
                {
                    if (splitted[i][j] != words[c])
                    {
                        splitted[i][j] = words[c];
                    }
                    c++;
                }
            }

            List<string> result = new List<string>(); //джойним наш второй массив, зная границы, где должны быть переносы строк
            foreach (List<string> line in splitted)
            {
                result.Add(String.Join(" ", line));
            }
            return String.Join("\n", result); //готовая строка с пропавшими словами
        }

        private List<List<string>> SplitText(List<List<string>> text)
        { //на вход - список списков строк стиха, на выходе - список списков отдельных слов
            List<List<string>> result = new List<List<string>>();
            foreach (var item in text)
            {
                result.Add(string.Join(" ", item).Split(' ').ToList());
            }
            return result;
        }

        private List<string> JoinText(List<List<string>> text)
        { //на вход - список списков строк, на выходе - список строф
            List<string> result = new List<string>();
            foreach (var item in text)
            {
                result.Add(string.Join("\n", item));
            }
            return result;
        }
    }
}