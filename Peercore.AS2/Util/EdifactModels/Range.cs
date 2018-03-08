using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiSegment, EdiPath("RNG")]
    public class Range
    {
        [EdiValue("X(3)", Path = "RNG/0/0")]
        public string MeasurementUnitCode { get; set; }

        [EdiValue("X(18)", Path = "RNG/1/0")]
        public decimal? Minimum { get; set; }

        [EdiValue("X(18)", Path = "RNG/1/1")]
        public decimal? Maximum { get; set; }
    }
}