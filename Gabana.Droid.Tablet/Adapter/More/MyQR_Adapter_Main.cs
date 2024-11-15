using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.Setting;
using AndroidX.CardView.Widget;
using System.ComponentModel;
using System.Diagnostics;
using TinyInsightsLib;
using Gabana.ORM.MerchantDB;

namespace Gabana.Droid.Tablet.Adapter.More
{
    class MyQR_Adapter_Main : RecyclerView.Adapter
    {
        ListMyQRCode listitem;
        List<Branch> lstBranch;
        public event EventHandler<int> ItemClick;
        public bool CheckNet = false;
        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }
        public MyQR_Adapter_Main(ListMyQRCode l, bool c)
        {
            listitem = l;
            CheckNet = c;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            ListViewMyQRCodeHolder vh = holder as ListViewMyQRCodeHolder;
            try
            {
                vh.QrComment.Text = listitem.myQrCodes[position].Comments?.ToString();
                vh.QrComment.Visibility = ViewStates.Visible;
                if (vh.QrComment.Text == "")
                {
                    vh.QrComment.Visibility = ViewStates.Gone;
                }
                vh.QrCodeName.Text = listitem.myQrCodes[position].MyQrCodeName;
                if (listitem.myQrCodes[position].FMyQrAllBranch == 'A')
                {
                    vh.QrBranch.Text = "All Branch";
                }
                else
                {
                    List<Branch> detail = new List<Branch>();
                    detail = lstBranch.Where(x => x.SysBranchID == listitem.myQrCodes[position].SysBranchID).ToList();
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
                var cloudpath = listitem[position].PicturePath;
                var localpath = listitem[position].PictureLocalPath;

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
                        var local = listitem[position].PictureLocalPath;
                        Android.Net.Uri uri = Android.Net.Uri.Parse(local);
                        vh.ImageQRCode.SetImageURI(uri);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(localpath))
                    {
                        var local = listitem[position].PictureLocalPath;
                        Android.Net.Uri uri = Android.Net.Uri.Parse(local);
                        vh.ImageQRCode.SetImageURI(uri);
                    }
                    else
                    {
                        vh.ImageQRCode.SetImageResource(Resource.Mipmap.DefaultMyQR);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InstantiateItemAsync at MyQrCode_Adapter_Main");
            }
        }
        async void Branch()
        {
            BranchManage branchManage = new BranchManage();
            lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.myqr_adapter_main, parent, false);
            ListViewMyQRCodeHolder vh = new ListViewMyQRCodeHolder(itemView, OnClick);
            Branch();
            return vh;
        }



        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewMyQRCodeHolder : RecyclerView.ViewHolder
    {
        public TextView QrCodeName { get; set; }
        public TextView QrBranch { get; set; }
        public TextView QrComment { get; set; }
        public ImageView ImageQRCode { get; set; }
        public CardView CardAdapter { get; set; }
        public ListViewMyQRCodeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            QrCodeName = itemview.FindViewById<TextView>(Resource.Id.txtqrName);
            QrBranch = itemview.FindViewById<TextView>(Resource.Id.txtqrbranchName);
            QrComment = itemview.FindViewById<TextView>(Resource.Id.txtqrComment);
            ImageQRCode = itemview.FindViewById<ImageView>(Resource.Id.imgrQRCode);
            CardAdapter = itemview.FindViewById<CardView>(Resource.Id.CardAdapter);
            itemview.Click += (sender, e) => listener(base.Position);
        }
    }
}