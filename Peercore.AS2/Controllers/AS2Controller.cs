using log4net;
using Peercore.AS2.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace Peercore.AS2.Controllers
{
    /*-----------------------------------------------------------------
    HOW THIS WORKS ----------------------------------------------------
    -------------------------------------------------------------------
    When Alice wants to send an authenticated message to Bob, she should sign and encrypt the message. In particular, 
	    1) she prepends Bob's name to the message, 
	    2) signs this using her private key, 
	    3) appends her signature to the message, 
	    4) encrypts the whole thing under Bob's public key, 
	    5) and sends the resulting ciphertext to Bob. 
    Bob can decrypt, verify the signature, and confirm that this indeed came from Alice.

    -------------------------------------------------------------------
    HOW to create required keys (self signed) ------------------------- 
    -------------------------------------------------------------------

    [For Realtime operation - Sender]

        1) ssl-keygen [and follow the prompt and set name as alice_rsa (This will create 2 keys alice_rsa and alice_rsa.pub)]
        2) openssl req -new -x509 -key alice_rsa -out alice.pem -days 1095 [Step 1 of creating Alice's pfx file for signing (Can be used for encryption as well)]
        3) openssl x509 -outform der -in alice.pem -out alice.der [This is Alice's public cert, with her public key. Give this to Bob, Bob validates the signature]
    [For testing - Sender]
        4) openssl pkcs12 -inkey alice_rsa -in alice.pem -export -out txcert.pfx [Step 2 of creating Alice's pfx file for signing (Can be used for encryption as well)]  
            Install this in Bob's server
         
    [For Realtime operation - Receiver]
    
        1) ssl-keygen [and follow the prompt and set name as bob_rsa (This will create 2 keys bob_rsa and bob_rsa.pub)]
        2) openssl req -new -x509 -key bob_rsa -out bob.pem -days 1095 [Step 1 of creating Bob's pfx file for the encryption]
        3) openssl pkcs12 -inkey bob_rsa -in bob.pem -export -out bob.pfx [Step 2 of creating Bob's pfx file for the encryption]
        4) openssl x509 -outform der -in bob.pem -out bob.der [This is bob's public cert, with his public key. Give this to Alice, Alice encrypts using this]
    
    A tool will be provided and will be an integral part of the repository soon, which will generate self signed certificates
    ------------------------------------------------------------------*/

    [RoutePrefix("AS2")]
    public class AS2Controller : ApiController
    {
        private readonly ILog Log = LogManager.GetLogger(ConfigValues.LoggerID);

        /// <summary>
        /// Receives AS2 encrypted EDI messages and processes accordingly.
        /// </summary>
        [Route("Receive")]
        [AcceptVerbs("Get", "Post")]
        public void Receive()
        {
            try
            {
                Log.Debug("New Transaction -------------------------------------\n");
                HttpContext context = HttpContext.Current;
                AS2Receive as2Receive = new AS2Receive();
                AS2Send as2Send = new AS2Send();

                string sTo = context.Request.Headers["AS2-To"];
                string sFrom = context.Request.Headers["AS2-From"];
                string sMessageID = context.Request.Headers["Message-ID"];
                Log.Info(context.Request);

                if (context.Request.HttpMethod == "POST" || context.Request.HttpMethod == "PUT" ||
                   (context.Request.HttpMethod == "GET" && context.Request.QueryString.Count > 0))
                {

                    if (sFrom == null || sTo == null)
                    {
                        //Invalid AS2 Request.
                        //Section 6.2 The AS2-To and AS2-From header fields MUST be present in all AS2 messages
                        if (!(context.Request.HttpMethod == "GET" && context.Request.QueryString[0].Length == 0))
                        {
                            AS2Receive.BadRequest(context.Response, "Invalid or unauthorized AS2 request received.");
                        }
                    }
                    else
                    {
                        Log.Debug("Processing EDI transaction -------------------------------------\n");
                        as2Receive.Process(context.Request, WebConfigurationManager.AppSettings["DropLocation"]);
                        // Send ther MDN
                        Log.Debug("Sending MDN back -------------------------------------\n");
                        as2Send.SendMDN(context.Request, 50000, ConfigValues.SigningCertFilename, ConfigValues.SigningCertPassword);
                    }
                }
                else
                {
                    AS2Receive.GetAPIStatusMessage(context.Response);
                }

                context.Response.Write("SUCCESS");
                context.Response.StatusCode = 200;
                context.Response.StatusDescription = "Successful";
                Log.Debug("Transaction END-------------------------------------\n" +
                    "----------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat($"Critical error during EDI transaction processing\n" +
                    $"Exception : {ex.Message}\n" +
                    $"Inner Exception : {ex.InnerException?.Message}\n" +
                    $"Stack Trace : \n{ex.StackTrace}\n");
                Log.Error("Transaction FAILED-------------------------------------\n" +
                    "----------------------------------------------------------------------------------");
            }
        }
        /// <summary>
        /// Sends the test edi file from a predefined location. 
        /// THIS IS FOR THE TEST CHANNEL
        /// </summary>
        [Route("Send")]
        [AcceptVerbs("Get", "Post")]
        public void SendTestEDIFile()
        {
            try
            {
                Log.Info("Inside Send Method");
                string fileName = "SendFile.txt";

                byte[] file = File.ReadAllBytes(string.Format("{0}\\{1}", ConfigValues.PickLocation, fileName));
                ProxySettings proxy = new ProxySettings();

                AS2Send as2Send = new AS2Send();
                as2Send.SendFile(ConfigValues.DestinationUri, fileName, file, ConfigValues.LocalFrom, ConfigValues.LocalTo, proxy, 50000, ConfigValues.SigningCertFilename, ConfigValues.SigningCertPassword,
                    ConfigValues.RecipientPubCertFilename);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
        
    }
}
