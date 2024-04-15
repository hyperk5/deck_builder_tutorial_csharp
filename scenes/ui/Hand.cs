namespace DeckBuilder;

using Godot;

public partial class Hand : HBoxContainer {

  public CharacterStats CharacterStats { get; set; } = default!;

  private static readonly PackedScene _cardUIScene = GD.Load<PackedScene>("res://scenes/card/card_ui.tscn");

  // master flag for cards disabled flag.
  private bool _isDisabled;
  public bool IsDisabled { get => _isDisabled; set => SetIsDisabled(value); }

  public void AddCard(Card card) {
    var cardUI = _cardUIScene.Instantiate<CardUI>();
    cardUI.CharacterStats = CharacterStats;
    cardUI.Card = card;
    cardUI.ReparentRequest += OnReparentRequest;
    cardUI.CardIndex = GetChildCount();
    cardUI.IsDisabled = true;
    AddChild(cardUI);
  }

  public void DiscardCard(CardUI cardUI) => cardUI.QueueFree();

  public void SetIsDisabled(bool isDisabled) {
    _isDisabled = isDisabled;
    foreach (var c in GetChildren()) {
      if (c is CardUI tempCard) {
        tempCard.IsDisabled = _isDisabled;
      }
    }
  }

  private void OnReparentRequest(CardUI cardUI, int type) {
    // reparent and move same index
    if (type == CardUI.REPARENT_TYPE_CANCEL) {
      cardUI.Reparent(this);
      CallDeferred(MethodName.MoveChild, cardUI, cardUI.CardIndex);
    }

    // update IsDisabled Flag
    foreach (var c in GetChildren()) {
      if (c is CardUI tempCard) {
        tempCard.IsDisabled = type == CardUI.REPARENT_TYPE_SELECT || IsDisabled;
      }
    }

    // rearrange card index and discard played card
    if (type == CardUI.REPARENT_TYPE_PLAY) {
      var index = 0;
      foreach (var c in GetChildren()) {
        if (c is CardUI tempCard) {
          tempCard.CardIndex = index;
          ++index;
        }
      }

      // if the card is played, it should be in Discard.
      CharacterStats.Discard.AddCard(cardUI.Card);
    }
  }
}
