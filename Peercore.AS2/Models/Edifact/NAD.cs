namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization; 

    [EdiSegment, EdiPath("NAD")]
    public class NAD
    {
        [EdiValue("X(3)", Path = "NAD/0/0")]
        public string PartyQualifier { get; set; }

        [EdiValue("X(35)", Path = "NAD/1/0")]
        public string PartyId { get; set; }

        [EdiValue("X(3)", Path = "NAD/1/2")]
        public string ResponsibleAgency { get; set; }
    }
}