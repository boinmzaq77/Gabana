using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Adapter
{
    public class Printer_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<Bluetooth> listitem;
        public string positionClick;
        public string BranchID;
        string connect, notconnect;
        public Printer_Adapter_Main(List<Bluetooth> l)
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
                vh.BluetoothStatus.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblackcolor, null));
            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.printer_adapter_main, parent, false);
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
}