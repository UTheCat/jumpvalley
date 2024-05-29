using System;
using System.Collections.Generic;

namespace Jumpvalley.Animation
{
    /// <summary>
    /// Class that groups multiple <see cref="AnimatedNode"/>s together so that they can communicate with each other.
    /// <br/><br/>
    /// This class can also help make it so only one <see cref="AnimatedNode"/> within the group is visible at a time. 
    /// </summary>
    public partial class AnimatedNodeGroup
    {
        private AnimatedNode[] visibleNodes;

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
                if (value == _maxVisibleNodes) return;

                _maxVisibleNodes = value;
                visibleNodes = new AnimatedNode[Math.Max(value, 0)];
            }
        }

        public AnimatedNodeGroup()
        {
            NodeList = new Dictionary<string, AnimatedNode>();
            MaxVisibleNodes = -1;
        }
    }
}
