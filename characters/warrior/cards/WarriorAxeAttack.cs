namespace DeckBuilder;

using System.Collections.Generic;
using Godot;

public partial class WarriorAxeAttack : Card {
  public override void ApplyEffect(List<Node> targets) {
    var damageEffect = new DamageEffect {
      Amount = 6
    };
    damageEffect.Execute(targets);
  }
}
