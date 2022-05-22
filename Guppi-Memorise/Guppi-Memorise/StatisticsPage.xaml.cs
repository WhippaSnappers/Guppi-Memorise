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
    public partial class StatisticsPage : ContentPage
    {
        private StackLayout _stack = new StackLayout();

        public StatisticsPage()
        {
            InitializeComponent();

            var list = new[]
            {
                new {title = "������� �����:"},
                new {title = "������� ����-��������:"},
                new {title = "��������� ������ ��� ����������:"},
                new {title = "��������� ��������:"}
            };



            foreach(var n in list)
            {
                var control = new EntryTextOneLine
                {
                    EntryText = n.title
                };

                _stack.Children.Add(control);
            }

            Content = _stack;
        }
    }
}