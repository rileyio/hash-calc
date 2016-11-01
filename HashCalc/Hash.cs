using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace HashCalc
{
    /// <summary>
    /// Retrieve Algorithm via:      Algorithm.SHA512
    /// Available Methods/Props:     .Algo()   // Type:HashAlgorithm
    ///                              .Name     // Type:String
    /// </summary>
    public class Algorithm
    {
        public string Name;
        public HashAlgorithm Algo;

        private Algorithm(string algoString, HashAlgorithm algo)
        {
            Name = algoString;
            Algo = algo;
        }

        public static Algorithm MD5 { get { return new Algorithm("MD5", System.Security.Cryptography.MD5.Create()); } }
        public static Algorithm SHA1 { get { return new Algorithm("SHA1", System.Security.Cryptography.SHA1.Create()); } }
        public static Algorithm SHA256 { get { return new Algorithm("SHA256", System.Security.Cryptography.SHA256.Create()); } }
        public static Algorithm SHA512 { get { return new Algorithm("SHA512", System.Security.Cryptography.SHA512.Create()); } }
    }

    /// <summary>
    /// Used for calculating hash values
    /// </summary>
    internal class Hash
    {
        // For full Directory hashing
        //internal string PassedDirectoryPath;
        internal List<FileInfo> DirectoryFiles = new List<FileInfo>();

        // For individual file hashing
        internal FileInfo SelectedFile;

        private FileHash HashFile(Algorithm algo, string file)
        {
            byte[] ByteHash = null;
            StringBuilder sb = new StringBuilder();

            // Stream read file & compute hash
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ByteHash = algo.Algo.ComputeHash(fs);
            }

            // Byte array -> Hex String
            foreach (var b in ByteHash)
            {
                // X2 - Uppercase
                sb.Append(b.ToString("X2"));
            }

            return new FileHash(algo.Name, sb.ToString());
        }

        internal async Task<FileHash> GetFileHash(Algorithm algo)
        {
            return await Task.Run(() => HashFile(algo, SelectedFile.FullName));
        }

        internal Hash(DirectoryInfo directoryPath)
        {
            // Update class Variables
        }

        internal Hash(FileInfo file)
        {
            // Update Class Variables
            this.SelectedFile = file;
        }
    }
}
