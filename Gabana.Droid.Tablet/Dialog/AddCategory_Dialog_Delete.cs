using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class AddCategory_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static AddCategory_Dialog_Delete NewInstance()
        {
            var frag = new AddCategory_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }

        Button btn_cancel, btn_save;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.addcategory_dialog_delete, container, false);
            try
            {
                TextView textconfirm1 = view.FindViewById<TextView>(Resource.Id.textconfirm1);
                TextView textconfirm2 = view.FindViewById<TextView>(Resource.Id.textconfirm2);

                textconfirm1.Text = GetString(Resource.String.dialog_delete_category_1);
                textconfirm2.Text = GetString(Resource.String.dialog_delete_category_2);
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
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
                    MainDialog.CloseDialog();
                    return;
                }

                CategoryManage categoryManage = new CategoryManage();   
                ItemManage itemManage = new ItemManage();
                Category cateDelte = new Category();
                cateDelte = DataCashing.EditCategory;

                var UpdateItem = await itemManage.GetItembyCategory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                if (UpdateItem != null)
                {
                    foreach (var update in UpdateItem)
                    {
                        update.SysCategoryID = null;
                        var resultUpdate = await itemManage.UpdateItem(update);
                    }
                }
                cateDelte.DataStatus = 'D';
                cateDelte.FWaitSending = 2;
                cateDelte.DateModified = DateTime.UtcNow;
                var updateCate = await categoryManage.UpdateCategory(cateDelte);
                if (!updateCate)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                }
                else
                {
                    cateDelte.FWaitSending = 2;
                    await categoryManage.UpdateCategory(cateDelte);
                }

                DataCashing.EditCategory = null;
                Item_Fragment_AddCategory.editCategory = null;
                Item_Fragment_AddCategory.flagdatachange = false;
                Item_Fragment_Main.fragment_main.DeleteCategory(cateDelte);
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "default");                
                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }
    }
}