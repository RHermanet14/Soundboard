using Microsoft.Win32;
using System;
using System.IO;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Soundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int file_count, numCols, numRows;
        private string? folder_path;
        private string[]? directory;
        private static readonly string[] formats = [".mp3", ".wav", ".aiff", ".wma", ".aac", ".flac"];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void On_Startup(object sender, RoutedEventArgs e)
        {
            Load_Preferences();
            Load_Soundboard();
        }

        private void Load_Preferences()
        {
            // Load user preferences
            numCols = Properties.Settings.Default.numCols;
            if (!Properties.Settings.Default.folderPath.IsWhiteSpace() && Properties.Settings.Default.folderPath != null)
            {
                folder_path = Properties.Settings.Default.folderPath;
                return;
            }
            folder_path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            folder_path ??= Environment.CurrentDirectory;
        }

        private void Load_Soundboard()
        {
            Clear_Grid();
            Read_Folder();
            Initialize_Grid();
        }

        private void Clear_Grid()
        {
            g.Children.Clear();
            g.RowDefinitions.Clear();
            g.ColumnDefinitions.Clear();
        }

        private void Read_Folder()
        {
            if (folder_path == null || folder_path.IsWhiteSpace())
            {
                file_count = 0;
                return;
            }
            directory = [.. Directory.GetFiles(folder_path).Where(file => formats.Any(file.ToLower().EndsWith))];
            file_count = directory.Length;
        }

        private void Open_Folder(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog ofd = new()
            {
                InitialDirectory = folder_path,
                Title = "Choose a Folder"
            };
            if(ofd.ShowDialog() == true)
            {
                folder_path = ofd.FolderName;
                Properties.Settings.Default.folderPath = folder_path;
                Properties.Settings.Default.Save();
                Load_Soundboard();
            }
        }

        private void Close_Soundboard(object sender, RoutedEventArgs e)
        {
            folder_path = null;
            Properties.Settings.Default.folderPath = folder_path;
            Properties.Settings.Default.Save();
            Load_Soundboard();
        }

        private void Exit_Application(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Initialize_Grid()
        {
            if (file_count == 0 || folder_path == null)
            {
                // Do special screen for no files
                TextBlock noFilesText = new()
                {
                    Text = "Select a folder",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 24,
                };
                g.Children.Add(noFilesText);
                return;
            } else if (numCols == 0)
            {
                MessageBox.Show("Number of columns cannot be zero.");
                return;
            }
            for(int i = 0; i < numCols; i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
            }
            numRows = (int)(Math.Ceiling((decimal)file_count / (decimal)numCols));
            for (int i = 0; i < numRows; i++)
            {
                g.RowDefinitions.Add(new RowDefinition());
            }
            if (directory == null) return;
            int fileIndex = 0;
            foreach (string file in directory)
            {
                Button button = new()
                {
                    Content = file.Substring(folder_path.Length + 1, file.Length - folder_path.Length - 5),
                    Margin = new Thickness(20),
                    MinHeight = 50,
                    Tag = file
                };
                button.Click += Button_Click;
                Grid.SetRow(button, fileIndex / numCols);
                Grid.SetColumn(button, fileIndex % numCols);
                g.Children.Add(button);
                fileIndex++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // get which button was clicked
            if (e.Source is Button button)
            {
                if (button.Tag is string file)
                {
                    MediaPlayer player = new();
                    player.Open(new Uri($"{file}"));
                    player.Play();
                }
            }
        }
    }
}