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
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AzureTable : ContentPage
	{
        //Subscription key and region
        const string subscriptionKey = "0f20ae1d128643d492ad1af03d13d616";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/ocr";
        private MediaFile file = null;

        public AzureTable()
		{
            NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent();
            image.Source = "test.jpg";
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            //Check if camera is available
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            //Take photo
            file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            //Display the image
            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
               return file.GetStream();
            });

            //file.Dispose();
        }

        //Uploads to Custom Vision API, gets JSON response
        //Display response in Translation page
        private async void uploadImage(object sender, EventArgs e)
        {
            //Only if photo is taken
            if (file == null)
            {
                uploadingBusy.IsRunning = true;
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters.
                string requestParameters = "language=unk&detectOrientation=true";

                // Assemble the URI for the REST API Call.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                //converts local image to bytearray
                byte[] byteData = null;
                try
                {
                    var assembly = this.GetType().GetTypeInfo().Assembly;
                    string[] resourceNames = GetType().GetTypeInfo().Assembly.GetManifestResourceNames();
                    using (Stream s = assembly.GetManifestResourceStream("PictureTTT.upload.png"))
                    {
                        long length = s.Length;
                        byteData = new byte[(int)length];
                        s.Read(byteData, 0, (int)length);
                    }
                } catch (Exception error)
                {
                    await DisplayAlert("!", "!", "OK");
                    System.Diagnostics.Debug.WriteLine("#######: "+ error.Message);
                }

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                    {
                        // Header Content-Type "application/octet-stream" is local image file
                        // "application/json" is image url
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                        // Execute the REST API call.
                        response = await client.PostAsync(uri, content);

                        // Get the JSON response.
                        string contentString = await response.Content.ReadAsStringAsync();
                        jsonResponse.Text = contentString;

                    }

                uploadingBusy.IsRunning = false;

                //   await DisplayAlert("No Image!", "Please take a photo first", "OK");
                //return;
            }
            else
            {
                await MakeOCRRequest(file);
                file.Dispose();
            }
        }

        private static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        private async Task MakeOCRRequest(MediaFile file)
        {
            if (file == null)
                return;

            uploadingBusy.IsRunning = true;
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters.
            string requestParameters = "language=unk&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(file);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // Header Content-Type "application/octet-stream" is local image file
                // "application/json" is image url
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                jsonResponse.Text = contentString;

            }

            uploadingBusy.IsRunning = false;
        }


    }

}