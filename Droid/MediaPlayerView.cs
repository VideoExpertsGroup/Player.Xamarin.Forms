using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.App;

using Veg.Mediaplayer.Sdk;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Player.Xamarin.Forms.Droid;
using Java.Nio;


namespace Player.Xamarin.Forms.Droid {
    
    public class MediaPlayerView : ViewGroup, ISurfaceHolderCallback {
        private SurfaceView surfaceView;
        private ISurfaceHolder holder;
        private Surface cursurface;

        IWindowManager windowManager;
        Veg.Mediaplayer.Sdk.MediaPlayer _player = null;

        public MediaPlayer Player {
            get { return _player; }
        }

        public MediaPlayerView (Context context)
            : base (context)
        {

            System.Diagnostics.Debug.WriteLine(@"          binlog: MediaPlayerView constructor: ");

            surfaceView = new SurfaceView (context);

            AddView(surfaceView);

            windowManager = Context.GetSystemService (Context.WindowService).JavaCast<IWindowManager> ();

            _player = new MediaPlayer(context, false);

            holder = surfaceView.Holder;
            holder.AddCallback (this);
        }

        public bool updatePlayerSurface(){
            if (_player!=null && cursurface!=null){
                _player.SetSurface( cursurface);

                return true;
            }
            return false;
        }

        public bool hideSurface (bool isHide) {
            if (surfaceView!= null) {
                if (isHide) {
                    surfaceView.Visibility = Android.Views.ViewStates.Invisible;
                } else {
                    surfaceView.Visibility = Android.Views.ViewStates.Visible;
                }
                return true;
            }
            return false;
        }

        public bool updatePlayerSurfaceLayout() {

            int w_src = _player.VideoWidth;
            int h_src = _player.VideoHeight;

            if (w_src == 0 || h_src == 0) return false;

            int w_new = 0;
            int h_new = 0;
            int w_dst = this.Width;
            int h_dst = this.Height;

            double dst_ratio = (double)w_dst / (double)h_dst;
            double src_ratio = (double)w_src / (double)h_src;

            if (dst_ratio > src_ratio) {
                h_new = h_dst;
                w_new = (int) (h_new * src_ratio);
            } else {
                w_new = w_dst;
                h_new = (int) (w_new / src_ratio);
            }
            int w_delta = (w_dst - w_new) / 2;
            int h_delta = (h_dst - h_new) / 2;

            surfaceView.Layout( w_delta, h_delta, w_new+w_delta, h_new+h_delta);

            return true;
        }

        protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = ResolveSize (SuggestedMinimumWidth, widthMeasureSpec);
            int height = ResolveSize (SuggestedMinimumHeight, heightMeasureSpec);
            SetMeasuredDimension (width, height);

            System.Diagnostics.Debug.WriteLine(@"          binlog: OnMeasure cb: {0} {1}  ", width, height);

        }

        protected override void OnLayout (bool changed, int l, int t, int r, int b)
        {
            

            var msw = MeasureSpec.MakeMeasureSpec (r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec (b - t, MeasureSpecMode.Exactly);

            updatePlayerSurfaceLayout();

            System.Diagnostics.Debug.WriteLine(@"          binlog: OnLayout cb: {0} {1} {2} {3} => {4} {5} ", l, t, r, b, msw, msh);

        }

        public void SurfaceCreated (ISurfaceHolder holder)
        {
            try {
                System.Diagnostics.Debug.WriteLine(@"          binlog: Got surface");
                cursurface = holder.Surface;
                updatePlayerSurface();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine (@"          binlog: Got surface ERROR: ", ex.Message);
            }
        }

        public void SurfaceDestroyed (ISurfaceHolder holder)
        {
            System.Diagnostics.Debug.WriteLine(@"          binlog: SurfaceDestroyed cb: ");

        }

        public void SurfaceChanged (ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
            System.Diagnostics.Debug.WriteLine(@"          binlog: SurfaceChanged cb: ");

        }

    }
}
