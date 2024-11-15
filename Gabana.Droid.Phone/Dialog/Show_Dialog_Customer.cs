using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using System;

namespace Gabana.Droid
{
    public class Show_Dialog_Customer : Android.Support.V4.App.DialogFragment
    {
        public static Show_Dialog_Item main;
        private static string path;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }


        public static Show_Dialog_Customer NewInstance(string _path)
        {
            path = _path;
            var frag = new Show_Dialog_Customer { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.dialog_customer, container, false);
            try
            {
                string pathPlaceholder = "@mipmap/placeholder_card";
                Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
                FFImageLoading.Views.ImageViewAsync imageViewAsync = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.imgView);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete

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