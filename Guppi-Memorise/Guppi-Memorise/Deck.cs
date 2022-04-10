using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Guppi_Memorise {
    public class Deck {
        public string name;
        public ObservableCollection<Card> cards = new ObservableCollection<Card>();

        public Deck(string name = "Новая колода") {
            this.name = name;
        }
    }
}
