using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Interface;
using Gabana.Droid.ListData;
using Gabana.Droid.Phone;
using Gabana.Model;
using Xamarin.Essentials;

namespace Gabana.Droid.Adapter
{
    public class Adapter_Main_Activity : RecyclerView.Adapter
    {
        #region Events
        public delegate void MenuCelIndexDelegate(int MenuId, int id);
        public event MenuCelIndexDelegate OnMenuCelIndex;
        #endregion

        static int mMenuID;
        private List<Menuitem> listItem;
        public event EventHandler<int> ItemClick;

        

        public override int GetItemViewType(int position)
        {
            if (listItem.Count == 1)
            {
                return 0;
            }
            else
            {
                if (listItem.Count % 3 == 0)
                {
                    return 1;
                }
                else
                {
                    return (position > 1 && position == listItem.Count - 1) ? 0 : 1;
                }
            }
        }

        public Adapter_Main_Activity(int MenuID, List<Menuitem> menus)
        {
            mMenuID = MenuID;
            listItem = menus;
        }
        public override int ItemCount 
        {
            get { return listItem == null ? 0 : listItem.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewMenuHolder vh = holder as ListViewMenuHolder;               
               
                vh.MenuName.Text = listItem[position].MenuName?.ToString();
                vh.MenuIcon.SetImageResource(listItem[position].MenuIcon);

                //Utils.SetImage(vh.MenuIcon, listItem[position].MenuIcon, "Menu");

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "error" + ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.main_adepter_menu, parent, false);
            ListViewMenuHolder vh = new ListViewMenuHolder(itemView, OnClick);  

            CardView Imgcard = itemView.FindViewById<CardView>(Resource.Id.cardViewGrid);

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;

            Imgcard.LayoutParameters.Width = Convert.ToInt32(Convert.ToDecimal(width / 3).ToString("##"));
            Imgcard.LayoutParameters.Height = Convert.ToInt32(Convert.ToDecimal(width / 3).ToString("##"));

            return vh;
        }

        private void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
            OnMenuCelIndex?.Invoke(listItem[position].MenuId, listItem[position].MenuId);
        }


       
    }
}