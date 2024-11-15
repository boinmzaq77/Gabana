using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFImageLoading;
using Foundation;
using Gabana.Model;
using UIKit;

namespace Gabana.ios.Phone
{
    public class PaymentViewDataSource : UICollectionViewDataSource
    {
        public List<MenuPayment> mainItems;
        public PaymentViewDataSource(List<MenuPayment> mainItem)
        {
            mainItems = mainItem;
        }

        
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            PaymentViewCell mainItemnViewCell = collectionView.DequeueReusableCell("PaymentViewCell", indexPath) as PaymentViewCell;
            mainItemnViewCell.Name = mainItems[(int)indexPath.Row].itemName;
            mainItemnViewCell.Image = mainItems[(int)indexPath.Row].itemImage;
           //  add item add 
            return mainItemnViewCell;
        }
        

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return mainItems.Count;
        }
    }
}