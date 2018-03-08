using Peercore.Email.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Peercore.Email.Model
{  
    public class WebOrderHeaderModel
    {
        // These return the required pattern match in group 1
        // Take the first match only as these duplicates through the pages
        private static readonly string OrderDateRegex = "(?>Order Date: )([0-9/]{8})";
        private static readonly string ReqDateRegex = "(?>Required Date: )([0-9/]{8})";
        private static readonly string PurchaseCodeRegex = "(?>Order Number: )([0-9a-zA-Z]+)";
        private static readonly string BillToRegex = "(?>BILL TO:\n)([0-9]{1,4})";
        private static readonly string CommentRegex = "(?>TO:\n[0-9\\s]+PHONE:\n[0-9\\s]+\n)([!-~\\s]+)(\nLine)";

        public int WebId {get;set;}
        public DateTime? OrderDate {get;set; }
        public DateTime? DateRequired {get;set; }
        public string Comments {get;set;}
        public string OrderNote { get; set; }
        public string PurchaseCode {get;set;}
        public string Status {get;set;}
        public string CustomerCode { get; set; }
        public int AssigneeNo {get;set;}
        public double Freight {get;set;}
        public string IsSaved {get;set;}

        public int BillTo { get; set; }

        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public string MessageRefference { get; set; }
        public string OrderResponseNo { get; set; }
        public DateTime ResponseDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string BuyerOrderNo { get; set; }
        public string PromotionNo { get; set; }
        public string BuyingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string SupplierAddress { get; set; }
        public double TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public int DetailsStatus { get; set; }
        public int IsAckRequired { get; set; }
        public string AS2Identifier { get; set; }

        public List<WebOrderDetailModel> WebOrderDetailList { get; set; }
        
        //------------------------------------------------------------------------------
        public bool ParseOrderHeaderInformation(string input)
        {
            OrderDate = RegexParser.ParseDateString(OrderDateRegex, input);
            if (OrderDate == null)
                return false;

            DateRequired = RegexParser.ParseDateString(ReqDateRegex, input);
            if (DateRequired == null)
                return false;

            PurchaseCode = RegexParser.ParseString(PurchaseCodeRegex, input);

            BillTo = Convert.ToInt32(RegexParser.ParseString(BillToRegex, input));

            OrderNote = RegexParser.ParseString(CommentRegex, input);
            return true;
        }
        //------------------------------------------------------------------------------
        public bool ParseOrderDetailInformation(string input)
        {
            string[] lines = input.Split(new char[]{'\n','\r'});
            List<string> products = new List<string>();
            string regex = @"^I\d";
            foreach (string line in lines)
            {
                if (line.Length < 3)
                    continue;
                if (!Char.IsNumber(line.ToCharArray()[0]))
                    continue;
                string[] textSegments = line.Split(new char[] {' '});

                if (textSegments.Length > 2 && Regex.IsMatch(textSegments[1], regex))
                {
                    if (WebOrderDetailList == null)
                    {
                        WebOrderDetailList = new List<WebOrderDetailModel>();
                    }
                    WebOrderDetailModel wod = new WebOrderDetailModel();
                    string pfdCode = $"PFD Code : {textSegments[2]} | Desc : ";

                    string Description = "";
                    for (int i = 3; i < textSegments.Length - 2; ++i)
                        Description += textSegments[i] + " ";

                    wod.ProductNote = pfdCode + Description;

                    wod.CatlogCode  = textSegments[1];
                    wod.OrderQty    = Convert.ToInt32(textSegments[textSegments.Length - 2]);
                    wod.UnitCost    = Convert.ToDouble(textSegments[textSegments.Length - 1]);
                    wod.UnitPrice   = wod.UnitCost;
                    wod.WebId = this.WebId;
                    this.WebOrderDetailList.Add(wod);
                }           
            }
            return true;
        }
        //------------------------------------------------------------------------------
        public override string ToString()
        {
            string opString = "";
            opString += "\nOrder Message -----\n";
            opString += $"\tWeb ID        : {WebId}\n";
            opString += $"\tPurchase Code : {PurchaseCode}\n";
            opString += $"\tCustomer Code : {CustomerCode}\n";
            opString += $"\tOrder Date    : {OrderDate}\n";
            opString += $"\tRequired Date : {DateRequired}\n";
            opString += $"\tStatus        : {Status}\n";
            opString += $"\tWeb ID        : {WebId}\n";

            if (WebOrderDetailList.Count == 0)
            {
                opString += "\t<NO ITEMS>";
            }
            else
            {
                opString += "\t----- Order Items -----\n";
                int i = 0;
                foreach (WebOrderDetailModel wod in WebOrderDetailList)
                {
                    opString += $"\t\tOrder item {i++}\n" + wod.ToString();
                }
            }
            opString += "\nOrder Message end-----\n";
            return opString;
        }

    }
}
