using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Gabana.Droid.Tablet.Dialog;
using LinqToDB.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet
{
    public class MainDialog : DialogFragment
    {
        static MainDialog main;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static MainDialog NewInstance()
        {
            var frag = new MainDialog { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.maindialogfragment, container, false);
            try
            {
                Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                main = this;
                var getmessage = this.Arguments.GetString("message");
                if (string.IsNullOrEmpty(getmessage))
                {
                    if (main != null) main.Dismiss();
                    return view;
                }
                int myValue = Int32.Parse(this.Arguments.GetString("message"));                
                LoadFragment(myValue);
                return view;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message + " MainDialog",ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreateView MainDialog");
                return view;
            }

        }
        void LoadFragment(int id)
        {
            try
            {
                Fragment fragment = null;
                switch (id)
                {
                    case Resource.Layout.term_dialog_confirm:
                        fragment = Termpolicy_Dialog_Confirm.NewInstance();
                        break;
                    case Resource.Layout.login_dialog_updateapp:
                        fragment = Login_Dialog_UpdateApp.NewInstance();
                        break;
                    case Resource.Layout.cart_dialog_clearcart:
                        fragment = Cart_Dialog_ClearCart.NewInstance();
                        break;
                    case Resource.Layout.login_dialog_expiry:
                        fragment = Login_Dialog_Expiry.NewInstance();
                        break;
                    case Resource.Layout.login_dialog_expiryemp:
                        fragment = Login_Dialog_ExpiryEmp.NewInstance();
                        break;
                    case Resource.Layout.package_dialog_error:
                        fragment = Package_Dialog_Error.NewInstance();
                        break;
                    case Resource.Layout.package_dialog_success:
                        fragment = Package_Dialog_Success.NewInstance();
                        break;
                    case Resource.Layout.package_dialog_offline:
                        fragment = Package_Dialog_Offline.NewInstance();
                        break;
                    //case Resource.Layout.package_dialog_promotionref:
                    //    fragment = Package_Dialog_PromotionRef.NewInstance();
                    //    break;
                    case Resource.Layout.setting_dialog_deletenote:
                        fragment = Setting_Dialog_DeleteNote.NewInstance();
                        break;
                    case Resource.Layout.setting_dialog_repeatnote:
                        fragment = Setting_Dialog_RepeatNote.NewInstance();
                        break;
                    case Resource.Layout.setting_dialog_deletecatenote:
                        fragment = Setting_Dialog_DeleteNoteCate.NewInstance();
                        break;
                    case Resource.Layout.setting_dialog_deletemembertype:
                        fragment = Setting_Dialog_DeleteMemberType.NewInstance();
                        break;
                    case Resource.Layout.setting_dialog_deletegiftvoucher:
                        fragment = Setting_Dialog_DeleteGiftVoucher.NewInstance();
                        break;
                    case Resource.Layout.setting_dialog_deletemyqr:
                        fragment = Setting_Dialog_DeleteMyQR.NewInstance();
                        break;
                    case Resource.Layout.package_dialog_promotion:
                        fragment = Package_Dialog_Promotion.NewInstance();
                        break;
                    //case Resource.Layout.employee_dialog_selectdatauser:
                    //    fragment = Employee_Dailog_UserDublicate.NewInstance();
                    //    break;                    
                    case Resource.Layout.edit_dialog_back:
                        fragment = Edit_Dialog_Back.NewInstance();
                        break;
                    case Resource.Layout.add_dialog_back:
                        fragment = Add_Dialog_Back.NewInstance();
                        break;

                    case Resource.Layout.customer_dialog_delete:
                        fragment = Customer_Dialog_Delete.NewInstance();
                        break;

                    case Resource.Layout.item_dialog_offline:
                        fragment = AddItem_Dialog_Offline.NewInstance();
                        break;
                    case Resource.Layout.alert_dialog_offline:
                        fragment = Alert_Dialog_Offline.NewInstance();
                        break;
                    case Resource.Layout.additem_dialog_onhand:
                        fragment = AddItem_Dialog_OnHand.NewInstance();
                        break;
                    case Resource.Layout.void_dialog_main:
                        fragment = Void_Dialog_Main.NewInstance();
                        break;
                    case Resource.Layout.addtopping_dialog_delete:
                        fragment = Addtopping_Dialog_Delete.NewInstance();
                        break;
                    case Resource.Layout.additem_dialog_delete:
                        fragment = Additem_Dialog_Delete.NewInstance();
                        break;
                    case Resource.Layout.addcategory_dialog_delete:
                        fragment = AddCategory_Dialog_Delete.NewInstance();
                        break;
                    case Resource.Layout.setting_dialog_cashdrawerrole:
                        fragment = Setting_Dialog_CashDrawer.NewInstance();
                        break;
                    default:
                        break;
                }
                ChildFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LoadFragment");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public static void CloseDialog()
        {
            main?.Dismiss();
        }
    }

}