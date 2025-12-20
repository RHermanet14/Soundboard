using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Soundboard
{
    enum ThemeType
    {
        Unknown,
        Light,
        Dark,
        System,
    }
    internal class Theme
    {
        private readonly ThemeType Type;
        private readonly SolidColorBrush BackgroundColor;
        private readonly SolidColorBrush ButtonColor;
        private readonly SolidColorBrush TextColor;
        private readonly SolidColorBrush OutlineColor; // Menu and bottom grid

        public Theme() : this(ThemeType.Light) { }

        public Theme(ThemeType type)
        {
            Type = type;
            switch (Type)
            {
                case ThemeType.Light:
                    BackgroundColor = new(Color.FromRgb(255, 255, 255));
                    ButtonColor = new(Color.FromRgb(240, 240, 240));
                    TextColor = new(Color.FromRgb(0, 0, 0));
                    OutlineColor = new(Color.FromRgb(200, 200, 200));
                    break;
                case ThemeType.Dark:
                    BackgroundColor = new(Color.FromRgb(30, 30, 30));
                    ButtonColor = new(Color.FromRgb(50, 50, 50));
                    TextColor = new(Color.FromRgb(255, 255, 255));
                    OutlineColor = new(Color.FromRgb(80, 80, 80));
                    break;
                default:
                    BackgroundColor = new(Color.FromRgb(255, 255, 255));
                    ButtonColor = new(Color.FromRgb(240, 240, 240));
                    TextColor = new(Color.FromRgb(0, 0, 0));
                    OutlineColor = new(Color.FromRgb(200, 200, 200));
                    break;
            }
        }

        public void SetMainTheme(Grid grid, Grid bottom_grid, Menu menu)
        {
            grid.Background = BackgroundColor;
            foreach (UIElement control in grid.Children)
                if (control is Control uiControl)
                    uiControl.Background = ButtonColor;
            menu.Background = OutlineColor;
            bottom_grid.Background = OutlineColor;
            foreach (UIElement control in bottom_grid.Children)  
                if (control is StackPanel stackPanel)
                    foreach (UIElement stackControl in stackPanel.Children)
                        if (stackControl is Control button)
                            button.Background = ButtonColor;
            // Fix title bar color, adjust theme colors for betterness
        }

        public void SetPreferencesTheme(Window window)
        {
            window.Background = BackgroundColor;
        }
    }
}
