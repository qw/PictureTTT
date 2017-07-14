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
	public partial class AzureTable : ContentPage, APIHandlingListener
	{
        private APIHandlingModel model = APIHandlingModel.Instance;
        private const string localImagePath = "PictureTTT.upload.png";
        private MediaFile file = null;

        public AzureTable()
		{
            NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent();
            model.addListener(this);
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
            uploadingBusy.IsRunning = true;

            if (file == null)
            {
                await model.uploadFromGallery(localImagePath);
                //await DisplayAlert("No Image!", "Please take a photo first", "OK");
                //return;
            }
            else
            {
                await model.uploadFromCamera(file);
            }

            jsonResponse.Text = model.contentString;
            uploadingBusy.IsRunning = false;
        }

        public async void update()
        {
            await DisplayAlert("Upload Complete!", "Upload Complete!", "OK");
        }
    }

}