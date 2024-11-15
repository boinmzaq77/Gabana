using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class POSheaderViewCell : UICollectionViewCell
    {
        public POSheaderViewCell (IntPtr handle) : base (handle)
        {
        }
        public POSheaderViewCell()
        {

        }
        public string Name
        {
            get { return menuPOSName.Text; }
            set { menuPOSName.Text = value; }
        }
       
    }
}