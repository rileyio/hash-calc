using System;
using System.Reflection;
using System.Windows;

namespace HashCalc
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            // Populate Version TextBlock
            tbxVersion.Text = String.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private void btnSourceCode_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/TheJaydox/hash-calc");
        }

        private void txt3rdPartyLicenses_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void btnNewIssue_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/TheJaydox/hash-calc/issues/new");
        }

        private void btnCloseLicenses_Click(object sender, RoutedEventArgs e)
        {
            gridLicenses.Visibility = Visibility.Hidden;
        }

        private void btnLicenses_Click(object sender, RoutedEventArgs e)
        {
            new Licenses().Show();
        }

        private void btnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            Updater updater = new Updater();
            updater.CheckForUpdate();
        }
    }
}
