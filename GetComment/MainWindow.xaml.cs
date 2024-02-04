using System.Windows;

namespace GetComment
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFileWork.GetFolderPath();
            directoryDescription.Content = MainFileWork._path;
        }

        private void Button_Click_1(object _, RoutedEventArgs _1) =>
            MainFileWork.SetFile();
        
    }
}