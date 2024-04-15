namespace DeckBuilder;

using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class Card : Resource {

  #region Enum
  public enum CardType {
    ATTACK,
    SKILL,
    POWER
  }
  public enum TargetType {
    SELF,
    SINGLE_ENEMY,
    ALL_ENEMIES,
    EVERYONE
  }
  #endregion

  #region Export
  [ExportGroup("Card Attributes")]
  [Export]
  public string ID { get; set; } = default!;
  [Export]
  public CardType Type { get; set; } = default!;
  [Export]
  public TargetType Target { get; set; } = default!;
  [Export]
  public int Cost { get; set; }
  [Export]
  public AudioStream? Sound { get; set; }

  [ExportGroup("Card Visuals")]
  [Export]
  public Texture2D Icon { get; set; } = default!;
  [Export(PropertyHint.MultilineText)]
  public string TooltipText { get; set; } = "";
  #endregion

  public bool IsSingleTargeted() => Target == TargetType.SINGLE_ENEMY;

  public void Play(List<Node> targets, CharacterStats characterStats) {
    characterStats.Mana -= Cost;
    ApplyEffect(GetTargets(targets));
  }

  public virtual void ApplyEffect(List<Node> targets) { }

  private List<Node> GetTargets(List<Node> targets) {
    if (targets.Count == 0) {
      return new List<Node>();
    }

    var tree = targets[0].GetTree();
    return Target switch {
      TargetType.SELF => tree.GetNodesInGroup("player").ToList(),
      TargetType.SINGLE_ENEMY => GetSingleTargets(targets),
      TargetType.ALL_ENEMIES => tree.GetNodesInGroup("enemies").ToList(),
      TargetType.EVERYONE => tree.GetNodesInGroup("player").Concat(tree.GetNodesInGroup("enemies")).ToList(),
      _ => new List<Node>(),
    };
  }

  private static List<Node> GetSingleTargets(List<Node> targets) {
    if (targets.Count != 1 || targets[0] is not Enemy) {
      return new List<Node>();
    }
    return targets;
  }
}
