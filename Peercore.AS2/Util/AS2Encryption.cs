using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;

namespace Peercore.AS2.Util
{
    public static class EncryptionAlgorithm
    {
        public static string DES3 = "3DES";
        public static string RC2 = "RC2";
    }
    public class AS2Encryption
    {
        private static ILog Log = LogManager.GetLogger("EncryptionAlgorithm");

        /// <summary>
        /// The receivers PFX ( A file that contains both Private and Public Keys, This key is Confidential)
        /// </summary>
        private static X509Certificate2 receivers_pfx = null;

        /// <summary>
        /// The receivers .der (This file contains the certificate with the receiver's public key)
        /// Hand this over to BidVest etc.. DER, CER, CERT, etc
        /// THIS IS FOR THE TEST CHANNEL
        /// </summary>
        private static X509Certificate2 receivers_pub_key = new X509Certificate2();

        /// <summary>
        /// Decrypts the specified encoded encrypted message. This is for decoding received messages
        /// </summary>
        /// <param name="encodedEncryptedMessage">The encoded encrypted message.</param>
        /// <returns></returns>
        internal static byte[] Decrypt(byte[] encodedEncryptedMessage)
        {
            try
            {
                if (receivers_pfx == null)
                    receivers_pfx = new X509Certificate2(ConfigValues.RecipientCertFilename, ConfigValues.RecipientCertPassword);

                var certificateCollection = new X509Certificate2Collection(receivers_pfx);

                EnvelopedCms envelopedCms = new EnvelopedCms();
                envelopedCms.Decode(encodedEncryptedMessage);
                envelopedCms.Decrypt(certificateCollection);
                string s = envelopedCms.ToString();
                return envelopedCms.Encode();
            }
            catch (Exception ex)
            {
                Log.Error($"Decrypt Exception occured : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Encodes the specified ar message. THIS IS FOR THE TEST CHANNEL
        /// </summary>
        /// <param name="arMessage">The ar message.</param>
        /// <param name="signerCert">The signer cert.</param>
        /// <param name="signerPassword">The signer password.</param>
        /// <returns></returns>
        internal static byte[] Encode(byte[] arMessage, string signerCert, string signerPassword)
        {
            byte[] signature = null;
            try
            {
                X509Certificate2 cert = new X509Certificate2(signerCert, signerPassword);

                ContentInfo contentInfo = new ContentInfo(arMessage);

                SignedCms signedCms = new SignedCms(contentInfo, true);
                CmsSigner cmsSigner = new CmsSigner(cert);

                signedCms.ComputeSignature(cmsSigner);
                signature = signedCms.Encode();
            }
            catch (Exception ex)
            {
                Log.Error($"Encoding Exception occured : {ex.Message}");
            }

            return signature;
        }


        /// <summary>
        /// Encodes the specified ar message. THIS IS FOR THE TEST CHANNEL
        /// </summary>
        /// <param name="arMessage">The ar message.</param>
        /// <param name="signerCert">The signer cert.</param>
        /// <param name="signerPassword">The signer password.</param>
        /// <returns></returns>
        internal static byte[] EncodeMDN(byte[] arMessage, string signerCert, string signerPassword)
        {
            byte[] signature = null;
            try
            {
                X509Certificate2 cert = new X509Certificate2(signerCert, signerPassword);

                ContentInfo contentInfo = new ContentInfo(arMessage);

                SignedCms signedCms = new SignedCms(contentInfo, true);
                CmsSigner cmsSigner = new CmsSigner(cert);

                signedCms.ComputeSignature(cmsSigner);
                signature = signedCms.Encode();
            }
            catch (Exception ex)
            {
                Log.Error($"Encoding Exception occured : {ex.Message}");
            }

            return signature;
        }

        /// <summary>
        /// Encrypts the specified message. THIS IS FOR THE TEST CHANNEL
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipientCert">The recipient cert.</param>
        /// <param name="encryptionAlgorithm">The encryption algorithm.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">encryptionAlgorithm argument must be 3DES or RC2 - value specified was:" + encryptionAlgorithm</exception>
        internal static byte[] Encrypt(byte[] message, string recipientCert, string encryptionAlgorithm)
        {
            byte[] encoded = null;
            try
            {
                if (!string.Equals(encryptionAlgorithm, EncryptionAlgorithm.DES3) && !string.Equals(encryptionAlgorithm, EncryptionAlgorithm.RC2))
                    throw new ArgumentException("encryptionAlgorithm argument must be 3DES or RC2 - value specified was:" + encryptionAlgorithm);

                ContentInfo contentInfo = new ContentInfo(message);

                EnvelopedCms envelopedCms = new EnvelopedCms(contentInfo,
                    new AlgorithmIdentifier(new System.Security.Cryptography.Oid(encryptionAlgorithm))); // should be 3DES or RC2

                // This key exist during testing only
                receivers_pub_key.Import(ConfigValues.RecipientPubCertFilename);
                CmsRecipient recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, receivers_pub_key);

                envelopedCms.Encrypt(recipient);

                encoded = envelopedCms.Encode();
            }
            catch (Exception ex)
            {
                Log.Error($"Encrypting Exception occured : {ex.Message}");
            }

            return encoded;
        }

        /// <summary>
        /// Encrypts the specified message. THIS IS FOR THE TEST CHANNEL
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipientCert">The recipient cert.</param>
        /// <param name="encryptionAlgorithm">The encryption algorithm.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">encryptionAlgorithm argument must be 3DES or RC2 - value specified was:" + encryptionAlgorithm</exception>
        internal static byte[] EncryptMDN(byte[] message, string recipientCert, string encryptionAlgorithm)
        {
            byte[] encoded = null;
            try
            {
                if (!string.Equals(encryptionAlgorithm, EncryptionAlgorithm.DES3) && !string.Equals(encryptionAlgorithm, EncryptionAlgorithm.RC2))
                    throw new ArgumentException("encryptionAlgorithm argument must be 3DES or RC2 - value specified was:" + encryptionAlgorithm);

                ContentInfo contentInfo = new ContentInfo(message);

                EnvelopedCms envelopedCms = new EnvelopedCms(contentInfo,
                    new AlgorithmIdentifier(new System.Security.Cryptography.Oid(encryptionAlgorithm))); // should be 3DES or RC2

                // This key exist during testing only
                receivers_pub_key.Import(recipientCert);
                CmsRecipient recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, receivers_pub_key);

                envelopedCms.Encrypt(recipient);

                encoded = envelopedCms.Encode();
            }
            catch (Exception ex)
            {
                Log.Error($"Encrypting Exception occured : {ex.Message}");
            }

            return encoded;
        }

        /// <summary>
        /// Validates the signature.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        internal static bool validateSignature(string signature)
        {
            X509Certificate2 cert = new X509Certificate2(Encoding.ASCII.GetBytes(signature));

            if (cert.Verify() == false)
            {
                Log.Debug($"Signature validation failed.");
                return false;
            }

            Log.Debug($"Signature validation successful.");
            return true;
        }
    }
}