using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System;

namespace HashCalc
{
    internal class Calc
    {
        internal static MainWindow GUI;

        private FileInfo SelectedFile;
        private Hash FHash;
        private FileHashes FileHashValues = new FileHashes();
        private bool IsSingleFile;

        private FileInfo GetFileInfo(string pathToFile)
        {
            return new FileInfo(pathToFile);
        }

        internal async void CheckBoxEvent(Algorithm algo, CheckBox cb, ProgressBar pb, TextBox tb)
        {
            if (cb.IsChecked == true)
            {
                // Set Progress Bar visibility
                new GUIUpdate(pb, "Visibility", Visibility.Visible).Update();

                // Start Hash Task
                FileHash result = await FHash.GetFileHash(algo);

                // Update FileHashValues
                FileHashValues.Add(result);

                // Update Multiple elements
                new GUIUpdate(new List<GUIUpdate>() {
                    // When complete update GUI text box to contain calculated value
                    new GUIUpdate(tb, "Text", result.Value),
                    // Remove ProgressBar
                    new GUIUpdate(pb, "Visibility", Visibility.Hidden),
                    // Enable Compare button
                    new GUIUpdate(GUI.btnCompare, "IsEnabled", true),
                    // Enable Save to File Button
                    new GUIUpdate(GUI.btnSaveToFile, "IsEnabled", true)
                }).Update();
            }
        }

        internal void SaveToFile()
        {
            string destination = Path.GetDirectoryName(SelectedFile.FullName);
            Writer writer = new Writer(destination, SelectedFile, FileHashValues, IsSingleFile);
        }

        //internal static void CheckBoxUnchecked(ProgressBar pb, TextBox tb)
        //{
        //    // Set Progress Bar visibility
        //    MainWindow.UpdateGUI(new GUIUpdate(pb, "Visibility", Visibility.Hidden));

        //    // Set Textbox contents
        //    MainWindow.UpdateGUI(new GUIUpdate(tb, "Text", ""));
        //}

        internal Calc(UserFile file)
        {
            // Get File info for single file
            this.SelectedFile = GetFileInfo(file.FullFilePath);

            // Set Cacl properties
            this.IsSingleFile = true;

            // Setup Hash
            this.FHash = new Hash(this.SelectedFile);
        }
    }
}
