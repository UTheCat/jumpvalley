using Godot;

namespace Jumpvalley.Tweening
{
    /// <summary>
    /// <see cref="MethodTween"/> that can bind to the Process step of a Godot scene tree.
    /// <br/>
    /// Every process step, the tween will also get stepped until the playback of the tween is finished or stopped.
    /// </summary>
    public partial class SceneTreeTween : MethodTween
    {
        /// <summary>
        /// The scene tree associated with this tween.
        /// </summary>
        public SceneTree Tree { get; private set; }

        public SceneTreeTween(double transitionTime, Tween.TransitionType transitionType, Tween.EaseType easeType, SceneTree sceneTree) : base(transitionTime, transitionType, easeType) { }

        public SceneTreeTween() : base() { }

        public override bool IsPlaying
        {
            get => base.IsPlaying;
            protected set
            {
                // we'll need this original value to connect and disconnect from the SceneTree process event properly
                bool wasPlaying = base.IsPlaying;

                if (value)
                {
                    base.IsPlaying = true;

                    // only connect if the tween wasn't playing in the first place to avoid duplicate invocations
                    if (!wasPlaying)
                    {
                        Tree.ProcessFrame += HandleProcessStep;
                    }
                }
                else
                {
                    base.IsPlaying = false;

                    // only disconnect if the tween was playing in the first place to avoid possible Exception throws
                    if (wasPlaying)
                    {
                        Tree.ProcessFrame -= HandleProcessStep;
                    }
                }
            }
        }

        protected void HandleProcessStep()
        {
            if (IsPlaying)
            {
                Step();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
