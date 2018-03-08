using Peercore.AS2.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Peercore.Email.Model;
using Peercore.Email.DataService;
using System.Text;
using log4net;

namespace Peercore.AS2.Util
{
    

    public struct ProxySettings
    {
        public string Name;
        public string Username;
        public string Password;
        public string Domain;
    }

    /// <summary>
    /// AS2Sender, This sends a file wrapped in AS2 signed and encrypted upon the existance of the following, pointed by the web config
    /// SigningCertFilename and RecipientPubCertFilename
    /// THIS IS FOR TEST CHANNEL ONLY
    /// </summary>
    public class AS2Send
    {

        private static readonly ILog Log = LogManager.GetLogger("AS2Send");

        public HttpStatusCode SendFile(Uri uri, string filename, byte[] fileData, string from, string to, ProxySettings proxySettings
            , int timeoutMs, string signingCertFilename, string signingCertPassword, string recipientPubCertFilename /*with public key*/)
        {
            if (String.IsNullOrEmpty(filename)) throw new ArgumentNullException("filename");

            if (fileData.Length == 0) throw new ArgumentException("filedata");

            byte[] content = fileData;

            //Initialise the request
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(uri);

            if (!String.IsNullOrEmpty(proxySettings.Name))
            {
                WebProxy proxy = new WebProxy(proxySettings.Name);

                NetworkCredential proxyCredential = new NetworkCredential();
                proxyCredential.Domain = proxySettings.Domain;
                proxyCredential.UserName = proxySettings.Username;
                proxyCredential.Password = proxySettings.Password;

                proxy.Credentials = proxyCredential;

                http.Proxy = proxy;
            }

            //Define the standard request objects
            http.Method = "POST";

            http.AllowAutoRedirect = true;

            http.KeepAlive = true;

            http.PreAuthenticate = false; //Means there will be two requests sent if Authentication required.
            http.SendChunked = false;

            http.UserAgent = "PEERCORE AGENT";

            //These Headers are common to all transactions
            http.Headers.Add("Mime-Version", "1.0");
            http.Headers.Add("AS2-Version", "1.2");

            http.Headers.Add("AS2-From", from);
            http.Headers.Add("AS2-To", to);
            http.Headers.Add("Subject", filename);
            http.Headers.Add("Message-Id", "<AS2_" + DateTime.Now.ToString("hhmmssddd") + ">");
            http.Timeout = timeoutMs;

            string contentType = (Path.GetExtension(filename) == ".xml") ? "application/xml" : "application/EDIFACT";

            bool encrypt = !string.IsNullOrEmpty(recipientPubCertFilename);
            bool sign = !string.IsNullOrEmpty(signingCertFilename);

            if (!sign && !encrypt)
            {
                http.Headers.Add("Content-Transfer-Encoding", "binary");
                http.Headers.Add("Content-Disposition", "inline; filename=\"" + filename + "\"");
            }
            if (sign)
            {
                // Wrap the file data with a mime header
                content = AS2MIMEUtilities.CreateMessage(contentType, "binary", "", content);

                content = AS2MIMEUtilities.Sign(content, signingCertFilename, signingCertPassword, out contentType);

                http.Headers.Add("EDIINT-Features", "multiple-attachments");

            }
            if (encrypt)
            {
                if (string.IsNullOrEmpty(recipientPubCertFilename))
                {
                    throw new ArgumentNullException(recipientPubCertFilename, "if encrytionAlgorithm is specified then recipientCertFilename must be specified");
                }

                byte[] signedContentTypeHeader = System.Text.ASCIIEncoding.ASCII.GetBytes("Content-Type: " + contentType + Environment.NewLine);
                byte[] contentWithContentTypeHeaderAdded = AS2MIMEUtilities.ConcatBytes(signedContentTypeHeader, content);

                string ba2Str = System.Text.Encoding.Default.GetString(content);
                string s1 = System.Text.Encoding.Default.GetString(signedContentTypeHeader);
                string s2 = System.Text.Encoding.Default.GetString(contentWithContentTypeHeaderAdded);
                content = AS2Encryption.Encrypt(contentWithContentTypeHeaderAdded, recipientPubCertFilename, EncryptionAlgorithm.DES3);


                contentType += "application/pkcs7-mime; smime-type=enveloped-data; name=\"smime.p7m\"";
            }

            http.ContentType += contentType;
            http.ContentLength = content.Length;

            SendWebRequest(http, content);

            return HandleWebResponse(http);
        }


