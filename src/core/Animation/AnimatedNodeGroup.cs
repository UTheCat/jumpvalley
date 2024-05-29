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
        /// <summary>
        /// The nodes within the group.
        /// <br/><br/>
        /// In this dictionary, the keys are strings which are IDs that
        /// can be assigned to an <see cref="AnimatedNode"/> within the group.
        /// The corresponding value is the <see cref="AnimatedNode"/> associated with the ID. 
        /// </summary>
        public Dictionary<string, AnimatedNode> NodeList;
    }
}
