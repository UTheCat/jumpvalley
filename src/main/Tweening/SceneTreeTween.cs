using Godot;

namespace Jumpvalley.Tweening
{
    /// <summary>
    /// <see cref="MethodTween"/> that can bind to the Process step of a Godot scene tree.
    /// <br/>
    /// Every process step, the tween will also get stepped until the playback of the tween is finished or stopped.
    /// </summary>
    public partial class SceneTreeTween: MethodTween
    {
        /// <summary>
        /// The scene tree associated with this tween.
        /// </summary>
        public SceneTree SceneTree { get; private set; }

        public SceneTreeTween(double transitionTime, Tween.TransitionType transitionType, Tween.EaseType easeType, SceneTree sceneTree) : base(transitionTime, transitionType, easeType)
        {

        }

        public SceneTreeTween() : base() { }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
