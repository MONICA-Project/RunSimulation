using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RunSimulation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Read settings from env
            //Read settings:
            string MQTTServerAddress = Environment.GetEnvironmentVariable("MQTTServerAddress");
            if (MQTTServerAddress == null || MQTTServerAddress == "")
            {
                System.Console.WriteLine("ERROR:Missing MQTTServerAddress env variable. Process Exit.");
            }
            else settings.MQTTServerAddress = MQTTServerAddress;

            string GOST = Environment.GetEnvironmentVariable("GOST");
            if (GOST == null || GOST == "")
            {
                System.Console.WriteLine("ERROR:Missing GOST env variable. Process Exit.");
            }
            else settings.GOST = GOST;

            string DatabasePath = Environment.GetEnvironmentVariable("DatabasePath");
            if (DatabasePath == null || DatabasePath == "")
            {
                System.Console.WriteLine("ERROR:Missing DatabasePath env variable. Process Exit.");
            }
            else settings.DatabasePath = DatabasePath;

            string SettingsFile = Environment.GetEnvironmentVariable("SettingsFile");
            if (SettingsFile == null || SettingsFile == "")
            {
                System.Console.WriteLine("ERROR:Missing SettingsFile env variable. Process Exit.");
            }
            else settings.SettingsFile = SettingsFile;



            List<DatastreamControl> dsc = new List<DatastreamControl>();
            ManagedMqttClient mlSend = (ManagedMqttClient)new MqttFactory().CreateManagedMqttClient();
            // Setup and start a managed MQTT client.
            var optionsSend = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(System.Guid.NewGuid().ToString())
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(120))
                    .WithCommunicationTimeout(TimeSpan.FromSeconds(60))
                    .WithTcpServer(settings.MQTTServerAddress)
                    .Build())
                .Build();

            mlSend.StartAsync(optionsSend).Wait();

            using (StreamReader r = new StreamReader(settings.SettingsFile))
            {
                string json = r.ReadToEnd();
                dynamic simulations = JsonConvert.DeserializeObject(json);
                foreach (var item in simulations.simulation)
                {
                    string datastreamID = "99999999999";
                    OGCServiceCatalogue.ManageDataStreams mds = new OGCServiceCatalogue.ManageDataStreams();
                    if (mds.FindOrCreateOGCDatastream(item.thing, item.sensor, item.observedProperties, item.datastream, ref datastreamID))
                    {
                        DatastreamControl sim = new DatastreamControl();
                        sim.Start(datastreamID, settings.DatabasePath + item.simulationFile, mlSend);
                        dsc.Add(sim);
                    }
                    Console.WriteLine("{0} {1}", item.temp, item.vcc);
                }

            }
          

            CreateWebHostBuilder(args).Build().Run();

            System.Console.ReadLine();

            foreach (DatastreamControl sim in dsc)
                sim.stopped=true;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
