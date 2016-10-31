using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HashCalc
{
    internal class Hash
    {
        // For full Directory hashing
        internal string PassedDirectoryPath;
        internal List<FileInfo> DirectoryFiles = new List<FileInfo>();

        // For individual file hashing
        internal string PassedFilePath;
        internal FileInfo SelectedFile;
        internal FileHashes Hashes = new FileHashes();

        private FileInfo GetFileInfo(string pathToFile)
        {
            return new FileInfo(pathToFile);
        }

        private string HashFile(HashAlgorithm algo, string file)
        {
            byte[] ByteHash = null;
            StringBuilder sb = new StringBuilder();

            // Stream read file & compute hash
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ByteHash = algo.ComputeHash(fs);
            }

            // Byte array -> Hex String
            foreach (var b in ByteHash)
            {
                // X2 - Uppercase
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        internal async Task<string> GetFileHash(HashAlgorithm algo)
        {
            return await Task.Run(() => HashFile(algo, SelectedFile.FullName));
        }

        internal Hash(DirectoryInfo directoryPath)
        {
            // Update class Variables
        }

        internal Hash(UserFile file)
        {
            // Update Class Variables
            this.PassedFilePath = file.FullFilePath;
            // Get File info for single file
            this.SelectedFile = GetFileInfo(this.PassedFilePath);
        }
    }
}
