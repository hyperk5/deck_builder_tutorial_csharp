namespace DeckBuilder;

using Godot;

[GlobalClass]
public partial class EnemyStats : Stats {

  [Export]
  public PackedScene Ai { get; set; } = default!;

  public EnemyStats CreateInstance() {
    var instance = (EnemyStats)Duplicate();
    instance.Health = MaxHealth;
    instance.Block = 0;
    return instance;
  }
}
