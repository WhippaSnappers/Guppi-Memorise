using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Guppi_Memorise {
    public class Deck : BindableObject {
        public static readonly BindableProperty NameProperty = BindableProperty.Create("name", typeof(string), typeof(Deck), "Новая колода", BindingMode.TwoWay);
        public string name {
            get {
                return (string)GetValue(NameProperty);
            }
            set {
                SetValue(NameProperty, value);
            }
        }

        public ObservableCollection<Card> cards = new ObservableCollection<Card>();

        public int id {
            get; private set;
        }
        private static int count = 0; 

        public Deck(string name = "Новая колода") {
            id = count++;
            this.name = name;
        }
    }
}
