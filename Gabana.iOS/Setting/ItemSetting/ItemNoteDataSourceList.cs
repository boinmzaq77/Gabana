using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using System.Threading.Tasks;

using Gabana.ShareSource.Manage;

namespace Gabana.iOS
{
    public class ItemNoteDataSourceList : UICollectionViewDataSource
    {
        public List<Note> note;
        public UICollectionView collectionView;
        List<NoteCategory> CateList = new List<NoteCategory>();
        NoteCategoryManage Cate = new NoteCategoryManage();
        public itemNoteCollectionViewCellList choosecell;

        public ItemNoteDataSourceList(List<Note> noteList)
       {
            this.note = noteList;
            GetCatList();
           
       }
        public async void GetCatList()
        {
            CateList = await Cate.GetAllNoteCategory();
        }
        public void ReloadData(List<NoteCategory> CateList,List<Note> item)
        {
            this.CateList = CateList;
            this.note = item;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.collectionView = collectionView;
            var cell1 = collectionView.DequeueReusableCell("itemNoteCellList", indexPath) as itemNoteCollectionViewCellList;
            var cateName = CateList.Where(x => x.SysNoteCategoryID == this.note[(int)indexPath.Row].SysNoteCategoryID).Select(c=>c.Name).FirstOrDefault();
            cell1.NoteItem = this.note[(int)indexPath.Row].Message;
            cell1.NoteCategory = cateName;
            if (choosecell != null)
            {
                if (choosecell == cell1)
                {
                    choosecell = null;
                }
            }
            cell1.OnItemSwipe -= Cell_OnItemSwipe;
            cell1.OnItemSwipe += Cell_OnItemSwipe;
            cell1.OnDeleteItem -= Cell_OnDeleteItem;
            cell1.OnDeleteItem += Cell_OnDeleteItem;
            return cell1;
        }

        private void Cell_OnDeleteItem(itemNoteCollectionViewCellList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.note.Count;
        }
        private void Cell_OnItemSwipe(itemNoteCollectionViewCellList indexPath)
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
                frame.X = -60;
                choosecell.showbtndelete = true;
                indexPath.Frame = frame;
            });
        }
        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;
    }
}