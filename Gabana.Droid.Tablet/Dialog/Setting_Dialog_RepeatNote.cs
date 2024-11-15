using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Setting_Dialog_RepeatNote : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Dialog_RepeatNote NewInstance()
        {
            var frag = new Setting_Dialog_RepeatNote { Arguments = new Bundle() };
            return frag;
        }
        View view;
        TextView txtLine;
        Button btnCancel, btnSave;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.setting_dialog_repeatnote, container, false);
            try
            {
                btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
                btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
                txtLine = view.FindViewById<TextView>(Resource.Id.txtline);
                string textItemName = DataCashing.EditNote.Message;
                var text1 = GetText(Resource.String.dialog_addnote1);
                var text2 = GetText(Resource.String.dialog_additem2);
                txtLine.Text = GetText(Resource.String.dialog_addnote1) + " " +
                                textItemName + " " +
                                GetText(Resource.String.dialog_additem2) + " " +
                                GetText(Resource.String.dialog_additem3);

                btnCancel.Click += BtnCancel_Click;
                btnSave.Click += BtnSave_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;

        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                Setting_Fragment_AddNote.fragment_addnote.ManageNote();
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

    }
}