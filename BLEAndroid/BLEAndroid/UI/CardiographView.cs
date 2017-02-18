using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Graphics;
using Android.Widget;

namespace BLEAndroid
{
    public class CardiographView : View
    {
        protected Paint mPaint;
        protected Color mLineColor = Color.ParseColor("#76f112");
        protected Color mGridColor = Color.ParseColor("#1b4200");
        protected Color mSGridColor = Color.ParseColor("#092100");
        protected Color mBackgroundColor = Color.Black;
        protected int mWidth, mHeight;
        protected int mGridWidth = 50;
        protected int mSGridWidth = 10;
        //public Path mPath;
        private Context context;

        public CardiographView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public CardiographView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public CardiographView(Context context) : base(context)
        {
            Initialize();
        }

        private void Initialize()
        {
            mPaint = new Paint();
            //mPath = new Path();
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            mWidth = w;
            mHeight = h;
            base.OnSizeChanged(w, h, oldw, oldh);
        }

        private void initBackground(Canvas canvas)
        {
            canvas.DrawColor(mBackgroundColor);
            int vSNum = mWidth / mSGridWidth;
            int hSNum = mHeight / mSGridWidth;
            mPaint.Color = mSGridColor;
            mPaint.StrokeWidth = 2;
            //»­ÊúÏß
            for (int i = 0; i < vSNum + 1; i++)
            {
                canvas.DrawLine(i * mSGridWidth, 0, i * mSGridWidth, mHeight, mPaint);
            }
            //»­ºáÏß
            for (int i = 0; i < hSNum + 1; i++)
            {
                canvas.DrawLine(0, i * mSGridWidth, mWidth, i * mSGridWidth, mPaint);
            }

            int vNum = mWidth / mGridWidth;
            int hNum = mHeight / mGridWidth;
            mPaint.Color = mGridColor;
            mPaint.StrokeWidth = 2;
            //»­ÊúÏß
            for (int i = 0; i < vNum + 1; i++)
            {
                canvas.DrawLine(i * mGridWidth, 0, i * mGridWidth, mHeight, mPaint);
            }
            //»­ºáÏß
            for (int i = 0; i < hNum + 1; i++)
            {
                canvas.DrawLine(0, i * mGridWidth, mWidth, i * mGridWidth, mPaint);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            initBackground(canvas);
            //base.OnDraw(canvas);
        }
    }

    public class PathView : CardiographView
    {
        //public float XPosition;
        public MyPath path;
        public Path mPath;
        public int last;
        public PathView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public PathView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public PathView(Context context): base(context)
        {
            Initialize();
        }

        private void Initialize()
        {
            mPaint = new Paint();
            mPath = new Path();
            last = 0;
        }

        private void drawPath(Canvas canvas)
        {
            //if (path.mPath == null) path.mPath = new Path();
            
            //mPath.Reset();
            //mPath.MoveTo(0, mHeight / 2);
            //int temp = 0;
            //for (int i = 0; i < 10; i++)
            //{
            //    mPath.LineTo(temp + 20, 100);
            //    mPath.LineTo(temp + 70, mHeight / 2 + 50);
            //    mPath.LineTo(temp + 80, mHeight / 2);
            //    mPath.LineTo(temp + 200, mHeight / 2);
            //    temp += 200;
            //}
            //Console.WriteLine(mPath.IsEmpty ? "empty" : "not empty");
            mPaint.SetStyle(Paint.Style.Stroke);
            mPaint.Color = mLineColor;
            mPaint.StrokeWidth = 5;
            //canvas.Scale(1, 4000 / canvas.Height);
            //canvas.Density = 20;
            //Console.WriteLine(canvas.Density);
            //Console.WriteLine(canvas.Height);
            //Console.WriteLine(canvas.MaximumBitmapHeight);
            //Console.WriteLine(canvas.MaximumBitmapWidth);
            mPath.Reset();
            int _count = path.path.Count();
            //Console.WriteLine(_count);
            if (_count > 0)
            {
                mPath.MoveTo(0, path.path[_count - mWidth >= 0 ? _count - mWidth : 0].Y);
                int X=1;
                for (int i = (_count - mWidth + 1>0? _count - mWidth + 1:1); i < _count; i++)
                {
                    mPath.LineTo(X, path.path[i].Y);
                    X++;
                }
                canvas.DrawPath(mPath, mPaint);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            
            PostInvalidateDelayed(200);
            drawPath(canvas);
            //ScrollTo((int)path.path.Count()<mWidth?0: , 0);
            //Console.WriteLine(XPosition);
            //if(path.XPosition-last>1500)
            //{
            //    canvas.ClipRect(new Rect((int)path.XPosition - mWidth, 0, (int)path.XPosition, mHeight));
            //    last = (int)path.XPosition;
                
            //}
        }
    }
}