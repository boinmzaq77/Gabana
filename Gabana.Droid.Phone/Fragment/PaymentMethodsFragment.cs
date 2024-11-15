using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;

namespace Gabana.Droid
{
    public class PaymentMethodsFragment : Android.Support.V4.App.Fragment
    {
        public PaymentMethodsFragment Payment;
        Context context; 


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.payment_fragment_methods, container, false);
            Payment = this;
            
            return view;
        }

        public static PaymentMethodsFragment NewInstance()
        {
            var frag = new PaymentMethodsFragment { Arguments = new Bundle() };
            return frag;
        }
    }
}