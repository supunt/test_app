namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;
    using System.Collections.Generic;

    [EdiMessage]
    public class Quote
    {

        [EdiValue("X(14)", Path = "UNH/0/0")]
        public string MessageRef { get; set; }


        [EdiValue("X(6)", Path = "UNH/1/0")]
        public string MessageType { get; set; }

        [EdiValue("X(3)", Path = "UNH/1/1")]
        public string Version { get; set; }

        [EdiValue("X(3)", Path = "UNH/1/2")]
        public string ReleaseNumber { get; set; }

        [EdiValue("X(2)", Path = "UNH/1/3")]
        public string ControllingAgency { get; set; }

        [EdiValue("X(6)", Path = "UNH/1/4")]
        public string AssociationAssignedCode { get; set; }

        [EdiValue("X(35)", Path = "UNH/2/0")]
        public string CommonAccessRef { get; set; }

        [EdiValue("X(3)", Path = "BGM/0/0")]
        public string MessageName { get; set; }

        [EdiValue("X(35)", Path = "BGM/1/0")]
        public string DocumentNumber { get; set; }

        [EdiValue("X(3)", Path = "BGM/2/0", Mandatory = false)]
        public string MessageFunction { get; set; }

        [EdiValue("X(3)", Path = "BGM/3/0")]
        public string ResponseType { get; set; }

        [EdiCondition("137", Path = "DTM/0/0")]
        public DTM MessageDate { get; set; }

        [EdiCondition("163", Path = "DTM/0/0")]
        public DTM ProcessingStartDate { get; set; }

        [EdiCondition("164", Path = "DTM/0/0")]
        public DTM ProcessingEndDate { get; set; }

        public UTCOffset UTCOffset { get; set; }

        [EdiValue("X(3)", Path = "CUX/0/0")]
        public string CurrencyQualifier { get; set; }

        [EdiValue("X(3)", Path = "CUX/0/1")]
        public string ISOCurrency { get; set; }

        public List<NAD> NAD { get; set; }

        [EdiValue("X(3)", Path = "LOC/0/0")]
        public string LocationQualifier { get; set; }

        [EdiValue("X(3)", Path = "LOC/1/0")]
        public string LocationId { get; set; }

        [EdiValue("X(3)", Path = "LOC/1/2")]
        public string LocationResponsibleAgency { get; set; }

        public List<LineItem> Lines { get; set; }

        [EdiValue("X(1)", Path = "UNS/0/0")]
        public char? UNS { get; set; }

        [EdiValue("X(1)", Path = "UNT/0")]
        public int TrailerMessageSegmentsCount { get; set; }

        [EdiValue("X(14)", Path = "UNT/1")]
        public string TrailerMessageReference { get; set; }
    }
}