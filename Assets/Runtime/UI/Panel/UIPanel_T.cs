namespace LitGameFramework.UIFramework.Core.Panel
{
    public abstract class UIPanel<TParam> : UIBase
    {
        protected sealed override void OnShowInternal(object param)
        {
            if (param is TParam typed)
            {
                OnShow(typed);
            }
            else
            {
                throw new System.Exception($"{GetType().Name} 需要一个 {typeof(TParam).Name} 类型的参数");
            }
        }

        protected abstract void OnShow(TParam param);
    }
}
