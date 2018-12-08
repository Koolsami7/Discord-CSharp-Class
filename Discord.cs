using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Drawing;
namespace Discord_Bot
{
    public class Discord
    {
        private static bool loggedIn = false;
        private static string fingerprint;
        private static string authToken;
        private static string useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36";

        public string publicName = "null";

        public enum MenuLevel
        {
            GUILDS = 0,
            CHANNELS = 1
        }

        public MenuLevel menuLevel = MenuLevel.GUILDS;
        public bool login(string f, string a)
        {
            if (!loggedIn)
            {
                var client = new RestClient("https://discordapp.com/api/users/@me");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("cookie", "locale=en-US;");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("referer", "https://discordapp.com");
                request.AddHeader("accept", "*/*");
                request.AddHeader("user-agent", useragent);
                request.AddHeader("accept-language", "en-US");
                request.AddHeader("x-fingerprint", f);
                request.AddHeader("authorization", a);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new InvalidLoginException();
                }
                else
                {
                    loggedIn = true;
                    fingerprint = f;
                    authToken = a;
                    string res = response.Content.ToString();
                    dynamic userInfo = JsonConvert.DeserializeObject(res);
                    publicName = "@"+userInfo.username + "#" + userInfo.discriminator;
                    return true;
                }
            }
            else throw new AlreadyLoggedInException();
        }
        public string[][] getGuilds()
        {
            if (!loggedIn) throw new NotLoggedInException();
            menuLevel = MenuLevel.GUILDS;
            var client = new RestClient("https://discordapp.com/api/users/@me/guilds");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("cookie", "locale=en-US;");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("referer", "https://discordapp.com");
            request.AddHeader("accept", "*/*");
            request.AddHeader("user-agent", useragent);
            request.AddHeader("accept-language", "en-US");
            request.AddHeader("x-fingerprint", fingerprint);
            request.AddHeader("authorization", authToken);
            IRestResponse response = client.Execute(request);

            string res = response.Content.ToString();
            dynamic guildList = JsonConvert.DeserializeObject(res);
            string[][] guilds = new string[0xFF][];
            int i = 0;
            foreach (var guild in guildList)
            {
                guilds[i] = new string[] { guild.name, guild.id };
                i++;
            }
            return guilds;
        }

        public string[][] getChannels(string guildId)
        {
            if (!loggedIn) throw new NotLoggedInException();
            menuLevel = MenuLevel.CHANNELS;
            var client = new RestClient("https://discordapp.com/api/guilds/"+ guildId + "/channels");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("cookie", "locale=en-US;");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("referer", "https://discordapp.com");
            request.AddHeader("accept", "*/*");
            request.AddHeader("user-agent", useragent);
            request.AddHeader("accept-language", "en-US");
            request.AddHeader("x-fingerprint", fingerprint);
            request.AddHeader("authorization", authToken);
            IRestResponse response = client.Execute(request);

            string res = response.Content.ToString();
            dynamic channelList = JsonConvert.DeserializeObject(res);
            string[][] channels = new string[0xFF][];
            int i = 0;
            foreach (var channel in channelList)
            {
                if (channel.type == 0)
                {
                    channels[i] = new string[] { "#"+channel.name, channel.id };
                    i++;
                }
            }
            return channels;
        }
        public void sendMessage(long channelId, string message)
        {
            if (loggedIn)
            {
                var client = new RestClient("https://discordapp.com/api/v6/channels/" + channelId + "/typing");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("cookie", "locale=en-US;");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("referer", "https://discordapp.com");
                request.AddHeader("accept", "*/*");
                request.AddHeader("origin", "https://discordapp.com");
                request.AddHeader("authorization", authToken);
                request.AddHeader("user-agent", useragent);
                request.AddHeader("accept-language", "en-US");
                request.AddHeader("x-fingerprint", fingerprint);
                request.AddHeader("x-super-properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzcwLjAuMzUzOC4xMTAgU2FmYXJpLzUzNy4zNiIsImJyb3dzZXJfdmVyc2lvbiI6IjcwLjAuMzUzOC4xMTAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6MjkyMzIsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");
                IRestResponse response = client.Execute(request);

                client = new RestClient("https://discordapp.com/api/v6/channels/" + channelId + "/messages");
                request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("cookie", "locale=en-US;");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("referer", "https://discordapp.com");
                request.AddHeader("accept", "*/*");
                request.AddHeader("origin", "https://discordapp.com");
                request.AddHeader("authorization", authToken);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("user-agent", useragent);
                request.AddHeader("accept-language", "en-US");
                request.AddHeader("x-fingerprint", fingerprint);
                request.AddHeader("x-super-properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzcwLjAuMzUzOC4xMTAgU2FmYXJpLzUzNy4zNiIsImJyb3dzZXJfdmVyc2lvbiI6IjcwLjAuMzUzOC4xMTAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6MjkyMzIsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");
                request.AddParameter("application/json", "{\"content\":\"" + message + "\",\"nonce\":\"" + LongRandom(500000000000000, 599999999999999, new Random()) + "\",\"tts\":false}", ParameterType.RequestBody);
                response = client.Execute(request);
            }
            else throw new NotLoggedInException();
        }


        private static long LongRandom(long min, long max, Random rand)
        {
            long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max);
            return result;
        }
    }

    public class InvalidLoginException : Exception
    {
        public InvalidLoginException() { }
        public InvalidLoginException(string message) : base(message) { }
        public InvalidLoginException(string message, Exception inner) : base(message, inner) { }
    }
    public class AlreadyLoggedInException : Exception
    {
        public AlreadyLoggedInException() { }
        public AlreadyLoggedInException(string message) : base(message) { }
        public AlreadyLoggedInException(string message, Exception inner) : base(message, inner) { }
    }
    public class NotLoggedInException : Exception
    {
        public NotLoggedInException() { }
        public NotLoggedInException(string message) : base(message) { }
        public NotLoggedInException(string message, Exception inner) : base(message, inner) { }
    }
}
