using indice.Edi;
using log4net;
using Peercore.AS2.Util.EdifactModels;
using Peercore.Email.DataService;
using Peercore.Email.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Peercore.AS2.Util
{
    public class AS2Acknowledge
    {
        private static readonly ILog Log = LogManager.GetLogger(ConfigValues.LoggerID);

        public void SendPOAcknowledgements()
        {
            try
            {
                List<WebOrderHeaderModel> headerList = null;
                try
                {
                    // Get the Acknowledge pending PO list
                    headerList = GetPendingPOAckList();
                }
                catch (Exception ex)
                {
                    Log.Error($"AS2Acknowledge::Retreive POA error : {ex.Message}");
                }

                if (headerList != null && headerList.Count > 0)
                {

                    // Create acknowledge for each PO
                    foreach (WebOrderHeaderModel orderHeader in headerList)
                    {
                        List<WebOrderDetailModel> detailList = null;
                        try
                        {
                            //Get the item list                            
                            detailList = GetPOADetailList(orderHeader.WebId);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"AS2Acknowledge::Retreive POA Detail error : {ex.Message}");
                            break;
                        }

                        //Create acknowledge message
                        var grammar = EdiGrammar.NewEdiFact();
                        // var interchange = default(Interchange);

                        Interchange ediObj = null;
                        try
                        {
                            ediObj = CovertToInterChange(orderHeader, detailList);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"AS2Acknowledge::Create interchange object error : {ex.Message}");
                            break;
                        }

                        var output = new StringBuilder();
                        using (var writer = new EdiTextWriter(new StringWriter(output), grammar))
                        {
                            try
                            {
                                new EdiSerializer().Serialize(writer, ediObj);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"AS2Acknowledge::Serialization issue : {ex.Message}");
                                break;
                            }
                        }



                        //Send the acknowledgement 

                        //byte[] file = File.ReadAllBytes(string.Format("{0}\\{1}", ConfigValues.PickLocation, fileName));
                        string filename = "AS2_Ack_"+ orderHeader.WebId+"_"  + DateTime.Now.ToString("ddMMyyyy_hh_mm_ss") + ".txt";
                        System.IO.File.WriteAllText($"{Util.ConfigValues.DropLocation}\\{filename}", output.ToString());
                        byte[] file = Encoding.ASCII.GetBytes(output.ToString());
                        ProxySettings proxy = new ProxySettings();
                        

                        if (file.Length > 0)
                        {
                            try
                            {
                                AS2Send as2Send = new AS2Send();
                                as2Send.SendAcknowledgment(file, ConfigValues.LocalFrom, orderHeader.AS2Identifier, proxy, 50000, ConfigValues.SigningCertFilename, ConfigValues.SigningCertPassword);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"AS2Acknowledge::Acknowledgment sending issue : {ex.Message}");
                                break;
                            }


                            //Update DB fields for respond sending

                            try
                            {
                                UpdateWebOrderSyncStatus(orderHeader.WebId, orderHeader.ResponseDate);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"AS2Acknowledge::Update POA send status failed : {ex.Message}");
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"AS2Acknowledge::Error Occured : {ex.Message}");
            }

        }

        public List<WebOrderHeaderModel> GetPendingPOAckList()
        {
            OrderDataService OrderDataServ = new OrderDataService();

            try
            {
                // OrderDataServ = HttpContext.Current.Application["OrderDataServ"] as OrderDataService;
                return OrderDataServ.GetPendingWebOrderAcknowledgments();
            }
            catch (Exception ex)
            {
                Log.Error($"Order header read failed. {ex.Message} + Inner Ex : {ex.InnerException.Message}");
                throw;
            }
        }

        public List<WebOrderDetailModel> GetPOADetailList(int webId)
        {
            OrderDataService OrderDataServ = new OrderDataService();

            try
            {
                // OrderDataServ = HttpContext.Current.Application["OrderDataServ"] as OrderDataService;
                return OrderDataServ.GetWebOrderDetailsForAck(webId);
            }
            catch (Exception ex)
            {
                Log.Error($"Order detail read failed. {ex.Message} + Inner Ex : {ex.InnerException.Message}");
                throw;
            }
        }


        public bool UpdateWebOrderSyncStatus(int webId, DateTime responseDate)
        {
            OrderDataService OrderDataServ = new OrderDataService();

            try
            {
                //OrderDataServ = HttpContext.Current.Application["OrderDataServ"] as OrderDataService;
                return OrderDataServ.UpdateWebOrderSyncStatus(webId, responseDate);
            }
            catch (Exception ex)
            {
                Log.Error($"Order acknowledge status update failed. {ex.Message} + Inner Ex : {ex.InnerException.Message}");
                throw;
            }
        }

        private Interchange CovertToInterChange(WebOrderHeaderModel webOrderHeader, List<WebOrderDetailModel> webOrderDetail)
        {
            Interchange interchange = new Interchange();

            interchange.RecipientId = webOrderHeader.SenderAddress;
            interchange.SenderId = webOrderHeader.ReceiverAddress;

            Quote quote = new Quote();
            quote.MessageType = "ORDERS";
            quote.ReleaseNumber = "01B";
            quote.AssociationAssignedCode = "EAN010";
            quote.DocumentNumber = webOrderHeader.OrderResponseNo;
            quote.ReleaseNumber = "01B";
            quote.MessageFunction = webOrderHeader.DetailsStatus.ToString();

            DTM responseDate = new DTM();
            DTM deliveryDate = new DTM();

            responseDate.ID = 137;
            responseDate.DateTime = webOrderHeader.ResponseDate;
            responseDate.Code = 102;

            deliveryDate.ID = 2;
            deliveryDate.DateTime = webOrderHeader.DeliveryDate;
            deliveryDate.Code = 102;

            quote.MessageDate = responseDate;
            quote.DeliveryDate = deliveryDate;
            quote.DocumentNumber = webOrderHeader.BuyerOrderNo;

            NAD buingAddress = new NAD();
            NAD shippingAddress = new NAD();
            NAD supplierAddress = new NAD();

            buingAddress.PartyQualifier = "BY";
            buingAddress.PartyId = webOrderHeader.BuyingAddress;

            shippingAddress.PartyQualifier = "ST";
            shippingAddress.PartyId = webOrderHeader.ShippingAddress;

            supplierAddress.PartyQualifier = "SU";
            supplierAddress.PartyId = webOrderHeader.SupplierAddress;

            quote.NAD = new List<NAD>();
            quote.NAD.Add(buingAddress);
            quote.NAD.Add(shippingAddress);
            quote.NAD.Add(supplierAddress);

            List<LineItem> lineList = new List<LineItem>();

            int index = 1;
            foreach (WebOrderDetailModel item in webOrderDetail)
            {
                LineItem lineItem = new LineItem();
                lineItem.GTIN = item.GTINCode;
                lineItem.Code = item.ItemStatus.ToString();
                lineItem.LineNumber = index;
                lineItem.itemTypeIDCode = "SRV";

                Price priceItem = new Price();
                priceItem.Code = "AAF";
                priceItem.Amount = Convert.ToDecimal(item.ItemPrice);

                QTY quantityItem = new QTY();
                quantityItem.Quantity = Convert.ToInt32(item.BackOrderQty);
                quantityItem.QuantityType = "113";

                lineItem.Price = priceItem;
                lineItem.QTY = quantityItem;

                index++;

                lineList.Add(lineItem);
            }

            quote.Lines = lineList;
            interchange.QuoteMessage = quote;

            return interchange;

        }


    }
}