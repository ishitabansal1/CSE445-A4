using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.Text;

namespace ConsoleApp1
{
    public class Submission
    {
        // Replace these with your real GitHub Pages URLs
        public static string xmlURL = "https://ishitabansal1.github.io/CSE445-A4/blob/mainNationalParks.xml";
public static string xmlErrorURL = "https://github.com/ishitabansal1/CSE445-A4/blob/main/NationalParksErrors.xml";
public static string xsdURL = "https://github.com/ishitabansal1/CSE445-A4/blob/main/NationalParks.xsd";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            StringBuilder errors = new StringBuilder();

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags =
                    XmlSchemaValidationFlags.ReportValidationWarnings |
                    XmlSchemaValidationFlags.ProcessInlineSchema |
                    XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.Schemas.Add(null, xsdUrl);

                settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e)
                {
                    if (errors.Length > 0)
                    {
                        errors.Append("\n");
                    }

                    if (e.Exception != null)
                    {
                        errors.Append("Line " + e.Exception.LineNumber +
                                      ", Position " + e.Exception.LinePosition +
                                      ": " + e.Message);
                    }
                    else
                    {
                        errors.Append(e.Message);
                    }
                };

                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read())
                    {
                        // Reading triggers validation
                    }
                }

                if (errors.Length == 0)
                {
                    return "No errors are found";
                }

                return errors.ToString();
            }
            catch (XmlSchemaValidationException ex)
            {
                return "Line " + ex.LineNumber + ", Position " + ex.LinePosition + ": " + ex.Message;
            }
            catch (XmlException ex)
            {
                return "Line " + ex.LineNumber + ", Position " + ex.LinePosition + ": " + ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Q2.2
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
                return jsonText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Helper method to download content from URL
        private static string DownloadContent(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}
