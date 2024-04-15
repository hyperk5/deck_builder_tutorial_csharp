namespace DeckBuilder;

using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class StatsUI : HBoxContainer {
  public override partial void _Notification(int what);

  [Node] public HBoxContainer Block { get; set; } = default!;
  [Node("%BlockLabel")] public Label BlockLabel { get; set; } = default!;
  [Node] public HBoxContainer Health { get; set; } = default!;
  [Node("%HealthLabel")] public Label HealthLabel { get; set; } = default!;

  public void UpdateStats(Stats stats) {
    BlockLabel.Text = stats.Block.ToString();
    HealthLabel.Text = stats.Health.ToString();

    Block.Visible = stats.Block > 0;
    Health.Visible = stats.Health > 0;
  }
}
