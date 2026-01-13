using UnityEngine;
using UnityEngine.UI;

    public partial class Testpanel
    {
         [SerializeField] private Button _Button__Legacy_;
         [SerializeField] private Image _Image;
         [SerializeField] private Text _Text__Legacy_;

        protected override void GetUI()
        {
            base.GetUI();
            _Button__Legacy_ = GetBind<Button>(0);
            _Image = GetBind<Image>(1);
            _Text__Legacy_ = GetBind<Text>(2);
        }
    }
