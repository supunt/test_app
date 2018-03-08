using indice.Edi.Serialization;
using Peercore.Email.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peercore.AS2.Util.EdifactModels
{
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

        [EdiCondition("2", Path = "DTM/0/0")]
        public DTM DeliveryDate { get; set; }

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

        public FTX freeTextElement { get; set; }

        //---------------------------------------------------------------
        public void fillWebOrder(WebOrderHeaderModel webOrder, Interchange interchange)
        {
            webOrder.Comments = this.freeTextElement.value;
            webOrder.DateRequired = this.DeliveryDate.DateTime;
            webOrder.OrderDate = this.MessageDate.DateTime;
            webOrder.PurchaseCode = this.DocumentNumber;
            webOrder.Status = "Processing"; // TODO // 29 - Accept  27 - Reject  4-Change
            //webOrder.IsAckRequired = interchange.AckRequest;

            webOrder.SenderAddress = interchange.SenderId;
            webOrder.ReceiverAddress = interchange.RecipientId;
            webOrder.MessageRefference = this.MessageRef;
            webOrder.OrderResponseNo = "";
         //   webOrder.ResponseDate = this.MessageDate.DateTime;
         //   webOrder.DeliveryDate = this.DeliveryDate.DateTime;
            webOrder.BuyerOrderNo = this.DocumentNumber;
            webOrder.PromotionNo = "";
            webOrder.TotalAmount = 0;

            foreach (NAD NameAddress in this.NAD)
            {
                if (NameAddress.PartyId == "ST")
                {
                    webOrder.OrderNote = NameAddress.AddressString;
                    webOrder.BillTo = 540; // TODO
                }

                if (NameAddress.PartyQualifier == "ST")
                {
                    webOrder.ShippingAddress = NameAddress.PartyId;
                }
                else if (NameAddress.PartyQualifier == "BY")
                {
                    webOrder.BuyingAddress = NameAddress.PartyId;
                }
                else if (NameAddress.PartyQualifier == "SU")
                {
                    webOrder.SupplierAddress = NameAddress.PartyId;
                }
            }

            if (Lines.Count > 0)
            {
                webOrder.WebOrderDetailList = new List<WebOrderDetailModel>();

                foreach (LineItem li in this.Lines)
                {
                    WebOrderDetailModel wod = new WebOrderDetailModel();
                    wod.CatlogCode = li.PIA.ItemCode;
                    wod.ProductNote = li.PIA.ItemCode;
                    wod.OrderQty = li.QTY.Quantity;
                    wod.UnitCost = Decimal.ToDouble(li.Price.Amount);
                    wod.UnitPrice = Decimal.ToDouble(li.Price.Amount);
                    wod.GTINCode = li.GTIN;
                    wod.ItemStatus = 0; //3-changed  5-accepted  7-not accepted
                    //wod.BackOrderQty = li.QTY.Quantity;
                    //wod.ItemPrice = Decimal.ToDouble(li.Price.Amount);
                    webOrder.WebOrderDetailList.Add(wod);
                }
            }

            webOrder.ItemCount = interchange.QuoteMessage.Lines.Count;
            webOrder.DetailsStatus = 0;

        }
    }
}