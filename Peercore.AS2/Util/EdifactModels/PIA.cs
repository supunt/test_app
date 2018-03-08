using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiElement, EdiPath("PIA/1")]
    public class PIA
    {
        [EdiValue("X(6)", Path = "PIA/1/0")]
        public string ItemCode { get; set; }

        [EdiValue("X(2)", Path = "PIA/1/1")]
        public string ItemInternalCode { get; set; }
    }
}