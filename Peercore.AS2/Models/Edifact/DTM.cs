namespace Peercore.AS2.Models.Edifact
{
    using indice.Edi.Serialization;
    using indice.Edi.Utilities;
    using System;

    public struct DTMPeriod
    {
        public readonly DateTime From;
        public readonly DateTime To;

        public DTMPeriod(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public static DTMPeriod Parse(string text)
        {
            var textFrom = text?.Substring(0, 12);
            var textTo = text?.Substring(12, 12);
            return new DTMPeriod(
                    textFrom.ParseEdiDate("yyyyMMddHHmm"),
                    textTo.ParseEdiDate("yyyyMMddHHmm")
                );
        }

        public override string ToString()
        {
            return $"{From:yyyyMMddHHmm}{To:yyyyMMddHHmm}";
        }

        public static implicit operator string(DTMPeriod value)
        {
            return value.ToString();
        }

        // With a cast operator from string --> MyClass or MyStruct 
        // we can convert any edi component value to our custom implementation.
        public static explicit operator DTMPeriod(string value)
        {
            return DTMPeriod.Parse(value);
        }
    }

    [EdiElement, EdiPath("DTM/0")]
    public class DTM
    {
        [EdiValue("9(3)", Path = "DTM/0/0")]
        public int ID { get; set; }
        [EdiValue("X(12)", Path = "DTM/0/1", Format = "yyyyMMddHHmm")]
        public DateTime DateTime { get; set; }
        [EdiValue("9(3)", Path = "DTM/0/2")]
        public int Code { get; set; }

        public override string ToString()
        {
            return DateTime.ToString();
        }
    }
}