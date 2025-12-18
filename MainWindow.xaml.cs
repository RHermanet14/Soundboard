using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        //private static readonly string[] formats = [".mp3", ".wav", ".aiff", ".wma", ".aac", ".flac"]; // Change to application setting
        private System.Collections.Specialized.StringCollection Formats { get; set; } = [];
        private static MediaPlayer[] sounds = [];
        private bool is_fullscreen = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Startup
        private void On_Startup(object sender, RoutedEventArgs e)
        {
            Load_Soundboard();
            KeyDown += new KeyEventHandler(Fullscreen_Keys);
        }

        private void Fullscreen_Keys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
                Fullscreen(this, new RoutedEventArgs());
            else if (e.Key == Key.Escape && is_fullscreen)
                Fullscreen(this, new RoutedEventArgs());
        }
        #endregion

        #region main soundboard logic
        public void Load_Soundboard()
        {
            Load_Preferences();
            Clear_Grid();
            Clear_Sounds();
            Read_Folder();
            Initialize_Grid();
        }

        private void Load_Preferences()
        {
            // Load user preferences
            Button_Visibility_Helper(Properties.Settings.Default.cancel, clb, cancel_check);
            Button_Visibility_Helper(Properties.Settings.Default.refresh, rfb, refresh_check);
            Formats = Properties.Settings.Default.formats;
            numCols = Properties.Settings.Default.numCols;
            if (!Properties.Settings.Default.folderPath.IsWhiteSpace() && Properties.Settings.Default.folderPath != null && Directory.Exists(Properties.Settings.Default.folderPath))
            {
                folder_path = Properties.Settings.Default.folderPath;
                return;
            }
            folder_path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            folder_path ??= Environment.CurrentDirectory;
        }

        private static void Button_Visibility_Helper(bool visible, Button button, MenuItem menuItem)
        {
            if (visible)
            {
                button.Visibility = Visibility.Visible;
                menuItem.IsChecked = true;
            }
            else
            {
                button.Visibility = Visibility.Collapsed;
                menuItem.IsChecked = false;
            }
        }

        private void Clear_Grid()
        {
            g.Children.Clear();
            g.RowDefinitions.Clear();
            g.ColumnDefinitions.Clear();
        }

        private static void Clear_Sounds()
        {
            foreach (MediaPlayer player in sounds)
            {
                player.Close();
            }
            sounds = [];
        }

        private void Read_Folder()
        {
            if (folder_path == null || folder_path.IsWhiteSpace())
            {
                file_count = 0;
                return;
            }
            var formats = Formats.Cast<string>().ToArray();

            directory =
            [
                .. Directory.GetFiles(folder_path)
                .Where(file => formats.Any(ext =>
                file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            ];
            file_count = directory.Length;
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
            }
            else if (numCols == 0)
            {
                MessageBox.Show("Number of columns cannot be zero.");
                return;
            }
            for (int i = 0; i < numCols; i++)
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
                MediaPlayer player = new();
                player.Open(new Uri($"{file}"));
                player.Volume = Properties.Settings.Default.volume;
                sounds = [.. sounds, player];

                Button button = new()
                {
                    Content = file.Substring(folder_path.Length + 1, file.Length - folder_path.Length - 5),
                    Margin = new Thickness(20),
                    MinHeight = Properties.Settings.Default.minButtonSize,
                    Tag = fileIndex
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
                if (button.Tag is int index)
                {
                    sounds[index].Play();
                }
            }
        }
        #endregion

        #region File buttons
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
        #endregion

        #region view buttons
        private void Fullscreen(object sender, RoutedEventArgs e)
        {
            is_fullscreen = !is_fullscreen;
            if (is_fullscreen)
            {
                window.WindowState = WindowState.Maximized;
                window.WindowStyle = WindowStyle.None;
            }
            else
            {
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        private void Toggle_Cancel_Button(object sender, RoutedEventArgs e)
        {
            if (clb.IsVisible)
                clb.Visibility = Visibility.Collapsed;
            else
                clb.Visibility = Visibility.Visible;
            Properties.Settings.Default.cancel = !Properties.Settings.Default.cancel;
            Properties.Settings.Default.Save();
        }

        private void Toggle_Refresh_Button(object sender, RoutedEventArgs e)
        {
            if (rfb.IsVisible)
                rfb.Visibility = Visibility.Collapsed;
            else
                rfb.Visibility = Visibility.Visible;
            Properties.Settings.Default.refresh = !Properties.Settings.Default.refresh;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Option buttons
        private void Open_Preferences(object sender, RoutedEventArgs e)
        {
            PreferencesWindow pfw = new(this);
            pfw.Show();
        }

        private void Modify_Sounds(object sender, RoutedEventArgs e)
        {
            if (folder_path == null || folder_path.IsWhiteSpace())
                return;
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    Arguments = folder_path,
                    FileName = "explorer.exe",
                    Verb ="runas"
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open folder: {ex.Message}");
            }
        }

        private void Add_Sound(object sender, RoutedEventArgs e)
        {
            string filter = "";
            int i;
            for(i = 0; i < Formats.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(Formats[i]))
                    filter += "*" + Formats[i] + ";";
            }
            filter += "*" + Formats[i];
            filter = $"Acceptable Formats ({filter})|{filter}";
            OpenFileDialog ofd = new()
            {
                InitialDirectory = folder_path,
                Title = "Add a File",
                Filter = filter
            };
            if (ofd.ShowDialog() == true)
            {
                string file = ofd.FileName;
                int last_index = file.LastIndexOf('\\');
                if (last_index == -1 || folder_path == null)
                    return;
                file = file[(last_index + 1)..];
                string dest_path = System.IO.Path.Combine(folder_path, file);
                if(File.Exists(dest_path))
                {
                    MessageBox.Show("File already exists in the soundboard folder.");
                    return;
                }
                File.Copy(ofd.FileName, dest_path);
                Load_Soundboard();
            }
        }
        #endregion

        #region bottom buttons
        private void Refresh_Soundboard(object sender, RoutedEventArgs e)
        {
            Load_Soundboard();
        }

        private void Stop_Sounds(object sender, RoutedEventArgs e)
        {
            // Stop all sounds
            foreach (MediaPlayer player in sounds)
            {
                player.Stop();
            }
        }
        #endregion
    }
}