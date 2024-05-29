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
        /// List of currently visible AnimatedNodes.
        /// As the end of this list is reached, the earlier the AnimatedNode has been shown.
        /// </summary>
        private List<AnimatedNode> visibleNodes;

        /// <summary>
        /// The nodes within the group.
        /// <br/><br/>
        /// In this dictionary, the keys are strings which are IDs that
        /// can be assigned to an <see cref="AnimatedNode"/> within the group.
        /// The corresponding value is the <see cref="AnimatedNode"/> associated with the ID. 
        /// </summary>
        public Dictionary<string, AnimatedNode> NodeList;

        // Set to -2 initially so that the MaxVisibleNodes setter will actually work
        // when the AnimatedNodeGroup instance is being constructed.
        private int _maxVisibleNodes = -2;

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
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="AnimatedNodeGroup"/> 
        /// </summary>
        public AnimatedNodeGroup()
        {
            NodeList = new Dictionary<string, AnimatedNode>();
            visibleNodes = new List<AnimatedNode>();
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
        /// Hides one of the <see cref="AnimatedNode"/>s in <see cref="NodeList"/>.
        /// </summary>
        /// <param name="id">The string identifier of the animated node to hide</param>
        public void Hide(string id)
        {
            AnimatedNode node = NodeList[id];

            // For stability reasons, only hide the AnimatedNode if it was shown
            // via this class's Show method
            int index = visibleNodes.IndexOf(node);
            if (index >= 0)
            {
                node.IsVisible = false;
                visibleNodes.RemoveAt(index);
            }
        }

        /// <summary>
        /// Hides all of the <see cref="AnimatedNode"/>s that have been made visible
        /// by this instance of <see cref="AnimatedNodeGroup"/>. 
        /// </summary>
        public void HideAll()
        {
            foreach (AnimatedNode node in visibleNodes)
            {
                node.IsVisible = false;
            }

            visibleNodes.Clear();
        }

        /// <summary>
        /// Shows one of the <see cref="AnimatedNode"/>s in <see cref="NodeList"/>.
        /// </summary>
        /// <param name="id">The string identifier of the animated node to show</param>
        public void Show(string id)
        {
            AnimatedNode node = NodeList[id];

            if (!visibleNodes.Contains(node))
            {
                visibleNodes.Add(node);
                node.IsVisible = true;

                // If we went over the maximum visible node count,
                // hide visible nodes at the end of the list.
                if (visibleNodes.Count > MaxVisibleNodes)
                {
                    int excess = visibleNodes.Count - MaxVisibleNodes;
                    for (int i = 0; i < excess; i++)
                    {
                        AnimatedNode removedNode = visibleNodes[i];
                        removedNode.IsVisible = false;

                        visibleNodes.RemoveAt(visibleNodes.Count - 1);
                    }
                }
            }
        }
    }
}
