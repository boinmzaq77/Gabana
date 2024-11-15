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
    public class Topping_Dialog_Dublicate : AndroidX.Fragment.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView txtDetail;
        public static string Page;
        string textItemName = string.Empty;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Topping_Dialog_Dublicate NewInstance()
        {

            var frag = new Topping_Dialog_Dublicate { Arguments = new Bundle() };
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

                var text1 = GetText(Resource.String.dialog_additem1);
                var text2 = GetText(Resource.String.dialog_additem_itemname);
                var text3 = GetText(Resource.String.dialog_additem2);
                var text4 = GetText(Resource.String.dialog_additem_itemcode);
                var text5 = GetText(Resource.String.dialog_additem3);
                                

                if (Page == "POS_extra")
                {
                    textItemName = POS_Dialog_AddTopping.textItemName.Text;
                    if (DataCashing.Language == "th")
                    {
                        txtDetail.Text = text1 + text2 + " " + textItemName + " " + text3 + " " + text5;
                    }
                    else
                    {
                        txtDetail.Text = text1 + " " + text2 + " " + textItemName + " " + text3 + " " + text5;
                    }
                }
                else
                {
                    textItemName = Item_Fragment_AddTopping.textItemName.Text;
                    if (DataCashing.Language == "th")
                    {
                        txtDetail.Text = text1 + text2 + " " + textItemName + " " + text3 + " " + text5;
                    }
                    else
                    {
                        txtDetail.Text = text1 + " " + text2 + " " + textItemName + " " + text3 + " " + text5;
                    }
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

                if (Page == "POS_extra")
                {
                    POS_Dialog_AddTopping.dialog_addtopping.ManageTopping();
                    Page = string.Empty;
                }
                else
                {
                    Item_Fragment_AddTopping.fragment_addtopping.ManageTopping();
                }
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

        public static void SetPage(string _page)
        {
            Page = _page;
        }
    }
}