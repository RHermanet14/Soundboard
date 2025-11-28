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
using System.Media;

namespace Soundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int file_count, numCols, numRows;
        private string? folder_path;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void On_Startup(object sender, RoutedEventArgs e)
        {
            Load_Preferences();
            Read_Folder();
            Initialize_Grid();
        }

        private void Load_Preferences()
        {
            // Load user preferences
            numCols = Properties.Settings.Default.numCols;
            folder_path = Properties.Settings.Default.folderPath;
        }

        private void Read_Folder()
        {
            // Read sound files from folder
            file_count = 10; // Set amount for now
        }

        private void Initialize_Grid()
        {
            //numCols = g.ColumnDefinitions.Count;
            if (file_count == 0)
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
            for (int i = 0; i < file_count; i++)
            {
                Button button = new()
                {
                    Content = $"Button {i + 1}",
                    Margin = new Thickness(20),
                    MinHeight = 50
                };
                button.Click += Button_Click;
                Grid.SetRow(button, i / numCols);
                Grid.SetColumn(button, i % numCols);
                g.Children.Add(button);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // get which button was clicked
            if (e.Source is Button button)
            {
                if (button.Content is string content)
                {
                    MediaPlayer player = new();
                    player.Open(new Uri(@$"{folder_path}\{content} .mp3")); // Remove quotes from folder_path when saving
                    player.Play();
                }
            }
        }
    }
}