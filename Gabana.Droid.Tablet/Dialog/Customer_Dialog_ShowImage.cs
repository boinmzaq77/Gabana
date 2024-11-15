using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Customer_Dialog_ShowImage : AndroidX.Fragment.App.DialogFragment
    {
        public static Customer_Dialog_ShowImage main;
        private static string path;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Customer_Dialog_ShowImage NewInstance(string _path)
        {
            path = _path;
            var frag = new Customer_Dialog_ShowImage { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.customer_dialog_showimage, container, false);
            try
            {
                string pathPlaceholder = "@mipmap/placeholder_card";
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                FFImageLoading.Views.ImageViewAsync imageViewAsync = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.imgView);

                if (path != null)
                {
                    if (path.Contains("http"))
                    {
                        if (!string.IsNullOrEmpty(path))
                        {
                            ImageService.Instance.LoadUrl(path)
                                .LoadingPlaceholder(pathPlaceholder, FFImageLoading.Work.ImageSource.CompiledResource)
                                .WithCache(FFImageLoading.Cache.CacheType.Disk)
                                .Into(imageViewAsync);
                        }
                    }
                    else
                    {
                        Android.Net.Uri uri = Android.Net.Uri.Parse(path);
                        imageViewAsync.SetImageURI(uri);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
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
        internal static void SetPathImage(string i)
        {
            path = i;
        }
    }
}