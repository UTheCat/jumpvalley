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
        /// List of nodes
        /// </summary>
        public Dictionary<string, AnimatedNode> NodeList;
    }
}
