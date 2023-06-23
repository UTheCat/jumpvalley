using Godot;
using System.Security.Cryptography.X509Certificates;

namespace Jumpvalley.Tweening
{
    /// <summary>
    /// <see cref="MethodTween"/> that can bind to the Process step of a Godot scene tree.
    /// <br/>
    /// Every process step, the tween will also get stepped until the playback of the tween is finished or stopped.
    /// </summary>
    public partial class SceneTreeTween : MethodTween
    {
        private bool isProcessStepConnected = false;
        private SceneTree _tree;

        /// <summary>
        /// The scene tree associated with this tween.
        /// </summary>
        public SceneTree Tree
        {
            get => _tree;
            set
            {
                if (value == null)
                {
                    // disconnect first, since the original SceneTree will need to be known in order to disconnect from process step
                    DisconnectProcessStep();
                }

                _tree = value;
            }
        }

        /// <summary>
        /// Constructs a new instance of SceneTree
        /// </summary>
        /// <param name="transitionTime">See the base constructors for description</param>
        /// <param name="transitionType"></param>
        /// <param name="easeType"></param>
        /// <param name="sceneTree">The scene tree to associate the SceneTreeTween with. If the SceneTree to associate with isn't known or accessible yet, specify this parameter with null.</param>
        public SceneTreeTween(double transitionTime, Tween.TransitionType transitionType, Tween.EaseType easeType, SceneTree sceneTree) : base(transitionTime, transitionType, easeType)
        {
            Tree = sceneTree;
        }

        public SceneTreeTween() : base()
        {
            Tree = null;
        }

        public override bool IsPlaying
        {
            get => base.IsPlaying;
            protected set
            {
                base.IsPlaying = value;

                if (value)
                {
                    ConnectProcessStep();
                }
                else
                {
                    DisconnectProcessStep();
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

        private void DisconnectProcessStep()
        {
            if (isProcessStepConnected && _tree != null)
            {
                _tree.ProcessFrame -= HandleProcessStep;
                isProcessStepConnected = false;
            }
        }

        private void ConnectProcessStep()
        {
            if (isProcessStepConnected == false && _tree != null)
            {
                isProcessStepConnected = true;
                _tree.ProcessFrame += HandleProcessStep;
            }
        }

        // Base destructor pauses the tween, which automatically disconnects HandleProcessStep from SceneTree.ProcessFrame if needed.
    }
}
