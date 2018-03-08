using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiSegment, EdiSegmentGroup("LIN", SequenceEnd = "UNS")]
    public class LineItem
    {
        [EdiValue("X(1)", Path = "LIN/0/0")]
        public int LineNumber { get; set; }

        [EdiValue("9(3)", Path = "LIN/1/0")]
        public string Code { get; set; }

        [EdiValue("9(14)", Path = "LIN/2/0")]
        public string GTIN { get; set; }

        [EdiValue("X(3)", Path = "LIN/2/1")]
        public string itemTypeIDCode { get; set; }

        public PIA PIA { get; set; }

        public QTY QTY { get; set; }

        public Price Price { get; set; }
    }
}