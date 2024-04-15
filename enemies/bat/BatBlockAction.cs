namespace DeckBuilder;

using System.Threading.Tasks;
using Godot;

public partial class BatBlockAction : EnemyAction {
  [Export] public int Block { get; set; } = 4;

  public override async Task PerformAction() {
    if (Enemy == null || Target == null) {
      return;
    }
    var effect = new BlockEffect {
      Amount = Block
    };
    effect.Execute(new System.Collections.Generic.List<Node> { Enemy });
    PlaySound();
    var timer = GetTree().CreateTimer(0.6, false);
    await ToSignal(timer, Timer.SignalName.Timeout);
  }
}
