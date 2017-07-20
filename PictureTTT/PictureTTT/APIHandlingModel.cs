using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Reflection;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Linq;

namespace PictureTTT
{
    class APIHandlingModel
    {
        private List<APIHandlingListener> listeners = new List<APIHandlingListener>() {};
        public string customVisionJSONString { get; set; }
        public CustomVisionJSONObject OriginalLanguageJSONObject { get; set; }
        public string TranslatedText { get; set; }

        private static APIHandlingModel instance;
        public static APIHandlingModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new APIHandlingModel();
                }

                return instance;
            }
        }

        //Subscription key and region
        private const string cognitiveServicesKey = "0f20ae1d128643d492ad1af03d13d616";
        private const string cognitiveServicesUriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/ocr";
        private const string translateAuthTokenUriBase = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private const string translateAPIKey = "fa50d39abc5642deac47370f4e85c7b0";
        private const string translateAPIUriBase = "https://api.microsofttranslator.com/V2/Http.svc/Translate";
        private const string languageFrom = "en";
        private const string languageTo = "zh-cn";

        private APIHandlingModel() { }

        //TODO
        public async Task uploadFromGallery(String imagePath)
        {
            //Converts local image to bytearray
            byte[] byteData = null;
            var assembly = this.GetType().GetTypeInfo().Assembly;
            string[] resourceNames = GetType().GetTypeInfo().Assembly.GetManifestResourceNames();
            using (Stream s = assembly.GetManifestResourceStream(imagePath))
            {
                long length = s.Length;
                byteData = new byte[(int)length];
                s.Read(byteData, 0, (int)length);
            }

             await MakeOCRRequest(byteData);
        }

        public async Task uploadFromCamera(MediaFile file)
        {
            if (file == null)
                return;

            //Converts MediaFile into byte array
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            byte[] byteData = binaryReader.ReadBytes((int)stream.Length);

            await MakeOCRRequest(byteData);
            file.Dispose();
        }
         
        // This method makes two API requests. One posts an image to Microsoft's Text Extraction Custom Vision, and the second posts the result to the Microsoft Translate Text API
        public async Task MakeOCRRequest(byte[] byteData)
        {
            // Text extraction request
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cognitiveServicesKey);

            // Assemble the URI for the REST API Call.
            string uri = cognitiveServicesUriBase + "?language=unk&detectOrientation=true";

            HttpResponseMessage response;

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // Header Content-Type "application/octet-stream" is local image file
                // "application/json" is image url
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                customVisionJSONString = await response.Content.ReadAsStringAsync();
            }

            // Extract english from JSON response
            OriginalLanguageJSONObject = JsonConvert.DeserializeObject<CustomVisionJSONObject>(customVisionJSONString);

            // Get auth token
            client = new HttpClient();
            string authToken = null;
            using (StringContent content = new StringContent(""))
            {
              content.Headers.Add("Ocp-Apim-Subscription-Key", translateAPIKey);
              HttpResponseMessage responseToken = await client.PostAsync(translateAuthTokenUriBase, content);
              authToken = await responseToken.Content.ReadAsStringAsync();
            }

            // Get Translate API request
            client = new HttpClient();
            string requestURL = translateAPIUriBase + "?appid=Bearer " + authToken + "&from=" + languageFrom + "&to=" + languageTo + "&text=" + OriginalLanguageJSONObject.ToString();
            response = await client.GetAsync(requestURL);
            String xmlResponse = await response.Content.ReadAsStringAsync();

            // Convert XML response to String
            XDocument doc = new XDocument();
            doc = XDocument.Parse(xmlResponse);
            xmlResponse = "";
            foreach (var item in doc.Descendants())
            {
                xmlResponse += item.Value;
            }
            TranslatedText = FormatTranslatedResponse(xmlResponse);

            updateListeners();
        }

        public void addListener(APIHandlingListener listener)
        {
            listeners.Add(listener);
        }

        private void updateListeners()
        {
            foreach (APIHandlingListener l in listeners)
            {
                l.update();
            }
        }
        
        // Currently replaces white space with new line
        private string FormatTranslatedResponse(string response)
        {
            List<char> stringList = new List<char>();
            stringList = response.ToCharArray().ToList();

            for (int i = 0; i < stringList.Count; i++)
            {
                if (stringList[i] == ' ')
                { 
                    stringList[i] = '\n';
                }
            }

            return String.Concat(stringList);
        }

    }
}
