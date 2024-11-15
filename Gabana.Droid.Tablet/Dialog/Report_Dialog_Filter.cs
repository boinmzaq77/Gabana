using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Report;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Report_Dialog_Filter : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Report_Dialog_Filter NewInstance()
        {
            var frag = new Report_Dialog_Filter { Arguments = new Bundle() };
            return frag;
        }
        View view;
        Report_Dialog_Filter dialog_filter;
        public static int selectFilter;
        internal static void SetFilter(int filterReport)
        {
            selectFilter = filterReport;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.report_dialog_filter, container, false);
            try
            {
                dialog_filter = this;
                Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();
                if (Report_Fragment_ShowData.TypeReport.Contains("SalesReport") || Report_Fragment_ShowData.TypeReport.Contains("ProfitReport"))
                {
                    lnSortAZ.Visibility = ViewStates.Gone;
                    lnSortZA.Visibility = ViewStates.Gone;
                    lnSortTime.Visibility = ViewStates.Visible;
                }
                else
                {
                    lnSortAZ.Visibility = ViewStates.Visible;
                    lnSortZA.Visibility = ViewStates.Visible;
                    lnSortTime.Visibility = ViewStates.Gone;
                }
                selectFilter = Report_Fragment_ShowData.filterReport;
                lnBack.Click += LnBack_Click;
                btnTime.Click += (sender, e) =>
                {
                    selectFilter = 0;
                    SetFilterButton(selectFilter);
                };
                btnDes.Click += (sender, e) =>
                {
                    selectFilter = 1;
                    SetFilterButton(selectFilter);
                };
                btnAsc.Click += (sender, e) =>
                {
                    selectFilter = 2;
                    SetFilterButton(selectFilter);
                };
                btnAz.Click += (sender, e) =>
                {
                    selectFilter = 3;
                    SetFilterButton(selectFilter);
                };
                btnZa.Click += (sender, e) =>
                {
                    selectFilter = 4;
                    SetFilterButton(selectFilter);
                };

                btnSave.Click += BtnSave_Click;
                SetFilterButton(selectFilter);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            Report_Fragment_ShowData.filterReport = selectFilter; 
            Report_Fragment_ShowData.fragment_showdata.OnResume();
            this.Dialog.Dismiss();
        }

        private void SetFilterButton(int filter)
        {
            imgSelectTime.Visibility = ViewStates.Invisible;
            imgSelectDes.Visibility = ViewStates.Invisible;
            imgSelectAsc.Visibility = ViewStates.Invisible;
            imgSelectAz.Visibility = ViewStates.Invisible;
            imgSelectZa.Visibility = ViewStates.Invisible;

            switch (filter)
            {
                case 0:
                    imgSelectTime.Visibility = ViewStates.Visible;
                    break;
                case 1:
                    imgSelectDes.Visibility = ViewStates.Visible;
                    break;
                case 2:
                    imgSelectAsc.Visibility = ViewStates.Visible;
                    break;
                case 3:
                    imgSelectAz.Visibility = ViewStates.Visible;
                    break;
                case 4:
                    imgSelectZa.Visibility = ViewStates.Visible;
                    break;
                default:
                    break;
            }

            SetBtnSave();
        }
        private void SetBtnSave()
        {
            if (selectFilter != Report_Fragment_ShowData.filterReport)
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }
        ImageView imgSelectTime, imgSelectDes, imgSelectAsc, imgSelectAz, imgSelectZa;
        LinearLayout lnBack;
        FrameLayout lnSortTime, lnSortAZ, lnSortZA;
        Button btnTime, btnDes, btnAsc, btnAz, btnZa;
        Button btnSave;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            imgSelectTime = view.FindViewById<ImageView>(Resource.Id.imgSelectTime);
            imgSelectDes = view.FindViewById<ImageView>(Resource.Id.imgSelectDes);
            imgSelectAsc = view.FindViewById<ImageView>(Resource.Id.imgSelectAsc);
            imgSelectAz = view.FindViewById<ImageView>(Resource.Id.imgSelectAz);
            imgSelectZa = view.FindViewById<ImageView>(Resource.Id.imgSelectZa);

            btnTime = view.FindViewById<Button>(Resource.Id.btnTime);
            btnDes = view.FindViewById<Button>(Resource.Id.btnDes);
            btnAsc = view.FindViewById<Button>(Resource.Id.btnAsc);
            btnAz = view.FindViewById<Button>(Resource.Id.btnAz);
            btnZa = view.FindViewById<Button>(Resource.Id.btnZa);

            lnSortTime = view.FindViewById<FrameLayout>(Resource.Id.lnSortTime);
            lnSortAZ = view.FindViewById<FrameLayout>(Resource.Id.lnSortAZ);
            lnSortZA = view.FindViewById<FrameLayout>(Resource.Id.lnSortZA);

            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }
    }
}