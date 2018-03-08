using indice.Edi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
    [EdiElement, EdiPath("DTM/0")]
    public class DTM
    {
        [EdiValue("9(3)", Path = "DTM/0/0")]
        public int ID { get; set; }
        [EdiValue("X(8)", Path = "DTM/0/1", Format = "yyyyMMdd")]
        public DateTime DateTime { get; set; }
        [EdiValue("9(3)", Path = "DTM/0/2")]
        public int Code { get; set; }

        public override string ToString()
        {
            try
            {
                return DateTime.ToString();
            }
            catch
            {
                throw;
            }
        }
    }
}