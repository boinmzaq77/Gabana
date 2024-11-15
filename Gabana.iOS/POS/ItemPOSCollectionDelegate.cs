using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class ItemPOSCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemPOSCollectionViewCell cell = collectionView.CellForItem(indexPath) as ItemPOSCollectionViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }
            
            //cell.Alpha = 1;
            //var x = cell.Layer.Position;
            //var xx = cell.Layer.Position;
            //UIView.Animate(0.5, () =>
            //{
              
            //    x.X = collectionView.Frame.Width/2;
            //    x.Y = x.Y + collectionView.Frame.Height;
                
            //    cell.Layer.Position = x;
            //    //cell.Alpha = 0;
                
            //    cell.WidthAnchor.ConstraintEqualTo(0);
            //},()=> 
            //{
            //    cell.Layer.Position = xx;
            //    cell.Alpha = 1;

            //});
            

            OnItemSelected?.Invoke(indexPath);
            return true;
        }
        
         
        #region Events
        public delegate void itemPOSSelectedDelegate(NSIndexPath indexPath);
        public event itemPOSSelectedDelegate OnItemSelected;
        #endregion

    }
}