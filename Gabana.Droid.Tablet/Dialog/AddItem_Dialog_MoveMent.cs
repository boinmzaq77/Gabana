using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Tablet.Adapter.Items;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class AddItem_Dialog_MoveMent : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static AddItem_Dialog_MoveMent NewInstance()
        {
            var frag = new AddItem_Dialog_MoveMent { Arguments = new Bundle() };
            return frag;
        }
        AddItem_Dialog_MoveMent dialog_movement;
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.additem_dialog_movement, container, false);
            dialog_movement = this;
            try
            {
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);

                CombinUI();
                SetUIEvent();
                lstItemMovement = new List<ItemMovement>();
                if (item == null)
                {
                    return view;
                }
                offset = 0;
                islast = true;
                GetDataStock();
                SetDetailItem();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

            return view;
        }

        private async void SetDetailItem()
        {
            var paths = item.ThumbnailLocalPath;
            if (!string.IsNullOrEmpty(paths))
            {
                Android.Net.Uri uri = Android.Net.Uri.Parse(paths);
                imageViewItem.SetImageURI(uri);
                textViewItemName.Visibility = ViewStates.Gone;
            }
            else
            {
                string conColor = Utils.SetBackground(Convert.ToInt32(item.Colors));
                var color = Android.Graphics.Color.ParseColor(conColor);
                imageViewItem.SetBackgroundColor(color);
                textViewItemName.Text = item.ItemName?.ToString();
            }
            txtNameItem.Text = item.ItemName?.ToString();

            var stock = await GabanaAPI.GetDataStock((int)DataCashingAll.SysBranchId, (int)item.SysItemID);
            if (stock != null)
            {
                txtMinimaumStock.Text = stock.MinimumStock.ToString("#,###");
                txtStockBalance.Text = stock.BalanceStock.ToString("#,###");
            }
            else
            {
                txtMinimaumStock.Text = "0";
                txtStockBalance.Text = "0";
            }
            offset = 0;
            position = 0;

        }

        private void SetUIEvent()
        {
            swipRefresh.Refresh += (sender, e) =>
            {
                OnResume();
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            };

        }
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swipRefresh.Refreshing = false;
        }
        internal static void SetItem(Item i)
        {
            item = i;
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        int Last, offset, position;
        private static Item item;
        public static List<ItemMovement> lstItemMovement;
        ListStockMoveMent listStock;
        async void GetDataStock()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                lstItemMovement = await GabanaAPI.GetDataStockItemMovement(DataCashingAll.SysBranchId, (int)item.SysItemID, offset);
                if (lstItemMovement == null)
                {
                    lstItemMovement = new List<ItemMovement>();
                }

                Last = lstItemMovement.Count;
                listStock = new ListStockMoveMent(lstItemMovement);
                AddItem_Adapter_MoveMent additem_adapter_movement = new AddItem_Adapter_MoveMent(listStock);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvListStock.SetLayoutManager(mLayoutManager);
                rcvListStock.HasFixedSize = true;
                rcvListStock.SetItemViewCacheSize(100);
                if (Last == 100)
                {
                    additem_adapter_movement.OnCardCellbtnIndex += Additem_adapter_movement_OnCardCellbtnIndex; ;
                }
                rcvListStock.SetAdapter(additem_adapter_movement);
                position = additem_adapter_movement.ItemCount;

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDataStock at Stock");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        bool islast;

        private async void Additem_adapter_movement_OnCardCellbtnIndex()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                offset++;
                if (islast)
                {
                    position = lstItemMovement.Count - 1;
                    dialogLoading.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));

                    List<ItemMovement> lst = new List<ItemMovement>();
                    lst = await GabanaAPI.GetDataStockItemMovement(DataCashingAll.SysBranchId, (int)item.SysItemID, offset);
                    lstItemMovement.AddRange(lst);

                    if (Last == lstItemMovement.Count)
                    {
                        islast = false;
                    }
                    Last = lstItemMovement.Count;
                    listStock = new ListStockMoveMent(lstItemMovement);
                    var ad = rcvListStock.GetAdapter() as AddItem_Adapter_MoveMent;
                    ad.refresh(listStock);
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Bill_Adapter_Main_OnCardCellbtnIndex5 at ItemMovement");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        LinearLayout lnBack;
        SwipeRefreshLayout swipRefresh;
        ImageView imageViewItem;
        TextView textViewItemName, txtNameItem, txtMinimaumStock, textSignMoney, txtStockBalance;
        RecyclerView rcvListStock;

        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            swipRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipRefresh);
            imageViewItem = view.FindViewById<ImageView>(Resource.Id.imageViewItem);
            textViewItemName = view.FindViewById<TextView>(Resource.Id.textViewItemName);
            txtNameItem = view.FindViewById<TextView>(Resource.Id.txtNameItem);
            txtMinimaumStock = view.FindViewById<TextView>(Resource.Id.txtMinimaumStock);
            textSignMoney = view.FindViewById<TextView>(Resource.Id.textSignMoney);
            txtStockBalance = view.FindViewById<TextView>(Resource.Id.txtStockBalance);
            rcvListStock = view.FindViewById<RecyclerView>(Resource.Id.rcvListStock);



        }
    }
}