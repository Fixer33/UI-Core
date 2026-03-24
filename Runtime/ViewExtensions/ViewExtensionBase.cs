using UnityEngine;

namespace UI.ViewExtensions
{
    [RequireComponent(typeof(IView))]
    public abstract class ViewExtensionBase : ViewExtensionBase<IView>
    {
    }

    public abstract class ViewExtensionBase<TView> : MonoBehaviour, IViewExtension<TView>
        where TView : IView
    {
        public TView View => _view ??= GetComponent<TView>();
        private TView _view;
        
        void IViewExtension<TView>.ShowStart()
        {
            OnShowStart();
        }
        protected abstract void OnShowStart();

        void IViewExtension<TView>.Shown()
        {
            OnShown();
        }
        protected abstract void OnShown();

        void IViewExtension<TView>.HideStart()
        {
            OnHideStart();
        }
        protected abstract void OnHideStart();

        void IViewExtension<TView>.Hidden()
        {
            OnHidden();
        }
        protected abstract void OnHidden();
    }
}