using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public Path mPath;
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

        private void Initialize()
        {
            mPaint = new Paint();
            mPath = new Path();
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

        private void Initialize()
        {
            mPaint = new Paint();
            mPath = new Path();
        }

        private void drawPath(Canvas canvas)
        {
            mPath.Reset();
            mPath.MoveTo(0, mHeight / 2);
            int temp = 0;
            for (int i = 0; i < 10; i++)
            {
                mPath.LineTo(temp + 20, 100);
                mPath.LineTo(temp + 70, mHeight / 2 + 50);
                mPath.LineTo(temp + 80, mHeight / 2);
                mPath.LineTo(temp + 200, mHeight / 2);
                temp += 200;
            }
            mPaint.SetStyle(Paint.Style.Stroke);
            mPaint.Color = mLineColor;
            mPaint.StrokeWidth = 5;
            canvas.DrawPath(mPath, mPaint);
        }

        protected override void OnDraw(Canvas canvas)
        {
            drawPath(canvas);
            ScrollBy(1, 0);
            PostInvalidateDelayed(10);
        }
    }
}