namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;

    [EdiElement, EdiPath("PRI/0")]
    public class Price
    {
        [EdiValue("X(3)", Path = "PRI/0/0")]
        public string Code { get; set; }

        [EdiValue("X(15)", Path = "PRI/0/1")]
        public decimal? Amount { get; set; }

        [EdiValue("X(3)", Path = "PRI/0/2")]
        public string Type { get; set; }
    }
}