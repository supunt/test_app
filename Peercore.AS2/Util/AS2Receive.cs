using indice.Edi;
using log4net;
using Peercore.AS2.Util.EdifactModels;
using Peercore.DataAccess.Common.Exceptions;
using Peercore.Email.DataService;
using Peercore.Email.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Peercore.AS2.Util
{
    public class AS2Receive
    {
        private static readonly ILog Log = LogManager.GetLogger("AS2Receive");

        /// <summary>
        /// This is to check if the API is up and running
        /// </summary>
        /// <param name="response">The response.</param>
        public static void GetAPIStatusMessage(HttpResponse response)
        {
            response.StatusCode = 200;
            response.StatusDescription = "Okay";

            response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 3.2 Final//EN"">"
            + @"<HTML><HEAD><TITLE>Generic AS2 Receiver</TITLE></HEAD>"
            + @"<BODY><H1>200 Okay</H1><HR>This is to inform you that the AS2 interface is working and is "
            + @"accessable from your location.  This is the standard response to all who would send a GET "
            + @"request to this page instead of the POST context.Request defined by the AS2 Draft Specifications.<HR></BODY></HTML>");
        }

        /// <summary>
        /// Bads the request response
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="message">The message.</param>
        public static void BadRequest(HttpResponse response, string message)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.StatusDescription = "Bad context.Request";

            response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 3.2 Final//EN"">"
            + @"<HTML><HEAD><TITLE>400 Bad context.Request</TITLE></HEAD>"
            + @"<BODY><H1>400 Bad context.Request</H1><HR>There was a error processing this context.Request.  The reason given by the server was:"
            + @"<P><font size=-1>" + message + @"</Font><HR></BODY></HTML>");
        }

        /// <summary>
        /// Bads the request response
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="message">The message.</param>
        public static void UnhandledError(HttpResponse response, string message)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.StatusDescription = "Unhandled error";

            response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 3.2 Final//EN"">"
            + @"<HTML><HEAD><TITLE>400 Bad context.Request</TITLE></HEAD>"
            + @"<BODY><H1>400 Bad context.Request</H1><HR>There was a error processing this context.Request.  The reason given by the server was:"
            + @"<P><font size=-1>" + message + @"</Font><HR></BODY></HTML>");
        }

        /// <summary>
        /// Processes AS2 wrapped EDI message
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dropLocation">The drop location.</param>
        public void Process(HttpRequest request, string dropLocation)
        {
            string sTo = request.Headers["AS2-To"];
            string sFrom = request.Headers["AS2-From"];
            string dtf = $"{sFrom}_{sTo}_" + DateTime.Now.ToString("ddMMyyyy_hh_mm_ss") + ".txt";
            string filename = $"AS2_transaction_{dtf}";
            string extractfilename = $"AS2_transaction_extract_{dtf}";

            byte[] data = request.BinaryRead(request.TotalBytes);
            bool isEncrypted = request.ContentType.Contains("application/pkcs7-mime");
            bool isSigned = request.ContentType.Contains("application/pkcs7-signature");

            string message = string.Empty;

            Log.DebugFormat($"Process\n" +
                    $"\tFrom : {sFrom}\n" +
                    $"\tTo : {sTo}\n" + 
                    $"\tIs Signed : {isSigned}\n" + 
                    $"\tIs Encrypted : {isEncrypted}\n" +
                    $"\tContentType string : {request.ContentType}\n" +
                    $"Dump file name : {filename}\n");

            if (!isSigned && !isEncrypted) // not signed and not encrypted
            {
                message = System.Text.ASCIIEncoding.ASCII.GetString(data);
            }      
            if (isEncrypted) // encrypted and signed inside
            {
                byte[] decryptedData;

                try
                {
                    Log.Debug("Decrypting......");
                    decryptedData = AS2Encryption.Decrypt(data);
                    Log.Debug("Decrypting SUCCESS.");
                }
                catch (Exception ex)
                {
                    Log.Error($"Exception during decrypting {ex.Message}");

                    if (ex.InnerException != null)
                        Log.Error($"Inner Exception during decoding {ex.InnerException.Message}");

                    throw;
                }

                string messageWithContentTypeLineAndMIMEHeaders = System.Text.ASCIIEncoding.ASCII.GetString(decryptedData);

                // when encrypted, the Content-Type line is actually stored in the start of the message
                // Require multi split done by 'ExtractPayload'
                // First segment is data payload - THIS IS EDIFACT/XML data
                // Second payload is a public key file [Not essential]
                int firstBlankLineInMessage = messageWithContentTypeLineAndMIMEHeaders.IndexOf(Environment.NewLine + Environment.NewLine);
                string contentType = messageWithContentTypeLineAndMIMEHeaders.Substring(0, firstBlankLineInMessage);

                message = AS2MIMEUtilities.ExtractPayload(messageWithContentTypeLineAndMIMEHeaders, contentType);

                try
                {
                    System.IO.File.WriteAllText($"{Util.ConfigValues.DropLocation}\\{extractfilename}", message);
                    Log.Debug($"Message Extract written to file {Util.ConfigValues.DropLocation}\\{extractfilename}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to write message extract to file : {Util.ConfigValues.DropLocation}\\{extractfilename}\n" +
                        $"Exception : {ex.Message}\n");

                }

                string signature = "";

                if (isSigned)
                {
                    //int SecondBlankLineInMessage = message.IndexOf(Environment.NewLine + Environment.NewLine);
                    int signatureHeaderStart = message.IndexOf(Environment.NewLine + "--_");
                    string messageWithSignature = message;
                    Log.DebugFormat($"Message with embedded signature\n" +
                                $"\tSignature start found at : {signatureHeaderStart}\n" +
                                $"\tSignature start found invalid format : {message.IndexOf('\n' + "--_")}\n");

                    message = message.Substring(0, signatureHeaderStart);

                    string signaturePart = messageWithSignature.Substring(signatureHeaderStart);
                    int signatureStart = signaturePart.IndexOf(Environment.NewLine + Environment.NewLine);

                    signature = signaturePart.Substring(signatureStart + 4);

                    Log.Debug($"\n----------------------------------------------------------------------------------------------------------------------------------\n" +
                            $"Extracted EDI Message\n {message}\n" +
                            $"----------------------------------------------------------------------------------------------------------------------------------\n");

                    //TODO :: Kavisha - uncomment after testing 
                    if (AS2Encryption.validateSignature(signature) == false)
                    {
                        Log.Error($"Invalid signature {signature}");
                        return;
                    }
                }
                else
                {
                    int signatureHeaderStart = message.IndexOf(Environment.NewLine + "--_");
                    // Check if it is marked inside the mesage payload
                    if (signatureHeaderStart >= 0)
                    {

                        //int SecondBlankLineInMessage = message.IndexOf(Environment.NewLine + Environment.NewLine);

                        string messageWithSignature = message;
                        Log.DebugFormat($"Message with embedded signature\n" +
                                $"\tSignature start found at : {signatureHeaderStart}\n" +
                                $"\tSignature start found invalid format : {message.IndexOf('\n' + "--_")}\n");

                        message = message.Substring(0, signatureHeaderStart);

                        string signaturePart = messageWithSignature.Substring(signatureHeaderStart);
                        int signatureStart = signaturePart.IndexOf(Environment.NewLine + Environment.NewLine);

                        signature = signaturePart.Substring(signatureStart + 4);

                        Log.Debug($"\n----------------------------------------------------------------------------------------------------------------------------------\n" +
                            $"Extracted EDI Message\n {message}\n" +
                            $"----------------------------------------------------------------------------------------------------------------------------------\n");

                        //TODO :: Kavisha - uncomment after testing 
                        if (AS2Encryption.validateSignature(signature) == false)
                        {
                            Log.Error($"Invalid signature {signature}");
                            return;
                        }
                    }
                    else
                    {
                        Log.Error("Invalid payload [Not signed]");
                    }

                }

            }
            else
            {
                Log.Debug("Non-Encrypted..");
                if (isSigned)
                {
                    Log.Debug("Non-Encrypted signed..");
                    string messageWithMIMEHeaders = System.Text.ASCIIEncoding.ASCII.GetString(data);
                    message = AS2MIMEUtilities.ExtractPayload(messageWithMIMEHeaders, request.Headers["Content-Type"]);
                    Log.Debug($"Extracted message {message}");

                    string signaturePart = messageWithMIMEHeaders.Substring(message.Length);
                    string signature = AS2MIMEUtilities.ExtractPayload(signaturePart, request.Headers["Content-Type"]);

                    Log.Debug($"Extracted signature {signature}");

                    if (AS2Encryption.validateSignature(signature) == false)
                    {
                        Log.Error($"Invalid signature {signature}");
                        return;
                    }
                }
            }

            try
            {
                System.IO.File.WriteAllText($"{Util.ConfigValues.DropLocation}\\{filename}", message);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write dump to file : {Util.ConfigValues.DropLocation}\\{filename}\n" +
                    $"Exception : {ex.Message}\n");

            }

            var grammar = EdiGrammar.NewEdiFact();
            var interchange = default(Interchange);
            using (var stream = new StreamReader($"{Util.ConfigValues.DropLocation}\\{filename}"))
            {
                Quote quoteMessage = null;
                try
                {
                    interchange = new EdiSerializer().Deserialize<Interchange>(stream, grammar);
                    quoteMessage = interchange.QuoteMessage;
                }
                catch (Exception ex)
                {
                    Log.Error($"Content to EDI deserialization error : {ex.Message}\n" +
                        $"Payload File : {Util.ConfigValues.DropLocation}\\{filename}\n");
                    throw;
                }
                try
                {
                    if (quoteMessage != null)
                    {
                        if (quoteMessage.MessageName != "220")
                            return;

                        WebOrderHeaderModel webOrder = new WebOrderHeaderModel();
                        webOrder.AS2Identifier = request.Headers["AS2-From"];
                        quoteMessage.fillWebOrder(webOrder,interchange);
                        InsertOrderToDB(webOrder);
                        Log.Info("Order inserted ----------------");
                        Log.Info(webOrder.ToString());  
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Order insertion error : {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Log.Error($"\tInner Exception : {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Inserts the order to database.
        /// </summary>
        /// <param name="webOrder">The web order.</param>
        private void InsertOrderToDB(WebOrderHeaderModel webOrder)
        {
            OrderDataService OrderDataServ = null;
            try
            {
                OrderDataServ = HttpContext.Current.Application["OrderDataServ"] as OrderDataService;
                OrderDataServ.IncrementPFDWebOrderID(); // Increment before fetch
                webOrder.WebId = OrderDataServ.GetNextPFDWebOrderId();
                webOrder.CustomerCode = OrderDataServ.GetCustomerCodeById(webOrder.BillTo);

                string ackCustCode = OrderDataServ.GetAckCustCode(webOrder.CustomerCode);

                if (!string.IsNullOrEmpty(ackCustCode)) 
                    webOrder.IsAckRequired = 1;               
                else
                    webOrder.IsAckRequired = 0;

                if (string.IsNullOrEmpty(webOrder.CustomerCode) == false)
                {
                    webOrder.OrderNote = " ";
                }
            }
            catch (QueryExecutionException ex)
            {
                Log.Error($"Query execution exception {ex.Message}");
                throw;
            }

            webOrder.Comments = " ";

            bool success = false;
            try
            {
                success = OrderDataServ.InsertWebOrderHeader(webOrder);
            }
            catch (QueryExecutionException ex)
            {
                Log.Error($"Order header insertion failed. {ex.Message} + Inner Ex : {ex.InnerException.Message}");
                throw;
            }

            if (false == success)
                return;


            foreach (WebOrderDetailModel wod in webOrder.WebOrderDetailList)
            {
                try
                {
                    wod.WebId = webOrder.WebId;

                    if (string.IsNullOrEmpty(wod.CatlogCode)) {
                        wod.CatlogCode = OrderDataServ.GetCatlogCodeByGTIN(wod.GTINCode);
                        wod.ProductNote = wod.CatlogCode;
                    }
                    success = OrderDataServ.InsertWebOrderDetail(wod);
                }
                catch (QueryExecutionException ex)
                {
                    Log.Error($"Order detail insertion failed. {ex.Message} + Inner Ex : {ex.InnerException.Message}, rolling back.");
                    OrderDataServ.RollBackOrderReferences(webOrder.WebId);
                    throw;
                }
                if (success == false)
                {
                    Log.Error($"Order detail inserttion failed for web order id : {webOrder.WebId} , rolling back.");
                    OrderDataServ.RollBackOrderReferences(webOrder.WebId);
                    return;
                }

            }
        }

    }
}