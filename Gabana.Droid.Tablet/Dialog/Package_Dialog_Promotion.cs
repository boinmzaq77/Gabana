using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Package_Dialog_Promotion : AndroidX.Fragment.App.DialogFragment
    {
        Button btnApply;
        TextView txtPromotionCode;
        string PromotionCode;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Package_Dialog_Promotion NewInstance()
        {
            var frag = new Package_Dialog_Promotion { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.package_dialog_promotion, container, false);
            try
            {
                txtPromotionCode = view.FindViewById<TextView>(Resource.Id.txtPromotionCode);
                btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
                btnApply.Click += BtnApply_Click; ;
                txtPromotionCode.TextChanged += TxtPromotionCode_TextChanged; ;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                //GabanaLicenceDetail gabanaLicenceDetail = new GabanaLicenceDetail();
                GabanaLicenceModel gabanaLicence = new GabanaLicenceModel();
                gabanaLicence.PromotionCode = PromotionCode;
                gabanaLicence.MerchantID = DataCashingAll.MerchantId;

                ResultAPI result = await GabanaAPI.PostDataPromotion(gabanaLicence);
                if (result.Status)
                {
                    GabanaLicenceDetail Detail = new GabanaLicenceDetail();
                    Detail = JsonConvert.DeserializeObject<GabanaLicenceDetail>(result.Message);

                    AndroidX.Fragment.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.package_dialog_promotionref.ToString();
                    Package_Dialog_PromotionRef.SetTotalDayReceieve(Detail.TotalDayRecieved);
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    var transactionId = dialog.Show(ft, myMessage);
                }
                else
                {
                    string language = Preferences.Get("Language", "");
                    string Exception = UtilsAll.GetExceptionPromotion(result.Message, language);
                    Toast.MakeText(this.Activity, Exception, ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Dialog Promotion");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtPromotionCode_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            PromotionCode = txtPromotionCode.Text;
            if (!string.IsNullOrEmpty(PromotionCode))
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }

    }
}