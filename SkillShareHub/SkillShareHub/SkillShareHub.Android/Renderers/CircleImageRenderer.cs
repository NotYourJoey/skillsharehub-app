using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using SkillShareHub.Controls;
using SkillShareHub.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CircleImage), typeof(CircleImageRenderer))]
namespace SkillShareHub.Droid.Renderers
{
    public class CircleImageRenderer : ImageRenderer
    {
        public CircleImageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Control != null && e.NewElement != null)
            {
                CreateCircleImage();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Image.SourceProperty.PropertyName)
            {
                CreateCircleImage();
            }
        }

        private void CreateCircleImage()
        {
            try
            {
                var drawable = Control?.Drawable;
                if (drawable == null)
                    return;

                var bitmap = ((BitmapDrawable)drawable).Bitmap;
                if (bitmap == null)
                    return;

                var width = bitmap.Width;
                var height = bitmap.Height;
                var size = Math.Min(width, height);

                var x = (width - size) / 2;
                var y = (height - size) / 2;

                var squareBitmap = Bitmap.CreateBitmap(bitmap, x, y, size, size);
                var output = Bitmap.CreateBitmap(size, size, Bitmap.Config.Argb8888);

                using (var canvas = new Canvas(output))
                {
                    var paint = new Paint
                    {
                        AntiAlias = true,
                        Color = Android.Graphics.Color.White
                    };

                    // Use fully qualified names for Rect and RectF
                    var rect = new Android.Graphics.Rect(0, 0, size, size);
                    var rectF = new RectF(rect);
                    var radius = size / 2f;

                    canvas.DrawARGB(0, 0, 0, 0);
                    canvas.DrawRoundRect(rectF, radius, radius, paint);

                    paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                    canvas.DrawBitmap(squareBitmap, rect, rect, paint);
                }

                var roundedDrawable = new BitmapDrawable(Context.Resources, output);
                Control.SetImageDrawable(roundedDrawable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating circle image: {ex.Message}");
            }
        }
    }
}
