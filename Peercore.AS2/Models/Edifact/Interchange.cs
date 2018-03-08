namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;
    using System;

    public class Interchange
    {
        [EdiValue("X(4)", Mandatory = true, Path = "UNB/0")]
        public string SyntaxIdentifier { get; set; }

        [EdiValue("9(1)", Path = "UNB/0/1", Mandatory = true)]
        public int SyntaxVersion { get; set; }

        [EdiValue("X(35)", Path = "UNB/1/0", Mandatory = true)]
        public string SenderId { get; set; }
        [EdiValue("X(4)", Path = "UNB/1/1", Mandatory = true)]
        public string PartnerIDCodeQualifier { get; set; }
        [EdiValue("X(14)", Path = "UNB/1/2", Mandatory = false)]
        public string ReverseRoutingAddress { get; set; }

        [EdiValue("X(35)", Path = "UNB/2/0", Mandatory = true)]
        public string RecipientId { get; set; }

        [EdiValue("X(4)", Path = "UNB/2/1", Mandatory = true)]
        public string ParterIDCode { get; set; }
        [EdiValue("X(14)", Path = "UNB/2/2", Mandatory = false)]
        public string RoutingAddress { get; set; }

        [EdiValue("9(6)", Path = "UNB/3/0", Format = "ddMMyy", Description = "Date of Preparation")]
        [EdiValue("9(4)", Path = "UNB/3/1", Format = "HHmm", Description = "Time or Prep")]
        public DateTime DateOfPreparation { get; set; }

        [EdiValue("X(14)", Path = "UNB/4", Mandatory = true)]
        public string ControlRef { get; set; }

        [EdiValue("9(1)", Path = "UNB/8", Mandatory = false)]
        public int AckRequest { get; set; }

        public Quote QuoteMessage { get; set; }

        [EdiValue("X(1)", Path = "UNZ/0")]
        public int TrailerControlCount { get; set; }

        [EdiValue("X(14)", Path = "UNZ/1")]
        public string TrailerControlReference { get; set; }
    }
}