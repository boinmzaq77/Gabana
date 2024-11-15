using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using Gabana.Droid.Tablet.Fragments.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Report_Dialog_Share : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Report_Dialog_Share NewInstance()
        {
            var frag = new Report_Dialog_Share { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.report_dialog_share, container, false);
            try
            {
                LinearLayout lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                CardView lnPDF = view.FindViewById<CardView>(Resource.Id.lnPDF);
                CardView lnEmail = view.FindViewById<CardView>(Resource.Id.lnEmail);
                CardView lnExport = view.FindViewById<CardView>(Resource.Id.lnExport);
                lnPDF.Click += LnPDF_Click;
                lnEmail.Click += LnEmail_Click;
                lnExport.Click += LnExport_Click;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void LnExport_Click(object sender, EventArgs e)
        {
            await Report_Fragment_ShowData.fragment_showdata.LnExport_Click();
        }

        private async void LnEmail_Click(object sender, EventArgs e)
        {
            await Report_Fragment_ShowData.fragment_showdata.LnEmail_Click();
        }

        private async void LnPDF_Click(object sender, EventArgs e)
        {
            await Report_Fragment_ShowData.fragment_showdata.LnPDF_Click();
            this.Dialog.Dismiss();
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }
    }
}