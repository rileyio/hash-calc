/**
 * C# VersionParser
 * Author: Jared Booker <thejaydox> 
 * License: MIT
 * ++++++++++++
 * Version 1.0.1
 **/

using System;
using System.Text.RegularExpressions;

namespace VersionParser
{
    public class Version
    {
        /// <summary>
        /// Original string version (OtherType.ToString) to work with
        /// </summary>
        public string ParsedFormat = "0000";
        /// <summary>
        /// Original string version (OtherType.ToString) to work with
        /// </summary>
        public string Original = "";
        /// <summary>
        /// Parses a long type for usage/output
        /// </summary>
        public long ParsedLong
        {
            get
            {
                return (long)Int64.Parse(this.ParsedMajor + this.ParsedMinor + this.ParsedPatch);
            }
        }
        public string ParsedMajor
        {
            get
            {
                return this.Major.ToString(ParsedFormat);
            }
        }
        public string ParsedMinor
        {
            get
            {
                return this.Minor.ToString(ParsedFormat);
            }
        }
        public string ParsedPatch
        {
            get
            {
                return this.Patch.ToString(ParsedFormat);
            }
        }

        // Separated
        public int Major = 0;
        public int Minor = 0;
        public int Patch = 0;
    }

    // Compare return adv (enum)
    public enum UpgradeType
    {
        Major,
        Minor,
        Patch,
        None
    }

    public class Upgrade
    {
        public bool IsUpgrade;
        public UpgradeType UpgradeType;
        public VersionParser.Version ComparedVersion;

        public Upgrade(bool isUpgrade, UpgradeType upgradeType, Version version)
        {
            this.IsUpgrade = isUpgrade;
            this.UpgradeType = upgradeType;
            this.ComparedVersion = version;
        }
    }

    public class VP
    {
        // Number of zeros (by default = 4) to adjust each version block by
        private string ParsedFormat = "0000";

        // Set bool type
        public bool IsString = false;
        public bool IsVersion = false;
        public bool IsFloat = false;

        // [String || Bool]
        private dynamic Input;

        // Regex strings [Defaults = 2] amount to select is calculated
        // at the parse from string step
        private string Major = @"^[0-9]{0,2}";
        private string Minor = @"^[0-9]{0,2}\.([0-9]{0,2})";
        private string Patch = @"^[0-9]{0,2}\.[0-9]{0,2}\.([0-9]{0,2})";

        public Version Version;

        /******************************
         * Methods - PRIVATE
         */
        private void ParseFromString(string input, out int major, out int minor, out int patch)
        {
            // If nothing new is specified in VP.ParsedFormat, 0000 = the following will be updated
            // to {0,4}
            this.Major = String.Format(@"^[0-9]{{0,{0}}}", this.ParsedFormat.Length);
            this.Minor = String.Format(@"^[0-9]{{0,{0}}}\.([0-9]{{0,{0}}})", this.ParsedFormat.Length);
            this.Patch = String.Format(@"^[0-9]{{0,{0}}}\.[0-9]{{0,{0}}}\.([0-9]{{0,{0}}})", this.ParsedFormat.Length);

            // Remove any leading string (Example: v1.2.1 -> 1.2.1)
            string sVer = Regex.Replace(input, @"[a-z]", "", RegexOptions.IgnoreCase);
            //this.Version.String = sVer;
            // Extract Major version
            major = ParseBlock(sVer, this.Major, 0);
            // Extract Minor version
            minor = ParseBlock(sVer, this.Minor, 1);
            // Extract Patch version
            patch = ParseBlock(sVer, this.Patch, 1);
        }

        private UpgradeType DetermineUpgradeType(long input)
        {
            int diff = ((int)input - (int)this.Version.ParsedLong);

            if (diff > 0 && diff < 10000) return UpgradeType.Patch;
            if (diff > 9999 && diff < 100000000) return UpgradeType.Minor;
            if (diff > 99999999) return UpgradeType.Major;

            // Fallback
            return UpgradeType.None;
        }

