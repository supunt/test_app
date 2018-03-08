namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;

    [EdiSegment, EdiSegmentGroup("PRI")]
    public class PriceDetails
    {
        public Price Price { get; set; }

        public Range Range { get; set; }

    }
}