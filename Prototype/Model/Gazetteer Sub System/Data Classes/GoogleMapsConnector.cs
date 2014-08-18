using Prototype.Model.Global_Container_Classes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;

namespace Prototype.Model.Gazetteer_Sub_System
{
    /// <summary>
    /// A class that uses the Google Maps mapping API
    /// </summary>
    public class GoogleMapsConnector : IAddressSearch
    {
        Random random; //a random number generator used to create dummy travel time for appliance

        /// <summary>
        /// Constructor
        /// </summary>
        public GoogleMapsConnector()
        {
            //instantiate the random number generator
            random = new Random();
        }

        /// <summary>
        /// Performs an address search using the provided keyword and county
        /// </summary>
        /// <param name="keyword">The keyword to search for addresses with</param>
        /// <param name="county">The County to filter results by</param>
        /// <returns>An array of objects, each representing a single matching address</returns>
        public Address[] FindAddress(string keyword, string county)
        {
            //build the url string that is used to query Google Maps
            string url = "https://maps.googleapis.com/maps/api/geocode/xml?address=";

            if (keyword != string.Empty)
                url += ", " + keyword;

            if (county != string.Empty)
                url += ", " + county;

            url += ", UK";

            //connect to Google and download the results via an XML file
            //taken from http://stackoverflow.com/questions/3175790/how-can-i-download-an-xml-file-using-c
            string xml = null;
            try
            {
                using (var webClient = new WebClient())
                    xml = webClient.DownloadString(url);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error retrieving data from Google:" + Environment.NewLine + ex.ToString());
            }


            //create an XML object with the xml data
            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(xml);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error parsing XML data from Google:" + Environment.NewLine + ex.ToString());
            }

            //parse all matching addresses from the XML document into a list of addresses
            List<Address> addresses = new List<Address>();
            //find all Location elements in the XML file.  Read specific address details and store them in the list
            try
            {
                foreach (XElement result in doc.Descendants("result"))
                {
                    int addressId = -1;
                    string buildingResult = null;
                    string numberResult = null;
                    string streetResult = null;
                    string townResult = null;
                    string postcodeResult = null;
                    string countyResult = null;
                    double longitudeResult = 0;
                    double latitudeResult = 0;

                    foreach (XElement addressComponent in result.Descendants("address_component"))
                    {
                        switch (addressComponent.Element("type").Value)
                        {
                            case "premise":
                                buildingResult = addressComponent.Element("long_name").Value;
                                break;
                            case "street_number":
                                numberResult = addressComponent.Element("long_name").Value;
                                break;
                            case "route":
                                streetResult = addressComponent.Element("long_name").Value;
                                break;
                            case "locality":
                                townResult = addressComponent.Element("long_name").Value;
                                break;
                            case "postal_code":
                                postcodeResult = addressComponent.Element("long_name").Value;
                                break;
                            case "administrative_area_level_2":
                                countyResult = addressComponent.Element("long_name").Value;
                                break;
                            default:
                                break;
                        }
                    }

                    foreach (XElement addressComponent in result.Descendants("geometry"))
                    {
                        longitudeResult = Convert.ToDouble(addressComponent.Element("location").Element("lng").Value);
                        latitudeResult = Convert.ToDouble(addressComponent.Element("location").Element("lat").Value);
                    }

                    addresses.Add(new Address(id: addressId,
                                              building: buildingResult,
                                              number: numberResult,
                                              street: streetResult,
                                              town: townResult,
                                              postcode: postcodeResult,
                                              county: countyResult,
                                              longitude: longitudeResult,
                                              latitude: latitudeResult
                                              ));
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error finding address details in XML file:" + Environment.NewLine + ex.ToString());
            }

            //return the list of addresses
            return addresses.ToArray();
        }

        /// <summary>
        /// Calculates the number of seconds it takes to travel from one location to another.
        /// Uses the Google Distance Matrix API:
        /// https://developers.google.com/maps/documentation/distancematrix/
        /// </summary>
        /// <param name="startLat">The Latidude of the starting location.</param>
        /// <param name="startLong">The Longitude of the starting location.</param>
        /// <param name="endLat">The Latidude of the finishing location.</param>
        /// <param name="endLong">The Longitude of the finishing location.</param>
        /// <returns>The number of seconds it takes to travel from one location to another</returns>
        public int CalculateRoute(double startLat, double startLong, double endLat, double endLong)
        {
            //build the url string that is used to query Google Maps
            string url = "http://maps.googleapis.com/maps/api/distancematrix/xml?" +
                          "origins=" +
                          startLat.ToString() + "," +
                          startLong.ToString() + "&" +
                          "destinations=" +
                          endLat.ToString() + "," +
                          endLong.ToString() + "&" +
                          "&sensor=false";

            //FOR DEBUGGING PURPOSES - displays the raw results from the url string that is used to query Google Maps in the default browser
            //Process.Start(url);


            //connect to Google and download the results via an XML file
            //taken from http://stackoverflow.com/questions/3175790/how-can-i-download-an-xml-file-using-c
            string xml = null;
            try
            {
                using (var webClient = new WebClient())
                    xml = webClient.DownloadString(url);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error retrieving data from Google:" + Environment.NewLine + ex.ToString());
            }

            //System.Windows.Forms.MessageBox.Show(xml);

            //create an XML object with the xml data
            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(xml);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error parsing XML data from Google:" + Environment.NewLine + ex.ToString());
            }

            //read the travel time calculated by Google from the xml data
            string duration = null;
            try
            {
                foreach (XElement element in doc.Descendants("duration"))
                    duration = element.Element("value").Value;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error finding travel time data in XML file:" + Environment.NewLine + ex.ToString());
            }

            //return the travel time
            try
            {
                return Convert.ToInt32(duration);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error converting travel time into integer:" + Environment.NewLine + ex.ToString());
                return -1;
            }

        }

        /// <summary>
        /// Simulates the calculation of travel time between one location and another.
        /// </summary>
        /// <returns>A number between 100 and 4000 that is taken to be the number of seconds taken to travel between two locations</returns>
        public int CalculateRouteDummy()
        {
            return random.Next(100, 4000);
        }
    }

}
