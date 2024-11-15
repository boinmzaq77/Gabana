using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class Cart_Dialog_ItemstatusD : Android.Support.V4.App.Fragment
    {
        Context context;
        Button btn_save;
        TextView txtShow;
        static string Page;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Cart_Dialog_ItemstatusD NewInstance(string _page)
        {
            Page = _page;
            var frag = new Cart_Dialog_ItemstatusD { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.cart_dialog_itemstatusd, container, false);
            context = container.Context;

            btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
            txtShow = view.FindViewById<TextView>(Resource.Id.txtShow);

            if (Page == "itemstatusd")
            {
                //กรุณาลบสินค้า เนื่องจากสินค้าในตะกร้าถูกลบ
                txtShow.Text = GetString(Resource.String.cart_dialog_itemstatusd);
                btn_save.Text = GetString(Resource.String.login_fragment_main_btn_back);
                btn_save.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
            }
            else if (Page == "cartupdate")
            {
                //สินค้าในตะกร้ามีการอัปเดต
                txtShow.Text = GetString(Resource.String.cart_dialog_cartupdate);
                btn_save.Text = GetString(Resource.String.textok);
            }
            else
            {
                //updatecart
                //สินค้าในตะกร้ามีการอัปเดตสินค้า กรุณาตรวจสอบตะกร้าของคุณ
                txtShow.Text = GetString(Resource.String.cart_dialog_updatecart);
                btn_save.Text = GetString(Resource.String.login_fragment_main_btn_back);
                btn_save.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
            }

            btn_save.Click += BtnOK_Click;

            return view;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

    }
}