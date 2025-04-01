using Godot;

using UTheCat.Jumpvalley.Core.Levels;

namespace JumpvalleyApp.Testing
{
    /// <summary>
    /// A test that allows a user to see what the difficulty colors of Jumpvalley's primary difficulties would look like.
    /// </summary>
    public partial class DifficultyColorTest : System.IDisposable
    {
        private readonly string TEST_LEVEL_NAME = "A Mess of Parts";
        private readonly string TEST_LEVEL_AUTHOR = "DifficultyColorTester";

        public GridContainer UIGrid { get; private set; }

        public DifficultyColorTest()
        {
            UIGrid = new GridContainer();

            foreach (Difficulty d in DifficultyPresets.PRIMARY_DIFFICULTIES.Difficulties)
            {
                Label label = new Label();
                label.Text = $"{TEST_LEVEL_NAME} by {TEST_LEVEL_AUTHOR} [{d.Name} - {d.Rating}]";

                LabelSettings labelSettings = new LabelSettings();
                labelSettings.FontColor = d.Color;

                label.LabelSettings = labelSettings;
                UIGrid.AddChild(label);
            }
        }

        public void Dispose()
        {
            UIGrid.QueueFree();
            UIGrid.Dispose();
        }
    }
}
