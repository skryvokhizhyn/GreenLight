using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Util;
using Android.Graphics;

namespace GreenLightTracker.Src
{
    class PathView : View
    {
        private float[] Points { get; set; }
        private float[] CarPosition { get; set; }

        public PathView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public void SetPoints(GpsCoordinate car, ICollection<GpsCoordinate> coordinates)
        {
            if (coordinates == null)
            {
                Points = null;
                return;
            }

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

            if (car != null)
            {
                xMin = Math.Min(xMin, car.x);
                yMin = Math.Min(yMin, car.y);
                xMax = Math.Max(xMax, car.x);
                yMax = Math.Max(yMax, car.y);
            }

            var xDiff = xMax - xMin;
            var yDiff = yMax - yMin;

            var sz = Math.Min(Width, Height);

            var scale = (xDiff > yDiff) ? sz / xDiff : sz / yDiff;

            while (i > 0)
            {
                var y = Points[--i];
                Points[i] = sz - (float)((y - yMin) * scale);
                var x = Points[--i];
                Points[i] = (float)((x - xMin) * scale);
            }

            if (car != null)
            {
                CarPosition = new float[2];

                CarPosition[0] = (float)((car.x - xMin) * scale);
                CarPosition[1] = sz - (float)((car.y - yMin) * scale);
            }
            else
            {
                CarPosition = null;
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            canvas.DrawColor(Color.Black);

            if (Points != null)
            {
                canvas.DrawPoints(Points, 0, Points.Length, new Paint { Color = Color.White });
            }

            if (CarPosition != null)
            {
                canvas.DrawCircle(CarPosition[0], CarPosition[1], 4.0f, new Paint { Color = Color.GreenYellow,  });
            }
        }
    }
}