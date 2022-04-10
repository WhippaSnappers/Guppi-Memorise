using System;
using System.Collections.Generic;
using System.Text;

namespace Guppi_Memorise {
    public class Card {
        public string title, text;

        public Card(string title = "Новая карточка", string text = "Текст на карточке") {
            this.title = title;
            this.text = text;
        }
    }
}
