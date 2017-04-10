using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Web;
using System.Globalization;

namespace PaypalAPI
{   
    public class NVPAPICaller
    {
        private const string ACCT = "ACCT";
        private string APIPassword = "API Password";
        private string APISignature = "API Signature";
        public string APIUsername = "API Email";
        private string BNCode = "API Encode";
        private const bool bSandbox = true;
        private string cancelURL = ("Cancel URL");
        private const string CVV2 = "CVV2";
        private string pendpointurl = "API Link";
        private const string PWD = "PWD";
        private string returnURL = ("Pay korar por url");
        private static readonly string[] SECURED_NVPS = new string[] { "ACCT", "CVV2", "SIGNATURE", "PWD" };
        private const string SIGNATURE = "SIGNATURE";
        private string Subject = "";
        private const int Timeout = 0x2710;

        private string buildCredentialsNVPString()
        {
            NVPCodec codec = new NVPCodec();
            if (!IsEmpty(this.APIUsername))
            {
                codec["USER"] = this.APIUsername;
            }
            if (!IsEmpty(this.APIPassword))
            {
                codec["PWD"] = this.APIPassword;
            }
            if (!IsEmpty(this.APISignature))
            {
                codec["SIGNATURE"] = this.APISignature;
            }
            if (!IsEmpty(this.Subject))
            {
                codec["SUBJECT"] = this.Subject;
            }
            codec["VERSION"] = "84.0";
            return codec.Encode();
        }

        public bool ConfirmPayment(string finalPaymentAmount, string token, string PayerId, string currency, ref NVPCodec decoder, ref string retMsg)
        {
            string nvpRequest = new NVPCodec { 
                ["METHOD"] = "DoExpressCheckoutPayment",
                ["TOKEN"] = token,
                ["PAYMENTACTION"] = "Sale",
                ["PAYERID"] = PayerId,
                ["AMT"] = finalPaymentAmount,
                ["CURRENCYCODE"] = currency
            }.Encode();
            string nvpstring = this.HttpCall(nvpRequest);
            decoder = new NVPCodec();
            decoder.Decode(nvpstring);
            string str3 = decoder["ACK"].ToLower();
            if ((str3 != null) && ((str3 == "success") || (str3 == "successwithwarning")))
            {
                return true;
            }
            retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&Desc=" + decoder["L_SHORTMESSAGE0"] + "&Desc2=" + decoder["L_LONGMESSAGE0"];
            return false;
        }

        public bool ExpressCheckout(string name, string description, string price, string quantity, string currency, ref string token, ref string retMsg)
        {
            bool flag;
            try
            {
                string str = "www.paypal.com";
                NVPCodec codec = new NVPCodec {
                    ["METHOD"] = "SetExpressCheckout",
                    ["RETURNURL"] = this.returnURL,
                    ["CANCELURL"] = this.cancelURL
                };
                double num = Convert.ToDouble(quantity, CultureInfo.InvariantCulture);
                double num2 = Convert.ToDouble(price, CultureInfo.InvariantCulture);
                double num3 = num * num2;
                codec["L_PAYMENTREQUEST_0_NAME0"] = name;
                codec["L_PAYMENTREQUEST_0_DESC0"] = description;
                codec["L_PAYMENTREQUEST_0_AMT0"] = price;
                codec["L_PAYMENTREQUEST_0_QTY0"] = quantity;
                codec["PAYMENTREQUEST_0_AMT"] = num3.ToString();
                codec["PAYMENTREQUEST_0_ITEMAMT"] = num3.ToString();
                codec["PAYMENTREQUEST_0_PAYMENTACTION"] = "SALE";
                codec["PAYMENTREQUEST_0_CURRENCYCODE"] = currency;
                string nvpRequest = codec.Encode();
                string nvpstring = this.HttpCall(nvpRequest);
                NVPCodec codec2 = new NVPCodec();
                codec2.Decode(nvpstring);
                string str4 = codec2["ACK"].ToLower();
                if ((str4 != null) && ((str4 == "success") || (str4 == "successwithwarning")))
                {
                    token = codec2["TOKEN"];
                    string str5 = "https://" + str + "/cgi-bin/webscr?cmd=_express-checkout&token=" + token + "&useraction=COMMIT";
                    retMsg = str5;
                    return true;
                }
                retMsg = "ErrorCode=" + codec2["L_ERRORCODE0"] + "&Desc=" + codec2["L_SHORTMESSAGE0"] + "&Desc2=" + codec2["L_LONGMESSAGE0"];
                flag = false;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return flag;
        }

        public string GetCurrenUserDetails(string ServiceId, string HostelAdminId) => 
            (ServiceId + "_" + HostelAdminId);

        public bool GetDetails(string token, ref NVPCodec decoder, ref string retMsg)
        {
            string nvpRequest = new NVPCodec { 
                ["METHOD"] = "GetExpressCheckoutDetails",
                ["TOKEN"] = token
            }.Encode();
            string nvpstring = this.HttpCall(nvpRequest);
            decoder = new NVPCodec();
            decoder.Decode(nvpstring);
            string str3 = decoder["ACK"].ToLower();
            if ((str3 != null) && ((str3 == "success") || (str3 == "successwithwarning")))
            {
                return true;
            }
            retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&Desc=" + decoder["L_SHORTMESSAGE0"] + "&Desc2=" + decoder["L_LONGMESSAGE0"];
            return false;
        }

        public string HttpCall(string NvpRequest)
        {
            string pendpointurl = this.pendpointurl;
            string str2 = (NvpRequest + "&" + this.buildCredentialsNVPString()) + "&BUTTONSOURCE=" + HttpUtility.UrlEncode(this.BNCode);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(pendpointurl);
            request.Timeout = 0x2710;
            request.Method = "POST";
            request.ContentLength = str2.Length;
            try
            {
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(str2);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static bool IsEmpty(string s) => 
            ((s == null) || (s.Trim() == string.Empty));

        public void SetCredentials(string Userid, string Pwd, string Signature)
        {
            this.APIUsername = Userid;
            this.APIPassword = Pwd;
            this.APISignature = Signature;
        }
    }
}

