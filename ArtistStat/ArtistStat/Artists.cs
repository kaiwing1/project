using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArtistStat
{
    class Artists
    {
        public class Rootobject
        {
            public DateTime created { get; set; }
            public int count { get; set; }
            public int offset { get; set; }
            public Artist[] artists { get; set; }
        }

        public class Artist
        {
            public string id { get; set; }
            public string type { get; set; }
            public string typeid { get; set; }
            public int score { get; set; }
            public string genderid { get; set; }
            public string name { get; set; }
            public string sortname { get; set; }
            public string gender { get; set; }
            public string country { get; set; }
            public Area area { get; set; }
            public BeginArea beginarea { get; set; }
            public string[] ipis { get; set; }
            public string[] isnis { get; set; }
            public LifeSpan2 lifespan { get; set; }
            public Alias[] aliases { get; set; }
            public Tag[] tags { get; set; }
        }

        public class Area
        {
            public string id { get; set; }
            public string type { get; set; }
            public string typeid { get; set; }
            public string name { get; set; }
            public string sortname { get; set; }
            public LifeSpan lifespan { get; set; }
        }

        public class LifeSpan
        {
            public object ended { get; set; }
        }

        public class BeginArea
        {
            public string id { get; set; }
            public string type { get; set; }
            public string typeid { get; set; }
            public string name { get; set; }
            public string sortname { get; set; }
            public LifeSpan1 lifespan { get; set; }
        }

        public class LifeSpan1
        {
            public object ended { get; set; }
        }

        public class LifeSpan2
        {
            public string begin { get; set; }
            public object ended { get; set; }
        }

        public class Alias
        {
            public string sortname { get; set; }
            public string typeid { get; set; }
            public string name { get; set; }
            public string locale { get; set; }
            public string type { get; set; }
            public bool? primary { get; set; }
            public object begindate { get; set; }
            public object enddate { get; set; }
        }

        public class Tag
        {
            public int count { get; set; }
            public string name { get; set; }
        }

        public string name { get; set; }
    }
}
