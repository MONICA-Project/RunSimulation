using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Net.Http;
namespace OGCServiceCatalogue
{
    /// <summary>
    /// Manages creation of datastreams in OGS Sensorthings API
    /// </summary>
    public class ManageDataStreams
    {
        string baseUrl = "";
        string gostPrefix = "GOST";

            /// <summary>
            /// Using the parameters it will create the necessery part in 
            /// </summary>
            /// <param name="searchAndResult"></param>
            /// <returns>True if a datastream was found or created. False if it failed</returns>
        public bool FindOrCreateOGCDatastream(dynamic thing, dynamic sensor, dynamic observedProperty, dynamic datastream, ref string datastreamid)
        {
            
             baseUrl = RunSimulation.settings.GOST;
   
            string GOST_Prefix = Environment.GetEnvironmentVariable("GOST_Prefix");
            if (!(GOST_Prefix == null || GOST_Prefix == ""))
            {
                gostPrefix = GOST_Prefix;
            }

            bool success = false;
            string thingID = FindOrCreateThing(thing,ref  success);
            if (!success)
                return false;
            string sensorid = FindOrCreateSensor(sensor, ref success);
            if (!success)
                return false;
            string observedPropertyId = FindOrCreateObservedProperty(observedProperty, ref success);
            if (!success)
                return false;
            string streamId = FindOrCreateDatastream(datastream,thingID, observedPropertyId, sensorid, ref success);
            if (!success)
                return false;

            datastreamid = streamId;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalid"></param>
        /// <param name="description"></param>
        /// <param name="fixedLat"></param>
        /// <param name="fixedLon"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        string FindOrCreateThing(dynamic thing,ref bool success)
        {
            string retVal;
            success = false;
            bool foundThing = false;
            string JsonResult = "";
           
            //First search if it exists
            WebClient client = new WebClient();
            try
            {
                string url = baseUrl + "Things?$filter=name eq '" + thing.name + "'";
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
                client.Headers["Content-Type"] = "application/json";
                JsonResult = client.DownloadString(url);
                dynamic jsonRes = JValue.Parse(JsonResult);
                if(jsonRes.value.Count > 0)
                {
                    retVal = jsonRes.value[0]["@iot.id"];
                    success = true;
                    return retVal;
                }
                else
                    foundThing = false;
            }
            catch (WebException exception)
            {
              
                foundThing = false;
            }

          
            // Create Thing


            string payload = thing.ToString();


     
            client = new WebClient();
            try
            {
                string url = baseUrl + "Things";
                System.Console.WriteLine("Payload:" + payload);
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
                client.Headers["Content-Type"] = "application/json";
                JsonResult = client.UploadString(url, payload);
                dynamic jsonRes = JValue.Parse(JsonResult);
                retVal = jsonRes["@iot.id"];

            }
            catch (WebException exception)
            {
                System.Console.WriteLine("Create Thing failed:" + exception.Message);
                success = false;
                return "";
            }

            dynamic json = new JObject();
            json.name = thing.name;
            json.description = "Location of "+ thing.name;
            json.encodingType = "application/vnd.geo+json";

       
            dynamic loc = new JObject();
            loc.type = "Point";
            JArray coord = new JArray();
            JValue lon = new JValue(0.0);
            JValue lat = new JValue(0.0);
            coord.Add(lon);
            coord.Add(lat);

            loc.coordinates = coord;
           
            //Create Location
            json.location = loc;
            payload = json.ToString();
//            


            client = new WebClient();
            try
            {
                string url = baseUrl + "Things("+retVal+")/Locations";
                System.Console.WriteLine("Payload:" + payload);
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
                client.Headers["Content-Type"] = "application/json";
 
                JsonResult = client.UploadString(url, payload);
             
            }
            catch (WebException exception)
            {
                System.Console.WriteLine("Create Thing failed:" + exception.Message);
                success = false;
                return "";
            }



            success = true;
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SensorType"></param>
        /// <param name="UnitOfMeasurement"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        string FindOrCreateSensor(dynamic sensor, ref bool success)
        {
            string retVal;
            success = false;
            bool foundSensor = false;
            string JsonResult = "";

            //First search if it exists
            WebClient client = new WebClient();
            try
            {
                string url = baseUrl + "Sensors?$filter=name eq '" + sensor.name + "'";
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
           
                JsonResult = client.DownloadString(url);
                dynamic jsonRes = JValue.Parse(JsonResult);
                if (jsonRes.value.Count > 0)
                {
                    retVal = jsonRes.value[0]["@iot.id"];
                    success = true;
                    return retVal;
                }
                else
                    foundSensor = false;
            }
            catch (WebException exception)
            {

                foundSensor = false;
            }

            // Create Thing
            string payload = sensor.ToString();
                




            client = new WebClient();
            try
            {
                string url = baseUrl + "Sensors";
                System.Console.WriteLine("Payload:" + payload);
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
                client.Headers["Content-Type"] = "application/json";

                JsonResult = client.UploadString(url, payload);
                dynamic jsonRes = JValue.Parse(JsonResult);
                retVal = jsonRes["@iot.id"];
           
            }
            catch (WebException exception)
            {
                System.Console.WriteLine("Create Thing failed:" + exception.Message);
                success = false;
                return "";
            }

     



            success = true;
            return retVal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SensorType"></param>
        /// <param name="UnitOfMeasurement"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        string FindOrCreateObservedProperty(dynamic observedProperty, ref bool success)
        {
            string retVal;
            success = false;
            bool foundSensor = false;
            string JsonResult = "";

            //First search if it exists
            WebClient client = new WebClient();
            try
            {
                string url = baseUrl + "ObservedProperties?$filter=name eq '" + observedProperty.name + "'";
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
                client.Headers["Content-Type"] = "application/json";

                JsonResult = client.DownloadString(url);
                dynamic jsonRes = JValue.Parse(JsonResult);
                if (jsonRes.value.Count > 0)
                {
                    retVal = jsonRes.value[0]["@iot.id"];
                    success = true;
                    return retVal;
                }
                else
                    foundSensor = false;
            }
            catch (WebException exception)
            {

                foundSensor = false;
            }

            // Create Thing
            string payload = observedProperty.ToString();
//  

            client = new WebClient();
            try
            {
                string url = baseUrl + "ObservedProperties";
                System.Console.WriteLine("Payload:" + payload);
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
                client.Headers["Content-Type"] = "application/json";


                JsonResult = client.UploadString(url, payload);

                dynamic jsonRes = JValue.Parse(JsonResult);
                retVal = jsonRes["@iot.id"];

            }
            catch (WebException exception)
            {
                System.Console.WriteLine("Create Thing failed:" + exception.Message);
                success = false;
                return "";
            }





            success = true;
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExternalId"></param>
        /// <param name="Metadata"></param>
        /// <param name="SensorType"></param>
        /// <param name="UnitOfMeasurement"></param>
        /// <param name="thingid"></param>
        /// <param name="ObservedPropertyid"></param>
        /// <param name="Sensorid"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        string FindOrCreateDatastream(dynamic datastream, string thingid, string ObservedPropertyid, string Sensorid, ref bool success)
        {
            string retVal;
            success = false;
            bool foundDatastream = false;
            string JsonResult = "";

            //First search if it exists
            WebClient client = new WebClient();
            try
            {
              
                string url = baseUrl + "Datastreams?$filter=name eq '" + datastream.name + "'";
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
                client.Headers["Content-Type"] = "application/json";

                JsonResult = client.DownloadString(url);
                dynamic jsonRes = JValue.Parse(JsonResult);
                if (jsonRes.value.Count > 0)
                {
                    
                    retVal = jsonRes.value[0]["@iot.id"];
                    success = true;
                    return retVal;
                }
                else
                    foundDatastream = false;
            }
            catch (WebException exception)
            {

                foundDatastream = false;
            }

           


            dynamic thing = new JObject();
            thing["@iot.id"] = int.Parse(thingid);

            datastream.Thing = thing;

            dynamic ObservedProperty = new JObject();
            ObservedProperty["@iot.id"] = int.Parse(ObservedPropertyid);

            datastream.ObservedProperty = ObservedProperty;

            dynamic Sensor = new JObject();
            Sensor["@iot.id"] = int.Parse(Sensorid);

            datastream.Sensor = Sensor;
            // Create Thing
            string payload = datastream.ToString();
                

            client = new WebClient();
            try
            {
                string url = baseUrl + "Datastreams";
                System.Console.WriteLine("Payload:" + payload);
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Accept"] = "application/json";
                client.Headers["Content-Type"] = "application/json";

                JsonResult = client.UploadString(url, payload);
                dynamic jsonRes = JValue.Parse(JsonResult);
                retVal = jsonRes["@iot.id"];

            }
            catch (WebException exception)
            {
                System.Console.WriteLine("Create Thing failed:" + exception.Message);
                success = false;
                return "";
            }





            success = true;
            return retVal;
        }
    }
}
