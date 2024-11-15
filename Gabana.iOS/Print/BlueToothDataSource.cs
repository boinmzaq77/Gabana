using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace Gabana.iOS
{
    /// <summary>
    /// Waterfall collection data source.
    /// </summary>
    /// <remarks>
    /// Origionally created by Nicholas Tau on 6/30/14.
    /// Copyright (c) 2014 Nicholas Tau. All rights reserved.
    /// Ported from http://nshint.io/blog/2015/07/16/uicollectionviews-now-have-easy-reordering/ to
    /// Xamarin.iOS by Kevin Mullins.
    /// </remarks>
    public class BluetoothDataSource : UICollectionViewDataSource
    {
        List<Plugin.BLE.Abstractions.Contracts.IDevice> listfilter;

        #region Constructors
        public BluetoothDataSource(List<Plugin.BLE.Abstractions.Contracts.IDevice> listfilter)
        {
             this.listfilter = listfilter;
        }
        //#endregion

        public void ReloadData(List<Plugin.BLE.Abstractions.Contracts.IDevice> listfilter)
        {
            this.listfilter = listfilter;
        }



        //#region Override Methods
        public override nint NumberOfSections(UICollectionView collectionView)
        {
            // We only have one section
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            // Return the number of items
            //return name.Count;
            return listfilter.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            // Get a reusable cell and set it's title from the item
            var cell = collectionView.DequeueReusableCell("MyCell", indexPath) as BluetoothViewCellSetting;
            //cell.MusicName = PlayLists[(int)indexPath.Item];
            cell.MusicName = listfilter[(int)indexPath.Row].Name;
            return cell;
        }

        public override bool CanMoveItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            // We can always move items
            return false;
        }

        public override void MoveItem(UICollectionView collectionView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            // Reorder our list of items
            //var item = Numbers [(int)sourceIndexPath.Item];
            //Numbers.RemoveAt ((int)sourceIndexPath.Item);
            //Numbers.Insert ((int)destinationIndexPath.Item, item);
        }
        #endregion
        
    }
}

