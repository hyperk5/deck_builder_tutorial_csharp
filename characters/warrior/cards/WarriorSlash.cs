namespace DeckBuilder;

using System.Collections.Generic;
using Godot;

public partial class WarriorSlash : Card {
  public override void ApplyEffect(List<Node> targets) {
    var damageEffect = new DamageEffect {
      Amount = 4
    };
    damageEffect.Execute(targets);
  }
}
