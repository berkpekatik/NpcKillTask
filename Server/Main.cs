using CitizenFX.Core;
using Newtonsoft.Json;
using Server.Model;
using System;
using System.IO;
using System.Net;

namespace Server
{
    public class Main : BaseScript
    {
        private static ConfigModel config;
        public Main()
        {
            try
            {
                var file = File.ReadAllText(Environment.CurrentDirectory + @"\resources\test\config.json");
                config = JsonConvert.DeserializeObject<ConfigModel>(file);
                Debug.WriteLine("Config dosyasi basariyla okundu!" + config.ServerKey);
                IsWhitelist("anan");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Config dosyasi okunamadi! " + e.Message);
            }
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
        }

        private async void OnPlayerConnecting([FromSource]Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            if (config == null)
            {
                setKickReason("Config dosyasına ulaşılamadı lütfen kontrol edin!");
                return;
            }
            Debug.WriteLine();
            deferrals.defer();

            await Delay(0);

            var licenseIdentifier = player.Identifiers["steam"];
            Debug.WriteLine(licenseIdentifier);
            deferrals.update($"Oyuna giriş bilgilerin kontrol ediliyor {playerName}");

            if (!IsWhitelist(licenseIdentifier))
            {
                setKickReason($"Launcher olmadan giriş yapamazsın {playerName}");
                return;
            }

            ActiveUser(licenseIdentifier);
            deferrals.update($"Giriş yapılıyor... {playerName}");
            await Delay(2000);

            deferrals.done();
        }
        private bool IsWhitelist(string hex)
        {
           
            HttpWebRequest request = WebRequest.Create($"http://api.fivemcode.com/kontrol.php?sunucuid={config.ServerKey}&steamhexid={hex}") as HttpWebRequest;
            Debug.WriteLine(config.ServerKey);
            request.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml,         image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, /";
            request.Headers["Accept-Language"] = "tr-TR";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Method = "GET";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {

                var reader = new StreamReader(response.GetResponseStream());
                Debug.WriteLine(reader.ReadToEnd());
                return reader.ReadToEnd() == "Online";
            }
        }

        private void ActiveUser(string hex)
        {
            HttpWebRequest request = WebRequest.Create($"http://api.fivemcode.com/guncelle.php?sunucuid={config.ServerKey}&steamhexid={hex}&durum=0&online=1") as HttpWebRequest;
            request.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml,         image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, /";
            request.Headers["Accept-Language"] = "tr-TR";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {

            }
        }
    }
}
