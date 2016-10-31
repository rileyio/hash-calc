using Microsoft.Win32;
using System.ComponentModel;
using cslog;
using System.Windows;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Collections.Generic;

namespace HashCalc
{
    internal static class Calc
    {
        internal static MainWindow GUI;

        // Select File
        private static OpenFileDialog FileDialog;
        private static Hash FileHash;
        internal static UserFile SelectedFile;

        internal static async void CheckBoxEvent(HashAlgorithm algo, CheckBox cb, ProgressBar pb, TextBox tb)
        {
            if (cb.IsChecked == true)
            {
                // Set Progress Bar visibility
                new GUIUpdate(pb, "Visibility", Visibility.Visible).Update();

                // Start Hash Task
                string result = await FileHash.GetFileHash(algo);

                // Update Multiple elements
                new GUIUpdate(new List<GUIUpdate>() {
                    // When complete update GUI text box to contain calculated value
                    new GUIUpdate(tb, "Text", result),
                    // Remove ProgressBar
                    new GUIUpdate(pb, "Visibility", Visibility.Hidden),
                    // Enable Compare button
                    new GUIUpdate(GUI.btnCompare, "IsEnabled", true)
                }).Update();
            }
        }

        internal static void CheckBoxUnchecked(ProgressBar pb, TextBox tb)
        {
            // Set Progress Bar visibility
            MainWindow.UpdateGUI(new GUIUpdate(pb, "Visibility", Visibility.Hidden));

            // Set Textbox contents
            MainWindow.UpdateGUI(new GUIUpdate(tb, "Text", ""));
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
            SelectedFile = new UserFile(FileDialog);

            Logger.Log("File Selected: " + SelectedFile.Name);
            Logger.Object<UserFile>(SelectedFile);

            // Initialize Hash
            FileHash = new Hash(SelectedFile);

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
    }
}
