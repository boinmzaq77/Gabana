using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using System;

namespace Gabana.Droid
{
    public class DialogLoading : Android.Support.V4.App.DialogFragment
    {
        public static DialogLoading main;
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
            main = this;

            Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
            FFImageLoading.Views.ImageViewAsync imageViewAsync = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.imgView);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
            try
            {
                ImageService.Instance.LoadCompiledResource("@drawable/gabana_loading")
                .WithCache(FFImageLoading.Cache.CacheType.Memory)
                .Into(imageViewAsync);

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "some error occured, please check your network" + ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }
        public static void CloseDialog()
        {
            if (main != null) main.Dismiss();
        }
        public override void Dismiss()
        {
            if (main != null)
            {
                base.Dismiss();
            }
        }
    }
}