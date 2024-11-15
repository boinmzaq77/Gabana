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
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Setting_Dialog_DublicateNoteCate : AndroidX.Fragment.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        static string ItemName, ItemCode;
        TextView txtDetail;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Setting_Dialog_DublicateNoteCate NewInstance()
        {

            var frag = new Setting_Dialog_DublicateNoteCate { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.item_dialog_dublicate, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += Btn_save_Click;
                btn_cancel.Click += Btn_cancel_Click;

                txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);
                txtDetail.Text = string.Empty;


                var textItemName = Setting_Dialog_AddNote.addNotecate.textNameCategory.Text;

                var text1 = GetText(Resource.String.dialog_addnotecategory1);
                var text2 = GetText(Resource.String.dialog_additem2);
                var text3 = GetText(Resource.String.dialog_additem3);

                txtDetail.Text = text1 + " " + textItemName + " " + text2 + " " + text3;

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
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

                Setting_Dialog_AddNote.addNotecate.ManageNoteCategory();
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

    }
}