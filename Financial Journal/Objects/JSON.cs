using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Net;
using System.IO;
using Twilio.Rest.Api.V2010.Account;
using Twilio;
using Twilio.Types;


namespace Financial_Journal
{
    public enum CurrencyBase
    {
        CAD,
        USD
    }

    class JSON
    {

        public static string GET(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
            
        }

        public static void POST(string url, string jsonContent)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent);

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            long length = 0;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;
                }
            }
            catch (WebException ex)
            {
                // Log exception and throw as for GET example above
            }
        }

        #region Weather API

        public static double FarToCel(double f)
        {
            double c = 5.0 / 9.0 * (f - 32);

            return Math.Round(c, 1);
        }

        public static string ParseWeatherParameter(string line, string parameter)
        {
            int parameterIndex = line.IndexOf(parameter);

            string returnLine = "";
            int index = 0;

            try
            {
                while (!returnLine.Contains(",") && !returnLine.Contains("}"))
                {
                    returnLine += line[parameterIndex + parameter.Length + 2 + index++];
                }
            }
            catch
            {
                return "";
            }

            return returnLine.Substring(0, returnLine.Length - 1);
        }

        public static string Get_Weather_String(DateTime Date)
        {
            string API_Key = "6c245845dbc379864f39c1305786aaae"; //primary
            //string API_Key = "ca3b6d029c889b3520bbfc8252ea74d2"; //secondary
            //string API_Key = "6b29192476822a0c9e66d340c530a788"; //tertiary
            string Longitude = "-79.3832";
            string Latitude = "43.6532";
            string Exclusions = "?exclude=currently,hourly,minutely,alerts,flags";
            int Year = Date.Year;
            int Month = Date.Month;
            int Day = Date.Day;
            string Time = Year.ToString("D2") + "-" + Month.ToString("D2") + "-" + Day.ToString("D2") + "T12:00:00";

            return JSON.GET("https://api.darksky.net/forecast/" + API_Key + "/" + Latitude + "," + Longitude + "," + Time + "" + (Exclusions.Length > 9 ? Exclusions : "")).Split(new string[] { "\"daily\"" }, StringSplitOptions.None)[1];
        }

        public static Image Get_Weather_Icon(string weatherIconString)
        {
            if (weatherIconString.Contains("clear"))
            {
                return global::Financial_Journal.Properties.Resources.clear;
            }
            else if (weatherIconString.Contains("partly"))
            {
                return global::Financial_Journal.Properties.Resources.partly;
            }
            else if (weatherIconString.Contains("cloudy"))
            {
                return global::Financial_Journal.Properties.Resources.cloudy;
            }
            else if (weatherIconString.Contains("rain"))
            {
                return global::Financial_Journal.Properties.Resources.rain2;
            }
            else if (weatherIconString.Contains("wind"))
            {
                return global::Financial_Journal.Properties.Resources.wind;
            }
            else if (weatherIconString.Contains("snow"))
            {
                return global::Financial_Journal.Properties.Resources.snow;
            }
            else if (weatherIconString.Contains("sleet"))
            {
                return global::Financial_Journal.Properties.Resources.sleet;
            }
            else if (weatherIconString.Contains("fog"))
            {
                return global::Financial_Journal.Properties.Resources.fog;
            }

            return global::Financial_Journal.Properties.Resources.clear;
        }

        public static string Get_Weather_String(string weatherIconString)
        {
            if (weatherIconString.Contains("clear"))
            {
                return "Clear Skies";
            }
            else if (weatherIconString.Contains("partly"))
            {
                return "Partly Cloudy Skies";
            }
            else if (weatherIconString.Contains("cloudy"))
            {
                return "Cloudy Skies";
            }
            else if (weatherIconString.Contains("rain"))
            {
                return "Rainy Day";
            }
            else if (weatherIconString.Contains("wind"))
            {
                return "Windy Day";
            }
            else if (weatherIconString.Contains("snow"))
            {
                return "Snowy Day";
            }
            else if (weatherIconString.Contains("sleet"))
            {
                return "Wet Snow Day";
            }
            else if (weatherIconString.Contains("fog"))
            {
                return "Foggy Skies";
            }

            return "Clear Skies";
        }

