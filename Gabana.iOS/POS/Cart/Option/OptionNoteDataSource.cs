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

namespace Gabana.iOS
{
    public class OptionNoteDataSource : UICollectionViewDataSource
    {
        public List<Note> allNote = new List<Note>();
        CoreGraphics.CGRect frame;
        public OptionNoteDataSource(List<Note> all, CoreGraphics.CGRect frame)
        {
            this.allNote = all;
            this.frame = frame;
        }
        public void ReloadData(List<Note> all)
        {
            this.allNote = all;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("optionNoteViewCellList", indexPath) as OptionNoteCollectionViewCell;
            cell.Name = allNote[(int)indexPath.Row].Message ?? "";
            cell.size = frame.Width;
             UIView.Animate(0.7, () =>
                {
                    Utils.SetConstant(collectionView.Constraints, NSLayoutAttribute.Height, (int)cell?.Frame.Y + 45);
                });

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return allNote.Count;
        }

        #region Events
        public delegate void NoteSelectIndexDelegate(NSIndexPath indexPath);
        public event NoteSelectIndexDelegate OnNoteSelectIndex;
        #endregion
    }
}