using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Soundboard
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        private readonly MainWindow _main;
        private Theme? theme;
        private System.Collections.Specialized.StringCollection Formats { get; set; } = [];

        public PreferencesWindow(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void On_Startup(object sender, RoutedEventArgs e)
        {
            Load_Preferences();
        }

        private void Load_Preferences()
        {
            column_box.Text = Properties.Settings.Default.numCols.ToString();
            size_box.Text = Properties.Settings.Default.minButtonSize.ToString();
            volume_box.Text = (Properties.Settings.Default.volume * 100).ToString();
            volume_slider.Value = Properties.Settings.Default.volume * 100;
            format_box.Text = "";
            Formats = Properties.Settings.Default.formats;
            listbox.ItemsSource = Formats;
            Theme.SelectedIndex = Properties.Settings.Default.themeType - 1;
            Load_Theme();
        }

        private void Load_Theme()
        {
            theme = new((ThemeType)Properties.Settings.Default.themeType);
            theme.SetPreferencesTheme(window, preferences_label, preferences_grid, button_grid);
        }

        private void Save_Changes(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(column_box.Text, out int cols) && cols > 0)
                Properties.Settings.Default.numCols = cols;
            if (int.TryParse(size_box.Text, out int size) && size > 0)
                Properties.Settings.Default.minButtonSize = size;
            if (volume_slider.Value >= 0 && volume_slider.Value <= 100)
                Properties.Settings.Default.volume = volume_slider.Value / 100.0;
            if (Theme.SelectedIndex >= 0 && Theme.SelectedIndex <= 2)   
                Properties.Settings.Default.themeType = Theme.SelectedIndex + 1;
            Properties.Settings.Default.formats = Formats;
            Properties.Settings.Default.Save();
            Load_Theme();
            Change_Main_Window();
        }

        private void Restore_Default(object sender, RoutedEventArgs e)
        {
            string temp = Properties.Settings.Default.folderPath;
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.folderPath = temp;
            Properties.Settings.Default.Save();
            Load_Preferences();
            Change_Main_Window();
        }

        private void Change_Main_Window()
        {
            _main?.Load_Soundboard();
        }

        private void Volume_Slider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volume_box.Text = ((int)volume_slider.Value).ToString();
        }

        private void Volume_Box_Changed(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(volume_box.Text, out int vol) && vol >= 0 && vol <= 100)
                volume_slider.Value = vol;
        }

        private void Add_Format(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(format_box.Text)) return;
            if (Formats.Contains(format_box.Text))
            {
                MessageBox.Show("This format is already in the list.", "Duplicate Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!format_box.Text.StartsWith('.'))
            {
                MessageBox.Show("File format should start with a period (e.g., .mp3, .wav)", "Invalid Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Formats.Add(format_box.Text);
            listbox.Items.Refresh();
            format_box.Text = "";
        }

        private void Remove_Format(object sender, RoutedEventArgs e)
        {
            Formats.Remove((string)listbox.SelectedItem);
            listbox.Items.Refresh();
        }
    }
}