#endregion

        #region Currency API

        private static Dictionary<string, double> currencyDictionary = new Dictionary<string, double>();

        private static readonly List<string> currencyExclusionList = new List<string>() {"BGN", "BRL", "CHF", "CZKx", "DKK", "HRK", "HUF", "IDR", "ILS", 
            "INR", "MXN", "MYR", "NOK", "NZDx", "PHP", "PLNx", "RON", "RUB", "SEK", "SGD", "THB", "TRY", "ZAR", };

        public static void PopulateCurrencyDict(CurrencyBase cB = CurrencyBase.CAD)
        {
            string baseStr = "CAD";
            switch (cB)
            {
                case CurrencyBase.CAD:
                    baseStr = "CAD";
                    break;
                case CurrencyBase.USD:
                    baseStr = "USD";
                    break;
            }

            try
            {
                Diagnostics.WriteLine("Getting currency data!");
                string CurrencyAPIStr = JSON.GET("http://api.fixer.io/latest?base=" + baseStr);
                Diagnostics.WriteLine("Done getting currency data!");

                CurrencyAPIStr = CurrencyAPIStr.Substring(CurrencyAPIStr.IndexOf("\"rates\":") + 9, CurrencyAPIStr.Substring(CurrencyAPIStr.IndexOf("\"rates\":") + 9).Length - 2);

                string[] lines = CurrencyAPIStr.Split(new string[] {","} , StringSplitOptions.None);

                foreach (string currLine in lines)
                {
                    string[] curr = currLine.Split(new string[] { ":" }, StringSplitOptions.None);

                    string currencyName = curr[0].Substring(1, 3);
                    if (!currencyExclusionList.Contains(currencyName))
                        currencyDictionary.Add(currencyName, Convert.ToDouble(curr[1]));
                }
            }
            catch
            {
                // Cannot reach currency API
                Diagnostics.WriteLine("Error: Cannot reach currency API");
            }
        }

        public static Dictionary<string, double> GetCurrencyValues()
        {
            return currencyDictionary;
        }

        public static double GetCurrency(string currency)
        {
            if (currencyDictionary.ContainsKey(currency))
            {
                return currencyDictionary[currency];
            }
            return -1;
        }

        #endregion

        #region SMS API

        public static void SendSMS(string bodystr)
        {
            // Set our Account SID and Auth Token
            const string accountSid = "ACa0bd5223544e2489accd753e00336783";
            const string authToken = "f3735f258df488a2839ebeaf4f652203";
            //const string twilioApiKey = "SK09ad7c65e1f8f7c3f2ed97cf0b489f97";
            //const string twilioApiSecret = "0XeyKt5wVrLweWpEPVQSMAY4a6rzIDCc";

            // Initialize the Twilio client
            TwilioClient.Init(accountSid, authToken);


            // Iterate over all our friends
            // Send a new outgoing SMS by POSTing to the Messages resource
            MessageResource.Create(
                from: new PhoneNumber("16479314400"), // From number, must be an SMS-enabled Twilio number
                to: new PhoneNumber("16476995560"), // To number, if using Sandbox see note above
                // Message content
                body: bodystr);

            //Diagnostics.WriteLine("Sent message to {person.Value}");
        }

        #endregion

        #region Quotes API

        public static string GetQuotesAPI()
        {
            Diagnostics.WriteLine("Getting Quote API...");
            string quoteAPIURL = "http://api.forismatic.com/api/1.0/?method=getQuote&key=457653&format=xml&lang=en";

            string QuoteAPIStr = JSON.GET(quoteAPIURL);
            Diagnostics.WriteLine("Done getting Quote API!");

            return GetTextBetween(QuoteAPIStr, "<quoteText>", "</quoteText>");

        }
        #endregion

        public static string GetTextBetween(string fullStr, string frontRef, string backRef)
        {
            return fullStr.Substring(fullStr.IndexOf(frontRef) + frontRef.Length,
                fullStr.IndexOf(backRef) - fullStr.IndexOf(frontRef) - frontRef.Length);
        }
    }
}
