using Peercore.DataAccess.Common;
using Peercore.DataAccess.Common.Parameters;
using Peercore.DataAccess.Common.Utilties;
using Peercore.Email.Common;
using Peercore.Email.Model;
using Peercore.Workflow.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peercore.Email.DataService
{
    public class OrderDataService: BaseDataService<OrderDataService>
    {
        private DbSqlAdapter OrderSql { get; set; }

        ///---------------------------------------------------------------------------------------
        /// <summary>
        /// Registers the SQL.
        /// </summary>
        ///---------------------------------------------------------------------------------------
        private void RegisterSql()
        {
            this.OrderSql = new DbSqlAdapter("Peercore.Email.DataService.SQL.OrderSQL.xml",
                ApplicationService.Instance.DbProvider);
        }

        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDataService"/> class.
        /// </summary>
        ///---------------------------------------------------------------------------------------
        public OrderDataService()
        {
            RegisterSql();
        }

        ///---------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDataService"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        ///---------------------------------------------------------------------------------------
        public OrderDataService(DbWorkflowScope scope)
            : base(scope)
        {
            RegisterSql();
        }

        //---------------------------------------------------------------------------------------        
        /// <summary>
        /// Inserts the web order header.
        /// This is Soooo prone for race conditions as this is not the only application writing
        /// to this table. Workaround taken now is to schedule the two applications in 
        /// non conflicting windows. But THIS SHOULD BE PROPERLY ADDRESSED
        /// </summary>
        /// <param name="orderHeader">The order header.</param>
        /// <returns>Success or failure</returns>
        ///---------------------------------------------------------------------------------------
        public bool InsertWebOrderHeader(WebOrderHeaderModel orderHeader)
        {
            bool isSuccess = false;
            int NoRecord = 0;
            try
            {
                int assigneeNo = 0;
                float freight = 0.000F;
                string status = "Processing";

                DbInputParameterCollection paramCollection = new DbInputParameterCollection()
                {
                
                    DbInputParameter.GetInstance("@WebId",
                    DbType.Int32,
                    orderHeader.WebId),
                    
                    //DbInputParameter.GetInstance("@CustomerCode",
                    //DbType.String,
                    //orderHeader.CustomerCode),
                    
                    DbInputParameter.GetInstance("@OrderDate",
                    DbType.DateTime,
                    orderHeader.OrderDate),
                                                          
                    DbInputParameter.GetInstance("@DateRequired",
                    DbType.String,
                    orderHeader.DateRequired),
                    
                    DbInputParameter.GetInstance("@Comments",
                    DbType.String,
                    orderHeader.Comments),

                    DbInputParameter.GetInstance("@OrderNote",
                    DbType.String,
                    orderHeader.OrderNote),

                    DbInputParameter.GetInstance("@PurchaseCode",
                    DbType.String,
                    orderHeader.PurchaseCode),

                    DbInputParameter.GetInstance("@Status",
                    DbType.String,
                    status),

                    DbInputParameter.GetInstance("@IsAckReq",
                    DbType.Int32,
                    orderHeader.IsAckRequired),

                    DbInputParameter.GetInstance("@CustomerCode",
                    DbType.String,
                    orderHeader.CustomerCode),

                    DbInputParameter.GetInstance("@AssigneeNo",
                    DbType.Int32,
                    assigneeNo),

                    DbInputParameter.GetInstance("@Freight",
                    DbType.Int32,
                    freight),

                    DbInputParameter.GetInstance("@IsSaved",
                    DbType.String,
                    string.Empty), // OR application only display orders with is_saved blank

                    DbInputParameter.GetInstance("@IsProcessed",
                    DbType.Int32,
                    Convert.ToInt32("0")),

                    DbInputParameter.GetInstance("@DetailStatus",
                    DbType.Int32,
                    Convert.ToInt32("0")),

                    DbInputParameter.GetInstance("@SenderAddress",
                    DbType.String,
                    orderHeader.SenderAddress),

                    DbInputParameter.GetInstance("@ReceiverAddress",
                    DbType.String,
                    orderHeader.ReceiverAddress),

                    DbInputParameter.GetInstance("@MsgRef",
                    DbType.String,
                    orderHeader.MessageRefference),

                    DbInputParameter.GetInstance("@BuyerOrderNo",
                    DbType.String,
                    orderHeader.BuyerOrderNo),

                    DbInputParameter.GetInstance("@PromotionNo",
                    DbType.String,
                    orderHeader.PromotionNo),

                    DbInputParameter.GetInstance("@BuyingAddress",
                    DbType.String,
                    orderHeader.BuyingAddress),

                    DbInputParameter.GetInstance("@ShippingAddress",
                    DbType.String,
                    orderHeader.ShippingAddress),

                    DbInputParameter.GetInstance("@SupplierAddress",
                    DbType.String,
                    orderHeader.SupplierAddress),

                    DbInputParameter.GetInstance("@TotalAmount",
                    DbType.Decimal,
                    orderHeader.TotalAmount),

                    DbInputParameter.GetInstance("@ItemCount",
                    DbType.Int32,
                    orderHeader.ItemCount),

                    DbInputParameter.GetInstance("@AS2Identifier",
                    DbType.String,
                    orderHeader.AS2Identifier),

                };

                NoRecord = this.DataAcessService.ExecuteNonQuery(OrderSql["InsertWebOrderHeader"], paramCollection);
                if (NoRecord > 0)
                {
                    isSuccess = true;

                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return isSuccess;
        }
        //---------------------------------------------------------------------------------------
        // This is Soooo prone for race conditions as this is not the only application writing
        // to this table. Workaround taken now is to schedule the two applications in 
        // non conflicting windows. But THIS SHOULD BE PROPERLY ADDRESSED
        //---------------------------------------------------------------------------------------
        public bool InsertWebOrderDetail(WebOrderDetailModel orderDetail)
        {
            bool isSuccess = false;
            int NoRecord = 0;
            try
            {

                float backOrderQty = 0.000F;
                float unitDiscountPerc = 0.000F;
                float tax = 0.000F;
                float freeQty = 0.000F;
               // string catlogCode = string.Empty;

                DbInputParameterCollection paramCollection = new DbInputParameterCollection()
                {
                
                    DbInputParameter.GetInstance("@WebId",
                    DbType.Int32,
                    orderDetail.WebId),
                    
                    DbInputParameter.GetInstance("@CatlogCode",
                    DbType.String,
                    orderDetail.CatlogCode),
                    
                    DbInputParameter.GetInstance("@OrderQty",
                    DbType.Int32,
                    orderDetail.OrderQty),
                                                          
                    DbInputParameter.GetInstance("@BackOrderQty",
                    DbType.Int32,
                    backOrderQty),
                    
                    DbInputParameter.GetInstance("@UnitPrice",
                    DbType.Double,
                    orderDetail.UnitPrice),
                    
                    DbInputParameter.GetInstance("@UnitCost",
                    DbType.Double,
                    orderDetail.UnitCost),

                    DbInputParameter.GetInstance("@UnitDiscountPerc",
                    DbType.Double,
                    unitDiscountPerc),

                    DbInputParameter.GetInstance("@Tax",
                    DbType.Double,
                    tax),

                    DbInputParameter.GetInstance("@FreeQty",
                    DbType.Int32,
                    freeQty),

                    DbInputParameter.GetInstance("@ProductNote",
                    DbType.String,
                    orderDetail.ProductNote),

                    DbInputParameter.GetInstance("@ItemStatus",
                    DbType.Int32,
                    orderDetail.ItemStatus),

                    DbInputParameter.GetInstance("@GTINCode",
                    DbType.String,
                    orderDetail.GTINCode),
                };

                NoRecord = this.DataAcessService.ExecuteNonQuery(OrderSql["InsertWebOrderDetail"], paramCollection);
                if (NoRecord > 0)
                {
                    isSuccess = true;

                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return isSuccess;
        }
        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieves next available WebOrderId from web_sorder_header table
        /// Not the smartest syncroniztion
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------------------------
        public int GetNextPFDWebOrderId()
        {
            int webId = 0;
            try
            {
                webId = Convert.ToInt32(this.DataAcessService.GetOneValue(OrderSql["GetNextPFDWebOrderId"].Format(), null));
            }
            catch (Exception)
            {
                throw;
            }

            return webId;
        }
        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Increment web_id for PFD which is 100K ahead from others
        /// Not the smartest syncroniztion
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------------------------
        public void IncrementPFDWebOrderID()
        {
            try
            {
                this.DataAcessService.ExecuteNonQuery(OrderSql["IncrementCurrentPFDWebOrderID"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the customer code by Bill To number in the PFD file.
        /// Correspondence is such that 'Bill to' --> 'Customer.e_peer_key'
        /// </summary>
        /// <param name="billToNumber">The bill to number.</param>
        /// <returns></returns>
        //---------------------------------------------------------------------------------------
        public string GetCustomerCodeById(int billToNumber)
        {
            string CustCode = "";
            int bn = billToNumber;
            try
            {
                DbInputParameterCollection paramCollection = new DbInputParameterCollection()
                {

                    DbInputParameter.GetInstance("@BillToNumber",
                    DbType.Int32,
                    billToNumber)
                };
                CustCode = Convert.ToString(this.DataAcessService.GetOneValue(OrderSql["GetCustomerCode"], paramCollection), null);
            }
            catch (Exception)
            {
                throw;
            }

            return CustCode;
        }
        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Truncates all the order references for rollbacks.
        /// </summary>
        /// <param name="webId">The web identifier.</param>
        /// <returns></returns>
        //---------------------------------------------------------------------------------------
        public bool RollBackOrderReferences(int webId)
        {
            try
            {
                DbInputParameterCollection paramCollection = new DbInputParameterCollection()
                {

                    DbInputParameter.GetInstance("@WebID",
                    DbType.Int32,
                    webId)
                };

                this.DataAcessService.ExecuteNonQuery(OrderSql["DeleteAllExistingOrderDetails"], paramCollection);
                this.DataAcessService.ExecuteNonQuery(OrderSql["DeleteOrderHeader"], paramCollection);
                return true;
                
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<WebOrderHeaderModel> GetPendingWebOrderAcknowledgments()
        {

            DbDataReader idrOrder = null;
            WebOrderHeaderModel ObjOrder = null;

            try
            {

                List<WebOrderHeaderModel> orderList = new List<WebOrderHeaderModel>();
                idrOrder = this.DataAcessService.ExecuteQuery(OrderSql["GetWebOrdersForPOA"]);

                if (idrOrder != null && idrOrder.HasRows)
                {
                    while (idrOrder.Read())
                    {
                        ObjOrder = new WebOrderHeaderModel();
                        ObjOrder.WebId = ExtensionMethods.GetInt32(idrOrder, "web_id");
                        ObjOrder.DetailsStatus = ExtensionMethods.GetInt32(idrOrder, "detail_changes_status");
                        ObjOrder.SenderAddress = ExtensionMethods.GetString(idrOrder, "sender_edi").ToString();
                        ObjOrder.ReceiverAddress = ExtensionMethods.GetString(idrOrder, "receiver_edi").ToString();
                        ObjOrder.MessageRefference = ExtensionMethods.GetString(idrOrder, "msg_ref").ToString();
                        ObjOrder.OrderResponseNo = ExtensionMethods.GetString(idrOrder, "order_response_no").ToString();
                        ObjOrder.ResponseDate = ExtensionMethods.GetDateTime(idrOrder, "response_date");
                        ObjOrder.DeliveryDate = ExtensionMethods.GetDateTime(idrOrder, "date_required");
                        ObjOrder.BuyerOrderNo = ExtensionMethods.GetString(idrOrder, "buyer_order_no").ToString();
                        ObjOrder.PromotionNo = ExtensionMethods.GetString(idrOrder, "promotional_deal_no").ToString();

                        ObjOrder.BuyingAddress = ExtensionMethods.GetString(idrOrder, "buying_add_gs1_cd").ToString();
                        ObjOrder.ShippingAddress = ExtensionMethods.GetString(idrOrder, "shipping_add_gs1_cd").ToString();
                        ObjOrder.SupplierAddress = ExtensionMethods.GetString(idrOrder, "supplier_add_gs1_cd").ToString();
                        ObjOrder.TotalAmount = ExtensionMethods.GetDouble(idrOrder, "order_tot_amt");
                        ObjOrder.ItemCount = ExtensionMethods.GetInt32(idrOrder, "tot_no_of_items");
                        ObjOrder.AS2Identifier = ExtensionMethods.GetString(idrOrder, "as2_identifier").ToString();
                        orderList.Add(ObjOrder);
                    }
                }
                return orderList;

            }
            catch
            {
                throw;
            }
            finally
            {
                if (idrOrder != null && (!idrOrder.IsClosed))
                    idrOrder.Close();
            }
        }

        public List<WebOrderDetailModel> GetWebOrderDetailsForAck(int headerId)
        {

            DbDataReader idrOrder = null;
            WebOrderDetailModel ObjDetail = null;

            try
            {

                List<WebOrderDetailModel> detailList = new List<WebOrderDetailModel>();
                idrOrder = this.DataAcessService.ExecuteQuery(OrderSql["GetWebOrderDetailsById"].Format(headerId));

                if (idrOrder != null && idrOrder.HasRows)
                {
                    while (idrOrder.Read())
                    {
                        ObjDetail = new WebOrderDetailModel();
                        ObjDetail.WebId = ExtensionMethods.GetInt32(idrOrder, "web_id");
                        ObjDetail.ItemStatus = ExtensionMethods.GetInt32(idrOrder, "record_status");
                        ObjDetail.GTINCode = ExtensionMethods.GetString(idrOrder, "gtin_code").ToString();
                        ObjDetail.BackOrderQty = ExtensionMethods.GetDouble(idrOrder, "order_qty");
                        ObjDetail.ItemPrice = ExtensionMethods.GetDouble(idrOrder, "unit_cost");

                        detailList.Add(ObjDetail);
                    }
                }
                return detailList;

            }
            catch
            {
                throw;
            }
            finally
            {
                if (idrOrder != null && (!idrOrder.IsClosed))
                    idrOrder.Close();
            }
        }

        public bool UpdateWebOrderSyncStatus(int webId, DateTime responseDate)
        {
            bool isSuccess = false;
            int NoRecord = 0;
            try
            {
                DbInputParameterCollection paramCollection = new DbInputParameterCollection()
                {

                    DbInputParameter.GetInstance("@WebId",
                    DbType.Int32,
                    webId),

                    DbInputParameter.GetInstance("@ResponseDate",
                    DbType.DateTime,
                    responseDate),
                };

                NoRecord = this.DataAcessService.ExecuteNonQuery(OrderSql["UpdateSyncStatus"], paramCollection);
                if (NoRecord > 0)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return isSuccess;
        }

        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the catlog code by GTIN code
        /// </summary>
        /// <param name="gtinCode">Item GTIN Code</param>
        /// <returns></returns>
        //---------------------------------------------------------------------------------------
        public string GetCatlogCodeByGTIN(string gtinCode)
        {
            string catlogCode = "";
            try
            {
                DbInputParameterCollection paramCollection = new DbInputParameterCollection()
                {
                    DbInputParameter.GetInstance("@BarCode",
                    DbType.String,
                    gtinCode)
                };
                catlogCode = Convert.ToString(this.DataAcessService.GetOneValue(OrderSql["GetCatlogCode"], paramCollection), null);
            }
            catch (Exception ex)
            {
                return "";
            }
            return catlogCode;
        }


        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the catlog code by GTIN code
        /// </summary>
        /// <param name="gtinCode">Item GTIN Code</param>
        /// <returns></returns>
        //---------------------------------------------------------------------------------------
        public string GetAckCustCode(string custCode)
        {
            string retuenCustCode = "";
            try
            {
                DbInputParameterCollection paramCollection = new DbInputParameterCollection()
                {
                    DbInputParameter.GetInstance("@CustCode",
                    DbType.String,
                    custCode)
                };
                retuenCustCode = Convert.ToString(this.DataAcessService.GetOneValue(OrderSql["GetAckCustCode"], paramCollection), null);
            }
            catch (Exception ex)
            {
                return "";
            }
            return retuenCustCode;
        }

        //---------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the Acknolwedment AS2 detials
        /// </summary>
        /// <param name="gtinCode">Item GTIN Code</param>
        /// <returns></returns>
        //---------------------------------------------------------------------------------------
        public AS2CommunicationModel GetAS2CommunicationDetials(string identifier) {

            DbDataReader idrOrder = null;
            AS2CommunicationModel communicationObj = new AS2CommunicationModel();

            try
            {
                List<WebOrderDetailModel> detailList = new List<WebOrderDetailModel>();

                DbInputParameterCollection paramCollection = new DbInputParameterCollection()
                {
                    DbInputParameter.GetInstance("@identifier",
                    DbType.String,
                    identifier)
                };

                idrOrder = this.DataAcessService.ExecuteQuery(OrderSql["GetAS2CommunicationDetails"], paramCollection);

                if (idrOrder != null && idrOrder.HasRows)
                {
                    while (idrOrder.Read())
                    {
                        communicationObj.AS2Identifier = ExtensionMethods.GetString(idrOrder, "as2_identifier").ToString();
                        communicationObj.AS2MDNURL = ExtensionMethods.GetString(idrOrder, "as2_mdn_url").ToString();
                        communicationObj.CertificateName = ExtensionMethods.GetString(idrOrder, "certificate").ToString();
                        communicationObj.CertificateType = ExtensionMethods.GetString(idrOrder, "certificate_type").ToString();
                        communicationObj.MessageFormat = ExtensionMethods.GetString(idrOrder, "as2_message_format").ToString();
                        communicationObj.PayLoadType = ExtensionMethods.GetString(idrOrder, "as2_payload_type").ToString();
                    }
                }
                return communicationObj;
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                if (idrOrder != null && (!idrOrder.IsClosed))
                    idrOrder.Close();
            }
        }

    }
}
