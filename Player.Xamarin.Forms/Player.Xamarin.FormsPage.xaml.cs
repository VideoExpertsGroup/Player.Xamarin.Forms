using System;
using Xamarin.Forms;

namespace Player.Xamarin.Forms
{
    public partial class Player_Xamarin_FormsPage : ContentPage, IPlayerCallback
    {
        private bool screenshot_is_visible  = false;
        private bool record_is_progress = false;
        private int connect_state = 0;

        IPlayer crossPlayer;


        public void callback_status(int code)
        {
            switch(code){
                case 305: { //VrpFirstframe = 305;
                        Connect_btn.Text = "Disconnect";
                        connect_state = 1;

                        Record_btn.IsEnabled = true;
                        Screenshot_btn.IsEnabled = true;
                    } break;
                case 8:  // PlpCloseSuccessful = 8
                case 117: //CpDisconnectSuccessful = 117
                    {   
                        Connect_btn.Text = "Connect";
                        Screenshot_btn.Text = "ScreenShot";
                        connect_state = 0;

                        Record_btn.IsEnabled = false;
                        Screenshot_btn.IsEnabled = 
                        ScreenShot_view.IsVisible = 
                        screenshot_is_visible = false;

                    } break;
                case 110: { //CpRecordClosed = 110
                        Record_btn.Text = "Record";
                        record_is_progress = false;
                    } break;
                default: {
                        
                    } break;
            }

            Debug_lbl.Text = String.Format("Code: {0}", code);

            return;
        }

        public Player_Xamarin_FormsPage()
        {
            InitializeComponent();
        }

        void Connect_btn_click(object sender, EventArgs e) {
            if (crossPlayer == null) {
                crossPlayer = DependencyService.Get<IPlayer>();
                crossPlayer.player_init(Video_view, this);
            }
            if (crossPlayer == null) return;
            if (connect_state == 0)
            {
                Connect_btn.Text = "Connecting..";
                crossPlayer.player_open(URL_editbox.Text);
            } else if (connect_state == 1) {
                crossPlayer.player_close();
            }
        }

        void ScreenShot_btn_click (object sender, EventArgs e) {
            if (!screenshot_is_visible) {
                ScreenShot_view.IsVisible = true;
                Screenshot_btn.Text = "Close";

                if(crossPlayer != null) {
                    crossPlayer.player_screenshot(ScreenShot_view, Debug_lbl);
                }
            } else {
                ScreenShot_view.Source = null;

                ScreenShot_view.IsVisible = false;
                Screenshot_btn.Text = "ScreenShot";
            }
            screenshot_is_visible = !screenshot_is_visible;
        }

        void Record_btn_click (object sender, EventArgs e) {
            if (!record_is_progress) {
                record_is_progress = true;
                Record_btn.Text = "Recording..";
                crossPlayer.player_start_record();
            } else {
                crossPlayer.player_stop_record();
            }
        }

    }
}
