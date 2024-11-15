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
    public class POSMainItemViewDataSource : UICollectionViewDataSource
    {
        public List<POSMenuitem> POSmainItems = new List<POSMenuitem>();

        public POSMainItemViewDataSource(List<POSMenuitem> mainItem)
        {
            POSmainItems = mainItem;
        }

        
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            POSheaderViewCell OSmainItemnViewCell = collectionView.DequeueReusableCell("POSheaderViewCell", indexPath) as POSheaderViewCell;
            OSmainItemnViewCell.Name = POSmainItems[(int)indexPath.Row].POSMenuName;
            return OSmainItemnViewCell;
        }
        

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return POSmainItems.Count;
        }
    }
}