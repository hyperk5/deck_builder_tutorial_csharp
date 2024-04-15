namespace DeckBuilder;

using System.Threading.Tasks;
using Godot;

public partial class CrabAttackAction : EnemyAction {
  [Export] public int Damage { get; set; } = 7;

  public override async Task PerformAction() {
    if (Enemy == null || Target == null) {
      return;
    }
    var tween = CreateTween().SetTrans(Tween.TransitionType.Quint);
    var start = Enemy.GlobalPosition;
    var end = Target.GlobalPosition + (Vector2.Right * 32);
    var effect = new DamageEffect {
      Amount = Damage
    };
    tween.TweenProperty(Enemy, "global_position", end, 0.4);
    tween.TweenCallback(Callable.From(() => effect.Execute(new System.Collections.Generic.List<Node> { Target })));
    PlaySound();
    tween.TweenInterval(0.25);
    tween.TweenProperty(Enemy, "global_position", start, 0.4);
    await ToSignal(tween, Tween.SignalName.Finished);
  }
}
