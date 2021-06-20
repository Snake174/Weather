using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Weather
{
    class Weather
    {
        JObject data = null;

        public Weather()
        {
            Request r = new Request("https://api.openweathermap.org/data/2.5/forecast");
            r.AddParam("id", "1498894");
            r.AddParam("appid", "56cd4cecbeb2a4cec8a1ff5d6173b10a");
            r.AddParam("units", "metric");
            r.AddParam("lang", "ru");

            try
            {
                data = JObject.Parse(r.Send());
            }
            catch (Exception e)
            {
                data = null;
                Console.WriteLine($"Ошибка: {e.Message}");
            }
        }

        // Масимальное давление и когда оно
        public KeyValuePair<int, string> GetMaxPressure()
        {
            if (data == null)
            {
                return new KeyValuePair<int, string>(0, "-");
            }

            int pressure = 0;
            string date = "";
            IList<JObject> list = ((JArray)data["list"]).ToObject<IList<JObject>>();

            foreach (JObject d in list)
            {
                int p = Convert.ToInt32(d["main"]["grnd_level"].ToString());

                if (p > pressure)
                {
                    pressure = p;
                    date = d["dt_txt"].ToString();
                }
            }

            DateTime dt = DateTime.Parse(date);

            return new KeyValuePair<int, string>(pressure, dt.ToString("dd.MM.yyyy hh:mm:ss"));
        }

        // День с минимальной разницей между ночной и утренней температурой
        public string GetMinTempSubDay()
        {
            if (data == null)
            {
                return "";
            }

            IList<JObject> list = ((JArray)data["list"]).ToObject<IList<JObject>>();
            Dictionary<string, float> sub = new Dictionary<string, float>();
            
            foreach (JObject d in list)
            {
                DateTime dt = DateTime.Parse(d["dt_txt"].ToString());
                string date = dt.ToString("dd.MM.yyyy");
                string time = dt.ToString("hh:mm:ss");

                if (!sub.ContainsKey(date))
                {
                    sub.Add(date, float.MaxValue);
                }

                if (time == "00:00:00")
                {
                    if (sub[date] == float.MaxValue)
                    {
                        sub[date] = (float)Convert.ToDouble(d["main"]["temp"], CultureInfo.InvariantCulture.NumberFormat);
                    }
                    else
                    {
                        sub[date] = Math.Abs(sub[date] - (float)Convert.ToDouble(d["main"]["temp"], CultureInfo.InvariantCulture.NumberFormat));
                    }
                }

                if (time == "06:00:00")
                {
                    if (sub[date] == float.MaxValue)
                    {
                        sub[date] = (float)Convert.ToDouble(d["main"]["temp"], CultureInfo.InvariantCulture.NumberFormat);
                    }
                    else
                    {
                        sub[date] = Math.Abs((float)Convert.ToDouble(d["main"]["temp"], CultureInfo.InvariantCulture.NumberFormat) - sub[date]);
                    }
                }
            }

            var sSub = from entry in sub orderby entry.Value ascending select entry;

            return sSub.First().Key;
        }
    }
}
