using System;
using System.IO;
using System.Linq;

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

                // Sort array [md5, sha1, sha256, sha512]
                var hashes = Hashes.Hashes.OrderBy(hash => hash.Algorithm);

                // Insert Blank line
                writer.WriteLine("");

                // Write each Hash Algorithm from Collection (FIFO order list)
                foreach (FileHash sum in hashes)
                {
                    // Number of spaces for formatting
                    int spaces = 0;
                    switch (sum.Algorithm)
                    {
                        case "MD5":
                            spaces = 5;
                            break;
                        case "SHA1":
                            spaces = 4;
                            break;
                        case "SHA256":
                            spaces = 2;
                            break;
                        case "SHA512":
                            spaces = 2;
                            break;
                    }

                    writer.WriteLine(String.Format("{0}{1}{2}",
                        sum.Algorithm,
                        String.Concat(Enumerable.Repeat(" ", spaces)),
                        sum.Value
                    ));
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
