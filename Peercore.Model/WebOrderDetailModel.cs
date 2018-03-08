using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peercore.Email.Model
{
    public class WebOrderDetailModel
    {
        public int WebId {get;set;}
        public string CatlogCode {get;set;}
        public double OrderQty {get;set;}
        public double BackOrderQty { get; set; }
        public double UnitPrice {get;set;}
        public double UnitCost {get;set;}
        public double UnitDiscountPerc {get;set;}
        public double Tax {get;set;}
        public double FreeQty {get;set;}
        public string ProductNote { get; set; }

        public string GTINCode { get; set; }
        public int ItemStatus { get; set; }
        public double ItemPrice { get; set; }

        public override string ToString()
        {
            string opString = "";
            opString += $"\t\t\tCatlog Code   : {CatlogCode}\n";
            opString += $"\t\t\tOrder Qty     : {OrderQty}\n";
            opString += $"\t\t\tUnit Cost    : {UnitCost}\n";
            opString += $"\t\t\tUnit Price    : {UnitPrice}\n";
            return opString;
        }
    }
}
