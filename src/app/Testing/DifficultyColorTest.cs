using Godot;
using System.Collections.Generic;

using UTheCat.Jumpvalley.Core.Levels;

namespace UTheCat.Jumpvalley.App.Testing
{
    /// <summary>
    /// A test that allows a user to see what the difficulty colors of Jumpvalley's primary difficulties would look like.
    /// </summary>
    public partial class DifficultyColorTest : System.IDisposable
    {
        private readonly string TEST_LEVEL_NAME = "A Mess of Parts";
        private readonly string TEST_LEVEL_AUTHOR = "DifficultyColorTester";

        private List<LabelSettings> labelSettingsList = new List<LabelSettings>();

        public GridContainer UIGrid { get; private set; }

        private int _textOutlineSize = 0;
        public int TextOutlineSize
        {
            get => _textOutlineSize;
            set
            {
                _textOutlineSize = value;

                foreach (LabelSettings l in labelSettingsList) l.OutlineSize = value;
            }
        }

        public DifficultyColorTest()
        {
            UIGrid = new GridContainer();

            foreach (Difficulty d in DifficultyPresets.PRIMARY_DIFFICULTIES.Difficulties)
            {
                Label label = new Label();
                label.Text = $"{TEST_LEVEL_NAME} by {TEST_LEVEL_AUTHOR} [{d.Name} - {d.Rating}]";

                LabelSettings labelSettings = new LabelSettings();
                labelSettings.FontColor = d.Color;
                labelSettings.OutlineColor = new Color(0f, 0f, 0f);
                labelSettings.OutlineSize = _textOutlineSize;

                label.LabelSettings = labelSettings;
                labelSettingsList.Add(labelSettings);
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
