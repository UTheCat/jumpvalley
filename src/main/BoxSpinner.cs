using Godot;

// refer to this article for naming conventions:
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
public partial class BoxSpinner: Node {
    private CsgBox3D box;
    private double radiansPerSecond = 0;

    public BoxSpinner(CsgBox3D newBox, double newRadiansPerSecond) {
        box = newBox;
        radiansPerSecond = newRadiansPerSecond;
    }

    public void RotateInFrame(double delta) {
        box.RotateY((float) (radiansPerSecond * delta));
    }

    // overrides the _Process method that comes from the "Node" class
    public override void _Process(double delta) {

    }
}