using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace PictureTTT
{
    public partial class App : Application
    {
        private APIHandlingModel model;

        public App()
        {
            InitializeComponent();
            MainPage = new PictureTTT.MainPage();
            model = APIHandlingModel.Instance;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            this.SyncWithDatabase();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async void SyncWithDatabase()
        {
            await model.GetTable();
        }
    }

}
