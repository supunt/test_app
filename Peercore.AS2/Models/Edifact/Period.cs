namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;

    [EdiElement, EdiPath("DTM/0"), EdiCondition("324", Path = "DTM/0/0")]
    public class Period
    {
        [EdiValue("9(3)", Path = "DTM/0/0")]
        public int ID { get; set; }

        [EdiValue("9(24)", Path = "DTM/0/1")]
        public DTMPeriod Date { get; set; }

        [EdiValue("9(3)", Path = "DTM/0/2")]
        public int Code { get; set; }

        public override string ToString()
        {
            return $"{Date.From} | {Date.To}";
        }
    }
}