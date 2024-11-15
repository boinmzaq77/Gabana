using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Phone;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    public class MainDialog : DialogFragment
    {
        static MainDialog main;
        int Sysitem, TotalDayRecieved;
        string DeleteType, Page, Username, GiftVoucherCode, InsertRepeat, Item, DetailInsert, Event, ItemType, OpenPicturefrom,
            NoCustomerType, ClearCart, Membertype, MembertypeData, empoffline, stockoffline, Customer, PathCloudPicture,
            InsertOffline, Void, CashGuid, CashGuidData, ConfirmRemove, ListRemoveItem, TranWithDetails, CheckAddBack
            , ItemName, ItemCode, SubscriptSuccess, PassValue;


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
                int myValue = Int32.Parse(this.Arguments.GetString("message"));
                Sysitem = this.Arguments.GetInt("systemID");
                DeleteType = this.Arguments.GetString("deleteType");
                Page = this.Arguments.GetString("fromPage");
                Username = this.Arguments.GetString("username");
                GiftVoucherCode = this.Arguments.GetString("giftVoucherCode");
                InsertRepeat = this.Arguments.GetString("insertRepeat");//Page
                Item = this.Arguments.GetString("detailitem");//ชื่อ ItemName or รหัส ItemCode
                DetailInsert = this.Arguments.GetString("detailnnsert");//ชื่อ ItemName or รหัส ItemCode
                Event = this.Arguments.GetString("event");//ชื่อ insert , update 
                ItemType = this.Arguments.GetString("itemtype");//ชื่อ item , topping 
                OpenPicturefrom = this.Arguments.GetString("OpenPicture");
                NoCustomerType = this.Arguments.GetString("NoCustomerType");
                ClearCart = this.Arguments.GetString("clearcart");
                Membertype = this.Arguments.GetString("membertype");
                MembertypeData = this.Arguments.GetString("membertypedata");
                empoffline = this.Arguments.GetString("empoffline");
                stockoffline = this.Arguments.GetString("stockoffline");
                InsertOffline = this.Arguments.GetString("InsertOffline");
                ConfirmRemove = this.Arguments.GetString("confirmRemove");
                Customer = this.Arguments.GetString("insertcustomer");
                PathCloudPicture = this.Arguments.GetString("OpenCloudPicture");
                Void = this.Arguments.GetString("Void");
                CashGuid = this.Arguments.GetString("CashGuid");
                CashGuidData = this.Arguments.GetString("CashGuidData");
                ListRemoveItem = this.Arguments.GetString("ListRemoveItem");
                TranWithDetails = this.Arguments.GetString("TranWithDetails");
                CheckAddBack = this.Arguments.GetString("CheckAddBack");
                ItemName = this.Arguments.GetString("ItemName");
                ItemCode = this.Arguments.GetString("ItemCode");
                SubscriptSuccess = this.Arguments.GetString("SubscriptSuccess");
                TotalDayRecieved = this.Arguments.GetInt("TotalDayRecieved");
                PassValue = this.Arguments.GetString("PassValue");
                main = this;
                LoadFragment(myValue);
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreateView MainDialog");
                return view;
            }
        }
        void LoadFragment(int id)
        {
            try
            {
                Android.Support.V4.App.Fragment fragment = null;
                switch (id)
                {
                    case Resource.Layout.cartscan_dialog_items:
                        fragment = ScanCart_Dialog_Item.NewInstance();
                        break;
                    case Resource.Layout.logout_dialog_main:
                        fragment = Logout_Dialog_Main.NewInstance();
                        break;
                    case Resource.Layout.void_dialog_main:
                        if (Void == "Void")
                        {
                            fragment = Void_Dialog_Main.NewInstance();
                        }
                        if (Void == "VoidRole")
                        {
                            fragment = VoidRole_Dialog_Main.NewInstance();
                        }
                        break;
                    case Resource.Layout.term_dialog_confirm:
                        fragment = Termpolicy_Dialog_Confirm.NewInstance();
                        break;
                    case Resource.Layout.offline_dialog_main:
                        if (empoffline == "empoffline")
                        {
                            fragment = Offline_Dialog_Main.NewInstance();
                        }
                        if (stockoffline == "stockoffline")
                        {
                            fragment = StockOffline_Dialog_Main.NewInstance();
                        }
                        if (InsertOffline == "InsertOffline")
                        {
                            fragment = InsertOffline_Dialog_Main.NewInstance();
                        }
                        if (ConfirmRemove == "confirmRemove")
                        {
                            fragment = RemoveItem_Dialog_Main.NewInstance(TranWithDetails, ListRemoveItem);
                        }
                        if (SubscriptSuccess == "SubscriptSuccess")
                        {
                            fragment = Subscriped_Dialog.NewInstance();
                        }
                        break;
                    case Resource.Layout.employee_dialog_unadduser:
                        fragment = Employee_Dialog_Unadduser.NewInstance();
                        break;
                    case Resource.Layout.employee_dialog_selectdatauser:
                        fragment = Employee_Dialog_SelectData.NewInstance();
                        break;
                    case Resource.Layout.updateapp_dialog_main:
                        fragment = UpdateApp_Dialog_Main.NewInstance();
                        break;
                    case Resource.Layout.pos_dialog_deleteitem:
                        #region pos_dialog_deleteitem
                        //Item
                        if (DeleteType == "item")
                        {
                            fragment = Item_Dialog_DeleteItem.NewInstance(Sysitem, Page);
                        }
                        //Topping
                        if (DeleteType == "topping")
                        {
                            fragment = Item_Dialog_DeleteToppping.NewInstance(Sysitem, Page);
                        }
                        //Category
                        if (DeleteType == "category")
                        {
                            fragment = Item_Dialog_DeleteCategory.NewInstance(Page);
                        }
                        //Customer
                        if (DeleteType == "customer")
                        {
                            fragment = Customer_Dialog_Delete.NewInstance(Sysitem, Page);
                        }
                        //Customer
                        if (DeleteType == "employee")
                        {
                            fragment = Employee_Dialog_Delete.NewInstance(Username, Page);
                        }
                        //Branch
                        if (DeleteType == "branch")
                        {
                            fragment = Branch_Dialog_Delete.NewInstance(Sysitem, Page);
                        }
                        //Note
                        if (DeleteType == "note")
                        {
                            fragment = Note_Dialog_Delete.NewInstance(Page);
                        }
                        //NoteCategory
                        if (DeleteType == "notecategory")
                        {
                            fragment = NoteCategory_Dialog_Delete.NewInstance();
                        }
                        //QRCode
                        if (DeleteType == "myqrcode")
                        {
                            fragment = MyQrCode_Dialog_Delete.NewInstance();
                        }
                        //Gift Voucher
                        if (DeleteType == "giftvoucher")
                        {
                            fragment = GiftVoucher_Dialog_Delete.NewInstance(GiftVoucherCode, Page);
                        }
                        //Insert Item Repeat
                        if (InsertRepeat == "insertrepeat")
                        {
                            fragment = Item_Dialog_InsertRepeatItem.NewInstance(Page, Item, DetailInsert, Event, ItemType);
                        }
                        //Insert ItemCode Repeat
                        if (InsertRepeat == "inseritemcodetrepeat")
                        {
                            fragment = Item_Dialog_InsertRepeatItemCode.NewInstance(Page, Item, DetailInsert, Event);
                        }
                        //Insert ItemCode Itemname Repeat

                        //Insert Category Repeat
                        if (InsertRepeat == "inserCategory")
                        {
                            fragment = Item_Dialog_InsertRepeatCategory.NewInstance(Item, DetailInsert, Event);
                        }
                        //Insert Note Repeat
                        if (InsertRepeat == "inserNote")
                        {
                            fragment = Note_Dialog_InsertRepeat.NewInstance(Item, DetailInsert, Event);
                        }
                        //Insert NoteCategory Repeat
                        if (InsertRepeat == "inserNoteCategory")
                        {
                            fragment = NoteCategory_Dialog_InsertRepeat.NewInstance(Item, DetailInsert, Event);
                        }
                        //Insert NoteCustomer Repeat 
                        if (InsertRepeat == "insertcustomer")
                        {
                            fragment = Customer_Dialog_InsertRepeat.NewInstance(Item, DetailInsert, Event);
                        }
                        //ไม่มี Customer Type
                        if (NoCustomerType == "CustomerType")
                        {
                            fragment = AddCustomer_Dialog_CustomerType.NewInstance();
                        }
                        //Clear Cart at CartActivity
                        if (ClearCart == "clearcart")
                        {
                            fragment = Cart_Dialog_ClearCart.NewInstance();
                        }
                        //Membertype 
                        if (Membertype == "membertype")
                        {
                            fragment = Membertype_Dialog_Delete.NewInstance(MembertypeData, Page);
                        }
                        if (CashGuid == "CashGuid")
                        {
                            fragment = CashGuide_Dialog_Delete.NewInstance(CashGuidData, Page);
                        }
                        break;
                    #endregion
                    case Resource.Layout.addcustomer_dialog_addimage:
                        if (OpenPicturefrom == "item")
                        {
                            fragment = Item_Dialog_AddImage.NewInstance();
                        }
                        if (OpenPicturefrom == "topping")
                        {
                            fragment = Topping_Dialog_AddImage.NewInstance();
                        }
                        if (OpenPicturefrom == "customer")
                        {
                            fragment = Customer_Dialog_AddImage.NewInstance();
                        }
                        if (OpenPicturefrom == "qrcode")
                        {
                            fragment = MyQrCode_Dialog_AddImage.NewInstance();
                        }
                        if (OpenPicturefrom == "receipt")
                        {
                            fragment = MyQrReceipt_Dialog_AddImage.NewInstance();
                        }
                        break;
                    case Resource.Layout.dialog_item:
                        fragment = Show_Dialog_Item.NewInstance(PathCloudPicture);
                        break;
                    case Resource.Layout.pos_dialog_saveorder:
                        fragment = Pos_Dialog_SaveOrder.NewInstance();
                        break;
                    case Resource.Layout.order_dialog_openorder:
                        fragment = Order_Dialog_Openorder.NewInstance();
                        break;
                    case Resource.Layout.add_dialog_back:
                        fragment = Add_Dialog_Back.NewInstance(Page);
                        break;
                    case Resource.Layout.edit_dialog_back:
                        fragment = Edit_Dialog_Back.NewInstance(Page,PassValue);
                        break;
                    case Resource.Layout.item_dialog_same_id_name:
                        fragment = Item_Dialog_AleartSameNameID.NewInstance(ItemName, ItemCode, DetailInsert, Event);
                        break;
                    case Resource.Layout.package_dialog_promotion:
                        fragment = Package_Dialog_Promotion.NewInstance();
                        break;
                    case Resource.Layout.package_dialog_promotionref:
                        fragment = Package_Dialog_PromotionRef.NewInstance(TotalDayRecieved);
                        break;
                    case Resource.Layout.package_dialog_refresh:
                        fragment = Package_Dialog_Refresh.NewInstance();
                        break;
                    case Resource.Layout.update_dialog_nullcode:
                        fragment = Update_Dialog_NullCode.NewInstance();
                        break;
                    case Resource.Layout.update_dialog_error:
                        fragment = Update_Dialog_Error.NewInstance();
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
                    case Resource.Layout.cart_dialog_itemstatusd:
                        fragment = Cart_Dialog_ItemstatusD.NewInstance(Page);
                        break;
                    case Resource.Layout.qrcash_dialog:
                        fragment = Qrcash_Dialog.NewInstance();
                        break;
                    case Resource.Layout.qrcash_dialog_payment:
                        fragment = Qrcash_Dialog_Payment.NewInstance();
                        break;
                    case Resource.Layout.alert_dialog_paymentqrcode:
                        fragment = Alert_Dialog_PaymentQrCash.NewInstance();
                        break;
                }

                if (fragment == null)
                    return;

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
            if (main != null) main.Dismiss();
        }
    }
}