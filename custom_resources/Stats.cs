namespace DeckBuilder;

using Godot;

[GlobalClass]
public partial class Stats : Resource {
  [Export]
  public int MaxHealth { get; set; } = 1;
  [Export]
  public Texture2D Art { get; set; } = default!;

  private int _health;
  public int Health {
    get => _health;
    set {
      _health = Mathf.Clamp(value, 0, MaxHealth);
      EmitChanged();
    }
  }

  private int _block;
  public int Block {
    get => _block;
    set {
      _block = Mathf.Clamp(value, 0, 999);
      EmitChanged();
    }
  }

  public virtual void TakeDamage(int damage) {
    if (damage <= 0) {
      return;
    }
    var initialDamage = damage;
    damage = Mathf.Clamp(damage - Block, 0, damage);
    Block = Mathf.Clamp(Block - initialDamage, 0, Block);
    Health -= damage;
  }

  public void Heal(int amount) => Health += amount;
}
