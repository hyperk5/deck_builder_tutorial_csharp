namespace DeckBuilder;

using System.Collections.Generic;
using Godot;

public partial class WarriorBlock : Card {
  public override void ApplyEffect(List<Node> targets) {
    var blockEffect = new BlockEffect {
      Amount = 5
    };
    blockEffect.Execute(targets);
  }
}
