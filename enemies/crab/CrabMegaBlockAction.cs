namespace DeckBuilder;

using System.Threading.Tasks;
using Godot;


public partial class CrabMegaBlockAction : EnemyAction {
  [Export] public int Block { get; set; } = 15;
  [Export] public int HpThreshold { get; set; } = 6;
  private bool _isAlreadyUsed;

  public override bool IsPerformable() {
    if (Enemy == null || _isAlreadyUsed) {
      return false;
    }

    _isAlreadyUsed = Enemy.Stats.Health <= HpThreshold;
    return _isAlreadyUsed;
  }

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
