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
    public partial class AboutApp : ContentPage
    {
        public AboutApp()
        {
            InitializeComponent();
            //InitText();
        }

        private void InitText()
        {
            string txt = "Приложение для заучивания текстов и терминов. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            var tmpl = "<html>" +
                "<body style=\"text-align: justify; font-family: Exo;\">" +
                String.Format("<p>{0}</p>", txt) +
                "</body>" +
                "</html>";
            var src = new HtmlWebViewSource();
            src.Html = tmpl;
            //wv.Source = src;
        }
    }
}