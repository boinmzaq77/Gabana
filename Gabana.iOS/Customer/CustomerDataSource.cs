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
using System.IO;

namespace Gabana.iOS
{
    public class CustomerDataSource : UICollectionViewDataSource
    {
        
        CustomerManage setCustomer = new CustomerManage();
        public int merchantID;
        public UICollectionView collectionView;
        private List<Gabana.ORM.MerchantDB.Customer> customers;
        public CustomerCollectionViewCell choosecell;

        public CustomerDataSource(List<ORM.MerchantDB.Customer> cus)
        {
            this.customers = cus;
            //lstCustomer
        }

        public void ReloadData(List<Customer> cus)
        {
            this.customers = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("CustomerViewCell", indexPath) as CustomerCollectionViewCell;
            this.collectionView = collectionView;
            cell.Name = this.customers[(int)indexPath.Row].CustomerName;
            if (MainController.checknet)
            {
                if (string.IsNullOrEmpty(this.customers[(int)indexPath.Row].ThumbnailLocalPath))
                {
                    if (string.IsNullOrEmpty(this.customers[(int)indexPath.Row].PicturePath))
                    {
                        cell.Imageghavenet = null;
                    }
                    else
                    {
                        cell.Imageghavenet = this.customers[(int)indexPath.Row];
                    }
                }
                else
                {
                    
                    cell.Imageghavenet = this.customers[(int)indexPath.Row];
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.customers[(int)indexPath.Row].ThumbnailLocalPath))
                {
                    cell.Imagegnothavenet = null;
                    
                }
                else
                {
                    
                    cell.Imagegnothavenet = this.customers[(int)indexPath.Row];
                }
            }
            //if ((string.IsNullOrEmpty(this.customers[(int)indexPath.Row].PicturePath) && CustomerController.checknet) || (string.IsNullOrEmpty(this.customers[(int)indexPath.Row].ThumbnailLocalPath) && !CustomerController.checknet))
            //{
            //    cell.Image = null;
            //}
            //else
            //{
            //    //var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //    cell.Image = this.customers[(int)indexPath.Row];
            //}
            
            if (choosecell != null)
            {
                if (choosecell == cell)
                {
                    choosecell = null;
                }
            }
            cell.OnItemSwipe -= Cell_OnItemSwipe;
            cell.OnItemSwipe += Cell_OnItemSwipe;
            cell.OnDeleteItem -= Cell_OnDeleteItem;
            cell.OnDeleteItem += Cell_OnDeleteItem;
            cell.OnItem -= Cell_OnItem;
            cell.OnItem += Cell_OnItem;
            OnCardscrollCell?.Invoke(indexPath);
            return cell;
        }

        private void Cell_OnDeleteItem(CustomerCollectionViewCell indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }
        private void Cell_OnItem(CustomerCollectionViewCell indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCell?.Invoke(indexPathQRcode);
        }
        private void Cell_OnItemSwipe(CustomerCollectionViewCell indexPath)
        {
             
            if (choosecell != null)
            {
                UIView.Animate(0.7, () =>
                {
                    var frame2 = choosecell.Frame;
                    frame2.X = 0;
                    choosecell.showbtndelete = false;
                    choosecell.Frame = frame2;
                });
            };
            choosecell = indexPath;
            UIView.Animate(0.7, () =>
            {
                var frame = indexPath.Frame;
                frame.X = -80;
                choosecell.showbtndelete = true;
                indexPath.Frame = frame;
            });
             
        }
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.customers != null)
                return this.customers.Count;
            else
                return 0;
        }

        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;

        public delegate void CardCell(NSIndexPath indexPath);
        public event CardCell OnCardCell;

        public delegate void CardscrollCell(NSIndexPath indexPath);
        public event CardscrollCell OnCardscrollCell;
    }
}