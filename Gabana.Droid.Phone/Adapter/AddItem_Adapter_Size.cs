using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using System;

namespace Gabana.Droid.Adapter
{
    public class AddItem_Adapter_Size : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItemExSize lssize;
        public string positionClick;


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
                    AddItemActivity.createItem.DeleteExSize(lssize[position]);
                };

                vh.ExSizeName.TextChanged += async (s, e) =>
                {
                    await AddItemActivity.createItem.EditItemExSize();
                    AddItemActivity.createItem.CheckDataChange();
                };

                vh.Price.KeyPress += async (s, e) =>
                {
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                        e.Handled = true;
                        decimal Price = Convert.ToDecimal(vh.Price.Text);
                        //textCurrency.Visibility = ViewStates.Visible;
                        vh.Price.Text = Utils.DisplayDecimal(Price);
                        vh.Price.SetSelection(vh.Price.Text.Length);
                        await AddItemActivity.createItem.EditItemExSize();
                        AddItemActivity.createItem.CheckDataChange();
                    }
                    else
                    {
                        e.Handled = false; //if you want that character appeared on the screen
                    }
                };

                vh.EstimateCost.KeyPress += async (s, e) =>
                {
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                        e.Handled = true;
                        var EstimateCost = Convert.ToDecimal(vh.EstimateCost.Text);
                        //textCurrency.Visibility = ViewStates.Visible;
                        vh.EstimateCost.Text = Utils.DisplayDecimal(EstimateCost);
                        vh.EstimateCost.SetSelection(vh.Price.Text.Length);
                        await AddItemActivity.createItem.EditItemExSize();
                        AddItemActivity.createItem.CheckDataChange();
                    }
                    else
                    {
                        e.Handled = false; //if you want that character appeared on the screen
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
    }
}