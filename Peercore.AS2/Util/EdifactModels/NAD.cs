using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiSegment, EdiPath("NAD")]
    public class NAD
    {
        [EdiValue("X(3)", Path = "NAD/0/0")]
        public string PartyQualifier { get; set; }

        [EdiValue("X(35)", Path = "NAD/1/0")]
        public string PartyId { get; set; }

        [EdiValue("X(3)", Path = "NAD/1/2")]
        public string ResponsibleAgency { get; set; }

        [EdiValue("X(100)", Path = "NAD/2/0")]
        public string AddressString { get; set; }
    }
}