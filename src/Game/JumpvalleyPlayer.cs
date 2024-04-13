using Godot;
using Jumpvalley.Players;

namespace Jumpvalley.Game
{
    /// <summary>
    /// Full implementation of the Player class specific to Jumpvalley itself
    /// </summary>
    public partial class JumpvalleyPlayer : Player
    {
        public JumpvalleyPlayer(SceneTree tree, Node rootNode) : base(tree, rootNode) { }
    }
}
