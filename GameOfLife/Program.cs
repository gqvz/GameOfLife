using GameOfLife;
using PixelWindowSystem;

var appManager = new GameOfLifeAppManager();

var window = new PixelWindow(GameOfLifeAppManager.WindowWidth, GameOfLifeAppManager.WindowHeight, GameOfLifeAppManager.PixelScale,
    "Game of life", appManager, framerateLimit: GameOfLifeAppManager.EditingFramerate);

window.Run();