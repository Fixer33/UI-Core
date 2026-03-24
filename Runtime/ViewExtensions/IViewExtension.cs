namespace UI.ViewExtensions
{
    public interface IViewExtension : IViewExtension<IView>
    {
    }
    
    public interface IViewExtension<out TView>
    where TView : IView
    {
        public TView View { get; }
        
        internal void ShowStart();
        internal void Shown();
        internal void HideStart();
        internal void Hidden();
    }
}