namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;

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