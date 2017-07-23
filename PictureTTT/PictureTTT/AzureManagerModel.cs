using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PictureTTT.JSONObjects;
using Microsoft.WindowsAzure.MobileServices;

namespace PictureTTT
{
    public class AzureManagerModel
    {
        private MobileServiceClient client;
        private IMobileServiceTable<PictureTTTTable> pictureTTTTable;
        //private APIHandlingModel model = APIHandlingModel.Instance;
        public PictureTTTTable databaseJSONObject { get; set; }

        private static AzureManagerModel instance;
        public static AzureManagerModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManagerModel();
                }

                return instance;
            }
        }

        private AzureManagerModel()
        {
            client = new MobileServiceClient("https://picturettt.azurewebsites.net");
            pictureTTTTable = client.GetTable<PictureTTTTable>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public async Task GetTable()
        {
            List<PictureTTTTable> list = await pictureTTTTable.ToListAsync();
            databaseJSONObject = list[0]; 
            //databaseJSONObject = await pictureTTTTable.LookupAsync("695adb78-4337-482a-b04d-db893b6be7e3");
        }

        public async Task UpdateText(PictureTTTTable databaseJSONObject)
        {
            if (databaseJSONObject != null)
                await pictureTTTTable.UpdateAsync(databaseJSONObject);
        }
    }
}
