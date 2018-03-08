using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiElement, EdiPath("DTM/0"), EdiCondition("ZZZ", Path = "DTM/0/0")]
    public class UTCOffset
    {
        [EdiValue("X(3)", Path = "DTM/0/0")]
        public int? ID { get; set; }
        [EdiValue("9(1)", Path = "DTM/0/1")]
        public int Hours { get; set; }
        [EdiValue("9(3)", Path = "DTM/0/2")]
        public int Code { get; set; }

        public override string ToString()
        {
            return Hours.ToString();
        }
    }
}