using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading;
using Polly;
using Polly.Retry;
using System.Configuration;
using ConsoleTables;

namespace ArtistStat
{
    class Program
    {

 

        static async Task Main(string[] args)
        {
            await ProcessRepositories();
        }

 
        private static async Task<string> GetLyrics(string artist, string song)
        {
            
            //string requrestURL = "https://api.lyrics.ovh/v1/" + artist + "/" + song;
            //Console.WriteLine(requrestURL);
            
            string requrestURL = string.Format(ConfigurationManager.AppSettings.Get("URLGetLyrics"), artist , song);
            var responseLyrics = await GetResponse<Lyrics.Rootobject>(requrestURL);
            if (responseLyrics is not null)
                return responseLyrics.lyrics;
            else
                return string.Empty;

        }

        private static int GetWordCount(string lyrics)
        {
            if (lyrics != string.Empty)
            {
                char[] delimiters = new char[] { ' ', '\r', '\n' };
                return lyrics.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
            }
            else
                return 0;
        }


        private static async Task ProcessRepositories()
        {

            Console.Write("Please Enter an Artist: ");

            string targetArtistName = Console.ReadLine();

            Artists.Artist targetArtist = await GetArtistAsync(targetArtistName);

            if (targetArtist == null)
            {
                Console.WriteLine("Sorry, Artist - " + targetArtistName + " not found.");
                Environment.Exit(0);
            }

            

            var allRelease = await GetAllReleaseByArtist(targetArtist.id, 0);

            List<Releases.Release> savedAlbum = new List<Releases.Release>();
            List<Recordings.Recording> allSongList = new List<Recordings.Recording>();

            if (allRelease.Count() == 0 )
            {
                Console.WriteLine("Sorry, No Album found for Artist - " + targetArtistName + " ");
                Environment.Exit(0);
            }

            foreach (var item in allRelease)
            {
                Console.Write(string.Format("\r{0}", "".PadLeft(Console.CursorLeft, ' ')));
                Console.Write(string.Format("\r{0}", "Processing - Getting Album : " + item.title));
                if (!savedAlbum.Any(u => u.title == item.title))
                {
                    var allAlbum = await GetAllTrackByRelease(item.id, targetArtist.id);
                    savedAlbum.Add(item);
                    
                    foreach (var song in allAlbum)
                    {
                        

                        song.year = !string.IsNullOrEmpty(song.releases[0].date) ? song.releases[0].date.Substring(0, 4) : string.Empty;
                        allSongList.Add(song);
                    }


                    //Console.WriteLine("duplicated album");
                }
            }

            

            foreach (var item in allSongList)
            {

                Console.Write(string.Format("\r{0}", "".PadLeft(Console.CursorLeft, ' ')));
                Console.Write(string.Format("\r{0}", "Processing - Getting Lyrics : " + item.title));
                var existWordCount = (from e in allSongList
                                      where e.title == item.title
                                      select e.wordCount).FirstOrDefault();
                if (existWordCount == 0)
                {
                    var lyrics = GetLyrics(targetArtistName, item.title);
                    item.wordCount = GetWordCount(await lyrics);
                }
                else
                    item.wordCount = existWordCount;

            }




            var wordCounts = from a in allSongList
                             where a.wordCount > 0
                             select a.wordCount;


            double meanOfValues = wordCounts.Average();
            double sumOfSquares = 0.0;
            foreach (int a in wordCounts)
            {
                sumOfSquares += Math.Pow((a - meanOfValues), 2.0);
            }

            int countOfValues = wordCounts.Count();
            double varianceOfValues = sumOfSquares / (countOfValues - 1);
            double standardDeviationOfValues =  Math.Sqrt(varianceOfValues);

            

            Console.Write(string.Format("\r{0}", "".PadLeft(Console.CursorLeft, ' ')));
            Console.WriteLine("");

            var tableAvg = new ConsoleTable("Data", "Value");
            tableAvg.AddRow("Average Words per song", meanOfValues);
            tableAvg.AddRow("Minimum", wordCounts.Min());
            tableAvg.AddRow("Maximum", wordCounts.Max());
            tableAvg.AddRow("Variance", varianceOfValues);
            tableAvg.AddRow("Standard deviation", standardDeviationOfValues);
            tableAvg.Write(Format.Alternative);
            Console.WriteLine();

            var avgByYear = (from a in allSongList
                             where !String.IsNullOrEmpty(a.year)
                             group a by a.year into y
                             orderby y.Key
                             select new
                             {
                                 Year = y.Key,
                                 average = y.Average(x => x.wordCount),
                             }

                );

            //foreach (var item in avgByYear)
                //Console.WriteLine("Average Value in" + item.Year + ":" + item.average);

            var tableAvgYear = new ConsoleTable("Year", "Average Words by Year");

            foreach (var item in avgByYear)
            {
                tableAvgYear.AddRow(item.Year, item.average);
            }
            tableAvgYear.Write(Format.Alternative);
            Console.WriteLine();

            var avgByAlbum = (from a in allSongList
                            where !String.IsNullOrEmpty(a.year)
                            group a by new
                            {
                                a.releases[0].title,
                                a.year
                            } into y
                            orderby y.Key.year
                            select new
                            {
                                Album = y.Key.title,
                                Year = y.Key.year,
                                Average = y.Average(x => x.wordCount),
                            }

            );


            var tableAvgAlbum = new ConsoleTable("Album", "Year", "Average Word By Album");

            foreach (var item in avgByAlbum)
            {
                tableAvgAlbum.AddRow(item.Album, item.Year, item.Average);
            }
            tableAvgAlbum.Write(Format.Alternative);
            Console.WriteLine();
            Console.ReadLine();

        }

