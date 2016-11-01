using System;
using System.IO;

namespace HashCalc
{
    class Writer
    {
        private FileInfo InitialFile;
        private FileHashes Hashes;
        private string Destination;

        private void WriteSingleFile()
        {
            using (TextWriter writer = File.CreateText(String.Format("{0}\\{1}.sums", Destination, InitialFile.Name)))
            {
                // Write file name & details
                writer.WriteLine(String.Format("Name:\t{0}", InitialFile.Name));
                writer.WriteLine(String.Format("Bytes:\t{0}", InitialFile.Length));

                // Insert Blank line
                writer.WriteLine("");

                // Write each Hash Algorithm from Collection (FIFO order list)
                foreach (FileHash sum in Hashes.Hashes)
                {
                    writer.WriteLine(String.Format("{0}\t{1}", sum.Algorithm, sum.Value));
                }

                writer.Close();
            }
        }

        internal Writer(string destination, FileInfo file, FileHashes fileHashes, bool single)
        {
            // Set Class properties
            this.InitialFile = file;
            this.Destination = destination;
            this.Hashes = fileHashes;

            if (single == true) { WriteSingleFile(); }
        }
    }
}
