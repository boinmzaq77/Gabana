using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gabana.Droid.ListData
{
    public class ListDiscount
    {
        public List<ORM.MerchantDB.DiscountTemplate> Discounts;
        static List<ORM.MerchantDB.DiscountTemplate> builitem;

        public ListDiscount(List<ORM.MerchantDB.DiscountTemplate> discountTemplates)
        {
            builitem = discountTemplates;
            this.Discounts = builitem;
        }
        public int Count
        {
            get
            {
                return Discounts == null ? 0 : Discounts.Count;
            }
        }
        public ORM.MerchantDB.DiscountTemplate this[int i]
        {
            get { return Discounts == null ? null : Discounts[i]; }
        }
    }
    public class ListViewDiscountHolder : RecyclerView.ViewHolder
    {
        public TextView Discount { get; set; }
        public ListViewDiscountHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Discount = itemview.FindViewById<TextView>(Resource.Id.textView1);
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    public class ListNoteCategory
    {
        public List<ORM.MerchantDB.NoteCategory> notecategory;
        static List<ORM.MerchantDB.NoteCategory> builitem;
        public ListNoteCategory(List<ORM.MerchantDB.NoteCategory> notecategory)
        {
            builitem = notecategory;
            this.notecategory = builitem;
        }
        public int Count
        {
            get
            {
                return notecategory == null ? 0 : notecategory.Count;
            }
        }
        public ORM.MerchantDB.NoteCategory this[int i]
        {
            get { return notecategory == null ? null : notecategory[i]; }
        }
    }
    public class ListViewCategoryHolder : RecyclerView.ViewHolder
    {
        public TextView CategoryName { get; set; }
        public TextView CategoryItem { get; set; }
        public ImageView Check { get; set; }
        public View Lineblue { get; set; }
        public ListViewCategoryHolder(View itemview, Action<int> listener) : base(itemview)
        {
            CategoryName = itemview.FindViewById<TextView>(Resource.Id.txtNameCategory);
            CategoryItem = itemview.FindViewById<TextView>(Resource.Id.txtCategoryTotal);
            Check = itemview.FindViewById<ImageView>(Resource.Id.imgCheck);

            Lineblue = itemview.FindViewById<View>(Resource.Id.lineblue);
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.LongClick += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public void Select()
        {
            CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            Lineblue.Visibility = ViewStates.Visible;
        }

        public void NotSelect()
        {
            CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
            Lineblue.Visibility = ViewStates.Gone;
        }
    }
    public class ListViewItemHolder : RecyclerView.ViewHolder
    {
        public ImageView ColorItemView { get; set; }
        public ImageView PicItemView { get; set; }
        public ImageView PicItemViewRow { get; set; }
        public TextView ItemNameView { get; set; }
        public TextView ItemName { get; set; }
        public TextView CategoryItem { get; set; }
        public TextView ItemPrice { get; set; }
        public TextView PriceTotal { get; set; }
        public TextView PriceBeforeDis { get; set; }
        public ImageView Alert { get; set; }
        public TextView CountItem { get; set; }
        public LinearLayout lnEditItem { get; set; }
        public FrameLayout frameAdd { get; set; }
        public ImageView imgShowAnime { get; set; }
        public LinearLayout lnQuantity { get; set; }
        public LinearLayout lnOption { get; set; }
        public LinearLayout lnChangePrice { get; set; }
        public LinearLayout lnDiscount { get; set; }
        public LinearLayout lnDelete { get; set; }
        public RelativeLayout OptionList { get; set; }
        public LinearLayout lnFavorite { get; set; }
        public ImageButton Favorite { get; set; }
        public ImageView DiscountRowImage { get; set; }
        public RelativeLayout lnComment { get; set; }
        public LinearLayout Discount { get; set; }
        public TextView Comment { get; set; }
        public TextView DiscountItem { get; set; }
        public LinearLayout lnColorBottom { get; set; }
        public LinearLayout lnStatusD { get; set; }
        public Button btnDeleteItemD { get; set; }

        public ListViewItemHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ColorItemView = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            PicItemView = itemview.FindViewById<ImageView>(Resource.Id.imagePicItem);
            //PicItemViewRow = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            ItemNameView = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            ItemName = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            CategoryItem = itemview.FindViewById<TextView>(Resource.Id.txtCategoryItem);
            ItemPrice = itemview.FindViewById<TextView>(Resource.Id.txtItemPrice);
            PriceTotal = itemview.FindViewById<TextView>(Resource.Id.txtPriceTotal);
            PriceBeforeDis = itemview.FindViewById<TextView>(Resource.Id.txtPriceBeforeDis);
            Alert = itemview.FindViewById<ImageView>(Resource.Id.imagealert);
            CountItem = itemview.FindViewById<TextView>(Resource.Id.textCountITem);
            frameAdd = itemview.FindViewById<FrameLayout>(Resource.Id.frameAdd);
            imgShowAnime = itemview.FindViewById<ImageView>(Resource.Id.imgShowAnime);

            lnEditItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnEditItem);
            lnQuantity = itemview.FindViewById<LinearLayout>(Resource.Id.lnQuantity);
            lnOption = itemview.FindViewById<LinearLayout>(Resource.Id.lnOption);
            lnChangePrice = itemview.FindViewById<LinearLayout>(Resource.Id.lnPrice);
            lnDiscount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscount);
            lnDelete = itemview.FindViewById<LinearLayout>(Resource.Id.lnDelete);
            OptionList = itemview.FindViewById<RelativeLayout>(Resource.Id.lnOptionlist);
            Discount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscountItem);
            DiscountItem = itemview.FindViewById<TextView>(Resource.Id.textItemDiscount);
            lnFavorite = itemview.FindViewById<LinearLayout>(Resource.Id.lnFav);
            Favorite = itemview.FindViewById<ImageButton>(Resource.Id.btnFavorite);
            DiscountRowImage = itemview.FindViewById<ImageView>(Resource.Id.imageDiscountRow);

            lnComment = itemview.FindViewById<RelativeLayout>(Resource.Id.lnCommentItem);
            Comment = itemview.FindViewById<TextView>(Resource.Id.textCommentItem);

            lnStatusD = itemview.FindViewById<LinearLayout>(Resource.Id.lnStatusD);
            btnDeleteItemD = itemview.FindViewById<Button>(Resource.Id.btnDeleteItemD);
            lnColorBottom = itemview.FindViewById<LinearLayout>(Resource.Id.lnColorBottom);

#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public void Select()
        {
            Favorite.SetBackgroundResource(Resource.Mipmap.Fav);
        }

        public void NotSelect()
        {
            Favorite.SetBackgroundResource(Resource.Mipmap.Unfav);
        }
    }

    public class ListViewPosItemHolder : RecyclerView.ViewHolder
    {
        public ImageView ColorItemView { get; set; }
        public ImageView PicItemView { get; set; }
        public TextView ItemNameView { get; set; }
        public TextView ItemName { get; set; }
        public TextView CategoryItem { get; set; }
        public TextView ItemPrice { get; set; }
        public TextView PriceTotal { get; set; }
        public TextView PriceBeforeDis { get; set; }
        public ImageView Alert { get; set; }
        public TextView CountItem { get; set; }
        public LinearLayout lnEditItem { get; set; }
        public FrameLayout frameAdd { get; set; }
        public ImageView imgShowAnime { get; set; }
        public LinearLayout lnQuantity { get; set; }
        public LinearLayout lnOption { get; set; }
        public LinearLayout lnChangePrice { get; set; }
        public LinearLayout lnDiscount { get; set; }
        public LinearLayout lnDelete { get; set; }
        public RelativeLayout OptionList { get; set; }
        public LinearLayout lnFavorite { get; set; }
        public ImageButton Favorite { get; set; }
        public ListViewPosItemHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ColorItemView = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            PicItemView = itemview.FindViewById<ImageView>(Resource.Id.imagePicItem);
            ItemNameView = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            ItemName = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            CategoryItem = itemview.FindViewById<TextView>(Resource.Id.txtCategoryItem);
            ItemPrice = itemview.FindViewById<TextView>(Resource.Id.txtItemPrice);
            PriceTotal = itemview.FindViewById<TextView>(Resource.Id.txtPriceTotal);
            PriceBeforeDis = itemview.FindViewById<TextView>(Resource.Id.txtPriceBeforeDis);
            Alert = itemview.FindViewById<ImageView>(Resource.Id.imagealert);
            CountItem = itemview.FindViewById<TextView>(Resource.Id.textCountITem);
            frameAdd = itemview.FindViewById<FrameLayout>(Resource.Id.frameAdd);
            imgShowAnime = itemview.FindViewById<ImageView>(Resource.Id.imgShowAnime);

            lnEditItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnEditItem);
            lnQuantity = itemview.FindViewById<LinearLayout>(Resource.Id.lnQuantity);
            lnOption = itemview.FindViewById<LinearLayout>(Resource.Id.lnOption);
            lnChangePrice = itemview.FindViewById<LinearLayout>(Resource.Id.lnPrice);
            lnDiscount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscount);
            lnDelete = itemview.FindViewById<LinearLayout>(Resource.Id.lnDelete);
            OptionList = itemview.FindViewById<RelativeLayout>(Resource.Id.lnOptionlist);

            lnFavorite = itemview.FindViewById<LinearLayout>(Resource.Id.lnFav);
            Favorite = itemview.FindViewById<ImageButton>(Resource.Id.btnFavorite);
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete

        }
    }
    public class ListViewReceiptHolder : RecyclerView.ViewHolder
    {
        public TextView textCountITem { get; set; }
        public TextView txtNameItem { get; set; }
        public TextView txtPrice { get; set; }
        //public TextView txtDiscountName { get; set; }
        public TextView txtDiscount { get; set; }
        public LinearLayout lnOptionlist { get; set; }
        public LinearLayout lnDiscountItem { get; set; }
        public LinearLayout lnCommetnItem { get; set; }
        public TextView CommentItem { get; set; }
        public ListViewReceiptHolder(View itemview, Action<int> listener) : base(itemview)
        {
            textCountITem = itemview.FindViewById<TextView>(Resource.Id.textCountITem);
            txtNameItem = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            txtPrice = itemview.FindViewById<TextView>(Resource.Id.txtPrice);
            //txtDiscountName = itemview.FindViewById<TextView>(Resource.Id.txtDiscountName);
            txtDiscount = itemview.FindViewById<TextView>(Resource.Id.textItemDiscount);
            lnOptionlist = itemview.FindViewById<LinearLayout>(Resource.Id.lnOptionlist);
            lnDiscountItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscountItem);
            lnCommetnItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnCommentItem);
            CommentItem = itemview.FindViewById<TextView>(Resource.Id.txtCommentItem);

