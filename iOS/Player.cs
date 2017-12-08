using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using CoreGraphics;
using Foundation;
using UIKit;
using MediaPlayerSDK;

using Player.Xamarin.Forms.iOS;

[assembly: Dependency(typeof(PlayerX))]

namespace Player.Xamarin.Forms.iOS
{
    public class PlayerX : NSObject, IPlayer
    {

        public partial class Callback : MediaPlayerSDK.MediaPlayerCallback {
            public PlayerX _delegate;

            public override int OnReceiveData(MediaPlayerSDK.MediaPlayer player, IntPtr buffer, int size, nint pts)
            {
                return 0;
            }

            public override int Status(MediaPlayerSDK.MediaPlayer player, int arg)
            {
                //System.Console.WriteLine(String.Format("<binary> status: {0} \n ", arg));

                if (arg == (int)MediaPlayerSDK.MediaPlayerNotifyCodes.CpRecordClosed)
                {
                    string recfile = player.RecordGetFileName(0);
                    System.Console.Write(String.Format("CLOSED: file recorded: {0} \n", recfile));

                    BeginInvokeOnMainThread(() => {
                        if (UIKit.UIVideo.IsCompatibleWithSavedPhotosAlbum(recfile))
                        {
                            System.Console.Write(String.Format("PHOTOS: Can save \n"));

                            UIKit.UIVideo.SaveToPhotosAlbum(recfile, (path, error) => {
                                System.Console.Write(String.Format("PHOTOS: Saved succesfully \n"));
                            });
                        }
                        else
                        {
                            System.Console.Write(String.Format("PHOTOS: Can't save \n"));
                        }
                    });

                }

                if (_delegate != null) {
                    if (_delegate.callback != null) {
                        BeginInvokeOnMainThread(() =>
                        {
                            _delegate.callback.callback_status(arg);
                        });
                    } else {
                        System.Console.WriteLine(String.Format(" No listener's callback  \n "));
                    }
                } else {
                    System.Console.WriteLine(String.Format(" No listener \n "));
                }



                return 0;
            }

        }

        private MediaPlayerSDK.MediaPlayer player = null;
        private IPlayerCallback callback = null;
        private string filepath;

        public bool player_init(MediaPlayerView view, IPlayerCallback cb)
        {
            if (player == null)
            {
                var renderer = Platform.GetRenderer(view);
                UIView nativeView = renderer.NativeView;

                player = new MediaPlayerSDK.MediaPlayer(nativeView.Bounds);

                if (player != null)
                {

                    UIView vv = player.ContentView;

                    if (vv != null)
                    {
                        nativeView.AddSubview(vv);
                        callback = cb;

                        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        filepath = Path.Combine(documents, "..", "tmp");
                    } else {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }

            return false;
        }

        public bool player_open(string url)
        {
            if (player != null) {
                player.Open(new MediaPlayerConfig
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
                }, new Callback { _delegate = this });
                int rc = player.UpdateView;

                return true;
            }
            return false;
        }

        public bool player_close()
        {
            if (player != null) {
                player.Close();
                return true;
            }
            return false;
        }

        public bool player_start_record()
        {
            if (player != null) {
                player.RecordSetup(filepath,
                         MediaPlayerRecordFlags.FastStart | MediaPlayerRecordFlags.PtsCorrection | MediaPlayerRecordFlags.FragKeyframe, 0, 0, "rec"
                        );

                player.RecordStart();
                return true;
            }

            return false;
        }

        public bool player_stop_record()
        {
            if (player != null) {
                player.RecordStop();
                return true;
            }

            return false;
        }

        public bool player_screenshot(Image ssView, Label debug)
        {
            if (player != null)
            {
                //Task sstask = new Task(() => { 

                new System.Threading.Thread(new System.Threading.ThreadStart(() => {

                    int val = 1920 * 1080 * 4 + 8; //ARGB 1920x1080 + 8(iosspecific)
                    IntPtr b = Marshal.AllocHGlobal(val);


                    int val1 = -1; //return realsize after getvideoshot
                    int val2 = -1; //return realsize after getvideoshot 
                    int val3 = 0;  //return realsize after getvideoshot

                    int rc = player.GetVideoShot(b, ref val, ref val1, ref val2, ref val3);
                    if (rc == -2)
                    {
                        System.Console.WriteLine(String.Format("<binary> rc: {0} , not enough space allocated\n", rc));
                    return ;
                    }
                    else if (rc == -1)
                    {
                        System.Console.WriteLine(String.Format("<binary> rc: {0} , error\n", rc));
                    return ;
                    }

                    CGDataProvider provider = new CGDataProvider(b, val);
                    int bitsPerComponent = 8;
                    int bitsPerPixel = 32;
                    int bytesPerRow = val3;

                    CGColorSpace colorspaceref = CGColorSpace.CreateDeviceRGB();
                    CGBitmapFlags bitmapinfo = CGBitmapFlags.ByteOrder32Little | CGBitmapFlags.NoneSkipFirst;
                    CGColorRenderingIntent renderintent = CGColorRenderingIntent.Default;

                    CGImage imageref = new CGImage(val1, val2, bitsPerComponent, bitsPerPixel, val3, colorspaceref, bitmapinfo, provider, null, true, renderintent);



                    UIImage img = new UIImage(imageref);


                    if (img != null)
                    {
                        InvokeOnMainThread(() =>
                        {
                            ImageSource imgsrc = ImageSource.FromStream(() => img.AsPNG().AsStream());
                                
                            ssView.Source = imgsrc;
                            debug.Text = "ScreenShot!";

                            return;
                        });
                    }
                    Marshal.FreeHGlobal(b);
                })).Start();
             }

            return false;
        }
    }
}
