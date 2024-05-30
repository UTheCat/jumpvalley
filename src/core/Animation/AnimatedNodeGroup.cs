using System;
using System.Collections.Generic;

namespace Jumpvalley.Animation
{
    /// <summary>
    /// Class that groups multiple <see cref="AnimatedNode"/>s together so that they can communicate with each other.
    /// <br/><br/>
    /// This class can also help make it so only a limited number of <see cref="AnimatedNode"/>s within the group
    /// can be shown at a time.
    /// </summary>
    public partial class AnimatedNodeGroup
    {
        /// <summary>
        /// List of currently visible <see cref="AnimatedNode"/>s.
        /// As the end of this list is reached, the earlier the <see cref="AnimatedNode"/> has been shown.
        /// </summary>
        public List<AnimatedNode> VisibleNodes;

        /// <summary>
        /// The nodes within the group.
        /// <br/><br/>
        /// In this dictionary, the keys are strings which are IDs that
        /// can be assigned to an <see cref="AnimatedNode"/> within the group.
        /// The corresponding value is the <see cref="AnimatedNode"/> associated with the ID.
        /// </summary>
        public Dictionary<string, AnimatedNode> NodeList;

        private int _maxVisibleNodes;

        /// <summary>
        /// The maximum amount of <see cref="AnimatedNode"/>s within <see cref="NodeList"/>
        /// that can be shown at a time.
        /// <br/><br/>
        /// Set this to -1 to specify that there should be no maximum amount.
        /// </summary>
        public int MaxVisibleNodes
        {
            get => _maxVisibleNodes;
            set
            {
                if (value < -1) throw new ArgumentOutOfRangeException("", "Maximum amount must be set to an integer greater than or equal to -1");

                _maxVisibleNodes = value;
                HideExcessVisibleNodes();
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="AnimatedNodeGroup"/>
        /// </summary>
        public AnimatedNodeGroup()
        {
            NodeList = new Dictionary<string, AnimatedNode>();
            VisibleNodes = new List<AnimatedNode>();
            MaxVisibleNodes = -1;
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
                // Remove the AnimatedNode from the visible nodes list,
                // just in case it's still in there.
                int index = VisibleNodes.IndexOf(NodeList[id]);
                if (index >= 0)
                {
                    VisibleNodes.RemoveAt(index);
                    NodeList.Remove(id);

                    RaiseVisibleNodesUpdated();
                }
                else
                {
                    NodeList.Remove(id);
                }
            }
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
            }
        }

        /// <summary>
        /// Returns whether or not the node in the group assigned to the specified
        /// string identifier (ID) is visible.
        /// <br/><br/>
        /// The node is considered "visible" by this function if it's in the group
        /// and is in the <see cref="VisibleNodes"/> list.
        /// </summary>
        /// <param name="id">The string identifier of the node</param>
        /// <returns>Whether or not the node is visible</returns>
        public bool IsNodeVisible(string id)
        {
            AnimatedNode node;
            NodeList.TryGetValue(id, out node);

            if (node != null)
            {
                return VisibleNodes.Contains(node);
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

            // For stability reasons, only hide the AnimatedNode if it was shown
            // via this class's Show method
            int index = VisibleNodes.IndexOf(node);
            if (index >= 0)
            {
                node.IsVisible = false;
                VisibleNodes.RemoveAt(index);

                RaiseVisibleNodesUpdated();
            }
        }

        /// <summary>
        /// Hides all of the <see cref="AnimatedNode"/>s that have been made visible
        /// by this instance of <see cref="AnimatedNodeGroup"/>.
        /// </summary>
        public void HideAll()
        {
            foreach (AnimatedNode node in VisibleNodes)
            {
                node.IsVisible = false;
            }

            VisibleNodes.Clear();

            RaiseVisibleNodesUpdated();
        }

        private bool ShouldRemoveExcessVisibleNodes()
        {
            int maxVisibleNodes = MaxVisibleNodes;
            return maxVisibleNodes >= 0 && VisibleNodes.Count > maxVisibleNodes;
        }

        private void HideExcessVisibleNodes()
        {
            int maxVisibleNodes = MaxVisibleNodes;

            // If we went over the maximum visible node count,
            // hide visible nodes at the end of the list.
            if (ShouldRemoveExcessVisibleNodes())
            {
                int excess = VisibleNodes.Count - maxVisibleNodes;
                for (int i = 0; i < excess; i++)
                {
                    AnimatedNode removedNode = VisibleNodes[i];
                    removedNode.IsVisible = false;

                    VisibleNodes.RemoveAt(VisibleNodes.Count - 1);
                }

                RaiseVisibleNodesUpdated();
            }
        }

        /// <summary>
        /// Shows one of the <see cref="AnimatedNode"/>s in <see cref="NodeList"/>.
        /// </summary>
        /// <param name="id">The string identifier of the animated node to show</param>
        public void Show(string id)
        {
            AnimatedNode node = NodeList[id];

            if (!VisibleNodes.Contains(node))
            {
                if (MaxVisibleNodes != 0)
                {
                    VisibleNodes.Add(node);
                    node.IsVisible = true;
                }

                // We don't want to raise the VisibleNodesUpdated
                // event twice at a time
                if (ShouldRemoveExcessVisibleNodes())
                {
                    HideExcessVisibleNodes();
                }
                else
                {
                    RaiseVisibleNodesUpdated();
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="VisibleNodes"/> list.
        /// <br/><br/>
        /// This can be used to account for cases where the visibility of the group's
        /// <see cref="AnimatedNode"/>s have been toggled externally. 
        /// </summary>
        public void UpdateVisibleNodesList()
        {
            
        }

        /// <summary>
        /// Event raised when the <see cref="VisibleNodes"/> list is updated
        /// </summary>
        public event EventHandler VisibleNodesUpdated;

        protected void RaiseVisibleNodesUpdated()
        {
            VisibleNodesUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
