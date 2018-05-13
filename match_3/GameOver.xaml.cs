using System.Windows;
using System.Windows.Controls;

namespace match_3
{
    /// <summary>
    /// Interaction logic for GameOver.xaml
    /// </summary>
    public partial class GameOver : UserControl
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Points { get; }

        public GameOver(int points)
        {
            InitializeComponent();
            Points = points;
            DataContext = this;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }
    }
}
