using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet
{
    public class DialogLoading : AndroidX.Fragment.App.DialogFragment
    {
        public static DialogLoading main;
#pragma warning disable CS0649 // Field 'DialogLoading.context' is never assigned to, and will always have its default value null
        Context context;
#pragma warning restore CS0649 // Field 'DialogLoading.context' is never assigned to, and will always have its default value null
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static DialogLoading NewInstance()
        {
            var frag = new DialogLoading { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.dialog_loading, container, false);
            Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            ImageViewAsync imageViewAsync = view.FindViewById<ImageViewAsync>(Resource.Id.imgView);
            try
            {
                ImageService.Instance.LoadCompiledResource("@drawable/gabana_loading")
                .WithCache(FFImageLoading.Cache.CacheType.Memory)
                .Into(imageViewAsync);

            }
            catch (Exception ex)
            {
                Toast.MakeText(context, "some error occured, please check your network" + ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }
        public static void CloseDialog()
        {
            if (main != null) main.Dismiss();
        }
        public override void Dismiss()
        {
            base.Dismiss();
        }
    }
}