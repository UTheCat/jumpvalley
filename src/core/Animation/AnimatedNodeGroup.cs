using System;
using System.Collections.Generic;

namespace UTheCat.Jumpvalley.Core.Animation
{
    /// <summary>
    /// Class that groups multiple <see cref="AnimatedNode"/>s together so that they can communicate with each other.
    /// <br/><br/>
    /// This class can also help make it so only one <see cref="AnimatedNode"/> within the group
    /// can be shown at a time.
    /// </summary>
    public partial class AnimatedNodeGroup : IDisposable
    {
        private AnimatedNode _currentlyVisibleNode;

        /// <summary>
        /// The node within the <see cref="AnimatedNodes"/> list that's currently visible.
        /// <br/><br/>
        /// This is set to <c>null</c> if <see cref="CanOnlyShowOneNode"/> is set to <c>false</c>.  
        /// </summary>
        public AnimatedNode CurrentlyVisibleNode
        {
            get => _currentlyVisibleNode;
            private set
            {
                if (_currentlyVisibleNode == value) return;

                _currentlyVisibleNode = value;
                RaiseCurrentlyVisibleNodeChanged();
            }
        }

        /// <summary>
        /// The nodes within the group.
        /// <br/><br/>
        /// In this dictionary, the keys are strings which are IDs that
        /// can be assigned to an <see cref="AnimatedNode"/> within the group.
        /// The corresponding value is the <see cref="AnimatedNode"/> associated with the ID.
        /// </summary>
        public Dictionary<string, AnimatedNode> NodeList { get; private set; }

        private bool _canOnlyShowOneNode;

        /// <summary>
        /// Whether or not only one node within the group can be shown at a time
        /// </summary>
        public bool CanOnlyShowOneNode
        {
            get => _canOnlyShowOneNode;
            set
            {
                _canOnlyShowOneNode = value;

                if (!value)
                {
                    CurrentlyVisibleNode = null;
                }
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="AnimatedNodeGroup"/>
        /// </summary>
        public AnimatedNodeGroup()
        {
            NodeList = new Dictionary<string, AnimatedNode>();
            CanOnlyShowOneNode = false;
            CurrentlyVisibleNode = null;
        }

        /// <summary>
        /// Removes the <see cref="AnimatedNode"/> from the group that corresponds
        /// to the string identifier as specified in the <paramref name="id"/> parameter.
        /// </summary>
        /// <param name="id">The string identifier of the <see cref="AnimatedNode"/> to remove</param>
        public void Remove(string id)
        {
            if (NodeList.ContainsKey(id))
            {
                AnimatedNode node = NodeList[id];
                node.VisibilityChanged -= HandleVisiblityToggled;

                if (CurrentlyVisibleNode == node)
                {
                    CurrentlyVisibleNode = null;
                }

                NodeList.Remove(id);
            }
        }

        /// <summary>
        /// Removes all of the <see cref="AnimatedNode"/>s within <see cref="NodeList"/>,
        /// disconnecting this <see cref="AnimatedNodeGroup"/>'s visibility toggle handler
        /// from the <see cref="AnimatedNode"/>s.
        /// </summary>
        public void ClearNodeList()
        {
            foreach (KeyValuePair<string, AnimatedNode> pair in NodeList)
            {
                pair.Value.VisibilityChanged -= HandleVisiblityToggled;
            }

            CurrentlyVisibleNode = null;
            NodeList.Clear();
        }

        /// <summary>
        /// Adds an <see cref="AnimatedNode"/> to this <see cref="AnimatedNodeGroup"/>.
        /// </summary>
        /// <param name="id">The string identifier to assign to the <see cref="AnimatedNode"/></param>
        /// <param name="node">The <see cref="AnimatedNode"/> to add</param>
        public void Add(string id, AnimatedNode node)
        {
            if (!NodeList.ContainsKey(id))
            {
                NodeList.Add(id, node);

                if (node.IsVisible)
                {
                    UpdateCurrentlyVisibleNode(node);
                }

                node.VisibilityChanged += HandleVisiblityToggled;
            }
        }

        private void UpdateCurrentlyVisibleNode(AnimatedNode node)
        {
            if (CanOnlyShowOneNode)
            {
                CurrentlyVisibleNode = node;

                foreach (KeyValuePair<string, AnimatedNode> pair in NodeList)
                {
                    AnimatedNode pairNode = pair.Value;

                    if (pairNode != node)
                    {
                        pairNode.IsVisible = false;
                    }
                }
            }
        }

        private void HandleVisiblityToggled(object o, bool isVisible)
        {
            AnimatedNode node = o as AnimatedNode;

            if (node != null)
            {
                if (isVisible)
                {
                    UpdateCurrentlyVisibleNode(node);
                }
                else
                {
                    if (CurrentlyVisibleNode == node)
                    {
                        CurrentlyVisibleNode = null;
                    }
                }
            }
        }

        /// <summary>
        /// Returns whether or not the node in the group assigned to the specified
        /// string identifier (ID) is visible.
        /// </summary>
        /// <param name="id">The string identifier of the node</param>
        /// <returns>Whether or not the node is visible</returns>
        public bool IsNodeVisible(string id)
        {
            AnimatedNode node;
            NodeList.TryGetValue(id, out node);

            if (node != null)
            {
                return node.IsVisible;
            }

            return false;
        }

        /// <summary>
        /// Hides one of the <see cref="AnimatedNode"/>s in <see cref="NodeList"/>.
        /// </summary>
        /// <param name="id">The string identifier of the animated node to hide</param>
        public void Hide(string id)
        {
            AnimatedNode node = NodeList[id];

            if (node != null)
            {
                node.IsVisible = false;
            }
        }

        /// <summary>
        /// Hides all of the <see cref="AnimatedNode"/>s that have been made visible
        /// by this instance of <see cref="AnimatedNodeGroup"/>.
        /// </summary>
        public void HideAll()
        {
            foreach (KeyValuePair<string, AnimatedNode> pair in NodeList)
            {
                pair.Value.IsVisible = false;
            }

            CurrentlyVisibleNode = null;
        }

        /// <summary>
        /// Shows one of the <see cref="AnimatedNode"/>s in <see cref="NodeList"/>.
        /// </summary>
        /// <param name="id">The string identifier of the animated node to show</param>
        public void Show(string id)
        {
            AnimatedNode node = NodeList[id];

            if (node != null)
            {
                node.IsVisible = true;
            }
        }

        public void Dispose()
        {
            ClearNodeList();
        }

        /// <summary>
        /// Event raised when the value of <see cref="CurrentlyVisibleNode"/> changes 
        /// </summary>
        public event EventHandler CurrentlyVisibleNodeChanged;

        protected void RaiseCurrentlyVisibleNodeChanged()
        {
            CurrentlyVisibleNodeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
