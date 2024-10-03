using Godot;

// refer to this article for naming conventions:
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
namespace JumpvalleyGame
{
    public partial class MeshSpinner : Node
    {
        private MeshInstance3D mesh;
        private double radiansPerSecond = 0;

        public MeshSpinner(MeshInstance3D newMesh, double newRadiansPerSecond)
        {
            mesh = newMesh;
            radiansPerSecond = newRadiansPerSecond;
        }

        public void RotateInFrame(double delta)
        {
            mesh.RotateY((float)(radiansPerSecond * delta));
        }

        // overrides the _Process method that comes from the "Node" class
        public override void _Process(double delta)
        {
            RotateInFrame(delta);
        }
    }
}
