using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;

namespace Gabana.Droid.Tablet.Helper
{
    public class MyButton
    {
        private int imageResId, textSize, pos;
        private string text, color;
        private RectF clickRegion;
        private MyButtonClickListener listener;
        private Context context;
        private Resources resources;
        public MyButton(Context context, string text, int textSize, int imageResId, string color, MyButtonClickListener listener)
        {
            this.context = context;
            this.text = text;
            this.textSize = textSize;
            this.imageResId = imageResId;
            this.color = color;
            this.listener = listener;
            resources = context.Resources;
        }

        public bool OnClick(float x, float y)
        {
            if (clickRegion != null && clickRegion.Contains(x, y))
            {
                listener.OnClick(pos);
                return true;
            }
            return false;
        }

        public void OnDraw(Canvas c, RectF rectF, int pos)
        {
            Paint p = new Paint();
            p.Color = Color.ParseColor(color);
            c.DrawRect(rectF, p);

            //text
            p.Color = Color.White;
            p.TextSize = textSize;

            Rect r = new Rect();
            float cHeight = rectF.Height();
            float cWidth = rectF.Width();
            p.TextAlign = Paint.Align.Left;
            p.GetTextBounds(text, 0, text.Length, r);
            float x = 0, y = 0;
            if (imageResId == 0)
            {
                x = cWidth / 2f - r.Width() / 2f - r.Left;
                y = cHeight / 2f + r.Height() / 2f - r.Bottom;
                c.DrawText(text, rectF.Left + x, rectF.Top + y, p);
            }
            else
            {
                Drawable d = ContextCompat.GetDrawable(context, imageResId);
                Bitmap bitmap = DrawableToBitmap(d);
                //c.DrawBitmap(bitmap, (rectF.Left + rectF.Right) / 2, (rectF.Top + rectF.Bottom) / 2, p);
                c.DrawBitmap(bitmap, rectF.Left + 5, rectF.Top + 5, p);
            }
            clickRegion = rectF;
            this.pos = pos;
        }

        private Bitmap DrawableToBitmap(Drawable d)
        {
            if (d is BitmapDrawable)
                return ((BitmapDrawable)d).Bitmap;
            Bitmap bitmap = Bitmap.CreateBitmap(d.IntrinsicWidth, d.IntrinsicHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            d.SetBounds(0, 0, canvas.Width, canvas.Height);
            d.Draw(canvas);
            return bitmap;
        }
    }
}