#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
    public class ListShowNoteDeatil
    {
        public List<showNoteDetail> Note;
        static List<showNoteDetail> builitem;
        public ListShowNoteDeatil(List<showNoteDetail> noteDetails)
        {
            builitem = new List<showNoteDetail>();
            Note = new List<showNoteDetail>();
            builitem = noteDetails;
            Note = builitem;
        }

        public int Count
        {
            get
            {
                return Note.Count;
            }
        }

        public showNoteDetail this[int i]
        {
            get { return Note.Count == 0 ? null : Note[i]; }
        }
    }
    public class ListViewShowNoteDeatailHolder : RecyclerView.ViewHolder
    {
        public TextView NoteName { get; set; }
        public TextView SubNote { get; set; }
        public ListViewShowNoteDeatailHolder(View itemview, Action<int> listener) : base(itemview)
        {
            NoteName = itemview.FindViewById<TextView>(Resource.Id.txtNameNote);
            SubNote = itemview.FindViewById<TextView>(Resource.Id.txtSubNote);
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
    public class ListBranch
    {
        public List<ORM.MerchantDB.Branch> branches;
        static List<ORM.MerchantDB.Branch> builitem;
        public ListBranch(List<ORM.MerchantDB.Branch> branch)
        {
            builitem = branch;
            this.branches = builitem;
        }
        public int Count
        {
            get
            {
                return branches == null ? 0 : branches.Count;
            }
        }
        public ORM.MerchantDB.Branch this[int i]
        {
            get { return branches == null ? null : branches[i]; }
        }
    }
    public class ListViewBranchHolder : RecyclerView.ViewHolder
    {
        public TextView BranchName { get; set; }
        public ImageButton BtnCheck { get; set; }
        public LinearLayout lnSelectBranch { get; set; }

        public ListViewBranchHolder(View itemview, Action<int> listener) : base(itemview)
        {
            BranchName = itemview.FindViewById<TextView>(Resource.Id.txtBranchName);
            BtnCheck = itemview.FindViewById<ImageButton>(Resource.Id.btnCheck);
            lnSelectBranch = itemview.FindViewById<LinearLayout>(Resource.Id.lnSelectBranch);
 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 
        }
    }
    public class ListViewCustomerHolder : RecyclerView.ViewHolder
    {
        public TextView CustomerName { get; set; }
        public ImageButton SelectCustomer { get; set; }
        public ImageView imageCustomer { get; set; }

        public ListViewCustomerHolder(View itemview, Action<int> listener) : base(itemview)
        {
            CustomerName = itemview.FindViewById<TextView>(Resource.Id.txtNameCustomer);
            SelectCustomer = itemview.FindViewById<ImageButton>(Resource.Id.imgSelect);
            imageCustomer = itemview.FindViewById<ImageView>(Resource.Id.imageCustomer);


#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete

        }
    }
    public class ListCurrency
    {
        public List<Currency> currency;
        static List<Currency> builitem;
        public ListCurrency()
        {
            builitem = GabanaModel.gabanaMain.currency;
            this.currency = builitem;
        }
        public int Count
        {
            get
            {
                return currency == null ? 0 : currency.Count;
            }
        }
        public Currency this[int i]
        {
            get { return currency == null ? null : currency[i]; }
        }
    }
    public class ListViewCurrencyHolder : RecyclerView.ViewHolder
    {
        public ImageView ImgCurrency { get; set; }
        public TextView CurrencyName { get; set; }
        public ImageView ImgSelect { get; set; }

        public ListViewCurrencyHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImgCurrency = itemview.FindViewById<ImageView>(Resource.Id.imgCurrency);
            CurrencyName = itemview.FindViewById<TextView>(Resource.Id.txtCurrencyName);
            ImgSelect = itemview.FindViewById<ImageView>(Resource.Id.imgSelect);

 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }
    public class ListMenu
    {
        public List<Menuitem> menuitems;
        static List<Menuitem> builitem;
        public ListMenu()
        {
            if (GabanaModel.gabanaMain.menu != null)
            {
                builitem = GabanaModel.gabanaMain.menu;
                this.menuitems = builitem;
            }
        }
        public int Count
        {
            get
            {
                return menuitems == null ? 0 : menuitems.Count;
            }
        }
        public Menuitem this[int i]
        {
            get { return menuitems == null ? null : menuitems[i]; }
        }
    }
    public class ListViewMenuHeaderHolder : RecyclerView.ViewHolder
    {
        public TextView MenuHeaderName { get; set; }

        public ListViewMenuHeaderHolder(View itemview, Action<int> listener) : base(itemview)
        {
            MenuHeaderName = itemview.FindViewById<TextView>(Resource.Id.txtNameCategory);

 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }
    public class ListViewBillHistoryHolder : RecyclerView.ViewHolder
    {
        public TextView Day { get; set; }
        public TextView NumberDate { get; set; }
        public ImageView PaymentIcon { get; set; }
        public TextView BillNo { get; set; }
        public TextView Price { get; set; }
        public TextView Time { get; set; }
        public TextView CustomerName { get; set; }
        public LinearLayout Headerbar { get; set; }
        public TextView StatusVoid { get; set; }
        public TextView StatusPending { get; set; }


        public ListViewBillHistoryHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Day = itemview.FindViewById<TextView>(Resource.Id.txtDay);
            NumberDate = itemview.FindViewById<TextView>(Resource.Id.txtnumberdate);
            PaymentIcon = itemview.FindViewById<ImageView>(Resource.Id.imageViewPaymentIcon);
            BillNo = itemview.FindViewById<TextView>(Resource.Id.txtBillNo);
            Price = itemview.FindViewById<TextView>(Resource.Id.txtPrice);
            Time = itemview.FindViewById<TextView>(Resource.Id.txtshowtime);
            CustomerName = itemview.FindViewById<TextView>(Resource.Id.txtCustomerName);
            Headerbar = itemview.FindViewById<LinearLayout>(Resource.Id.lnheaderbar);
            StatusVoid = itemview.FindViewById<TextView>(Resource.Id.txtVoid);
            StatusPending = itemview.FindViewById<TextView>(Resource.Id.txtPending);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }
    public class ListBillHistory
    {
        public List<Model.TransHistory> Trans;
        static List<Model.TransHistory> builitem;
        public ListBillHistory(List<Model.TransHistory> transHistories)
        {
            builitem = transHistories;
            this.Trans = builitem;
        }

        public int Count
        {
            get
            {
                return Trans == null ? 0 : Trans.Count;
            }
        }

        public Model.TransHistory this[int i]
        {
            get { return Trans == null ? null : Trans[i]; }
        }
    }

    public class ListBillHistoryNew
    {
        public List<Model.TransHistoryNew> Trans;
        static List<Model.TransHistoryNew> builitem;
        public ListBillHistoryNew(List<Model.TransHistoryNew> transHistories)
        {
            builitem = transHistories;
            this.Trans = builitem;
        }

        public int Count
        {
            get
            {
                return Trans == null ? 0 : Trans.Count;
            }
        }

        public Model.TransHistory this[int i]
        {
            get { return Trans == null ? null : Trans[i]; }
        }
    }


    public class ListNoteData
    {
        public List<Note> Note;
        static List<Note> builitem;
        public ListNoteData(List<Note> noteDetails)
        {
            builitem = noteDetails;
            Note = builitem;
        }

        public int Count
        {
            get
            {
                return Note.Count;
            }
        }

        public Note this[int i]
        {
            get { return Note.Count == 0 ? null : Note[i]; }
        }
    }

    public class ListViewNoteDataHolder : RecyclerView.ViewHolder
    {
        public TextView NoteName { get; set; }
        public TextView NoteCategory { get; set; }
        public ListViewNoteDataHolder(View itemview, Action<int> listener) : base(itemview)
        {
            NoteName = itemview.FindViewById<TextView>(Resource.Id.txtNameNote);
            NoteCategory = itemview.FindViewById<TextView>(Resource.Id.txtSubNote);

 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }

    public class ListItemExSize
    {
        public List<ItemExSize> itemexsizes;
        static List<ItemExSize> builitem;
        public ListItemExSize(List<ItemExSize> lsitemExSizes)
        {
            builitem = lsitemExSizes;
            this.itemexsizes = builitem;
        }

        public int Count
        {
            get
            {
                return itemexsizes == null ? 0 : itemexsizes.Count;
            }
        }
        public ORM.MerchantDB.ItemExSize this[int i]
        {
            get { return itemexsizes == null ? null : itemexsizes[i]; }
        }
    }
    public class ListViewItemExSizeHolder : RecyclerView.ViewHolder
    {
        public TextView ExSizeNo { get; set; }
        public EditText ExSizeName { get; set; }
        public EditText Price { get; set; }
        public EditText EstimateCost { get; set; }
        public EditText Comments { get; set; }
        public ImageButton btnDelete { get; set; }

        public ImageButton SelectCustomer { get; set; }
        public ListViewItemExSizeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ExSizeName = itemview.FindViewById<EditText>(Resource.Id.txtExSizeName);
            Price = itemview.FindViewById<EditText>(Resource.Id.txtPrice);
            EstimateCost = itemview.FindViewById<EditText>(Resource.Id.txtEstimate);
            btnDelete = itemview.FindViewById<ImageButton>(Resource.Id.btnDelete);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }
    public class ListViewOptionSizeHolder : RecyclerView.ViewHolder
    {
        public ImageButton SelectSize { get; set; }
        public TextView SizeName { get; set; }
        public TextView Price { get; set; }

        public ImageButton SelectCustomer { get; set; }
        public ListViewOptionSizeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            SelectSize = itemview.FindViewById<ImageButton>(Resource.Id.btnSelectSize);
            SizeName = itemview.FindViewById<TextView>(Resource.Id.textSizeName);
            Price = itemview.FindViewById<TextView>(Resource.Id.textSizePrice);


#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete

        }
    }

    public class ListViewExtraDataHolder : RecyclerView.ViewHolder
    {
        public TextView textToppingName { get; set; }
        public TextView txtUnitExtra { get; set; }
        public TextView textPriceExtra { get; set; }
        public ImageButton btnSelectExtra { get; set; }
        public FrameLayout btnReExtra { get; set; }
        public FrameLayout btnAddExtra { get; set; }


        public ListViewExtraDataHolder(View itemview, Action<int> listener) : base(itemview)
        {
            textToppingName = itemview.FindViewById<TextView>(Resource.Id.textToppingName);
            txtUnitExtra = itemview.FindViewById<TextView>(Resource.Id.txtUnitExtra);
            textPriceExtra = itemview.FindViewById<TextView>(Resource.Id.textPriceExtra);
            btnSelectExtra = itemview.FindViewById<ImageButton>(Resource.Id.btnSelectExtra);
            btnReExtra = itemview.FindViewById<FrameLayout>(Resource.Id.btnReExtra);
            btnAddExtra = itemview.FindViewById<FrameLayout>(Resource.Id.btnAddExtra);
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    public class ListViewCategoryExtraDataHolder : RecyclerView.ViewHolder
    {
        public TextView txtNameCategory { get; set; }
        public View lineblue { get; set; }
        public ListViewCategoryExtraDataHolder(View itemview, Action<int> listener) : base(itemview)
        {
            txtNameCategory = itemview.FindViewById<TextView>(Resource.Id.txtNameCategory);
            lineblue = itemview.FindViewById<View>(Resource.Id.lineblue);

#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete

        }
    }

    public class ListViewNoteOptionHolder : RecyclerView.ViewHolder
    {
        public TextView textSizeName { get; set; }
        public ListViewNoteOptionHolder(View itemview, Action<int> listener) : base(itemview)
        {
            textSizeName = itemview.FindViewById<TextView>(Resource.Id.textSizeName);

#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete

        }
    }

    public class ListViewEmployeeHolder : RecyclerView.ViewHolder
    {
        public ImageView EmpImage { get; set; }
        public TextView EmpName { get; set; }
        public TextView EmpPosition { get; set; }
        public ImageView ActiveImage { get; set; }
        public Switch ActiveSwich { get; set; }
        public ImageView Check { get; set; }

        public ListViewEmployeeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            EmpImage = itemview.FindViewById<ImageView>(Resource.Id.imgProfile);
            EmpName = itemview.FindViewById<TextView>(Resource.Id.textNameEmp);
            EmpPosition = itemview.FindViewById<TextView>(Resource.Id.textPositionEmp);
            ActiveImage = itemview.FindViewById<ImageView>(Resource.Id.imgShowActiveEmp);
            ActiveSwich = itemview.FindViewById<Switch>(Resource.Id.switchActive);
            Check = itemview.FindViewById<ImageView>(Resource.Id.imgCheck);

#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    public class ListOption
    {
        public List<TranDetailItemTopping> detailItemToppings;
        static List<TranDetailItemTopping> builitem;
        public ListOption(List<TranDetailItemTopping> detailItemToppings)
        {
            builitem = detailItemToppings;
            this.detailItemToppings = builitem;
        }
        public int Count
        {
            get
            {
                return detailItemToppings == null ? 0 : detailItemToppings.Count;
            }
        }
        public TranDetailItemTopping this[int i]
        {
            get { return detailItemToppings == null ? null : detailItemToppings[i]; }
        }
    }

    public class ListViewOptionHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }
        public TextView Price { get; set; }


        public ListViewOptionHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Name = itemview.FindViewById<TextView>(Resource.Id.textName);
            Price = itemview.FindViewById<TextView>(Resource.Id.textPrice);

#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    public class ListPayment
    {
        public List<TranPayment> tranPayments;
        static List<TranPayment> builitem;
        public ListPayment(List<TranPayment> tranPayments)
        {
            builitem = tranPayments;
            this.tranPayments = builitem;
        }
        public int Count
        {
            get
            {
                return tranPayments == null ? 0 : tranPayments.Count;
            }
        }
        public TranPayment this[int i]
        {
            get { return tranPayments == null ? null : tranPayments[i]; }
        }
    }
    public class ListViewPaymentHolder : RecyclerView.ViewHolder
    {
        public ImageView ImgPayment { get; set; }
        public TextView NamePayment { get; set; }
        public TextView Price { get; set; }

        public ListViewPaymentHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImgPayment = itemview.FindViewById<ImageView>(Resource.Id.imgPayment);
            NamePayment = itemview.FindViewById<TextView>(Resource.Id.txtPaymentType);
            Price = itemview.FindViewById<TextView>(Resource.Id.txtPrice);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }

    public class ListGiftVoucher
    {
        public List<GiftVoucher> vouchers;
        static List<GiftVoucher> builitem;
        public ListGiftVoucher(List<GiftVoucher> vouchers)
        {
            builitem = vouchers;
            this.vouchers = builitem;
        }
        public int Count
        {
            get
            {
                return vouchers == null ? 0 : vouchers.Count;
            }
        }
        public GiftVoucher this[int i]
        {
            get { return vouchers == null ? null : vouchers[i]; }
        }
    }

    public class ListViewGiftVoucherHolder : RecyclerView.ViewHolder
    {
        public TextView Discount { get; set; }
        public TextView GiftVoucherName { get; set; }
        public TextView GiftVoucherID { get; set; }
        public ImageButton Check { get; set; }

        public ListViewGiftVoucherHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Discount = itemview.FindViewById<TextView>(Resource.Id.textDiscount);
            GiftVoucherName = itemview.FindViewById<TextView>(Resource.Id.textGiftName);
            GiftVoucherID = itemview.FindViewById<TextView>(Resource.Id.textGiftID);
            Check = itemview.FindViewById<ImageButton>(Resource.Id.btnCheck);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }

    public class ListMyQRCode
    {
        public List<MyQrCode> myQrCodes;
        static List<MyQrCode> builitem;
        public ListMyQRCode(List<MyQrCode> myQrCodes)
        {
            builitem = myQrCodes;
            this.myQrCodes = builitem;
        }
        public int Count
        {
            get
            {
                return myQrCodes == null ? 0 : myQrCodes.Count;
            }
        }
        public MyQrCode this[int i]
        {
            get { return myQrCodes == null ? null : myQrCodes[i]; }
        }
    }

    public class ListViewMyQRCodeHolder : RecyclerView.ViewHolder
    {
        public TextView QrCodeName { get; set; }
        public TextView QrBranch { get; set; }
        public TextView QrComment { get; set; }
        public ImageView ImageQRCode { get; set; }
        public CardView CardAdapter { get; set; }
        public ListViewMyQRCodeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            QrCodeName = itemview.FindViewById<TextView>(Resource.Id.txtqrName);
            QrBranch = itemview.FindViewById<TextView>(Resource.Id.txtqrbranchName);
            QrComment = itemview.FindViewById<TextView>(Resource.Id.txtqrComment);
            ImageQRCode = itemview.FindViewById<ImageView>(Resource.Id.imgrQRCode);
            CardAdapter = itemview.FindViewById<CardView>(Resource.Id.CardAdapter);
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    public class ListBluetooth
    {
        static List<Bluetooth> builitem;
        public List<Bluetooth> bluetooths;
        public ListBluetooth()
        {
            if (GabanaModel.gabanaMain.bluetooth != null)
            {
                builitem = GabanaModel.gabanaMain.bluetooth;
                this.bluetooths = builitem;
            }
        }
        public int Count
        {
            get
            {
                return bluetooths == null ? 0 : bluetooths.Count;
            }
        }
        public Bluetooth this[int i]
        {
            get { return bluetooths == null ? null : bluetooths[i]; }
        }
    }

    public class ListViewBluetoothsHolder : RecyclerView.ViewHolder
    {
        //public TextView BluetoothID { get; set; }
        public TextView BluetoothName { get; set; }
        public TextView BluetoothStatus { get; set; }
        public ImageView BluetoothSetting { get; set; }
        public ListViewBluetoothsHolder(View itemview, Action<int> listener) : base(itemview)
        {
            //BluetoothID = itemview.FindViewById<TextView>(Resource.Id.txtBluetoothID);
            BluetoothName = itemview.FindViewById<TextView>(Resource.Id.txtBluetoothName);
            BluetoothStatus = itemview.FindViewById<TextView>(Resource.Id.txtBluetoothStatus);
            BluetoothSetting = itemview.FindViewById<ImageView>(Resource.Id.imageSetting);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }

    public class ListOrders
    {
        public List<OrderNew> Trans;
        static List<OrderNew> builitem;
        public ListOrders(List<OrderNew> tranOrder)
        {
            if (tranOrder != null)
            {
                builitem = tranOrder;
                this.Trans = builitem;
            }
        }

        public int Count
        {
            get
            {
                return Trans == null ? 0 : Trans.Count;
            }
        }

        public OrderNew this[int i]
        {
            get { return Trans == null ? null : Trans[i]; }
        }

    }

    public class ListViewOrderHolder : RecyclerView.ViewHolder
    {
        public TextView Header { get; set; }
        public TextView Name { get; set; }
        public TextView Comment { get; set; }
        public TextView Device { get; set; }
        public TextView Date { get; set; }
        public TextView Amount { get; set; }

        public ListViewOrderHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Header = itemview.FindViewById<TextView>(Resource.Id.textHead);
            Name = itemview.FindViewById<TextView>(Resource.Id.txtName);
            Comment = itemview.FindViewById<TextView>(Resource.Id.txtComment);
            Device = itemview.FindViewById<TextView>(Resource.Id.txtDevice);
            Date = itemview.FindViewById<TextView>(Resource.Id.txtDate);
            Amount = itemview.FindViewById<TextView>(Resource.Id.txtAmount);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }
    public class ListEmpRole
    {
        public List<EmployeeRole> menuitems;
        static List<EmployeeRole> builitem;
        public ListEmpRole()
        {
            if (GabanaModel.gabanaMain.empRole != null)
            {
                builitem = GabanaModel.gabanaMain.empRole;
                this.menuitems = builitem;
            }
        }
        public int Count
        {
            get
            {
                return menuitems == null ? 0 : menuitems.Count;
            }
        }
        public EmployeeRole this[int i]
        {
            get { return menuitems == null ? null : menuitems[i]; }
        }
    }

    public class ListViewEmpRoleHolder : RecyclerView.ViewHolder
    {
        public ImageView EmpPosition { get; set; }
        public TextView Name { get; set; }
        public TextView Detail { get; set; }
        public ImageButton Check { get; set; }

        public ListViewEmpRoleHolder(View itemview, Action<int> listener) : base(itemview)
        {
            EmpPosition = itemview.FindViewById<ImageView>(Resource.Id.imgPosition);
            Name = itemview.FindViewById<TextView>(Resource.Id.textName);
            Detail = itemview.FindViewById<TextView>(Resource.Id.textDetail);
            Check = itemview.FindViewById<ImageButton>(Resource.Id.btnCheck);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }

    public class ListBestSallItem
    {
        public List<Gabana3.JAM.Dashboard.BestSellingItem> bestSellings;
        static List<Gabana3.JAM.Dashboard.BestSellingItem> builitem;
        public ListBestSallItem(List<Gabana3.JAM.Dashboard.BestSellingItem> bestSellings)
        {
            builitem = bestSellings.OrderByDescending(x => x.totalAmount).ToList();
            this.bestSellings = builitem;
        }
        public int Count
        {

            get
            {
                if (bestSellings.Count <= 5)
                {
                    return bestSellings == null ? 0 : bestSellings.Count;
                }
                else
                {
                    return bestSellings == null ? 0 : 5;
                }
            }
        }
        public Gabana3.JAM.Dashboard.BestSellingItem this[int i]
        {
            get { return bestSellings == null ? null : bestSellings[i]; }
        }
    }
    public class ListViewBestSaleHolder : RecyclerView.ViewHolder
    {
        public ImageView ImageItem { get; set; }
        public TextView ShortName { get; set; }
        public TextView Name { get; set; }
        public TextView TotalSale { get; set; }
        public ImageButton ImageMoveMent { get; set; }
        public ImageView Check { get; set; }

        public ListViewBestSaleHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImageItem = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            ShortName = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            Name = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            TotalSale = itemview.FindViewById<TextView>(Resource.Id.txtTotalSale);
            ImageMoveMent = itemview.FindViewById<ImageButton>(Resource.Id.imageMoveMent);
            Check = itemview.FindViewById<ImageView>(Resource.Id.imgCheck);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }
    public class ListStockMoveMent
    {
        public List<ItemMovement> stockMovement;
        static List<ItemMovement> builitem;
        public ListStockMoveMent(List<ItemMovement> stockMovement)
        {
            builitem = stockMovement.OrderByDescending(x => x.MovementDate).ToList();
            this.stockMovement = builitem;
        }
        public int Count
        {
            get
            {
                return stockMovement == null ? 0 : stockMovement.Count;
            }
        }
        public ItemMovement this[int i]
        {
            get { return stockMovement == null ? null : stockMovement[i]; }
        }
    }
    public class ListViewStockHolder : RecyclerView.ViewHolder
    {
        public ImageView ImageStock { get; set; }
        public TextView TypeStock { get; set; }
        public TextView UserEdit { get; set; }
        public TextView Date { get; set; }
        public TextView Unit { get; set; }

        public ListViewStockHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImageStock = itemview.FindViewById<ImageView>(Resource.Id.imageStock);
            TypeStock = itemview.FindViewById<TextView>(Resource.Id.txtTypeStock);
            UserEdit = itemview.FindViewById<TextView>(Resource.Id.txtUserEdit);
            Date = itemview.FindViewById<TextView>(Resource.Id.txtDate);
            Unit = itemview.FindViewById<TextView>(Resource.Id.txtUnit);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }
    public class ListTimeReport
    {
        public List<TimeReport> times;
        static List<TimeReport> builitem;
        public ListTimeReport(List<TimeReport> times)
        {
            builitem = times;
            this.times = builitem;
        }
        public int Count
        {
            get
            {
                return times == null ? 0 : times.Count;
            }
        }
        public TimeReport this[int i]
        {
            get { return times == null ? null : times[i]; }
        }
    }

    public class ListViewTimeReporHolder : RecyclerView.ViewHolder
    {
        public TextView Time { get; set; }
        public TextView Amount { get; set; }
        public ImageView Profile { get; set; }
        public ImageView Position { get; set; }
        public TextView TextPosition { get; set; }

        public ListViewTimeReporHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Time = itemview.FindViewById<TextView>(Resource.Id.txtDate);
            Amount = itemview.FindViewById<TextView>(Resource.Id.txtAmount);
            Profile = itemview.FindViewById<ImageView>(Resource.Id.imageCustomer);
            Position = itemview.FindViewById<ImageView>(Resource.Id.imagePosition);
            TextPosition = itemview.FindViewById<TextView>(Resource.Id.txtPositionEmp);
 
            itemview.Click += (sender, e) => listener(base.Position);

        }
    }
    //public class ListPaymentType
    //{
    //    public List<PaymentType> payments;
    //    static List<PaymentType> builitem;
    //    public ListPaymentType(List<PaymentType> lstPayment)
    //    {
    //        builitem = lstPayment;
    //        this.payments = builitem;
    //    }
    //    public int Count
    //    {
    //        get
    //        {
    //            return payments == null ? 0 : payments.Count;
    //        }
    //    }
    //    public PaymentType this[int i]
    //    {
    //        get { return payments == null ? null : payments[i]; }
    //    }

    //}
    public class ListViewPaymentReporHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; set; }
        public TextView Type { get; set; }
        public TextView Amount { get; set; }
        public TextView Percent { get; set; }

        public ListViewPaymentReporHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Image = itemview.FindViewById<ImageView>(Resource.Id.imageColor);
            Type = itemview.FindViewById<TextView>(Resource.Id.txtType);
            Amount = itemview.FindViewById<TextView>(Resource.Id.txtAmount);
            Percent = itemview.FindViewById<TextView>(Resource.Id.txtPercent);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }

    public class ListViewMembertypeHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }

        public ListViewMembertypeHolder(View itemview, Action<int> listener) : base(itemview)
        {

            Name = itemview.FindViewById<TextView>(Resource.Id.txtName);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }
    public class ListViewBestEmpeHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }
        public TextView TotalSale { get; set; }
        public ProgressBar Progress { get; set; }

        public ListViewBestEmpeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Name = itemview.FindViewById<TextView>(Resource.Id.txtName);
            TotalSale = itemview.FindViewById<TextView>(Resource.Id.txtTotalSale);
            Progress = itemview.FindViewById<ProgressBar>(Resource.Id.progressBar);


 
#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete
 

        }
    }



}