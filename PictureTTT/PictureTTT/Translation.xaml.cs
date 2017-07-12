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
	public partial class Translation : ContentPage
	{
		public Translation ()
		{
            NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent();
		}
    }
}