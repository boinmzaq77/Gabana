using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class DialogEditItem : Android.Support.V4.App.DialogFragment
    {

        LinearLayout lnFav, lnEdit, lnOption, lnDelete, imageViewcolorItem;
        TextView textViewItemName, txtNameItem, txtPrice;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static DialogEditItem NewInstance()
        {
            var frag = new DialogEditItem { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_edititem, container, false);
            try
            {
                Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                textViewItemName = view.FindViewById<TextView>(Resource.Id.textViewItemName);
                txtNameItem = view.FindViewById<TextView>(Resource.Id.txtNameItem);
                txtPrice = view.FindViewById<TextView>(Resource.Id.txtPrice);

                imageViewcolorItem = view.FindViewById<LinearLayout>(Resource.Id.imageViewcolorItem);
                lnFav = view.FindViewById<LinearLayout>(Resource.Id.lnFav);
                lnEdit = view.FindViewById<LinearLayout>(Resource.Id.lnEdit);
                lnOption = view.FindViewById<LinearLayout>(Resource.Id.lnOption);
                lnDelete = view.FindViewById<LinearLayout>(Resource.Id.lnDelete);

                if (DataCashing.DialogShowItem != null)
                {
                    try
                    {
                        string conColor = SetBackground(Convert.ToInt32(DataCashing.DialogShowItem.Colors));
                        var color = Android.Graphics.Color.ParseColor(conColor);
                        imageViewcolorItem.SetBackgroundColor(color);
                    }
                    catch (Exception)
                    {
                        Toast.MakeText(Application.Context, "Error Parsecolor", ToastLength.Short).Show();
                    }

                    textViewItemName.Text = DataCashing.DialogShowItem.ItemName;
                    txtNameItem.Text = DataCashing.DialogShowItem.ItemName;
                    var price = DataCashing.DialogShowItem.Price;
                    txtPrice.Text = "฿ " + price.ToString("#,###.##");
                }

                lnEdit.Click += LnEdit_Click;
                lnDelete.Click += LnDelete_Click;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void LnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Dismiss();

                //เรียก Dialog Confirm Delete มาแสดง

                Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
                DialogConfirmDelete dialogDelete = new DialogConfirmDelete();
                var transactionId = dialogDelete.Show(ft, "DialogConfirmDelete");


            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
                return;
            }
        }

        private void LnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                //เปิด AddItemActivity สำหรับแก้ไขข้อมูล
                var IntentEdit = new Intent(Context, typeof(AddItemActivity));
                IntentEdit.PutExtra("EditItem", "Edit");
                StartActivity(IntentEdit);
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
                return;
            }
        }

        public string SetBackground(int color)
        {
            string hexValue = color.ToString("X");
            string hexcolor;
            if (hexValue.Length == 4)
            {
                hexcolor = "#00" + hexValue;
            }
            else if (hexValue.Length == 5)
            {
                hexcolor = "#0" + hexValue;
            }
            else
            {
                hexcolor = "#" + hexValue;
            }
            return hexcolor;
        }
    }
}