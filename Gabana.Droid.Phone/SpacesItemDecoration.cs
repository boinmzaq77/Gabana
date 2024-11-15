using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace Gabana.Droid.Phone
{
    internal class SpacesItemDecoration : RecyclerView.ItemDecoration
    {
        private int space;

        public SpacesItemDecoration(int _space)
        {
            this.space = _space;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);
            //outRect.Top = space;
            //outRect.Bottom = space;
            //= outRect.Left = outRect.Right = v;

        }
    }
}