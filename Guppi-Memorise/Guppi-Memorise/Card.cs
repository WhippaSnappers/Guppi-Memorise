using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Guppi_Memorise {
    public class Card : BindableObject {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("title", typeof(string), typeof(Card), "Новая карточка", BindingMode.TwoWay);
        public static readonly BindableProperty TextProperty = BindableProperty.Create("text", typeof(string), typeof(Card), "Текст на карточке", BindingMode.TwoWay);
        public string title { 
            get {
                return (string)GetValue(TitleProperty);
            }
            set {
                SetValue(TitleProperty, value);
            }
        }
        public string text {
            get {
                return (string)GetValue(TextProperty);
            }
            set {
                SetValue(TextProperty, value);
            }
        }

        public int id {
            get; private set;
        }
        private static int count = 0;

        public Card(string title = "Новая карточка", string text = "Текст на карточке") {
            id = count++;
            this.title = title;
            this.text = text;
        }
    }
}
