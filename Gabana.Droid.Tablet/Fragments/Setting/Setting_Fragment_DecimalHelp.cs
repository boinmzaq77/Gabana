using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_DecimalHelp : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_DecimalHelp NewInstance()
        {
            Setting_Fragment_DecimalHelp frag = new Setting_Fragment_DecimalHelp();
            return frag;
        }
        public static Setting_Fragment_DecimalHelp fragment_currency;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_decimalhelp, container, false);
            try
            {
                CombineUI();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }

        }

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}
        }

        View view;

        ImageView imgType0, imgType1, imgType2, imgType3, imgType4, imgType5;
        EditText textType4;
        LinearLayout lnType4;
        string selectType, multiply;
        LinearLayout lnBack;
        private void CombineUI()
        {

            imgType0 = view.FindViewById<ImageView>(Resource.Id.imgType0);
            imgType1 = view.FindViewById<ImageView>(Resource.Id.imgType1);
            imgType2 = view.FindViewById<ImageView>(Resource.Id.imgType2);
            imgType3 = view.FindViewById<ImageView>(Resource.Id.imgType3);
            imgType4 = view.FindViewById<ImageView>(Resource.Id.imgType4);
            imgType5 = view.FindViewById<ImageView>(Resource.Id.imgType5);

            textType4 = view.FindViewById<EditText>(Resource.Id.textType4);

            TextView textHeader1 = view.FindViewById<TextView>(Resource.Id.textHeader1);
            TextView textHeader2 = view.FindViewById<TextView>(Resource.Id.textHeader2);
            TextView textHeader3 = view.FindViewById<TextView>(Resource.Id.textHeader3);
            TextView textHeader4 = view.FindViewById<TextView>(Resource.Id.textHeader4);
            TextView textHeader5 = view.FindViewById<TextView>(Resource.Id.textHeader5);
            TextView textHeader6 = view.FindViewById<TextView>(Resource.Id.textHeader6);

            TextView textView1 = view.FindViewById<TextView>(Resource.Id.textView1);
            TextView textView2 = view.FindViewById<TextView>(Resource.Id.textView2);
            TextView textView3 = view.FindViewById<TextView>(Resource.Id.textView3);
            TextView textView4 = view.FindViewById<TextView>(Resource.Id.textView4);
            TextView textView5 = view.FindViewById<TextView>(Resource.Id.textView5);
            TextView textView6 = view.FindViewById<TextView>(Resource.Id.textView6);
            TextView textView7 = view.FindViewById<TextView>(Resource.Id.textView7);
            TextView textView8 = view.FindViewById<TextView>(Resource.Id.textView8);
            TextView textView9 = view.FindViewById<TextView>(Resource.Id.textView9);
            TextView textView10 = view.FindViewById<TextView>(Resource.Id.textView10);
            TextView textView11 = view.FindViewById<TextView>(Resource.Id.textView11);
            TextView textView12 = view.FindViewById<TextView>(Resource.Id.textView12);
            TextView textView13 = view.FindViewById<TextView>(Resource.Id.textView13);
            TextView textView14 = view.FindViewById<TextView>(Resource.Id.textView14);
            TextView textView15 = view.FindViewById<TextView>(Resource.Id.textView15);
            TextView textView16 = view.FindViewById<TextView>(Resource.Id.textView16);
            TextView textView17 = view.FindViewById<TextView>(Resource.Id.textView17);
            TextView textView18 = view.FindViewById<TextView>(Resource.Id.textView18);
            TextView textView19 = view.FindViewById<TextView>(Resource.Id.textView19);
            TextView textView20 = view.FindViewById<TextView>(Resource.Id.textView20);
            TextView textView21 = view.FindViewById<TextView>(Resource.Id.textView21);
            if (DataCashing.Language == "th")
            {
                textHeader1.Text = "ไม่ปัดเศษ";
                textHeader2.Text = "การปัดเศษแบบที่ 1";
                textHeader3.Text = "การปัดเศษแบบที่ 2";
                textHeader4.Text = "การปัดเศษแบบที่ 3";
                textHeader5.Text = "การปัดเศษแบบที่ 4";
                textHeader6.Text = "การปัดเศษแบบที่ 5";

                textView1.Text = "เช่น 3.15 ไม่ปัดเศษเป็น 3.25";

                textView2.Text = "0.00 < X <= 0.25 = 0.25";
                textView3.Text = "0.25 < X <= 0.50 = 0.50";
                textView4.Text = "0.50 < X <= 0.75 = 0.75";
                textView5.Text = "X > 0.75 = 1";

                textView6.Text = "เช่น 3.15 ปัดเศษเป็น 3.25";
                textView7.Text = "เช่น 3.35 ปัดเศษเป็น 3.50";
                textView8.Text = "เช่น 3.55 ปัดเศษเป็น 3.75";
                textView9.Text = "เช่น 3.85 ปัดเศษเป็น 4.00";

                textView10.Text = "0 < X < 1 = 1";
                textView11.Text = "เช่น 3.25 ปัดเศษเป็น 4.00";

                textView12.Text = "0 < X < 0.5 = 0";
                textView13.Text = "X >= 0.5 = 1";
                textView14.Text = "เช่น 3.35 ปัดเศษเป็น 3.00";
                textView15.Text = "เช่น 3.55 ปัดเศษเป็น 4.00";

                textView16.Text = "0 < X < 0.5 = 1";
                textView17.Text = "0.5 < X < 1 = 1";
                textView18.Text = "เช่น ตัวคูณ 10, 4.00 ปัดเศษเป็น 0.00";
                textView19.Text = "เช่น ตัวคูณ 10, 5.00 ปัดเศษเป็น 10.00";

                textView20.Text = "X < 1.9 = 1";
                textView21.Text = "เช่น 3.90 ปัดเศษเป็น 3.00";

            }
            else
            {
                textHeader1.Text = "Not rounded";
                textHeader2.Text = "Option 1";
                textHeader3.Text = "Option 2";
                textHeader4.Text = "Option 3";
                textHeader5.Text = "Option 4";
                textHeader6.Text = "Option 5";

                textView1.Text = "For example 3.25 No rounded to 3.25";

                textView2.Text = "0.00 < X <= 0.25 = 0.25";
                textView3.Text = "0.25 < X <= 0.50 = 0.50";
                textView4.Text = "0.50 < X <= 0.75 = 0.75";
                textView5.Text = "X > 0.75 = 1";

                textView6.Text = "For example 3.15 Round up to 3.25";
                textView7.Text = "For example 3.35 Round up to 3.50";
                textView8.Text = "For example 3.55 Round up to 3.75";
                textView9.Text = "For example 3.85 Round up to 4.00";

                textView10.Text = "0 < X < 1 = 1";
                textView11.Text = "For example 3.25 Round up to 4.00";

                textView12.Text = "0 < X < 0.5 = 0";
                textView13.Text = "X >= 0.5 = 1";
                textView14.Text = "For example 3.35  Round down to 3.00";
                textView15.Text = "For example 3.55  Round up to 4.00";

                textView16.Text = "0 < X < 0.5 = 1";
                textView17.Text = "0.5 < X < 1 = 1";
                textView18.Text = "For example Multiply 10, 4.00  Round down to 0.00";
                textView19.Text = "For example Multiply 10, 5.00  Round up to 10.00";

                textView20.Text = "X < 1.9 = 1";
                textView21.Text = "For example 3.90 Round down to 3.00";

            }


            LinearLayout lnType0 = view.FindViewById<LinearLayout>(Resource.Id.lnType0);
            LinearLayout lnType1 = view.FindViewById<LinearLayout>(Resource.Id.lnType1);
            LinearLayout lnType2 = view.FindViewById<LinearLayout>(Resource.Id.lnType2);
            LinearLayout lnType3 = view.FindViewById<LinearLayout>(Resource.Id.lnType3);
            lnType4 = view.FindViewById<LinearLayout>(Resource.Id.lnType4);
            LinearLayout lnType5 = view.FindViewById<LinearLayout>(Resource.Id.lnType5);


            lnType0.Click += LnType0_Click;
            lnType1.Click += LnType1_Click;
            lnType2.Click += LnType2_Click;
            lnType3.Click += LnType3_Click;
            lnType4.Click += LnType4_Click;
            lnType5.Click += LnType5_Click;

            selectType = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING;
            multiply = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT;
            if (string.IsNullOrEmpty(multiply))
            {
                textType4.Visibility = ViewStates.Invisible;
            }
            else
            {
                textType4.Text = multiply;
                MainActivity.main_activity.OpenKeyboard(textType4);
            }
            SetShowSelect(selectType);

            Button btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
            btnApply.Click += BtnApply_Click;
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;

        }

        private async void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting , "setting", "decimal");
        }

        private async void BtnApply_Click(object sender, EventArgs e)
        {
            string text = "";
            if (selectType == "4")
            {
                text = textType4.Text;
            }
            Setting_Fragment_Decimal.SetDecimalType(selectType, text);
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "decimal");
        }
        private void SetShowSelect(string type)
        {
            imgType0.Visibility = ViewStates.Gone;
            imgType1.Visibility = ViewStates.Gone;
            imgType2.Visibility = ViewStates.Gone;
            imgType3.Visibility = ViewStates.Gone;
            imgType4.Visibility = ViewStates.Gone;
            imgType5.Visibility = ViewStates.Gone;
            textType4.Visibility = ViewStates.Invisible;
            switch (type)
            {
                case "0":
                    imgType0.Visibility = ViewStates.Visible;
                    break;
                case "1":
                    imgType1.Visibility = ViewStates.Visible;
                    break;
                case "2":
                    imgType2.Visibility = ViewStates.Visible;
                    break;
                case "3":
                    imgType3.Visibility = ViewStates.Visible;
                    break;
                case "4":
                    imgType4.Visibility = ViewStates.Visible;
                    textType4.Visibility = ViewStates.Visible;
                    break;
                case "5":
                    imgType5.Visibility = ViewStates.Visible;
                    break;
            }

            selectType = type;
        }
        private void LnType5_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("6");
            //Finish();
            SetShowSelect("5");
        }

        private void LnType4_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("5");
            //Finish();         
            lnType4.Click += (sender, e) =>
            {
                textType4.PerformClick();
                textType4.RequestFocus();
                textType4.SetSelection(textType4.Text.Length);
                textType4.FocusableInTouchMode = true;
                textType4.Clickable = true;
                MainActivity.main_activity.OpenKeyboard(textType4);
            };
            SetShowSelect("4");

        }
        private void LnType3_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("4");
            //Finish();
            SetShowSelect("3");
        }

        private void LnType2_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("3");
            //Finish();
            SetShowSelect("2");
        }
        private void LnType1_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("2");
            //Finish();
            SetShowSelect("1");
        }
        private void LnType0_Click(object sender, EventArgs e)
        {
            //DecimalActivity.SetDecimalType("1");
            SetShowSelect("0");
        }
    }
}