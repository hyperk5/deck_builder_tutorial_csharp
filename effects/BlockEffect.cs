namespace DeckBuilder;

using System.Collections.Generic;
using Godot;

public partial class BlockEffect : Effect {
  public int Amount { get; set; }

  public override void Execute(List<Node> targets) {
    foreach (var target in targets) {
      if (target is Player player) {
        player.Stats.Block += Amount;
      }
      if (target is Enemy enemy) {
        enemy.Stats.Block += Amount;
      }
    }
  }
}
