using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PictureTTT.JSONObjects;
using Microsoft.WindowsAzure.MobileServices;

namespace PictureTTT
{
    public class AzureManagerModel : APIHandlingListener
    {
        private MobileServiceClient client;
        private IMobileServiceTable<DatabaseJSONObject> PictureTTTTable;
        private APIHandlingModel model;
        private DatabaseJSONObject databaseJSONObject;

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
            client = new MobileServiceClient("https://picturettt.azurewebsites.net/");
            PictureTTTTable = client.GetTable<DatabaseJSONObject>();
            model = APIHandlingModel.Instance;
            model.addListener(this);
            GetTable();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public async void GetTable()
        {
            List<DatabaseJSONObject> list = await PictureTTTTable.ToListAsync();
            databaseJSONObject = list[0];
        }

        public async Task UpdateText()
        {
            model.databaseJSONObject.From = model.languageFrom;
            model.databaseJSONObject.To = model.languageTo;
            model.databaseJSONObject.OriginalText = model.OriginalText;
            model.databaseJSONObject.TranslatedText = model.TranslatedText;

            await PictureTTTTable.UpdateAsync(model.databaseJSONObject);
        }

        public async void update()
        {
            await UpdateText();
        }
    }
}
