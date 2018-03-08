namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;
    using System.Collections.Generic;

    [EdiSegment, EdiSegmentGroup("LIN", SequenceEnd = "UNS")]
    public class LineItem
    {
        [EdiValue("X(1)", Path = "LIN/0/0")]
        public int LineNumber { get; set; }

        [EdiValue("9(3)", Path = "LIN/1/0")]
        public string Code { get; set; }

        public ItemNumber NumberIdentification { get; set; }

        public Period Period { get; set; }

        public List<PriceDetails> Prices { get; set; }
    }
}