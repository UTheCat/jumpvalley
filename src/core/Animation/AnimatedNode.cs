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
        /// Classes deriving from <see cref="AnimatedNode"/> will typically display show/hide animations when this property is toggled. 
        /// <br/><br/>
        /// This property does not indicate whether or not <see cref="ActualNode"/> is actually visible. For example,
        /// <see cref="IsVisible"/> could be set to false while <see cref="ActualNode"/> is still visible
        /// (but an animation is playing to hide <see cref="ActualNode"/>).
        /// <br/><br/>
        /// To determine whether or not <see cref="ActualNode"/> is actually visible, use <see cref="ActualNode"/>'s Visible property
        /// (or a similar equivalent if <see cref="ActualNode"/> isn't a <see cref="Node2D"/>, <see cref="Node3D"/>, or <see cref="Control"/>). 
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
        /// Creates a new instance of <see cref="AnimatedNode"/> 
        /// </summary>
        /// <param name="actualNode">The root node of the Godot control to be animated</param>
        public AnimatedNode(Node actualNode)
        {
            if (actualNode == null) throw new ArgumentNullException("actualNode", "The actualNode argument (argument #1) cannot be null.");

            ActualNode = actualNode;
        }

        /// <summary>
        /// Event that's raised when the <see cref="IsVisible"/> is set or changed.
        /// The boolean argument of this event is the new value of <see cref="IsVisible"/>.
        /// <br/><br/>
        /// This event will fire even if something sets the <see cref="IsVisible"/> property to the same value
        /// (meaning the truth value of <see cref="IsVisible"/> didn't change after setting the property.)
        /// Store the "old/previous" value of <see cref="IsVisible"/> somewhere to check if the value of
        /// <see cref="IsVisible"/> actually changed. 
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
        /// <br/><br/>
        /// By default, the implementation of this Dispose() method does nothing to <see cref="ActualNode"/>
        /// (e.g. the default implementation of this Dispose() method doesn't free <see cref="ActualNode"/> from memory).
        /// </summary>
        public new void Dispose() => QueueFree();
    }
}
