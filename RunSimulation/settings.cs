using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RunSimulation
{
    public static class settings
    {
        public static string MQTTServerAddress = "127.0.0.1";
        public static string GOST = "http://127.0.0.1:8090/v1.0/";
        public static string DatabasePath = ".";
        public static string SettingsFile = ".\\SimulatorControl3.json";
    }
}
