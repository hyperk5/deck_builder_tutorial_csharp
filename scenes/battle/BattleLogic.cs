namespace DeckBuilder;

using Chickensoft.LogicBlocks;

public class BattleLogic : LogicBlock<BattleLogic.State> {

  public override State GetInitialState() => new Init();

  public static class Input {
    public readonly record struct PlayerIsReady;
    public readonly record struct PlayerTurnEnd;
    public readonly record struct EnemyTurnEnd;
    public readonly record struct PlayerDead;
    public readonly record struct EnemiesAllDead;
  }

  public static class Output {
    public readonly record struct InitEnter;
    public readonly record struct PlayerTurnEnter;
    public readonly record struct PlayerTurnExit;
    public readonly record struct EnemyTurnEnter;
    public readonly record struct GameOverEnter;
    public readonly record struct VictoryEnter;
  }

  public abstract record State : StateLogic {

  }

  public record Init : State, IGet<Input.PlayerIsReady> {
    public Init() {
      OnEnter<Init>(_ => Context.Output(new Output.InitEnter()));
    }
    public State On(Input.PlayerIsReady input) => new PlayerTurn();
  }

  public record PlayerTurn : State, IGet<Input.PlayerTurnEnd>, IGet<Input.EnemiesAllDead> {
    public PlayerTurn() {
      OnEnter<PlayerTurn>(_ => Context.Output(new Output.PlayerTurnEnter()));
      OnExit<PlayerTurn>(_ => Context.Output(new Output.PlayerTurnExit()));
    }
    public State On(Input.PlayerTurnEnd input) => new EnemyTurn();
    public State On(Input.EnemiesAllDead input) => new Victory();
  }

  public record EnemyTurn : State, IGet<Input.EnemyTurnEnd>, IGet<Input.PlayerDead> {
    public EnemyTurn() {
      OnEnter<EnemyTurn>(_ => Context.Output(new Output.EnemyTurnEnter()));
    }
    public State On(Input.EnemyTurnEnd input) => new PlayerTurn();
    public State On(Input.PlayerDead input) => new GameOver();
  }

  public record Victory : State {
    public Victory() {
      OnEnter<Victory>(_ => Context.Output(new Output.VictoryEnter()));
    }
  }

  public record GameOver : State {
    public GameOver() {
      OnEnter<GameOver>(_ => Context.Output(new Output.GameOverEnter()));
    }
  }

}
