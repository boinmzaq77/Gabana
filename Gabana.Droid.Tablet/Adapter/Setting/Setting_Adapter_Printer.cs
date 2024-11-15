using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Setting
{
    internal class Setting_Adapter_Printer : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<Bluetooth> listitem;
        public string positionClick;
        public string BranchID;
        string connect, notconnect;

        public Setting_Adapter_Printer(List<Bluetooth> l)
        {
            listitem = l;
        }
        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewBluetoothsHolder vh = holder as ListViewBluetoothsHolder;
            vh.BluetoothName.Text = listitem[position].BluetoothName;
            if (DataCashingAll.setting.BLUETOOTH1 != "" && listitem[position].BluetoothName == DataCashingAll.setting.BLUETOOTH1 && listitem[position].Address == DataCashingAll.setting.IPADDRESS)
            {
                vh.BluetoothStatus.Text = connect;
                vh.BluetoothSetting.SetBackgroundResource(Resource.Mipmap.BluetoothB);
                vh.BluetoothStatus.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                vh.BluetoothStatus.Text = notconnect;
                vh.BluetoothSetting.SetBackgroundResource(Resource.Mipmap.BluetoothG);
                vh.BluetoothStatus.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.setting_adapter_printer, parent, false);
            ListViewBluetoothsHolder vh = new ListViewBluetoothsHolder(itemView, OnClick);
            Android.Content.Res.Resources res = parent.Resources;
            connect = res.GetString(Resource.String.printer_activity_connect);
            notconnect = res.GetString(Resource.String.printer_activity_notconnect);

            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewBluetoothsHolder : RecyclerView.ViewHolder
    {
        //public TextView BluetoothID { get; set; }
        public TextView BluetoothName { get; set; }
        public TextView BluetoothStatus { get; set; }
        public ImageView BluetoothSetting { get; set; }
        public ListViewBluetoothsHolder(View itemview, Action<int> listener) : base(itemview)
        {
            //BluetoothID = itemview.FindViewById<TextView>(Resource.Id.txtBluetoothID);
            BluetoothName = itemview.FindViewById<TextView>(Resource.Id.txtBluetoothName);
            BluetoothStatus = itemview.FindViewById<TextView>(Resource.Id.txtBluetoothStatus);
            BluetoothSetting = itemview.FindViewById<ImageView>(Resource.Id.imageSetting);

            itemview.Click += (sender, e) => listener(base.Position);
        }
    }
}