using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ORM.MerchantDB;
using System.Threading.Tasks;
using static Gabana.iOS.CustomerController;
using Gabana3.JAM.Trans;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class OrderDataSource : UICollectionViewDataSource
    {
        //public List<Gabana.ORM.MerchantDB.Branch> branches;
        private List<OrderNew> order;
        UIViewController uIView;
        public OrderDataSource(List<OrderNew> order, UIViewController orderController)
        {
            this.order = order;
            this.uIView = orderController;
            //lstCustomer
        }

        public void ReloadData(List<OrderNew> order, UIViewController orderController)
        {
            this.order = order;
            this.uIView = orderController;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell;
            if (this.order[(int)indexPath.Row].Fhead)
            {
                cell = collectionView.DequeueReusableCell("OrderHeadCollectionViewCell", indexPath) as OrderHeadCollectionViewCell;
                (cell as OrderHeadCollectionViewCell).Name = this.order[(int)indexPath.Row].orderName;
                (cell as OrderHeadCollectionViewCell).sizeWidth = (int)this.uIView.View.Frame.Width;
            }
            else
            {
                cell = collectionView.DequeueReusableCell("OrderViewCell", indexPath) as OrderCollectionViewCell;
                (cell as OrderCollectionViewCell).Name = this.order[(int)indexPath.Row].orderName;
                (cell as OrderCollectionViewCell).price = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(this.order[(int)indexPath.Row].grandTotal);
                (cell as OrderCollectionViewCell).date = Utils.ShowDate(this.order[(int)indexPath.Row].tranDate);
                (cell as OrderCollectionViewCell).device = "Device" + this.order[(int)indexPath.Row].deviceNo;
                (cell as OrderCollectionViewCell).sizeWidth = (int)this.uIView.View.Frame.Width;
                var com = "";
                if (!String.IsNullOrEmpty(this.order[(int)indexPath.Row].comments))
                {
                    com = ", " + this.order[(int)indexPath.Row].comments;
                }
                (cell as OrderCollectionViewCell).comment = com;
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.order.Count;
        }
    }
}