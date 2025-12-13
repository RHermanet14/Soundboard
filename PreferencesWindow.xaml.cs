using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
            format_box.Text = "mp3";
        }

        private void Save_Changes(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(column_box.Text, out int cols) && cols > 0)
                Properties.Settings.Default.numCols = cols;
            if (int.TryParse(size_box.Text, out int size) && size > 0)
                Properties.Settings.Default.minButtonSize = size;
            if (volume_slider.Value >= 0 && volume_slider.Value <= 100)
                Properties.Settings.Default.volume = volume_slider.Value / 100.0;
            Properties.Settings.Default.Save();
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
    }
}
