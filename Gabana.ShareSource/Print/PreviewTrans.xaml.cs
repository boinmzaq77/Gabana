using Gabana.ShareSource.Abstracts;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gabana.ShareSource.Print
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreviewTrans : ContentView
    {
        public PreviewTrans()
        {
            //InitializeComponent();
        }

        private void PersonListName_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {

        }
    }
}
