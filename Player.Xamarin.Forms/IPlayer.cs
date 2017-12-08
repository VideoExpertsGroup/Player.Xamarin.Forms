using System;
using Xamarin.Forms;

namespace Player.Xamarin.Forms
{
    public interface IPlayer {
        bool player_init(MediaPlayerView view, IPlayerCallback cb);
        bool player_open(string url);
        bool player_close();
        bool player_start_record();
        bool player_stop_record();
        bool player_screenshot(Image ssView, Label debug);
    }

}
