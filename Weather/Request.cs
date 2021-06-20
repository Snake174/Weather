using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Weather
{
    class Request
    {
        private string URL;
        public string Status { get; set; }
        private List<string> urlParams = new List<string>();

        public Request(string url)
        {
            URL = url;
        }

        public void AddParam(string key, string value)
        {
            urlParams.Add($"{key}={value}");
        }

        public string Send()
        {
            if (urlParams.Count > 0)
            {
                URL += "?" + string.Join("&", urlParams);
            }

            WebRequest request = WebRequest.Create(URL);
            request.Method = "POST";

            string postData = "";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            Status = ((HttpWebResponse)response).StatusDescription;

            string responseFromServer = "";

            using (dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }

            response.Close();

            return responseFromServer;
        }
    }
}
