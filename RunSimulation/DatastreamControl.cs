using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Net;
using System.IO;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Options;
using System.Threading;

namespace RunSimulation
{
    public class DatastreamControl
    {
        public bool stopped = false;
        public bool really_stopped = false;
        string simulationDB = "";
        string datastream = "";
        static MQTTnet.Extensions.ManagedClient.ManagedMqttClient mlSend = null;
        public Thread workThread;

        public void Start(string datastream, string simulationFile, ManagedMqttClient ml)
        {
             this.datastream = datastream;
            //Register datastream
            simulationDB = simulationFile;
            mlSend = ml;
            ThreadStart myThreadStart = new ThreadStart(this.init);
            workThread = new Thread(myThreadStart);
            workThread.Start();
        }
        public void init()
        {
            DoIt();
        }

        public bool DoIt()
        {
            while (true && !stopped)
            {
                using (var connection = new SqliteConnection("Data Source=" + simulationDB + ";Cache=Shared"))
                {
                    connection.Open();      // open connection

                    string sqlquery = "select * from observations order by id asc ";        // select all rows and sort them by ascending id
                    DateTime lastValue = DateTime.Now;
                    SqliteCommand executeCommand = new SqliteCommand(sqlquery, connection); // prepare query
                    try
                    {
                        var reader = executeCommand.ExecuteReader();                        // execute query
                        int i = 0;
                        while (reader.Read() && !stopped)
                        {
                            string jsonStr = reader["data"].ToString();
                            dynamic json = JValue.Parse(jsonStr);
                            DateTime newValue = json.phenomenonTime;
                            TimeSpan ts = newValue - lastValue;
                            if (ts.TotalMilliseconds > 100)
                                System.Threading.Thread.Sleep((int)ts.TotalMilliseconds - 100);
                            //System.Threading.Thread.Sleep(100);
                            lastValue = newValue;
                            json.phenomenonTime = (DateTime.Now).ToString("O");
                            json.resultTime = (DateTime.Now).ToString("O");
                            try
                            {
                                if (json.result.response != null)
                                    if (json.result.response.value != null)
                                        if (json.result.response.value[0] != null)
                                            if (json.result.response.value[0].endTime != null)
                                            {
                                                DateTime end = json.result.response.value[0].endTime;
                                                json.result.response.value[0].endTime = (DateTime.Now).ToString("O");
                                                if (json.result.response.value[0].startTime != null)
                                                {
                                                    DateTime start = json.result.response.value[0].startTime;
                                                    TimeSpan diff = end - start;
                                                    json.result.response.value[0].startTime = (DateTime.Now.AddMilliseconds(-1 * diff.TotalMilliseconds));
                                                }

                                            }
                            }
                            catch { }
                            try
                            {
                                if (json.result.timeStamp != null)
                                    json.result.timeStamp = (DateTime.Now).ToString("O");
                            }
                            catch { }
                            try
                            {
                                if (json.result.endtime != null)
                                {
                                    DateTime end = json.result.endtime;
                                    json.result.endtime = (DateTime.Now).ToString("O");
                                    if (json.result.startime != null)
                                    {
                                        DateTime start = json.result.startime;
                                        TimeSpan diff = end - start;
                                        json.result.startime = (DateTime.Now.AddMilliseconds(-1 * diff.TotalMilliseconds));
                                    }

                                }
                            }
                            catch { }

                           string mqttMess = json.ToString();
                            PublishMessage(datastream, mqttMess);

                        }
                    }
                    catch (Exception e)
                    { }
                }
                really_stopped = true;
            }
            return true;
        }
        public void PublishMessage(string datastream, string payload)
        {
            try
            {

                //PublishAsync("GOST/Datastreams(" + datastream + ")/Observations", payload);

                var message = new MQTTnet.MqttApplicationMessageBuilder()
                                 .WithTopic("GOST/Datastreams(" + datastream + ")/Observations")
                                 .WithPayload(payload)
                                 .WithAtMostOnceQoS()
                                 .WithRetainFlag(false)
                                 .Build();
                List<MQTTnet.MqttApplicationMessage> mqttmess = new List<MQTTnet.MqttApplicationMessage>();
                mqttmess.Add(message);
                mlSend.PublishAsync(mqttmess).Wait();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Exception:" + e.Message + " " + e.Source + " " + e.StackTrace);
            }

        }
        public static async Task PublishAsync(string topic, string payload, bool retainFlag = true, int qos = 1) =>

  await mlSend.PublishAsync(new MqttApplicationMessageBuilder()

    .WithTopic(topic)

    .WithPayload(payload)

    .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)

    .WithRetainFlag(retainFlag)

    .Build());
    }
}
