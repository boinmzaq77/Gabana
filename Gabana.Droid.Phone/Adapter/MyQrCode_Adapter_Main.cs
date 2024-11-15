using Android.App;
using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid.Adapter
{

    class MyQrCode_Adapter_Main : PagerAdapter
    {
        ListMyQRCode listMyQRCode;
        LayoutInflater layoutInflater;
        Activity activity;
        List<Branch> lstBranch;
        public event EventHandler<int> ItemClick;
        public bool CheckNet = false;

        public MyQrCode_Adapter_Main(ListMyQRCode l, Activity activity, bool c)
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