        public HttpStatusCode SendAcknowledgment(byte[] fileData, string from, string to, ProxySettings proxySettings
            , int timeoutMs, string signingCertFilename, string signingCertPassword)
        {

            try
            {
                byte[] content = fileData;


                OrderDataService OrderDataServ = new OrderDataService();
                AS2CommunicationModel commDetails = OrderDataServ.GetAS2CommunicationDetials(to);

                //  if (commDetails != null)
                //  {

                //Initialise the request
                HttpWebRequest http = (HttpWebRequest)WebRequest.Create(commDetails.AS2MDNURL);
                string recipientPubCertFilename = commDetails.CertificateName;

                if (!String.IsNullOrEmpty(proxySettings.Name))
                {
                    WebProxy proxy = new WebProxy(proxySettings.Name);

                    NetworkCredential proxyCredential = new NetworkCredential();
                    proxyCredential.Domain = proxySettings.Domain;
                    proxyCredential.UserName = proxySettings.Username;
                    proxyCredential.Password = proxySettings.Password;

                    proxy.Credentials = proxyCredential;

                    http.Proxy = proxy;
                }

                //Define the standard request objects
                http.Method = "POST";

                http.AllowAutoRedirect = true;

                http.KeepAlive = true;

                http.PreAuthenticate = false; //Means there will be two requests sent if Authentication required.
                http.SendChunked = false;

                http.UserAgent = "PEERCORE AGENT";

                //These Headers are common to all transactions
                http.Headers.Add("Mime-Version", "1.0");
                http.Headers.Add("AS2-Version", "1.2");

                http.Headers.Add("AS2-From", from);
                http.Headers.Add("AS2-To", to);
                http.Headers.Add("Subject", "PO Acknowledgment");
                http.Headers.Add("Message-Id", "<AS2_" + DateTime.Now.ToString("hhmmssddd") + ">");
                http.Timeout = timeoutMs;

                string contentType = "application/EDIFACT";

                bool encrypt = !string.IsNullOrEmpty(recipientPubCertFilename);
                bool sign = !string.IsNullOrEmpty(signingCertFilename);

                if (!sign && !encrypt)
                {
                    http.Headers.Add("Content-Transfer-Encoding", "binary");
                    http.Headers.Add("Content-Disposition", "inline");
                }
                if (sign)
                {
                    // Wrap the file data with a mime header
                    content = AS2MIMEUtilities.CreateMessage(contentType, "binary", "", content);

                    content = AS2MIMEUtilities.Sign(content, signingCertFilename, signingCertPassword, out contentType);

                    http.Headers.Add("EDIINT-Features", "multiple-attachments");

                }
                if (encrypt)
                {
                    if (string.IsNullOrEmpty(recipientPubCertFilename))
                    {
                        throw new ArgumentNullException(recipientPubCertFilename, "if encrytionAlgorithm is specified then recipientCertFilename must be specified");
                    }

                    byte[] signedContentTypeHeader = System.Text.ASCIIEncoding.ASCII.GetBytes("Content-Type: " + contentType + Environment.NewLine);
                    byte[] contentWithContentTypeHeaderAdded = AS2MIMEUtilities.ConcatBytes(signedContentTypeHeader, content);

                    string ba2Str = System.Text.Encoding.Default.GetString(content);
                    string s1 = System.Text.Encoding.Default.GetString(signedContentTypeHeader);
                    string s2 = System.Text.Encoding.Default.GetString(contentWithContentTypeHeaderAdded);
                    content = AS2Encryption.Encrypt(contentWithContentTypeHeaderAdded, recipientPubCertFilename, EncryptionAlgorithm.DES3);


                    if (commDetails.MessageFormat == "SMIME")
                        contentType += "application/pkcs7-mime; smime-type=enveloped-data; name=\"smime.p7m\"";
                }

                http.ContentType += contentType;
                http.ContentLength = content.Length;

                SendWebRequest(http, content);

                return HandleWebResponse(http);

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public HttpStatusCode SendMDN(HttpRequest request, int timeoutMs, string signingCertFilename, string signingCertPassword)
        {

            try
            {
                string fileData = "This is an automated MDN";
                byte[] content = Encoding.ASCII.GetBytes(fileData);


                OrderDataService OrderDataServ = new OrderDataService();
                AS2CommunicationModel commDetails = OrderDataServ.GetAS2CommunicationDetials(request.Headers["AS2-From"]);

                //Initialise the request
                HttpWebRequest http = (HttpWebRequest)WebRequest.Create(commDetails.AS2MDNURL);
                string recipientPubCertFilename = commDetails.CertificateName;

                //Define the standard request objects
                http.Method = "POST";

                http.AllowAutoRedirect = true;

                http.KeepAlive = true;

                http.PreAuthenticate = false; //Means there will be two requests sent if Authentication required.
                http.SendChunked = false;

                //http.UserAgent = "PEERCORE AGENT";

                //These Headers are common to all transactions
                http.Headers.Add("Mime-Version", "1.0");
                http.Headers.Add("AS2-Version", "1.2");
                //http.Headers.Add("Date", DateTime.Now.ToString());

                http.Headers.Add("AS2-From", request.Headers["AS2-To"]);
                http.Headers.Add("AS2-To", request.Headers["AS2-From"]);
                http.Headers.Add("Subject", "PO MDN");
                http.Headers.Add("Message-Id", request.Headers["Message-Id"]!=null? request.Headers["Message-Id"]:"0");
                http.Timeout = timeoutMs;

                string contentType = "message/disposition-notification";

                bool encrypt = !string.IsNullOrEmpty(recipientPubCertFilename);
                bool sign = !string.IsNullOrEmpty(signingCertFilename);

                if (!sign && !encrypt)
                {
                    http.Headers.Add("Content-Transfer-Encoding", "binary");
                    http.Headers.Add("Content-Disposition", "inline");
                }
                if (sign)
                {
                    // Wrap the file data with a mime header
                    content = AS2MIMEUtilities.CreateMessage(contentType, "binary", "", content);

                    content = AS2MIMEUtilities.Sign(content, signingCertFilename, signingCertPassword, out contentType);

                    http.Headers.Add("EDIINT-Features", "multiple-attachments");

                }
                if (encrypt)
                {
                    string certificateFullPath = "";
                    if (string.IsNullOrEmpty(recipientPubCertFilename))
                    {
                        throw new ArgumentNullException(recipientPubCertFilename, "if encrytionAlgorithm is specified then recipientCertFilename must be specified");
                    }
                    else {
                        certificateFullPath = ConfigValues.CertificateFilePath + recipientPubCertFilename;
                    }

                    byte[] signedContentTypeHeader = System.Text.ASCIIEncoding.ASCII.GetBytes("Content-Type: " + contentType + Environment.NewLine);
                    byte[] contentWithContentTypeHeaderAdded = AS2MIMEUtilities.ConcatBytes(signedContentTypeHeader, content);

                    string ba2Str = System.Text.Encoding.Default.GetString(content);
                    string s1 = System.Text.Encoding.Default.GetString(signedContentTypeHeader);
                    string s2 = System.Text.Encoding.Default.GetString(contentWithContentTypeHeaderAdded);
                    content = AS2Encryption.EncryptMDN(contentWithContentTypeHeaderAdded, certificateFullPath, EncryptionAlgorithm.DES3);

                  //  contentType += "multipart/report; report-type=disposition-notification; boundary=\"fredi.boundary.mult.sig.mdn\"; charset=utf-8";
                }

                http.ContentType += contentType;
                http.ContentLength = content.Length;

                SendWebRequest(http, content);

                return HandleWebResponse(http);

            }
            catch (Exception ex)
            {
                Log.Error($"Exception during sending MDN :: {ex.Message}");
                throw;
            }
        }


        private static HttpStatusCode HandleWebResponse(HttpWebRequest http)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)http.GetResponse();

                response.Close();
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void SendWebRequest(HttpWebRequest http, byte[] fileData)
        {
            try
            {
                Stream oRequestStream = http.GetRequestStream();
                oRequestStream.Write(fileData, 0, fileData.Length);
                oRequestStream.Flush();
                oRequestStream.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}