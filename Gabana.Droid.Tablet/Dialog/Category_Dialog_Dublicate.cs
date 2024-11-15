using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Fragments.Items;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Category_Dialog_Dublicate : AndroidX.Fragment.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        static string ItemName, ItemCode;
        TextView txtDetail;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Category_Dialog_Dublicate NewInstance()
        {
            var frag = new Category_Dialog_Dublicate { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.item_dialog_dublicate, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

                txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);
                txtDetail.Text = string.Empty;

                var textItemName = Item_Fragment_AddCategory.txtNameCategory.Text;
                var text1 = GetText(Resource.String.dialog_addcategory1);
                var text2 = GetText(Resource.String.dialog_additem_itemname);
                var text5 = GetText(Resource.String.dialog_additem3);

                if (DataCashing.Language == "th")
                {
                    txtDetail.Text = text1 + text2 + " " + textItemName + " " + text5;
                }
                else
                {
                    txtDetail.Text = text1 + " " + text2 + " " + textItemName + " " + text5;
                }
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

                Item_Fragment_AddCategory.fragment_addcategory.ManageCategory();
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
            Dismiss();
        }

        public static void SetDataItem(string _itemname, string _itemcode)
        {
            ItemName = _itemname;
            ItemCode = _itemcode;
        }
    }
}