        private static async Task<Artists.Artist> GetArtistAsync(string artistname)
        {


            //string requrestURL = $"https://musicbrainz.org/ws/2/artist?query=:{artistname}&fmt=json&score=100&limit=1";

            string requrestURL = string.Format(ConfigurationManager.AppSettings.Get("URLSearchArtist"),artistname);
            //Console.WriteLine(requrestURL);
            //var streamTask = client.GetStreamAsync(requrestURL);
            //var responseArtist = await JsonSerializer.DeserializeAsync<Artists.Rootobject>(await streamTask);
            var responseArtist = await GetResponse<Artists.Rootobject>(requrestURL);

            if (responseArtist.count > 0)
                return responseArtist.artists[0];
            else
                return default;
        }

        private static async Task<IEnumerable<Releases.Release>> GetAllReleaseByArtist(string arid, int offset)
        {
            //string requrestURL = $"https://musicbrainz.org/ws/2/release?query=arid:{arid}%20AND%20country:GB%20AND%20primarytype:Album&fmt=json&limit=1&offset=0";
            string requrestURL = string.Format(ConfigurationManager.AppSettings.Get("URLGetAlbumCount"), arid);
            var responseRecording = await GetResponse<Releases.Rootobject>(requrestURL);
            Releases.Release[] result = new Releases.Release[responseRecording.count];

            while (offset < responseRecording.count)
            {
                //requrestURL = $"https://musicbrainz.org/ws/2/release?query=arid:{arid}%20AND%20country:GB%20AND%20primarytype:Album&fmt=json&limit=100&offset={offset}";
                requrestURL = string.Format(ConfigurationManager.AppSettings.Get("URLGetAlbumList"), arid , offset);
                //streamTask = client.GetStreamAsync(requrestURL);
                responseRecording = await GetResponse<Releases.Rootobject>(requrestURL);
                responseRecording.releases.CopyTo(result, offset);
                offset += 100;
            }

            return result;
        }


        private static async Task<IEnumerable<Recordings.Recording>> GetAllTrackByRelease(string reid, string arid)
        {
            string requrestURL = $"https://musicbrainz.org/ws/2/recording?query=reid:{reid}%20AND%20arid:{arid}&fmt=json";
            //string requrestURL = string.Format(ConfigurationManager.AppSettings.Get("URLGetAllTrack"), reid, arid);
            var responseRecording = await GetResponse<Recordings.Rootobject>(requrestURL);
            return responseRecording.recordings;

        }




        public static async Task<T> GetResponse<T>(string path)
        {
            //var streamTask = client.GetStreamAsync(requrestURL);
            //int MAX_RETRIES = 3;
            try
            {
                var retryPolicy = Policy.Handle<HttpRequestException>(
                    ex => ex.StatusCode == System.Net.HttpStatusCode.BadRequest || ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(new[]
              {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15)
              });

                var response = default(T);

                await retryPolicy.ExecuteAsync(async () =>
                {

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                    client.DefaultRequestHeaders.Add("User-Agent", "ArtistStat/0.1.1 ( kaiwing1@outlook.com )");
                    //Console.WriteLine(path);
                    var streamTask = client.GetStreamAsync(path);
                    response = (T)await JsonSerializer.DeserializeAsync(await streamTask, typeof(T));
                    client.Dispose();

                }
                );

                return response;
            }
             catch (HttpRequestException e)
            {
                //Console.Write(string.Format("\r{0}", "".PadLeft(Console.CursorLeft, ' ')));
                //Console.WriteLine(e.StatusCode + "-" + path);
                return default(T);
            }
            

        }
    }

}
