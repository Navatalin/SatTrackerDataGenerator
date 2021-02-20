using System;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SatTrackerDataGenerator
{
    class Program
    {
        private static Timer MessageSendTimer;
        private static Timer SatFileUpdateTimer;
        private static MessageHandler messageHandler;
        private 
        static void Main(string[] args)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://celestrak.com/NORAD/elements/starlink.txt", @"D:\Dev\TestData\starlink.txt");
            }
            var chunks = Chunk(GetSatNames(), 10);
            messageHandler = new MessageHandler(chunks);
            SetTimer();
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
            MessageSendTimer.Stop();
            MessageSendTimer.Dispose();
            SatFileUpdateTimer.Stop();
            SatFileUpdateTimer.Dispose();
            messageHandler.Dispose();
        }
        static List<List<T>> Chunk<T>(IEnumerable<T> data, int size)
        {
            return data
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / size)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        private static void SetTimer()
        {
            SatFileUpdateTimer = new Timer(3.6e6 * 2);
            SatFileUpdateTimer.Elapsed += UpdateSatFile;
            SatFileUpdateTimer.AutoReset = true;
            SatFileUpdateTimer.Enabled = true;

            MessageSendTimer = new Timer(200);
            MessageSendTimer.Elapsed += SendMessageEvent;
            MessageSendTimer.AutoReset = true;
            MessageSendTimer.Enabled = true;
        }

        private static void SendMessageEvent(Object source, ElapsedEventArgs e)
        {
            messageHandler.RunMessages();
        }

        private static void UpdateSatFile(Object source, ElapsedEventArgs e)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://celestrak.com/NORAD/elements/starlink.txt", @"D:\Dev\TestData\starlink.txt");
            }

            var chunks = Chunk(GetSatNames(), 10);
            while (messageHandler.running)
            {
                Console.WriteLine("Waiting for message handler to stop running");
            }
            messageHandler.updateChunks(chunks);

        }

        private static List<string> GetSatNames()
        {
            var sat_lines = File.ReadLines(@"D:\Dev\TestData\starlink.txt");
            var sat_names = new List<string>();
            foreach (var line in sat_lines)
            {
                if (!char.IsDigit(line[0]))
                {
                    sat_names.Add(line.Trim());
                }
            }
            return sat_names;
        }
    }
}
