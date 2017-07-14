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

namespace PictureTTT
{
    class APIHandlingModel
    {
        private List<APIHandlingListener> listeners = new List<APIHandlingListener>() { };

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
        const string subscriptionKey = "0f20ae1d128643d492ad1af03d13d616";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/ocr";

        public string contentString { get; set; }

        private APIHandlingModel() { }

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
         
        public async Task MakeOCRRequest(byte[] byteData)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters.
            string requestParameters = "language=unk&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // Header Content-Type "application/octet-stream" is local image file
                // "application/json" is image url
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                contentString = await response.Content.ReadAsStringAsync();
            }

            //extract english from JSON response
            //do API call to Translate service
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

    }
}
