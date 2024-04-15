namespace DeckBuilder;

using Godot;

[GlobalClass]
public partial class CardPile : Resource {
  [Export] public Godot.Collections.Array<Card> Cards { get; set; } = new Godot.Collections.Array<Card>();

  public bool IsEmpty() => Cards.Count == 0;

  public Card DrawCard() {
    // IllegalIndexException may occur
    var card = Cards[0];
    Cards.RemoveAt(0);
    return card;
  }

  public void AddCard(Card card) => Cards.Add(card);

  public void Shuffle() => Cards.Shuffle();

  public void Clear() => Cards.Clear();
}
