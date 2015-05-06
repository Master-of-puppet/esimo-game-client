using Electrotank.Electroserver5.Api;
/// <summary>
/// state of game
/// </summary>
public interface IGameState {
    GameStateStatus GameState { get; }
}
