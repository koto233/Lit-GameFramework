using UnityEngine;
using UnityEngine.UI;

    public partial class SamplePanel
    {
        private @Button @_auto_Button_Button;
        private @Image @_auto_Image_Image;
        private @Text @_auto_Text_Text;

        protected override void GetUI()
        {
            base.GetUI();
            @_auto_Button_Button = GetBind<@Button>(0);
            @_auto_Image_Image = GetBind<@Image>(1);
            @_auto_Text_Text = GetBind<@Text>(2);
        }
    }
