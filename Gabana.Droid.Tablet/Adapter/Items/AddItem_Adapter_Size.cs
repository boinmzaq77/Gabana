using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Items
{
    internal class AddItem_Adapter_Size : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItemExSize lssize;
        public string positionClick;
        public static string Page;


        public AddItem_Adapter_Size(ListItemExSize l)
        {
            lssize = l;
        }
        public override int ItemCount
        {
            get { return lssize == null ? 0 : lssize.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewItemExSizeHolder vh = holder as ListViewItemExSizeHolder;
                if (string.IsNullOrEmpty(lssize[position].ExSizeName))
                {
                    vh.ExSizeName.Text = string.Empty;
                    vh.Price.Text = string.Empty;
                    vh.EstimateCost.Text = string.Empty;
                }
                else
                {
                    vh.ExSizeName.Text = lssize[position].ExSizeName?.ToString();
                    vh.Price.Text = Utils.DisplayDecimal(lssize[position].Price);
                    vh.EstimateCost.Text = Utils.DisplayDecimal(lssize[position].EstimateCost);
                }

                vh.btnDelete.Click += (s, d) =>
                {
                    try
                    {
                        if (Page == "POS_item")
                        {
                            POS_Dialog_AddItem.dialog_additem.DeleteExSize(lssize[position]);
                            Page = string.Empty;
                        }
                        else
                        {
                            Item_Fragment_AddItem.fragment_additem.DeleteExSize(lssize[position]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                    }
                };

                vh.ExSizeName.TextChanged += async (s, e) =>
                {
                    try
                    {
                        if (Page == "POS_item")
                        {
                            POS_Dialog_AddItem.dialog_additem.EditItemExSize();
                            POS_Dialog_AddItem.dialog_additem.CheckDataChange();
                            Page = string.Empty;
                        }
                        else
                        {
                            Item_Fragment_AddItem.fragment_additem.EditItemExSize();
                            Item_Fragment_AddItem.fragment_additem.CheckDataChange();
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                    }
                };

                vh.Price.KeyPress += async (s, e) =>
                {
                    try
                    {
                        if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                        {
                            e.Handled = true;
                            decimal Price = Convert.ToDecimal(vh.Price.Text);
                            vh.Price.Text = Utils.DisplayDecimal(Price);
                            vh.Price.SetSelection(vh.Price.Text.Length);

                            if (Page == "POS_item")
                            {
                                POS_Dialog_AddItem.dialog_additem.EditItemExSize();
                                POS_Dialog_AddItem.dialog_additem.CheckDataChange();
                                Page = string.Empty;
                            }
                            else
                            {
                                Item_Fragment_AddItem.fragment_additem.EditItemExSize();
                                Item_Fragment_AddItem.fragment_additem.CheckDataChange();
                            }
                        }
                        else
                        {
                            e.Handled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                    }
                };

                vh.EstimateCost.KeyPress +=  (s, e) =>
                {
                    try
                    {
                        if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                        {
                            e.Handled = true;
                            var EstimateCost = Convert.ToDecimal(vh.EstimateCost.Text);
                            //textCurrency.Visibility = ViewStates.Visible;
                            vh.EstimateCost.Text = Utils.DisplayDecimal(EstimateCost);
                            vh.EstimateCost.SetSelection(vh.Price.Text.Length);

                            if (Page == "POS_item")
                            {
                                POS_Dialog_AddItem.dialog_additem.EditItemExSize();
                                POS_Dialog_AddItem.dialog_additem.CheckDataChange();
                                Page = string.Empty;
                            }
                            else
                            {
                                Item_Fragment_AddItem.fragment_additem.EditItemExSize();
                                Item_Fragment_AddItem.fragment_additem.CheckDataChange();
                            }
                        }
                        else
                        {
                            e.Handled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Application.Context, ex.Message,ToastLength.Short).Show();

                    }
                };

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.additem_adapter_size, parent, false);
            ListViewItemExSizeHolder vh = new ListViewItemExSizeHolder(itemView, OnClick);

            vh.Price.Hint = Utils.DisplayDecimal(0);
            vh.EstimateCost.Hint = Utils.DisplayDecimal(0);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

        public static void SetPage(string _page)
        {
            Page = _page;
        }

    }

}