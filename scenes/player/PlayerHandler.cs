namespace DeckBuilder;

using System.Linq;
using System.Threading.Tasks;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class PlayerHandler : Node {
  [Signal]
  public delegate void PlayerDeadEventHandler();
  [Signal]
  public delegate void DamageTakenEventHandler();


  public override partial void _Notification(int what);

  public Hand Hand { get; set; } = default!;

  private CharacterStats _characterStats = default!;

  public void StartBattle(CharacterStats characterStats) {
    if (_characterStats != null) {
      _characterStats.Changed -= OnCharacterStatsChanged;
      _characterStats.DamageTaken -= OnDamageTaken;
    }

    _characterStats = characterStats;
    _characterStats.Changed += OnCharacterStatsChanged;
    _characterStats.DamageTaken += OnDamageTaken;
    _characterStats.DrawPile = (CardPile)_characterStats.Deck.Duplicate(true);
    _characterStats.DrawPile.Shuffle();
    _characterStats.Discard = new CardPile();
  }

  public async Task StartTurn() {
    Hand.IsDisabled = true;
    _characterStats.Block = 0;
    _characterStats.ResetMana();
    await DrawCards(_characterStats.CardPerTurn);
    Hand.IsDisabled = false;
  }

  public async Task EndTurn() {
    Hand.IsDisabled = true;
    await DiscardCards();
  }

  private void DrawCard() {
    Hand.IsDisabled = true;
    ReshuffleDeckFromDiscard();
    Hand.AddCard(_characterStats.DrawPile.DrawCard());
    ReshuffleDeckFromDiscard();
  }

  private async Task DrawCards(int amount) {
    var tween = CreateTween();
    foreach (var i in Enumerable.Range(0, amount)) {
      tween.TweenCallback(Callable.From(DrawCard));
      tween.TweenInterval(0.25);
    }
    Hand.IsDisabled = false;
    await ToSignal(tween, Tween.SignalName.Finished);
  }

  private async Task DiscardCards() {
    Hand.IsDisabled = true;
    var tween = CreateTween();
    foreach (var node in Hand.GetChildren()) {
      if (node is CardUI cardUI) {
        tween.TweenCallback(Callable.From(() => _characterStats.Discard.AddCard(cardUI.Card)));
        tween.TweenCallback(Callable.From(() => Hand.DiscardCard(cardUI)));
        tween.TweenInterval(0.25);
      }
    }
    await ToSignal(tween, Tween.SignalName.Finished);
  }

  private void ReshuffleDeckFromDiscard() {
    if (!_characterStats.DrawPile.IsEmpty()) {
      return;
    }
    while (!_characterStats.Discard.IsEmpty()) {
      _characterStats.DrawPile.AddCard(_characterStats.Discard.DrawCard());
    }
    _characterStats.DrawPile.Shuffle();
  }

  private void OnCharacterStatsChanged() {
    if (_characterStats.Health <= 0) {
      EmitSignal(SignalName.PlayerDead);
    }
  }

  private void OnDamageTaken() => EmitSignal(SignalName.DamageTaken);
}
