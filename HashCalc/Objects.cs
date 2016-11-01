using System;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;

namespace HashCalc
{
    internal enum Algo
    {
        MD5,
        SHA1,
        SHA256,
        SHA512,
        Invalid
    }

    /// <summary>
    /// Pseudo collection of Flat Color Brushes
    /// </summary>
    public class FlatColor
    {
        public static Brush Red = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c"));
        public static Brush Green = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ecc71"));
        public static Brush Blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498db"));
        // Darker variants
        public static Brush DarkRed = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c0392b"));
        public static Brush DarkGreen = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60"));
        public static Brush DarkBlue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2980b9"));
    }

    /// <summary>
    /// File Hash value and Algorithm name
    /// </summary>
    internal class FileHash
    {
        internal string Algorithm;
        internal string Value;

        internal FileHash(string algorithm, string value)
        {
            this.Algorithm = algorithm;
            this.Value = value;
        }
    }

    /// <summary>
    /// Collection of File Hashes - Example usage: for a single file, collect all
    ///                                            algorithms calculated with.
    /// </summary>
    internal class FileHashes
    {
        internal List<FileHash> Hashes = new List<FileHash>();

        internal void Add(FileHash hash)
        {
            this.Hashes.Add(hash);
        }
    }

    [Serializable]
    internal class UserFile
    {
        internal string Name;
        internal string Extension;
        internal string FullFilePath;

        public UserFile(OpenFileDialog file)
        {
            this.Name = file.SafeFileName;
            this.FullFilePath = file.FileName;
            this.Extension = Path.GetExtension(this.FullFilePath);
        }
    }

    // For updating a set of GUI elements.
    // - txt{algo}
    // - pb{algo}
    // - chk{algo}
    internal class GUIAlgoElements
    {
        internal ProgressBar progresbar { get; set; }
        internal TextBox textbox { get; set; }
        internal CheckBox checkbox { get; set; }
    }

    /// <summary>
    /// default:MainWindow Update to perform
    /// </summary>
    internal class GUIUpdate
    {
        internal List<GUIUpdate> UpdateList = new List<GUIUpdate>();
        // Required
        internal dynamic Element;
        internal string Property;
        internal dynamic Value;

        /// <summary>
        /// Setup GUIUpdate instance for default:MainWindow
        /// </summary>
        /// <param name="el"></param>
        /// <param name="prop"></param>
        /// <param name="val"></param>
        public GUIUpdate(dynamic el, string prop, dynamic val)
        {
            // Element, Property & Value to update prop to
            this.Element = el;
            this.Property = prop;
            this.Value = val;

            this.UpdateList.Add(this);
        }

        public GUIUpdate(List<GUIUpdate> multiple)
        {
            this.UpdateList = multiple;
        }

        /// <summary>
        /// Commit change
        /// </summary>
        public void Update()
        {
            if (this.UpdateList.Count > 1)
            {
                // Call update - Multiple
                MainWindow.UpdateGUI(UpdateList);
            }
            else
            {
                MainWindow.UpdateGUI(this);
            }
        }
    }
}
