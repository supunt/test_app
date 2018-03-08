using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiSegment, EdiPath("FTX")]
    public class FTX
    {
        [EdiValue("X(3)", Path = "FTX/0/0")]
        public string purpose { get; set; }

        [EdiValue("X(3)", Path = "FTX/1/0")]
        public string function { get; set; }

        [EdiValue("X(3)", Path = "FTX/2/0")]
        public string referece { get; set; }

        [EdiValue("X(50)", Path = "FTX/3/0")]
        public string value { get; set; }
    }
}