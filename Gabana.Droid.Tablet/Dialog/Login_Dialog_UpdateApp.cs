using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Gabana.Droid.Tablet.Dialog
{
    public class Login_Dialog_UpdateApp : AndroidX.Fragment.App.DialogFragment
    {
        Context context;
        Button btn_save;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }
        public static Login_Dialog_UpdateApp NewInstance()
        {
            var frag = new Login_Dialog_UpdateApp { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.login_dialog_updateapp, container, false);
            context = container.Context;


            btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
            btn_save.Click += BtnOK_Click;

            return view;
        }
        private void BtnOK_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.seniorsoft.Gabana3");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }
    }
}