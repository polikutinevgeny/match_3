using System.Windows.Controls;

namespace match_3
{
    public static class Switcher
    {
        public static MainWindow PageSwitcher;

        public static void Switch(UserControl nextPage)
        {
            PageSwitcher.Navigate(nextPage);
        }
    }
}