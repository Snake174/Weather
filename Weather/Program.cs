using System;
using System.Collections.Generic;

namespace Weather
{
    class Program
    {
        static void Main(string[] args)
        {
            Weather w = new Weather();
            KeyValuePair<int, string> p = w.GetMaxPressure();
            Console.WriteLine($"Максимальное давление: {p.Key} Когда: {p.Value}");
            Console.WriteLine($"День с минимальной разницей между ночной и утренней температурой: {w.GetMinTempSubDay()}");
            Console.ReadLine();
        }
    }
}
