using Foundation;
using System;
using UIKit;

namespace Gabana.iOS
{
    public partial class BluetoothViewCellSetting : UICollectionViewCell
    {
        UILabel musicName;
        public BluetoothViewCellSetting(IntPtr handle) : base (handle)
        {
            ContentView.Layer.BorderWidth = 1.0f;
            ContentView.Layer.BorderColor = new CoreGraphics.CGColor(red: 162 / 255f, green: 162 / 255f, blue: 162 / 255f, alpha: 1);
            musicName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            musicName.BackgroundColor = UIColor.White;
            ContentView.AddSubview(musicName);
            //ContentView.BackgroundColor = UIColor.Blue;

            musicName.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 5).Active = true;
            musicName.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 5).Active = true;
            musicName.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -5).Active = true;
            musicName.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -5).Active = true;
        }

        public string MusicName
        {
            get
            {
                return musicName.Text;
            }

            set
            {
                musicName.Text = value;

            }
        }
    }
}