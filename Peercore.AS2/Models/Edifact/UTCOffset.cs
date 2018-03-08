namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;

    [EdiElement, EdiPath("DTM/0"), EdiCondition("ZZZ", Path = "DTM/0/0")]  
    public class UTCOffset
    {
        [EdiValue("X(3)", Path = "DTM/0/0")]
        public int? ID { get; set; }
        [EdiValue("9(1)", Path = "DTM/0/1")]
        public int Hours { get; set; }
        [EdiValue("9(3)", Path = "DTM/0/2")]
        public int Code { get; set; }

        public override string ToString()
        {
            return Hours.ToString();
        }
    }
}