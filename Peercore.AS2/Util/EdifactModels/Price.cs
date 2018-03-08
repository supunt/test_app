using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiElement, EdiPath("PRI/0")]
    public class Price
    {
        public Price()
        {
            Amount = new Decimal(0.0);
        }

        [EdiValue("X(3)", Path = "PRI/0/0")]
        public string Code { get; set; }

        [EdiValue("X(15)", Path = "PRI/0/1")]
        public decimal Amount { get; set; }

    }
}