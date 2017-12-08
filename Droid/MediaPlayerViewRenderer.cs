
using System;
using Android.Hardware;
using Player.Xamarin.Forms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(Player.Xamarin.Forms.MediaPlayerView), typeof(MediaPlayerViewRenderer))]

namespace Player.Xamarin.Forms.Droid
{
    public class MediaPlayerViewRenderer: ViewRenderer <Player.Xamarin.Forms.MediaPlayerView, Player.Xamarin.Forms.Droid.MediaPlayerView>
    {
        MediaPlayerView mpView;

        public MediaPlayerView mediaPlayerView {
            get { return mpView; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Player.Xamarin.Forms.MediaPlayerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                System.Diagnostics.Debug.WriteLine(@"          binlog: OnElementChanged controll == null ");

                mpView = new MediaPlayerView(Context);
                SetNativeControl(mpView);
            }
            if (e.OldElement != null)
            {
                System.Diagnostics.Debug.WriteLine(@"          binlog: OnElementChanged e.OldElement != null ");
            }
            if (e.NewElement != null)
            {
                System.Diagnostics.Debug.WriteLine(@"          binlog: OnElementChanged e.NewElement != null ");
                if (mpView != null)
                {
                    SetNativeControl(mpView);
                }
                UpdateLayout();
            }
        }

        protected override void Dispose(bool disposing)
        {
            System.Diagnostics.Debug.WriteLine(@"          binlog: Dispose ");
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
    }
}