using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.ShareSource;
using Gabana3.JAM.Dashboard;
using System;
using System.Collections.Generic;
using TinyInsightsLib;

namespace Gabana.Droid.Adapter
{
    public class Dashboard_Adapter_BestEmp : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public string positionClick;
        List<BestEmployee> bestEmp;
        private decimal maxvalue;
        List<string> ProcessbarColor = new List<string> { "#0095DA", "#33ABE0", "#66BFE9", "#99D5F1", "#CDEAF8" };
        public Dashboard_Adapter_BestEmp(List<BestEmployee> b, decimal m)
        {
            bestEmp = b;
            maxvalue = m;
        }

        public override int ItemCount
        {
            get { return bestEmp == null ? 0 : bestEmp.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewBestEmpeHolder vh = holder as ListViewBestEmpeHolder;
                vh.Name.Text = bestEmp[position].employeeName?.ToString();
                //vh.Progress.Max = (int)maxvalue;
                vh.Progress.Progress = (int)bestEmp[position].totalAmount;
                //vh.Progress.SetBackgroundColor(Android.Graphics.Color.ParseColor(ProcessbarColor[position]));
                if (position < ProcessbarColor.Count)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    vh.Progress.ProgressDrawable.SetColorFilter(Android.Graphics.Color.ParseColor(ProcessbarColor[position]), Android.Graphics.PorterDuff.Mode.Multiply);
#pragma warning restore CS0618 // Type or member is obsolete
                }
                //vh.Progress.SetOutlineSpotShadowColor(Android.Graphics.Color.ParseColor("#66BFE9"));
                vh.TotalSale.Text = Utils.DisplayDecimal(bestEmp[position].totalAmount);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBindViewHolder at Dashboard");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.dashboard_adapter_bestemp, parent, false);
            ListViewBestEmpeHolder vh = new ListViewBestEmpeHolder(itemView, OnClick);

            TextView textSignMoney = itemView.FindViewById<TextView>(Resource.Id.textSignMoney);
            ProgressBar progressbar = itemView.FindViewById<ProgressBar>(Resource.Id.progressBar);
            progressbar.Max = (int)maxvalue;
            var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            textSignMoney.Text = CURRENCYSYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
}