using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistStat
{
    class Recordings
    {

        public class Rootobject
        {
            public DateTime created { get; set; }
            public int count { get; set; }
            public int offset { get; set; }
            public Recording[] recordings { get; set; }
        }

        public class Recording
        {
            public string id { get; set; }
            public int score { get; set; }
            public string title { get; set; }
            public string disambiguation { get; set; }
            public bool? video { get; set; }
            public ArtistCredit[] artistcredit { get; set; }
            public string firstreleasedate { get; set; }
            public Release[] releases { get; set; }
            public int length { get; set; }
            public string[] isrcs { get; set; }
            public int wordCount { get; set; }
            public string year { get; set; }
        }

        public class ArtistCredit
        {
            public string name { get; set; }
            public Artist artist { get; set; }
            public string joinphrase { get; set; }
        }

        public class Artist
        {
            public string id { get; set; }
            public string name { get; set; }
            public string sortname { get; set; }
            public string disambiguation { get; set; }
            public Alias[] aliases { get; set; }
        }

        public class Alias
        {
            public string sortname { get; set; }
            public string name { get; set; }
            public string locale { get; set; }
            public string type { get; set; }
            public bool? primary { get; set; }
            public object begindate { get; set; }
            public object enddate { get; set; }
            public string typeid { get; set; }
        }

        public class Release
        {
            public string id { get; set; }
            public string statusid { get; set; }
            public int count { get; set; }
            public string title { get; set; }
            public string status { get; set; }
            public string disambiguation { get; set; }
            public ArtistCredit1[] artistcredit { get; set; }
            public ReleaseGroup releasegroup { get; set; }
            public string date { get; set; }
            public string country { get; set; }
            public ReleaseEvents[] releaseevents { get; set; }
            public int trackcount { get; set; }
            public Medium[] media { get; set; }
        }

        public class ReleaseGroup
        {
            public string id { get; set; }
            public string typeid { get; set; }
            public string primarytypeid { get; set; }
            public string title { get; set; }
            public string primarytype { get; set; }
            public string[] secondarytypes { get; set; }
            public string[] secondarytypeids { get; set; }
            public string disambiguation { get; set; }
        }

        public class ArtistCredit1
        {
            public string name { get; set; }
            public Artist1 artist { get; set; }
        }

        public class Artist1
        {
            public string id { get; set; }
            public string name { get; set; }
            public string sortname { get; set; }
            public string disambiguation { get; set; }
        }

        public class ReleaseEvents
        {
            public string date { get; set; }
            public Area area { get; set; }
        }

        public class Area
        {
            public string id { get; set; }
            public string name { get; set; }
            public string sortname { get; set; }
            public string[] iso31661codes { get; set; }
        }

        public class Medium
        {
            public int position { get; set; }
            public string format { get; set; }
            public Track[] track { get; set; }
            public int trackcount { get; set; }
            public int trackoffset { get; set; }
        }

        public class Track
        {
            public string id { get; set; }
            public string number { get; set; }
            public string title { get; set; }
            public int length { get; set; }
            
        }

    }
}
