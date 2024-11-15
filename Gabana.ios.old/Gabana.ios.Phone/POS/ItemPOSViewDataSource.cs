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
    public class ItemPOSViewDataSource : UICollectionViewDataSource
    {
        public List<itemPOS> mainItems;
        public string filter;
        public int Flag;
        public ItemPOSViewDataSource(List<itemPOS> mainItem,string filTer,int flag)
        {
            mainItems = mainItem;
            filter = filTer;
            Flag = flag;
        }

        
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            //menuItem[(int)x.Row].POSMenuName
            itemPOSViewCell ItemnViewCell = collectionView.DequeueReusableCell("itemPOSViewCell", indexPath) as itemPOSViewCell;
            ItemnViewCell.Name = null;
            ItemnViewCell.Price = null;
            ItemnViewCell.Image = "";
            if (filter!="" && filter!=null && Flag == 0)
            {
                if (mainItems[(int)indexPath.Row].itemName != "+")
                {
                    if (filter == mainItems[(int)indexPath.Row].itemType)
                    {
                        ItemnViewCell.Name = mainItems[(int)indexPath.Row].itemName;
                        ItemnViewCell.Price = mainItems[(int)indexPath.Row].itemCost.ToString("N2");
                        ItemnViewCell.Image = mainItems[(int)indexPath.Row].itemImage;
                    }
                }
                else
                {
                    ItemnViewCell.Name = " ";
                }
            }
            else if(filter ==null && Flag==0)
            {
                if (mainItems[(int)indexPath.Row].itemName != "+")
                {
                    ItemnViewCell.Name = mainItems[(int)indexPath.Row].itemName;
                    ItemnViewCell.Price = mainItems[(int)indexPath.Row].itemCost.ToString("N2");
                    ItemnViewCell.Image = mainItems[(int)indexPath.Row].itemImage;
                }
                else
                {
                    ItemnViewCell.Name = " ";
                }
            }
            else
            {
                //flag =1 is scan page list
                ItemnViewCell.Name = mainItems[(int)indexPath.Row].itemName;
                ItemnViewCell.Price = mainItems[(int)indexPath.Row].itemCost.ToString("N2");
                ItemnViewCell.Image = mainItems[(int)indexPath.Row].itemImage;
            }

            if(ItemnViewCell.Name == null && ItemnViewCell.Price == null)
            {
                ItemnViewCell.Hidden = true;
            }
            else
            {
                ItemnViewCell.Hidden = false;
            }
            return ItemnViewCell;
        }
        

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return mainItems.Count;
        }
    }
}