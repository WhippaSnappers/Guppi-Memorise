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
    public class EntryTextOneLine : Grid
    {
        private static Style _textStyle = new Style(typeof(Label))
        {
            Setters =
            {
                new Setter { Property = Label.TextColorProperty, Value = "#012545" },
                new Setter { Property = Label.FontSizeProperty, Value = 15 },
                new Setter { Property = Label.FontFamilyProperty, Value = "Russo One" },
                new Setter { Property = HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                new Setter { Property = VerticalOptionsProperty, Value = LayoutOptions.CenterAndExpand },
                new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.End }
            }
        };

        private Label _text = new Label
        {
            Style = _textStyle
        };

        private Entry _entry = new Entry
        {
            VerticalOptions   = LayoutOptions.CenterAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        public string EntryText {
            get {
                return _text.Text;
            }
            set {
                _text.Text = value;
            }
        }

        public EntryTextOneLine()
        {
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.3, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });

            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            Children.Add(_text, 0, 0);
            Children.Add(_entry, 1, 0);
        }
    }
}
