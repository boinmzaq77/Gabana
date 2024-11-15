﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Alert_Dialog_PaymentQrCash : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Alert_Dialog_PaymentQrCash NewInstance()
        {
            var frag = new Alert_Dialog_PaymentQrCash { Arguments = new Bundle() };
            return frag;
        }
        Button btnOK;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.alert_dialog_paymentqecash, container, false);
            try
            {
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                btnOK.Click += BtnOK_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
           this.Dialog.Dismiss();
        }
    }
}