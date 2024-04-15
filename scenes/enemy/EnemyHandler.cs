namespace DeckBuilder;

using System.Threading.Tasks;
using Godot;

public partial class EnemyHandler : Node {
  [Signal]
  public delegate void EnemiesAllDeadEventHandler();

  public override void _Ready() => ChildOrderChanged += OnChildOrderChanged;

  public void ResetEnemyActions() {
    foreach (var child in GetChildren()) {
      if (child is Enemy enemy) {
        enemy.SetCurrentAction(null);
        enemy.UpdateAction();
      }
    }
  }

  public async Task StartTurn() {
    foreach (var child in GetChildren()) {
      if (child is Enemy enemy) {
        await enemy.DoTurn();
      }
    }
  }

  private void OnChildOrderChanged() {
    if (GetChildCount() == 0) {
      EmitSignal(SignalName.EnemiesAllDead);
    }
  }
}
