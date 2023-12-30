using Godot;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// Event arguments for <see cref="InteractiveNode.NodeMetadataChanged"/>
    /// </summary>
    public partial class NodeMetadataChangedArgs
    {
        /// <summary>
        /// The name of the metadata entry that had its value changed.
        /// </summary>
        public string MetadataName = "";

        /// <summary>
        /// The old value of the metadata entry
        /// </summary>
        public Variant OldValue;

        /// <summary>
        /// The new value of the metadata entry
        /// </summary>
        public Variant NewValue;

        public NodeMetadataChangedArgs(string metadataName, Variant oldValue, Variant newValue)
        {
            MetadataName = metadataName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
