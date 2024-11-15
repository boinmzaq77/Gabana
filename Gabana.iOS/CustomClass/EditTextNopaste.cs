using Foundation;
using ObjCRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.CustomClass
{
    internal class EditTextNopaste : UITextField
    {
        public override bool CanPerform(Selector action, NSObject withSender)
        {
            if (action == new Selector("paste:"))
                return false;
            else
                
            return base.CanPerform(action, withSender);
        }
    }
}