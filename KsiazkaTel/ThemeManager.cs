using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PhoneBookWPF
{
    public enum Theme
    {
        Default,
        Dark,
        Hacker
    }

    public static class ThemeManager
    {
        private static Theme _currentTheme = Theme.Default;

        public static Theme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    ApplyTheme(value);
                    ThemeChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler ThemeChanged;

        private static readonly Dictionary<Theme, ColorScheme> _themes = new Dictionary<Theme, ColorScheme>
        {
            [Theme.Default] = new ColorScheme
            {
                PrimaryColor = Color.FromRgb(33, 150, 243),    // Niebieski
                SecondaryColor = Color.FromRgb(76, 175, 80),   // Zielony
                BackgroundColor = Color.FromRgb(255, 255, 255), // Biały
                TextColor = Color.FromRgb(33, 33, 33),         // Ciemnoszary
                BorderColor = Color.FromRgb(224, 224, 224),    // Jasnoszary
                CardColor = Color.FromRgb(250, 250, 250),      // Bardzo jasny szary
                HighlightColor = Color.FromRgb(227, 242, 253)  // Jasny niebieski
            },
            [Theme.Dark] = new ColorScheme
            {
                PrimaryColor = Color.FromRgb(63, 81, 181),     // Ciemny niebieski
                SecondaryColor = Color.FromRgb(233, 30, 99),   // Różowy
                BackgroundColor = Color.FromRgb(30, 30, 30),   // Ciemnoszary
                TextColor = Color.FromRgb(240, 240, 240),      // Jasnoszary
                BorderColor = Color.FromRgb(60, 60, 60),       // Szary
                CardColor = Color.FromRgb(40, 40, 40),         // Ciemny szary
                HighlightColor = Color.FromRgb(55, 71, 161)    // Ciemny niebieski 2
            },
            [Theme.Hacker] = new ColorScheme
            {
                PrimaryColor = Color.FromRgb(0, 255, 0),       // Zielony (hackerski)
                SecondaryColor = Color.FromRgb(0, 200, 0),     // Ciemniejszy zielony
                BackgroundColor = Color.FromRgb(0, 20, 0),     // Ciemnozielone/czarne
                TextColor = Color.FromRgb(0, 255, 0),          // Zielony
                BorderColor = Color.FromRgb(0, 100, 0),        // Ciemny zielony
                CardColor = Color.FromRgb(0, 30, 0),           // Bardzo ciemny zielony
                HighlightColor = Color.FromRgb(0, 150, 0)      // Średni zielony
            }
        };

        public static ColorScheme GetCurrentColorScheme()
        {
            return _themes[CurrentTheme];
        }

        private static void ApplyTheme(Theme theme)
        {
            var colorScheme = _themes[theme];
            var app = Application.Current;

            // Aktualizujemy zasoby aplikacji
            app.Resources["PrimaryColor"] = new SolidColorBrush(colorScheme.PrimaryColor);
            app.Resources["SecondaryColor"] = new SolidColorBrush(colorScheme.SecondaryColor);
            app.Resources["BackgroundColor"] = new SolidColorBrush(colorScheme.BackgroundColor);
            app.Resources["TextColor"] = new SolidColorBrush(colorScheme.TextColor);
            app.Resources["BorderColor"] = new SolidColorBrush(colorScheme.BorderColor);
            app.Resources["CardColor"] = new SolidColorBrush(colorScheme.CardColor);
            app.Resources["HighlightColor"] = new SolidColorBrush(colorScheme.HighlightColor);

            // Dodatkowe style dla kontrolek
            app.Resources["ButtonBackground"] = new SolidColorBrush(colorScheme.PrimaryColor);
            app.Resources["ButtonForeground"] = new SolidColorBrush(Colors.White);
            app.Resources["ListBackground"] = new SolidColorBrush(colorScheme.CardColor);
            app.Resources["ListForeground"] = new SolidColorBrush(colorScheme.TextColor);
        }

        public static void Initialize()
        {
            // Inicjalizacja zasobów
            var app = Application.Current;

            // Dodaj zasoby jeśli nie istnieją
            if (!app.Resources.Contains("PrimaryColor"))
            {
                ApplyTheme(CurrentTheme);
            }
        }
    }

    public class ColorScheme
    {
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Color BorderColor { get; set; }
        public Color CardColor { get; set; }
        public Color HighlightColor { get; set; }
    }
}