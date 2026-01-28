namespace LitGameFramework.UIFramework.Core.Panel
{
    public abstract class UIPanel : UIPanel<Unit>
    {
        protected sealed override void OnShow(Unit param)
        {
            OnShow();
        }

        protected abstract void OnShow();
    }

    public readonly struct Unit
    {
        public static readonly Unit Default = new Unit();
    }

}
