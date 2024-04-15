namespace DeckBuilder;

using System.Collections.Generic;
using Godot;

public partial class DamageEffect : Effect {
  public int Amount { get; set; }

  public override void Execute(List<Node> targets) {
    foreach (var target in targets) {
      if (target is Player player) {
        player.TakeDamage(Amount);
      }
      if (target is Enemy enemy) {
        enemy.TakeDamage(Amount);
      }
    }
  }
}
