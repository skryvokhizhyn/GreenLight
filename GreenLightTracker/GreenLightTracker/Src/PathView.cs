using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;

namespace GreenLightTracker.Src
{
    class PathView : View
    {
        public float[] Points { private get; set; }

        public PathView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public void SetPoints(ICollection<GpsCoordinate> coordinates)
        {
            Points = new float[coordinates.Count * 2];

            var xMin = Double.MaxValue;
            var yMin = Double.MaxValue;
            var xMax = Double.MinValue;
            var yMax = Double.MinValue;

            int i = 0;

            foreach (var coord in coordinates)
            {
                xMin = Math.Min(xMin, coord.x);
                yMin = Math.Min(yMin, coord.y);

                xMax = Math.Max(xMax, coord.x);
                yMax = Math.Max(yMax, coord.y);

                Points[i++] = (float)coord.x;
                Points[i++] = (float)coord.y;
            }

            var xDiff = xMax - xMin;
            var yDiff = yMax - yMin;

            var sz = Math.Min(Width, Height);

            var scale = (xDiff > yDiff) ? sz / xDiff : sz / yDiff;

            while (i > 0)
            {
                var y = Points[--i];
                Points[i] = (float)((y - yMin) * scale);
                var x = Points[--i];
                Points[i] = (float)((x - xMin) * scale);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (Points == null)
                return;

            var paint = new Paint
            {
                Color = Color.White
            };

            canvas.DrawColor(Color.Black);
            canvas.DrawPoints(Points, 0, Points.Length, paint);
        }
    }
}