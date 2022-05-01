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

        public MemorisingPage() {
            InitializeComponent();
        }

        private void OpenInfo(object sender, EventArgs e) {
            DisplayAlert("Инфо", "На данном этапе ваш текст разбивается на фрагменты. Ваша задача - собрать их в правильном порядке, в конце концов собрав полный текст.", "Ок");
        }
    }
}