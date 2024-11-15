using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using System.Threading.Tasks;

namespace Gabana.iOS
{
    public class ItemNoteDataSourceList : UICollectionViewDataSource
    {
        public List<Gabana.ORM.MerchantDB.TranDetailItem> note;
       // Gabana.ShareSource.Manage.CategoryManage category = new Gabana.ShareSource.Manage.CategoryManage();
       public ItemNoteDataSourceList()
       {
            ItemNoteDataList();
       }
        public async Task ItemNoteDataList()
        {
            this.note = new List<Gabana.ORM.MerchantDB.TranDetailItem>();
            // this.Categories = await category.GetAllCategory();
            this.note.Add(new TranDetailItem
            {
                MerchantID = 1,
                SysBranchID = 1,
                TranNo = "1",
                DetailNo = 1,
                ItemName = "test1",
                SaleItemType = 'A',
                FProcess = 1,
                TaxType = 'V',
                Quantity = '5',
                Price = 500,
                SubAmount = 10,
                Discount = 10,
                Amount = 10,
                VatAmount = 10,
                TaxBaseAmount = 10,
                //TotalCost = 500
            });
            
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell1 = collectionView.DequeueReusableCell("itemNoteCellList", indexPath) as itemNoteCollectionViewCellList;
            cell1.NoteItem = this.note[(int)indexPath.Row].Discount.ToString();
          
            return cell1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.note.Count;
        }
    }
}