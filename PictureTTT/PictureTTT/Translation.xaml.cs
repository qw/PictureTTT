using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PictureTTT
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Translation : ContentPage, APIHandlingListener
	{
        private APIHandlingModel model = APIHandlingModel.Instance;

        public Translation ()
		{
            NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent();
            model.addListener(this);
        }

        public void update()
        {
            EnglishTextLabel.Text = model.OriginalLanguageJSONObject.ToString();
            ChineseTextLabel.Text = model.TranslatedText;
        }
    }
}