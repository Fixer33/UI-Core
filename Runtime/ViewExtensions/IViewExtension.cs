namespace UI.ViewExtensions
{
    /// <summary>
    /// Non-generic marker interface for view extensions bound to an <see cref="IView"/>.
    /// </summary>
    public interface IViewExtension : IViewExtension<IView>
    {
    }
    
    /// <summary>
    /// Contract for extending view lifecycle with custom behaviour.
    /// Implement this to hook into view visibility transitions.
    /// </summary>
    /// <typeparam name="TView">A concrete view type implementing <see cref="IView"/>.</typeparam>
    public interface IViewExtension<out TView>
    where TView : IView
    {
        /// <summary>
        /// The view this extension is attached to.
        /// </summary>
        public TView View { get; }
        
        /// <summary>
        /// Called when the view starts showing (before visual transition begins).
        /// </summary>
        internal void ShowStart();
        
        /// <summary>
        /// Called after the view has been fully shown.
        /// </summary>
        internal void Shown();
        
        /// <summary>
        /// Called when the view starts hiding (before visual transition begins).
        /// </summary>
        internal void HideStart();
        
        /// <summary>
        /// Called after the view has been fully hidden.
        /// </summary>
        internal void Hidden();
    }
}