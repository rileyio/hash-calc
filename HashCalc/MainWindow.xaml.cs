using System.Windows;
using cslog;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Win32;
using System.ComponentModel;

namespace HashCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static OpenFileDialog FileDialog;
        private static UserFile SelectedFile;
        private static Calc HashCalc;
        private static MainWindow GUI;

        public MainWindow()
        {
            InitializeComponent();

            Logger.LogFile = "hashcalc.log";
            Logger.Log(Logger.Init);

            // Show Debug window
            //Logger.DebugWindow();

            Calc.GUI = this;
            GUI = this;
        }

        internal static void OpenFile()
        {
            FileDialog = new OpenFileDialog();
            FileDialog.Title = "Select File to Hash";
            FileDialog.Multiselect = false;
            FileDialog.FileOk += new CancelEventHandler(FileOK);

            FileDialog.ShowDialog();
            Logger.Log("Open File Dialog");
        }

        private static void FileOK(object sender, CancelEventArgs e)
        {
            GUI.ResetGUI();
            SelectedFile = new UserFile(FileDialog);

            Logger.Log("File Selected: " + SelectedFile.Name);
            Logger.Object<UserFile>(SelectedFile);

            // Initialize Hash
            HashCalc = new Calc(SelectedFile);

            // Update Multiple elements
            new GUIUpdate(new List<GUIUpdate>() {
                // Enable Clear button
                new GUIUpdate(GUI.btnClear, "IsEnabled", true),
                // Set GUI selected item
                new GUIUpdate(GUI.tbxSelected, "Text", SelectedFile.Name),
                // Hide INFO message
                new GUIUpdate(GUI.infoSelectFile, "Visibility", Visibility.Hidden),
                new GUIUpdate(GUI.gridOutput, "Visibility", Visibility.Visible)
            }).Update();
        }

        private void ResetGUI()
        {
            // Update Multiple elements
            new GUIUpdate(new List<GUIUpdate>() {
                // Reset Textboxes
                new GUIUpdate(this.txtMD5, "Text", ""),
                new GUIUpdate(this.txtSHA1, "Text", ""),
                new GUIUpdate(this.txtSHA256, "Text", ""),
                new GUIUpdate(this.txtSHA512, "Text", ""),
                // Reset Checkboxes
                new GUIUpdate(this.chkMD5, "IsChecked", false),
                new GUIUpdate(this.chkSHA1, "IsChecked", false),
                new GUIUpdate(this.chkSHA256, "IsChecked", false),
                new GUIUpdate(this.chkSHA512, "IsChecked", false),
                // Enable all checkboxes
                new GUIUpdate(this.chkMD5, "IsEnabled", true),
                new GUIUpdate(this.chkSHA1, "IsEnabled", true),
                new GUIUpdate(this.chkSHA256, "IsEnabled", true),
                new GUIUpdate(this.chkSHA512, "IsEnabled", true),
                // Hide Checkboxes & Text Inputs
                new GUIUpdate(this.gridOutput, "Visibility", Visibility.Hidden),
                // Reset UI to start
                new GUIUpdate(this.infoSelectFile, "Visibility", Visibility.Visible),
                // Reset File & path
                new GUIUpdate(this.tbxSelected, "Text", ""),
                // Disable compare button
                new GUIUpdate(this.btnCompare, "IsEnabled", false),
                // Disable clear button
                new GUIUpdate(this.btnClear, "IsEnabled", false),
                // Disable Save button
                new GUIUpdate(this.btnSaveToFile, "IsEnabled", false)
            })
            .Update(); // Perform update on all elements
        }

        /// <summary>
        /// Callable action to invoke update from another Task/Thread
        /// </summary>
        /// <param name="update">GUIUpdate Object containing Element & property to perform update</param>
        internal static void RunUpdateGUI(GUIUpdate update)
        {
            //Console.WriteLine(caller);
            Action action = () => UpdateGUI(update);
            Application.Current.Dispatcher.Invoke(action);
        }

        /// <summary>
        /// Update MainWindow GUI contained element (Default)
        /// </summary>
        /// <param name="update">GUIUpdate Object containing Element & property to perform update</param>
        internal static void UpdateGUI(GUIUpdate update)
        {
            string GUIUpdateEntry = String.Format("Updating: [{0}] Value: [{1}] ",
                update.Element.ToString(),
                update.Value.ToString());

            Logger.Log(GUIUpdateEntry);

            update.Element.GetType()
                .GetProperty(update.Property)
                .SetValue(update.Element, update.Value, null);
        }

        /// <summary>
        /// Update MainWindow GUI contained elements (Multiple Passed)
        /// </summary>
        /// <param name="GUIUpdates">List of GUIUpdates to perform</param>
        internal static void UpdateGUI(List<GUIUpdate> GUIUpdates)
        {
            Logger.Log(String.Format("Update GUI called +{0}", GUIUpdates.Count.ToString()));

            foreach (GUIUpdate update in GUIUpdates)
            {
                UpdateGUI(update);
            }
        }

        /// <summary>
        /// Update MainWindow GUI contained elements (Multiple Passed)
        /// </summary>
        /// <param name="window">List of GUIUpdates to perform</param>
        /// <param name="GUIUpdates">List of GUIUpdates to perform</param>
        internal static void UpdateGUI<T>(T window, GUIUpdate GUIUpdates)
        {
            Console.WriteLine(window);
        }

        /// <summary>
        /// Event Handler for Compare input text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCompareInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = (sender as TextBox).Text;
            Algo algo = GetAlgo(input);

            if (algo == Algo.Invalid)
            {

                Console.WriteLine("Invalid");

                // Update Multiple elements
                new GUIUpdate(new List<GUIUpdate>() {
                    new GUIUpdate(
                        tbxCompareMatchResults,
                        "Text",
                        "Invalid Input!"
                    ),

                    new GUIUpdate(
                        tbxCompareMatchResults,
                        "Foreground",
                        FlatColor.DarkRed
                    )
                }).Update();

            }
            else
            {
                string comparedResults = CompareValues(algo, input);

                // Update Results Text
                new GUIUpdate(
                     this.tbxCompareMatchResults,
                     "Text",
                     comparedResults
                 ).Update();

                if (comparedResults != "No Match!")
                {
                    new GUIUpdate(
                        tbxCompareMatchResults,
                        "Foreground",
                        // Update Text Color
                        FlatColor.DarkGreen
                    ).Update();
                }
                else
                {
                    new GUIUpdate(
                        tbxCompareMatchResults,
                        "Foreground",
                        // Update Text Color
                        FlatColor.DarkRed
                    ).Update();
                }
            }
        }

        private static Algo GetAlgo (string input)
        {
            switch (input.Length)
            {
                case 32:
                    return Algo.MD5;
                case 40:
                    return Algo.SHA1;
                case 64:
                    return Algo.SHA256;
                case 128:
                    return Algo.SHA512;
                default:
                    return Algo.Invalid;
            }
        }

        private string CompareValues(Algo algo, string input)
        {
            string md5 = txtMD5.Text.ToLower();
            string sha1 = txtSHA1.Text.ToLower();
            string sha256 = txtSHA256.Text.ToLower();
            string sha512 = txtSHA512.Text.ToLower();

            Console.WriteLine("md5 " + md5);
            Console.WriteLine("sha1 " + sha1);
            Console.WriteLine("sha256 " + sha256);
            Console.WriteLine("sha512 " + sha512);

            if (md5 == input.ToLower())
            {
                return "MD5 Match!";
            }
            else if (sha1 == input.ToLower())
            {
                return "SHA1 Match!";
            }
            else if (sha256 == input.ToLower())
            {
                return "SHA256 Match!";
            }
            else if (sha512 == input.ToLower())
            {
                return "SHA512 Match!";
            }
            else
            {
                return "No Match!";
            }
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFile();
            }
            catch (System.Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.ResetGUI();
        }

        private void chkMD5_Checked(object sender, RoutedEventArgs e)
        {
            HashCalc.CheckBoxEvent(
                Algorithm.MD5,
                chkMD5,
                pbMD5,
                txtMD5
            );

            // Disable this checkbox
            new GUIUpdate(this.chkMD5, "IsEnabled", false).Update();
        }

        private void chkSHA1_Checked(object sender, RoutedEventArgs e)
        {
            HashCalc.CheckBoxEvent(
                Algorithm.SHA1,
                chkSHA1,
                pbSHA1,
                txtSHA1
            );

            // Disable this checkbox
            new GUIUpdate(this.chkSHA1, "IsEnabled", false).Update();
        }

        private void chkSHA256_Checked(object sender, RoutedEventArgs e)
        {
            HashCalc.CheckBoxEvent(
                Algorithm.SHA256,
                chkSHA256,
                pbSHA256,
                txtSHA256
            );

            // Disable this checkbox
            new GUIUpdate(this.chkSHA256, "IsEnabled", false).Update();
        }

        private void chkSHA512_Checked(object sender, RoutedEventArgs e)
        {
            HashCalc.CheckBoxEvent(
                Algorithm.SHA512,
                chkSHA512,
                pbSHA512,
                txtSHA512
            );

            // Disable this checkbox
            new GUIUpdate(this.chkSHA512, "IsEnabled", false).Update();
        }

        private void btnCompare_Click_1(object sender, RoutedEventArgs e)
        {
            // Update Multiple elements
            new GUIUpdate(new List<GUIUpdate>() {
                // Clear Compare value
                new GUIUpdate(this.txtCompareInput, "Text", ""),
                // Remove results text
                new GUIUpdate(this.tbxCompareMatchResults, "Text", ""),
                // Show Compare Overlay
                new GUIUpdate(this.gridCompareOverlay, "Visibility", Visibility.Visible)
            }).Update();

            txtCompareInput.Focus();
        }

        private void btnCompareClose_Click(object sender, RoutedEventArgs e)
        {
            // Show Compare Overlay
            new GUIUpdate(this.gridCompareOverlay, "Visibility", Visibility.Hidden).Update();
        }

        private void ToggleCompareBtn (bool enabled)
        {
            new GUIUpdate(btnCompare, "IsEnabled", enabled).Update();
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            new About().Show();
        }

        private void btnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            HashCalc.SaveToFile();
        }
    }
}
