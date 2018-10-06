using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Util;
using Android.Graphics;

namespace GreenLightTracker.Src
{
    class PathView : View
    {
        private float[] m_allPoints;
        private float[] m_carPosition;
        private List<float[]> m_roadEndPositions = new List<float[]>();

        private List<float[]> m_coloredPoints = new List<float[]>();
        private readonly Color[] m_colorsForPoints;

        private float m_zoom;
        private float m_horizontalShift;
        private float m_verticalShift;

        private const float ZOOM_FACTOR = 0.1f;
        private const float MOVE_STEP = 0.1f; // 10%

        public double XMin { get; set; }
        public double YMin { get; set; }
        public double XMax { get; set; }
        public double YMax { get; set; }

        public PathView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            ResetZoomAndMove();

            m_colorsForPoints = new Color[] 
            {
                Color.Magenta, /*Color.Maroon, Color.Linen, Color.MintCream, Color.Moccasin,
                Color.LightBlue, Color.LightGoldenrodYellow,*/ Color.Silver, Color.Orange,
                /*Color.SpringGreen,*/ Color.Tan, Color.Tomato, Color.Turquoise, Color.Violet,
                Color.Wheat, Color.Salmon, Color.Purple, Color.Cyan, Color.Azure, Color.Aqua, Color.YellowGreen
            };
        }

        public void ResetZoomAndMove()
        {
            m_zoom = 1.0f;
            m_horizontalShift = 0.0f;
            m_verticalShift = 0.0f;

            Invalidate();
        }

        public void ZoomIn()
        {
            m_zoom += ZOOM_FACTOR;
            Invalidate();
        }

        public void ZoomOut()
        {
            m_zoom -= ZOOM_FACTOR;
            Invalidate();
        }
        
        public void MoveLeft()
        {
            var sz = Math.Min(Width, Height);
            m_horizontalShift -= sz * MOVE_STEP;

            Invalidate();
        }

        public void MoveRight()
        {
            var sz = Math.Min(Width, Height);
            m_horizontalShift += sz * MOVE_STEP;

            Invalidate();
        }

        public void MoveUp()
        {
            var sz = Math.Min(Width, Height);
            m_verticalShift -= sz * MOVE_STEP;

            Invalidate();
        }

        public void MoveDown()
        {
            var sz = Math.Min(Width, Height);
            m_verticalShift += sz * MOVE_STEP;

            Invalidate();
        }

        public void SetCarPosition(GpsCoordinate car)
        {
            if (car != null)
            {
                XMin = Math.Min(XMin, car.x);
                YMin = Math.Min(YMin, car.y);
                XMax = Math.Max(XMax, car.x);
                YMax = Math.Max(YMax, car.y);

                m_carPosition = new float[2];
                m_carPosition[0] = (float)car.x;
                m_carPosition[1] = (float)car.y;

                ScalePoints(m_carPosition, XMin, YMin, XMax, YMax);
            }
            else
            {
                m_carPosition = null;
            }
        }

        public void SetPoints(ICollection<GpsCoordinate> coordinates)
        {
            if (coordinates == null)
            {
                m_allPoints = null;
                return;
            }

            m_allPoints = new float[coordinates.Count * 2];

            int i = 0;

            foreach (var coord in coordinates)
            {
                m_allPoints[i++] = (float)coord.x;
                m_allPoints[i++] = (float)coord.y;
            }

            ScalePoints(m_allPoints, XMin, YMin, XMax, YMax);
        }

        public void AppendCoredPoints(ICollection<GpsCoordinate> points)
        {
            if (points == null)
                return;

            if (points.Count == 0)
                return;

            var rawPoints = new float[points.Count * 2];

            var i = 0;
            foreach (var p in points)
            {
                rawPoints[i++] = (float)p.x;
                rawPoints[i++] = (float)p.y;
            }

            ScalePoints(rawPoints, XMin, YMin, XMax, YMax);

            m_coloredPoints.Add(rawPoints);

            var roadEndPositions = new float[4] { rawPoints[0], rawPoints[1], rawPoints[i - 2], rawPoints[i - 1] };
            m_roadEndPositions.Add(roadEndPositions);
        }

        public void ResetColoredPoints()
        {
            m_coloredPoints.Clear();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            canvas.DrawColor(Color.Black);
            canvas.Translate(m_horizontalShift, m_verticalShift);
            canvas.Scale(m_zoom, m_zoom);

            if (m_allPoints != null)
            {
                canvas.DrawPoints(m_allPoints, 0, m_allPoints.Length, new Paint { Color = Color.White });
            }

            {
                var i = 0;
                foreach (var points in m_coloredPoints)
                {
                    var colorIndex = i++ % m_colorsForPoints.Length;
                    canvas.DrawPoints(points, 0, points.Length, new Paint { Color = m_colorsForPoints[colorIndex] });
                }
            }

            {
                var i = 0;
                foreach (var roadEndPosition in m_roadEndPositions)
                {
                    var colorIndex = i++ % m_colorsForPoints.Length;
                    canvas.DrawCircle(roadEndPosition[0], roadEndPosition[1], 4.0f, new Paint { Color = m_colorsForPoints[colorIndex] });
                    colorIndex = i++ % m_colorsForPoints.Length;
                    canvas.DrawCircle(roadEndPosition[2], roadEndPosition[3], 4.0f, new Paint { Color = m_colorsForPoints[colorIndex] });
                }
            }

            if (m_carPosition != null)
            {
                canvas.DrawCircle(m_carPosition[0], m_carPosition[1], 4.0f, new Paint { Color = Color.GreenYellow });
            }
        }

        private void ScalePoints(float[] rawPoints, double xMin, double yMin, double xMax, double yMax)
        {
            var xDiff = xMax - xMin;
            var yDiff = yMax - yMin;

            var sz = Math.Min(Width, Height);

            var scale = (xDiff > yDiff) ? sz / xDiff : sz / yDiff;

            var i = 0;
            while (i < rawPoints.Length)
            {
                var x = rawPoints[i];
                rawPoints[i++] = (float)((x - xMin) * scale);
                var y = rawPoints[i];
                rawPoints[i++] = sz - (float)((y - yMin) * scale);
            }
        }
    }
}