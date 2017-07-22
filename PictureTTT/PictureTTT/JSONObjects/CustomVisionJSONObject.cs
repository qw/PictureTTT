using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureTTT
{
    class CustomVisionJSONObject
    {
        public string Language { get; set; }
        public string TextAngle { get; set; }
        public string Orientation { get; set; }
        public List<Region> Regions { get; set; }

        public class Region
        {
            public string BoundingBox { get; set; }
            public List<SingleLine> Lines { get; set; }
        }

        public class SingleLine
        {
            public string BoundingBox { get; set; }
            public List<SingleWord> Words { get; set; }
        }

        public class SingleWord
        {
            public string BoundingBox { get; set; }
            public string Text { get; set; }
        }

        //Gets all Text formatted by Lines
        public override string ToString()
        {
            string s = "";
            foreach(Region r in Regions)
            {
                foreach(SingleLine sl in r.Lines)
                {
                    foreach(SingleWord sw in sl.Words)
                    {
                        s += sw.Text + " ";

                    }

                    s += "\n\n";
                }

                s += "\n";
            }

            return s;
        }
    }
}
