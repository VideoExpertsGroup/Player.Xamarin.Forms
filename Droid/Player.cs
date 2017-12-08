
using System;

using Veg.Mediaplayer.Sdk;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Player.Xamarin.Forms.Droid;
using Java.Nio;

using Android.Graphics;
using Android.Views;
using Android.App;
using Android.Runtime;
using System.IO;

[assembly: Dependency(typeof(PlayerD))]

namespace Player.Xamarin.Forms.Droid
{
    public class PlayerD: Activity, IPlayer, Veg.Mediaplayer.Sdk.MediaPlayer.IMediaPlayerCallback
    {
        private IPlayerCallback callback;
        private MediaPlayer _player = null;
        private MediaPlayerView x = null;
        private string filepath;

        public int OnReceiveData(ByteBuffer p0, int p1, long p2)
        {
            return 0;
        }
        public int Status (int p0)
        {
            if (callback != null) {
                System.Diagnostics.Debug.WriteLine(@"          binlog: Player callback {0} ", p0);

                RunOnUiThread(() =>
                {
                    callback.callback_status(p0);
                });
            }


            if (p0 == 5) { //PlpPlaySuccessful
                RunOnUiThread(() =>
                {
                    x.updatePlayerSurfaceLayout();
                });
            }

            return 0;
        }

        public bool player_close()
        {
            if (_player != null) {
                _player.Close();

                x.hideSurface(true);
                _player.SetSurface(null);


                return true;
            }
            return false;
        }

        public bool player_init(Forms.MediaPlayerView view, IPlayerCallback cb)
        {

            var renderer = (MediaPlayerViewRenderer)Platform.GetRenderer(view);

            x = renderer.mediaPlayerView;
            _player = x.Player;

            if (_player != null)
            {

                filepath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;


                callback = cb;

                return true;
            }
            return false;
        }


        public bool player_open(string url)
        {
            
            if (_player != null) {


                Veg.Mediaplayer.Sdk.MediaPlayerConfig conf = new Veg.Mediaplayer.Sdk.MediaPlayerConfig
                {
                    ConnectionUrl = url,
                    ConnectionNetworkProtocol = -1,
                    ConnectionBufferingTime = 5000,
                    ConnectionDetectionTime = 3000,
                    DecodingType = 1,
                    RendererType = 1,
                    SynchroEnable = 1,
                    SynchroNeedDropVideoFrames = 1,
                    EnableColorVideo = 1,
                    DataReceiveTimeout = 30000,
                    NumberOfCPUCores = 0
                };
                x.hideSurface(false);
                x.updatePlayerSurface();

                _player.Open( conf, this);

                _player.UpdateView();

                return true;
            }
            return false;
        }

        public bool player_screenshot(Image ssView, Label debug)
        {

            MediaPlayer.VideoShot shot = _player.GetVideoShot(-1,-1);

            if (shot!= null)
            {
                Bitmap bmp = Bitmap.CreateBitmap(shot.Width, shot.Height, Bitmap.Config.Argb8888);
                bmp.CopyPixelsFromBuffer(shot.Data.Rewind());

                var imgsrc = ImageSource.FromStream(() =>
                {
                    MemoryStream ms = new MemoryStream();
                    bmp.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                    ms.Seek(0L, SeekOrigin.Begin);
                    return ms;
                });
                RunOnUiThread(() =>
                {
                    debug.Text = "ScreenShot!";
                    ssView.Source = imgsrc;
                });
                return true;
            }
            return false;
        }


        public bool player_start_record()
        {

            if (_player != null) {
                int flags = MediaPlayer.PlayerRecordFlags.ForType(MediaPlayer.PlayerRecordFlags.PpRecordPtsCorrection) ;
                
                _player.RecordSetup(filepath, flags, 0, 0, "rec" );

                _player.RecordStart();
            }

            return false;
        }

        public bool player_stop_record()
        {

            if (_player!=null) {
                _player.RecordStop();
            }

            return false;
        }


    }
}
