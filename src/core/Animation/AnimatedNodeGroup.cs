using System;
using System.Collections.Generic;

namespace Jumpvalley.Animation
{
    /// <summary>
    /// Class that groups multiple <see cref="AnimatedNode"/>s together so that they can communicate with each other.
    /// <br/><br/>
    /// This class can also help make it so only one <see cref="AnimatedNode"/>s within the group
    /// can be shown at a time.
    /// </summary>
    public partial class AnimatedNodeGroup
    {
        /// <summary>
        /// The node within the <see cref="AnimatedNodes"/> list that's currently visible.
        /// </summary>
        public AnimatedNode CurrentlyVisibleNode { get; private set; }

        /// <summary>
        /// The nodes within the group.
        /// <br/><br/>
        /// In this dictionary, the keys are strings which are IDs that
        /// can be assigned to an <see cref="AnimatedNode"/> within the group.
        /// The corresponding value is the <see cref="AnimatedNode"/> associated with the ID.
        /// </summary>
        public Dictionary<string, AnimatedNode> NodeList { get; private set; }

        private bool _canOnlyShowOneNode;
        public bool CanOnlyShowOneNode
        {
            get => _canOnlyShowOneNode;
            set
            {
                _canOnlyShowOneNode = value;
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="AnimatedNodeGroup"/>
        /// </summary>
        public AnimatedNodeGroup()
        {
            NodeList = new Dictionary<string, AnimatedNode>();
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
                NodeList.Remove(id);
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
    }
}
