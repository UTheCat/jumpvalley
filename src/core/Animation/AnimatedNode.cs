using Godot;
using System;

namespace UTheCat.Jumpvalley.Core.Animation
{
    /// <summary>
    /// This class provides some components to bind to a Godot node,
    /// mainly to assist with writing animation code for the node.
    /// </summary>
    public partial class AnimatedNode : Node, IDisposable
    {
        /// <summary>
        /// The node to be animated
        /// </summary>
        public Node ActualNode { get; private set; }

        private bool _isVisible;

        /// <summary>
        /// Whether or not <see cref="ActualNode"/> should be visible.
        /// Toggling this property will run the animation for showing/hiding <see cref="ActualNode"/>.
        /// </summary>
        public virtual bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                RaiseVisibilityChanged(value);
            }
        }

        /// <summary>
        /// Creates a new instance of AnimatedControl
        /// </summary>
        /// <param name="actualNode">The root node of the Godot control to be animated</param>
        public AnimatedNode(Node actualNode)
        {
            if (actualNode == null) throw new ArgumentNullException("actualNode", "The actualNode argument (argument #1) cannot be null.");

            ActualNode = actualNode;
        }

        /// <summary>
        /// Event that's raised when <see cref="IsVisible"/> gets toggled.
        /// The boolean argument of this event is the new value of <see cref="IsVisible"/>
        /// </summary>
        public event EventHandler<bool> VisibilityChanged;

        protected void RaiseVisibilityChanged(bool isVisible)
        {
            VisibilityChanged?.Invoke(this, isVisible);
        }

        /// <summary>
        /// Disposes of this <see cref="AnimatedNode"/>.
        /// <br/><br/>
        /// To assist with garbage collection, this method will automatically call
        /// the <see cref="AnimatedNode"/>'s QueueFree method.
        /// </summary>
        public new void Dispose() => QueueFree();
    }
}
