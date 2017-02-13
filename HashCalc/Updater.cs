using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows;

namespace HashCalc
{
    class Updater
    {
        private string UpdateURI = "https://api.github.com/repos/TheJaydox/hash-calc/releases/latest";
        private void OpenUpdatePage(string remoteAddress)
        {
            System.Diagnostics.Process.Start(remoteAddress);
        }

        private string GetLocalVersion()
        {
            return Regex.Match(Assembly.GetExecutingAssembly().GetName().Version.ToString(), @"([0-9]{0,2}\.[0-9]{0,2}\.[0-9]{0,2})").Groups[0].Value.ToString();
        }

        private Update ParseUpdate(RawUpdate raw)
        {
            return new Update() {
                LocalVersion = new VersionParser.VP(this.GetLocalVersion()),
                RemoteVersion = new VersionParser.VP(raw.tag_name),
                Tag = raw.tag_name,
                Prerelease = raw.prerelease,
                Draft = raw.draft,
                URI = raw.html_url
            };
        }

        private void ParseReply(string resp)
        {
            try
            {
                RawUpdate se = new JavaScriptSerializer().Deserialize<RawUpdate>(resp);

                Update update = ParseUpdate(se);
                Console.WriteLine(String.Format("Update->Tag: {0}, HTMLuri: {1}, localVer: {2}, remoteVer: {3}, Body: {4}",
                    update.Tag,
                    update.URI,
                    update.LocalVersion.Version.Original,
                    update.RemoteVersion.Version.Original,
                    update.Body));

                // Show message box if update is available
                if (update.LocalVersion.Compare(update.RemoteVersion.Version).IsUpgrade)
                {
                    Console.WriteLine(String.Format("Update->Available! {0}", update.RemoteVersion.Version.Original));
                    this.PromptUpdateAvailable(update);
                }
                else
                {
                    Console.WriteLine("Update->None Available");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update[Catch]->", ex.Message);
            }
        }

        private void PromptUpdateAvailable(Update update)
        {
            MessageBoxResult result = MessageBox.Show(
                String.Format(
                    "HashCalc version {0} is now available, would you like to download the update?",
                    update.RemoteVersion.Version.Original),
                "Update Available!", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                this.OpenUpdatePage(update.URI);
            }
        }

        public void CheckForUpdate()
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "HashCalc");
            try
            {
                string reply = client.DownloadString(this.UpdateURI);

                this.ParseReply(reply);
            }
            catch (WebException ex)
            {
                Console.WriteLine(String.Format("Update[Catch]->{0}", ((HttpWebResponse)ex.Response).StatusCode));
            }

        }
    }
}
