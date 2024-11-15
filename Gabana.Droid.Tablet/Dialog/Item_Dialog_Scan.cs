using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using EDMTDev.ZXingXamarinAndroid;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Java.Lang.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Item_Dialog_Scan : AndroidX.Fragment.App.DialogFragment, IPermissionListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Item_Dialog_Scan NewInstance(string activityName)
        {
            var frag = new Item_Dialog_Scan { Arguments = new Bundle() };
            ActivityName = activityName;
            return frag;
        }

        View view;
        public static Item_Dialog_Scan scan;
        public static string ActivityName;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.item_dialog_scan, container, false);
            try
            {
                scan = this;
                
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();

                //Request Permission
                Dexter.WithActivity(this.Activity)
                      .WithPermission(Manifest.Permission.Camera)
                      .WithListener(scan)
                      .Check();

                _ = TinyInsights.TrackPageViewAsync("OnCreateView : Item_Dialog_Scan");
                return view;

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at POS Dialog Scan");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }

        public static ZXingScannerView ScannerView;
        LinearLayout lnBack;
       
        private void CombinUI()
        {
            ScannerView = view.FindViewById<ZXingScannerView>(Resource.Id.zxscan);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnDismiss(this.Dialog);
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            try
            {
                this.Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPayment_Click at Cartscan");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            ScannerView.SetResultHandler(new MyResultHandler(this));
            ScannerView.SetAutoFocus(true);
            ScannerView.SetLaserColor(Color.LightBlue);
            ScannerView.SetBorderColor(Color.Transparent);
            ScannerView.StartCamera();
        }
        private class MyResultHandler : AppCompatActivity, IResultHandler
        {
            private Item_Dialog_Scan dialog_scan;

            public MyResultHandler(Item_Dialog_Scan dialog_Scan)
            {
                this.dialog_scan = dialog_Scan;
            }

            public void HandleResult(ZXing.Result rawResult)
            {
                try
                {
                    Vibration.Vibrate();
                    var duration = TimeSpan.FromMilliseconds(1);
                    Vibration.Vibrate(duration);
                    var scanResult = rawResult.Text;

                    switch (ActivityName)
                    {
                        case "additem" :
                            Item_Fragment_AddItem.SetItemCode(scanResult);
                            break;  
                        case "item":
                            Item_Fragment_Main.SetItemCode(scanResult);
                            break;
                        default:
                            Item_Fragment_Main.SetItemCode(scanResult);
                            break;
                    }
                    scan.Dismiss();
                }
                catch (Exception ex)
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                    _ = TinyInsights.TrackPageViewAsync("HandleResult at Cartscan");
                    Toast.MakeText(this.Application, ex.Message, ToastLength.Short).Show();
                    Dexter.WithActivity(this)
                        .WithPermission(Manifest.Permission.Camera)
                        .WithListener(scan)
                        .Check();
                }
            }
        }        

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            Toast.MakeText(this.Activity, "You Must Enable Permission", ToastLength.Long).Show();
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {

        }
    }

}