namespace DeckBuilder;

using System.Collections.Generic;
using Chickensoft.LogicBlocks;
using Godot;

public class CardLogic : LogicBlock<CardLogic.State> {
  public CardLogic(CardUI cardUI) {
    Set(cardUI);
  }

  public override State GetInitialState() => new Base();

  public static class Input {
    public readonly record struct Left;
    public readonly record struct LeftOutsideControl;
    public readonly record struct LeftRelease(ulong EventTime);
    public readonly record struct Right;
    public readonly record struct Moving(ulong EventTime, Vector2 MousePosition);
  }

  public abstract record Output {
    public readonly record struct BaseEnter;
    public readonly record struct ClickedEnter;
    public readonly record struct DraggingEnter;
    public readonly record struct Motion;
    public readonly record struct AimingEnter;
    public readonly record struct AimingExit;
    public readonly record struct Play(List<Node> Targets);
    public readonly record struct ReleaseEnter;
  }

  public abstract record State : StateLogic {
    protected State OnSelect() {
      var cardUI = Get<CardUI>();
      if (cardUI.Targets.Count == 0) {
        return new Base();
      }
      Context.Output(new Output.Play(cardUI.Targets));
      return new Release();
    }
  }

  public record Base : State, IGet<Input.Left> {
    public Base() {
      OnEnter<Base>(_ => Context.Output(new Output.BaseEnter()));
    }
    public State On(Input.Left input) => new Clicked();
  }

  public record Clicked : State, IGet<Input.Moving> {
    public Clicked() {
      OnEnter<Clicked>(_ => Context.Output(new Output.ClickedEnter()));
    }
    public State On(Input.Moving input) => new Dragging { StartTime = input.EventTime };
  }

  public record Dragging : State,
  IGet<Input.Moving>,
  IGet<Input.Right>,
  IGet<Input.Left>,
  IGet<Input.LeftRelease> {
    public Dragging() {
      OnEnter<Dragging>(_ => Context.Output(new Output.DraggingEnter()));
    }
    public ulong StartTime { get; set; }

    public State On(Input.Moving input) {
      var cardUI = Get<CardUI>();
      if (cardUI.Card.Target == Card.TargetType.SINGLE_ENEMY && cardUI.Targets.Count > 0) {
        return new Aiming();
      }
      else {
        Context.Output(new Output.Motion());
        return this;
      }
    }
    public State On(Input.Right input) => new Base();
    public State On(Input.Left input) => OnSelect();
    public State On(Input.LeftRelease input) => input.EventTime - StartTime > 500 ? OnSelect() : this;
  }

  public record Aiming : State,
  IGet<Input.LeftOutsideControl>, IGet<Input.LeftRelease>, IGet<Input.Right>, IGet<Input.Moving> {
    private const int MOUSE_Y_SNAPBACK_THRESHOLD = 138;
    public Aiming() {
      OnEnter<Aiming>(_ => Context.Output(new Output.AimingEnter()));
      OnExit<Aiming>(_ => Context.Output(new Output.AimingExit()));
    }
    public State On(Input.LeftOutsideControl input) => OnSelect();
    public State On(Input.LeftRelease input) => OnSelect();
    public State On(Input.Right input) => new Base();
    public State On(Input.Moving input) => input.MousePosition.Y > MOUSE_Y_SNAPBACK_THRESHOLD ? new Base() : this;
  }

  public record Release : State {
    public Release() {
      OnEnter<Release>(_ => Context.Output(new Output.ReleaseEnter()));
    }
  }
}
