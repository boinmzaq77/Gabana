using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class EmployeeManageViewCell : UICollectionViewCell
    {
        UILabel lblEmployeeName,lblEmployeeRole;
        UISwitch statusEmp;
        UIView line;
        char ActiveEmp;

        public EmployeeManageViewCell(IntPtr handle) : base(handle)
        {
            lblEmployeeName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblEmployeeName.TextColor = UIColor.FromRGB(64,64,64);
            lblEmployeeName.TextAlignment = UITextAlignment.Left;
            lblEmployeeName.Font = lblEmployeeName.Font.WithSize(15);
            ContentView.AddSubview(lblEmployeeName);

            lblEmployeeRole = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblEmployeeRole.TextColor = UIColor.FromRGB(162,162,162);
            lblEmployeeRole.TextAlignment = UITextAlignment.Left;
            lblEmployeeRole.Font = lblEmployeeRole.Font.WithSize(15);
            ContentView.AddSubview(lblEmployeeRole);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226,226,226);
            ContentView.AddSubview(line);

            statusEmp = new UISwitch();
            statusEmp.TranslatesAutoresizingMaskIntoConstraints = false;
            statusEmp.OnTintColor = UIColor.FromRGB(0, 149, 218);
            statusEmp.ValueChanged += async (sender, e) =>
            {
               

                var username = lblEmployeeName.Text;
                var userUpdate = DataCashingAll.UserAccountInfo.Where(x => x.UserName == username).FirstOrDefault();
                if (statusEmp.On)
                {
                    ActiveEmp = 'A';
                }
                else if (!statusEmp.On)
                {
                    ActiveEmp = 'I';
                }

                var UpdateCloundProduct = await GabanaAPI.PutSeAuthDataUserAccessProducts(username, ActiveEmp);
                if (UpdateCloundProduct.Status)
                {
                    if (ActiveEmp == 'A')
                    {
                        userUpdate.UserAccessProduct = true;
                        statusEmp.On = true;
                    }
                    else
                    {
                        userUpdate.UserAccessProduct = false;
                        statusEmp.On = false;
                    }
                }
                else
                {
                    if (userUpdate.UserAccessProduct)
                    {
                        statusEmp.On = true;
                    }
                    else
                    {
                        statusEmp.On = false;
                    }
                }
            };
            ContentView.AddSubview(statusEmp);

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            lblEmployeeName.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor,-1).Active = true;
            lblEmployeeName.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblEmployeeName.RightAnchor.ConstraintEqualTo(statusEmp.SafeAreaLayoutGuide.LeftAnchor,-5).Active = true;

            lblEmployeeRole.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, -1).Active = true;
            lblEmployeeRole.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblEmployeeRole.WidthAnchor.ConstraintEqualTo(250).Active = true;

            statusEmp.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor,0).Active = true;
            statusEmp.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            statusEmp.WidthAnchor.ConstraintEqualTo(51).Active = true;
            statusEmp.HeightAnchor.ConstraintEqualTo(31).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        public bool switchEnable
        {
            get
            {
                return statusEmp.Enabled;
            }
            set
            {
                if (value)
                {
                    statusEmp.Enabled = true;
                }
                else
                {
                    statusEmp.Enabled = false;
                }
            }
        }
        public string Name
        {
            get { return lblEmployeeName.Text; }
            set { lblEmployeeName.Text = value; }
        }
        public string Permit
        {
            get { return lblEmployeeRole.Text; }
            set { lblEmployeeRole.Text = value; }
        }
        public int Status
        {
            get{
                if (statusEmp.On)
                {
                    return 1;
                }
                else if (!statusEmp.On)
                {
                    return 0;
                }
                else
                    return 99;
            }
            set {
                if (value == 1)
                {
                    statusEmp.On = true;
                    statusEmp.Hidden = false;
                }
                else if(value == 0)
                {
                    statusEmp.On = false;
                    statusEmp.Hidden = false;
                }
                else
                {
                    statusEmp.Hidden = true;
                }
            }
        }
    }
}