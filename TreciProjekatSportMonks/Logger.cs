using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreciProjekatSportMonks
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now}: {message}");
            // Ako želite da logujete u fajl
            // File.AppendAllText("log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

        public static void LogError(Exception ex)
        {
            Console.WriteLine($"{DateTime.Now}: ERROR - {ex.Message}");
            // Ako želite da logujete u fajl
            // File.AppendAllText("log.txt", $"{DateTime.Now}: ERROR - {ex.Message}{Environment.NewLine}");
        }
    }
}