        private Upgrade CompareUpgrade(Version compareVersion)
        {
            // Perform comparison of parsed
            bool isUpgrade = compareVersion.ParsedLong > this.Version.ParsedLong;
            UpgradeType upgradeType = this.DetermineUpgradeType(compareVersion.ParsedLong);
            Version comparedVersion = compareVersion;

            return new Upgrade(isUpgrade, upgradeType, comparedVersion);
        }

        private int ParseBlock(string inp, string rex, int expectRexKey)
        {
            int output;

            bool valid = Int32.TryParse(Regex.Match(inp, rex, RegexOptions.IgnoreCase).Groups[expectRexKey].Value, out output);
            if (valid)
            {
                return output;
            }
            // Fallback
            return 0;
        }

        /// <summary>
        /// Used as a pass to constructor by actual constructors & overloads
        /// </summary>
        /// <param name="version"></param>
        /// <param name="format"></param>
        private void InitializeParser(string version, string format)
        {
            // Create new version object
            this.Version = new Version();
            // Set input
            this.Input = version;
            // Set Original Version
            this.Version.Original = version;
            // Set format to use inside VP
            this.ParsedFormat = format.ToString();
            // Set format used inside Version
            this.Version.ParsedFormat = this.ParsedFormat;

            // Start Parse
            ParseFromString(this.Version.Original,
                out this.Version.Major,
                out this.Version.Minor,
                out this.Version.Patch);
        }

        /******************************
         * Methods - PUBLIC
         */

        public Upgrade Compare(string input)
        {
            Version _compareVer = new Version();

            this.ParseFromString(input,
                out _compareVer.Major,
                out _compareVer.Minor,
                out _compareVer.Patch);

            return CompareUpgrade(_compareVer);
        }

        public Upgrade Compare(Version version)
        {
            return CompareUpgrade(version);
        }

        public Upgrade Compare(float input)
        {
            Version _compareVer = new Version();

            this.ParseFromString(input.ToString(),
                out _compareVer.Major,
                out _compareVer.Minor,
                out _compareVer.Patch);

            return CompareUpgrade(_compareVer);
        }

        /******************************
         * Constructors +Overloads
         */
        /// <summary>
        /// Initialize Version Parser with a string version
        /// </summary>
        /// <param name="input">Example: v1.52.4</param>
        public VP(string input)
        {
            // Set type
            this.IsString = true;
            // Initialize
            this.InitializeParser(input, "0000");
        }

        /// <summary>
        /// Initialize Version Parser with Type VersionParser.Version object passed
        /// </summary>
        /// <param name="version">/param>
        public VP(VersionParser.Version version)
        {
            // Set type
            this.IsVersion = true;
            /**
             * Replace this.Version
             * Don't call private intialize
             */
            this.Version = version;
        }

        /// <summary>
        /// Initialize Version Parser with number Type Float passed
        /// </summary>
        /// <param name="input">Example: 2.4</param>
        public VP(float input)
        {
            // Set type
            this.IsFloat = true;
            // Initialize
            this.InitializeParser(input.ToString(), "0000");
        }

        /// <summary>
        /// Initialize Version Parser with a string version and a format defined
        /// </summary>
        /// <param name="input">Example: v1.52.4</param>
        /// <param name="format">Maximum amount of zeros that could ever be used, Example/Default: 0000</param>
        public VP(string input, string format = "0000")
        {
            // Set type
            this.IsString = true;
            // Initialize
            this.InitializeParser(input, format);
        }

        /// <summary>
        /// Initialize Version Parser with number Type Float passed
        /// </summary>
        /// <param name="input">Example: 2.4</param>
        /// <param name="format">Maximum amount of zeros that could ever be used, Example/Default: 0000</param>
        public VP(float input, string format = "0000")
        {
            // Set type
            this.IsFloat = true;
            // Initialize
            this.InitializeParser(input.ToString(), "0000");
        }
    }
}
