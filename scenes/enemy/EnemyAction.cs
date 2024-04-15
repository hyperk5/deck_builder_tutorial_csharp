namespace DeckBuilder;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public abstract partial class EnemyAction : Node {
  public enum Type {
    Conditional,
    ChanceBased
  }

  [Export] public Type ActionType { get; set; }
  [Export] public double ChanceWeight { get; set; }
  [Export] public Intent Intent { get; set; } = default!;
  [Export] public AudioStream Sound { get; set; } = default!;

  public double AccumulatedWeight { get; set; }

  public Enemy? Enemy { get; set; }

  public Player? Target { get; set; }

  public virtual bool IsPerformable() => false;

  public abstract Task PerformAction();

  public void PlaySound() => GetNode<SoundPlayer>("/root/SfxPlayer").Play(Sound);
}
