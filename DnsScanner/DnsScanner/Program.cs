using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DnsScanner
{
    class Program
    {
        static HttpClient _client;
        public static async Task<Tuple<string, string>> Load(string domain)
        {
            var stream = await _client.GetStringAsync($"https://{domain}.here_is)your_domain.com");
            return new Tuple<string, string>(domain, stream);
        }
        static void Main(string[] args)
        {
            var rpath = @"C:\Users\john\Desktop";
            var rfile = Path.Combine(rpath, "result.txt");
            File.WriteAllText(rfile, "");

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("xxx", "***", "/", "--here is domain--") { });
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            // ignore errors with certeficate
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            _client = new HttpClient(handler);


            var path2 = @"C:\cygwin64\home\john\proj\dnsscanner";
            var dictPath2 = Path.Combine(path2, "x.txt");

            var dict = File.ReadAllLines(dictPath2);

            for (int i = 0; i < dict.Length; i += 10)
            {
                var word = dict[i].Trim();
                if (Regex.Match(word, "^[a-z0-9]+[a-z0-9-_.]*$").Success)
                {
                    Console.WriteLine(word + " " + i + "/" + dict.Length);
                    var task = Load(word).ContinueWith(x =>
                    {
                        try
                        {
                            var w = x.Result.Item1;
                            var str = x.Result.Item2;
                            if (!str.Contains("Logout || check here it is not main/stub page of website"))
                            {
                                File.AppendAllText(rfile, w + "\n");
                                Console.WriteLine("==>" + w);
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                    });
                }
            }
            Console.WriteLine("==> END <==");
            Thread.Sleep(100000);
        }
    }
}
