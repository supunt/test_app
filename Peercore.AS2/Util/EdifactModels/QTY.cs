using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiElement, EdiPath("QTY/0")]
    public class QTY
    {
        [EdiValue("X(3)", Path = "QTY/0/0")]
        public string QuantityType { get; set; }

        [EdiValue("9(10)", Path = "QTY/0/1")]
        public int Quantity { get; set; }

        [EdiValue("X(3)", Path = "QTY/0/2")]
        public string Unit { get; set; }
    }
}