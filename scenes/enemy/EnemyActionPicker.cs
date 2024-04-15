namespace DeckBuilder;

using Godot;

[GlobalClass]
public partial class EnemyActionPicker : Node {

  private Enemy? _enemy;
  public Enemy? Enemy {
    get => _enemy;
    set => SetEnemy(value);
  }

  private Player? _target;
  public Player? Target {
    get => _target;
    set => SetTarget(value);
  }

  private double TotalWeight { get; set; }

  public override void _Ready() {
    Target = GetTree().GetFirstNodeInGroup("player") as Player;
    SetupChances();
  }

  public EnemyAction? GetAction() {
    var action = GetFirstConditionalAction();
    if (action != null) {
      return action;
    }
    return GetChanceBasedAction();
  }

  public EnemyAction? GetFirstConditionalAction() {
    foreach (var c in GetChildren()) {
      if (c is EnemyAction action
      && action.ActionType == EnemyAction.Type.Conditional
      && action.IsPerformable()) {
        return action;
      }
    }
    return null;
  }

  private EnemyAction? GetChanceBasedAction() {
    var roll = GD.RandRange(0, TotalWeight);
    foreach (var c in GetChildren()) {
      if (c is EnemyAction action
      && action.ActionType == EnemyAction.Type.ChanceBased
      && action.AccumulatedWeight >= roll) {
        return action;
      }
    }
    return null;
  }

  private void SetupChances() {
    foreach (var c in GetChildren()) {
      if (c is EnemyAction action && action.ActionType == EnemyAction.Type.ChanceBased) {
        TotalWeight += action.ChanceWeight;
        action.AccumulatedWeight = TotalWeight;
      }
    }
  }

  private void SetEnemy(Enemy? value) {
    _enemy = value;
    foreach (var c in GetChildren()) {
      if (c is EnemyAction action) {
        action.Enemy = _enemy;
      }
    }
  }

  private void SetTarget(Player? value) {
    _target = value;
    foreach (var c in GetChildren()) {
      if (c is EnemyAction action) {
        action.Target = _target;
      }
    }
  }
}
