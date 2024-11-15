using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Gabana.iOS.POS
{
	class MenuBarCollectionViewLayout : UICollectionViewLayout
	{
		private List<UICollectionViewLayoutAttributes> layoutAttributes;
		private nfloat contentWidth = 0;

		public MenuBarCollectionViewLayout()
		{
		}

       
        public override void PrepareLayout()
		{
			layoutAttributes = new List<UICollectionViewLayoutAttributes>();

			var itemCount = CollectionView.NumberOfItemsInSection(0);
			nfloat xOffset = 0;
			nfloat yOffset = 0;
			for (nint n = 0; n < itemCount; n++)
			{
				var indexPath = NSIndexPath.FromRowSection(n, 0);
				var itemSize = (SizeForItem == null) ? new CGSize(100, 100) : SizeForItem(CollectionView, this, indexPath);
				UICollectionViewLayoutAttributes attributes = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
				attributes.Frame = new CGRect(xOffset, yOffset, itemSize.Width, itemSize.Height);
				// change color
				layoutAttributes.Add(attributes);
				xOffset += itemSize.Width;
			}
			contentWidth = xOffset;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
		{
			var attributes = new List<UICollectionViewLayoutAttributes>();
			for (int i = 0; i < layoutAttributes.Count; i++)
			{
				if (layoutAttributes[i].Frame.IntersectsWith(rect))
				{
					attributes.Add(layoutAttributes[i]);
				}
			}
			return attributes.ToArray();
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath path)
		{
			return layoutAttributes[path.Row];
		}


		public override CGSize CollectionViewContentSize
		{
			get
			{
				if (CollectionView.NumberOfSections() == 0)
					return new CGSize(0f, 0f);

				var contentSize = CollectionView.Bounds.Size;
				contentSize.Width = contentWidth;
				return contentSize;
			}
		}


		#region Events
		public delegate CGSize MenuBarCollectionSizeDelegate(UICollectionView collectionView, MenuBarCollectionViewLayout layout, NSIndexPath indexPath);
		public event MenuBarCollectionSizeDelegate SizeForItem;
		#endregion

	}
}