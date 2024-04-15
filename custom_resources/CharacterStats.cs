namespace DeckBuilder;

using Godot;

[GlobalClass]
public partial class CharacterStats : Stats {
  [Signal]
  public delegate void DamageTakenEventHandler();

  #region Export
  [Export]
  public CardPile StartingDeck { get; set; } = default!;
  [Export]
  public int CardPerTurn { get; set; }
  [Export]
  public int MaxMana { get; set; }
  #endregion

  private int _mana;
  public int Mana {
    get => _mana;
    set {
      _mana = value;
      EmitChanged();
    }
  }
  public CardPile Deck { get; set; } = default!;
  public CardPile Discard { get; set; } = default!;
  public CardPile DrawPile { get; set; } = default!;

  public void ResetMana() => Mana = MaxMana;

  public override void TakeDamage(int damage) {
    var initialHealth = Health;
    base.TakeDamage(damage);
    if (initialHealth > Health) {
      EmitChanged();
      EmitSignal(SignalName.DamageTaken);
    }
  }

  public bool CanPlayCard(Card card) => Mana >= card.Cost;

  public CharacterStats CreateInstance() {
    var instance = (CharacterStats)Duplicate();

    instance.Health = MaxHealth;
    instance.Block = 0;
    instance.ResetMana();
    instance.Deck = (CardPile)StartingDeck.Duplicate();
    instance.Discard = new CardPile();
    instance.DrawPile = new CardPile();
    return instance;
  }
}
