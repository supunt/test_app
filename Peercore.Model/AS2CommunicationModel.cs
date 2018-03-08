using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peercore.Email.Model
{
    public class AS2CommunicationModel : BaseModel
    {
        public string AS2Identifier { get; set; }
        public string AS2URL { get; set; }
        public string AS2MDNURL { get; set; }
        public string SendingIP { get; set; }
        public string ListeningIP { get; set; }
        public string CertificateName { get; set; }
        public string CertificateType { get; set; }
        public string KeyStrength { get; set; }
        public string EncryptionAlgo { get; set; }
        public string ReceiptSignature { get; set; }
        public string MDNMode { get; set; }
        public string Transport { get; set; }
        public string MessageType { get; set; }
        public bool ISCompressed { get; set; }
        public bool ISCompressedBeforeSigning { get; set; }
        public string PayLoadType { get; set; }
        public string MessageFormat { get; set; }
        public string AS2Encoding { get; set; }
    }
}
