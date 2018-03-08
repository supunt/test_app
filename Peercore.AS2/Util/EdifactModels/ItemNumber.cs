using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiElement, EdiPath("LIN/2")]
    public class ItemNumber
    {
        [EdiValue("X(1)", Path = "LIN/2/0")]
        public string Number { get; set; }

        [EdiValue("9(3)", Path = "LIN/2/1")]
        public string Type { get; set; }

        [EdiValue("9(3)", Path = "LIN/2/2")]
        public string CodeListQualifier { get; set; }

        [EdiValue("9(3)", Path = "LIN/2/3")]
        public string CodeListResponsibleAgency { get; set; }

        public override string ToString()
        {
            return $"{Number} {Type} {CodeListQualifier}";
        }
    }
}