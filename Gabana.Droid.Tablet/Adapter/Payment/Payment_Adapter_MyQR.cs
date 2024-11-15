using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.Droid.Tablet.Adapter.Setting;
using TinyInsightsLib;
using ZXing;
using Android.Graphics;
using Android.Content.Res;
using System.IO;
using ZXing.Common;

namespace Gabana.Droid.Tablet.Adapter.Payment
{
    class Payment_Adapter_MyQR : PagerAdapter
    {
        ListMyQRCode listMyQRCode;
        LayoutInflater layoutInflater;
        Android.App.Activity activity;
        List<Branch> lstBranch;
        public event EventHandler<int> ItemClick;
        public bool CheckNet = false;

        public Payment_Adapter_MyQR(ListMyQRCode l, Android.App.Activity activity, bool c)
        {
            listMyQRCode = l;
            this.activity = activity;
            CheckNet = c;
            Branch();
        }
        public override int Count
        {
            get { return listMyQRCode == null ? 0 : listMyQRCode.Count; }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object obj)
        {
            return view == obj;
        }

        async void Branch()
        {
            BranchManage branchManage = new BranchManage();
            lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
        }
         
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
        }
        
        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View itemView = LayoutInflater.From(container.Context).Inflate(Resource.Layout.myqr_adapter_main, container, false);
            ListViewMyQRCodeHolder vh = new ListViewMyQRCodeHolder(itemView, OnClick);
            try
            {
                layoutInflater = (LayoutInflater)activity.ApplicationContext.GetSystemService(Context.LayoutInflaterService);
                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;

                vh.QrComment.Text = listMyQRCode.myQrCodes[position].Comments?.ToString();
                vh.QrComment.Visibility = ViewStates.Visible;
                if (vh.QrComment.Text == "")
                {
                    vh.QrComment.Visibility = ViewStates.Gone;
                }
                vh.QrCodeName.Text = listMyQRCode.myQrCodes[position].MyQrCodeName;
                if (listMyQRCode.myQrCodes[position].FMyQrAllBranch == 'A')
                {
                    vh.QrBranch.Text = "All Branch";
                }
                else
                {
                    List<Branch> detail = new List<Branch>();
                    detail = lstBranch.Where(x => x.SysBranchID == listMyQRCode.myQrCodes[position].SysBranchID).ToList();
                    if (detail != null)
                    {
                        foreach (var item in detail)
                        {
                            if (detail.Count == 1)
                            {
                                vh.QrBranch.Text = item.TaxBranchName;
                            }
                            else
                            {
                                vh.QrBranch.Text += item.TaxBranchName + ", ";
                            }
                        }
                    }
                }

                //Path image
                var cloudpath = listMyQRCode[position].PicturePath;
                var localpath = listMyQRCode[position].PictureLocalPath;

                if (CheckNet)
                {
                    if (string.IsNullOrEmpty(localpath))
                    {
                        if (string.IsNullOrEmpty(cloudpath))
                        {
                            //defalut
                            vh.ImageQRCode.SetImageResource(Resource.Mipmap.DefaultMyQR);
                        }
                        else
                        {
                            //cloud
                            Utils.SetImage(vh.ImageQRCode, cloudpath);
                        }
                    }
                    else
                    {
                        //local
                        var local = listMyQRCode[position].PictureLocalPath;
                        Android.Net.Uri uri = Android.Net.Uri.Parse(local);
                        vh.ImageQRCode.SetImageURI(uri);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(localpath))
                    {
                        var local = listMyQRCode[position].PictureLocalPath;
                        Android.Net.Uri uri = Android.Net.Uri.Parse(local);
                        vh.ImageQRCode.SetImageURI(uri);
                    }
                    else
                    {
                        vh.ImageQRCode.SetImageResource(Resource.Mipmap.DefaultMyQR);
                    }
                }

                
                #region 23/04/2567 เพิ่ม QRCodestring ของ MyQR สำหรับ generate รูปแสดงที่ display 2
                Android.Graphics.Bitmap bitmap = null;
                bitmap = BitmapFactory.DecodeFile(localpath);
                var reader = new BarcodeReader();
                var result2 = reader.Decode(bitmap);
                if (result2 != null)
                {
                    string qrstring = result2.Text;
                    //test qrstring to bitmap
                    //Utils.Setbitmap(vh.bitmapimgrQRCode, qrstring);
                    //vh.bitmapimgrQRCode.Visibility = ViewStates.Gone; ไม่แสดงที่แอป
                }
                #endregion

                container.AddView(itemView);
                return itemView;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InstantiateItemAsync at MyQrCode_Adapter_Main");
                return vh;
            }
        }

        [Obsolete]
        public override void DestroyItem(View container, int position, Java.Lang.Object view)
        {
            try
            {
                ((ViewPager)container).RemoveView((View)view);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
        
    }

}