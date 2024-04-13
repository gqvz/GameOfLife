using PixelWindowSystem;
using SFML.Graphics;
using SFML.Window;

namespace GameOfLife;

public class GameOfLifeAppManager : IPixelWindowAppManager
{
    public const int PixelScale = 1;
    public const int WindowWidth = 800;
    public const int WindowHeight = 450;
    
    public const int Width = WindowWidth / PixelScale;
    public const int Height = WindowHeight / PixelScale;
    
    public const uint EditingFramerate = 500;
    public const uint GameFramerate = 500;
    
    private static readonly (byte, byte, byte) Dead = (0, 0, 0);
    private static readonly (byte, byte, byte) Alive = (255, 255, 255);
    
    private PixelData _old = null!;
    private PixelData _new = null!;
    private RenderWindow _window = null!;

    private bool _editing = true;
    private bool _leftMouseButtonPressed;
    private bool _rightMouseButtonPressed;
    
    public void OnLoad(RenderWindow renderWindow)
    {
        _window = renderWindow;
        _old = new PixelData(Width, Height);
        _new = new PixelData(Width, Height);
        
        _window.KeyReleased += (_, args) =>
        {
            if (args.Code is Keyboard.Key.Enter) _editing = !_editing;
            _window.SetFramerateLimit(_editing ? EditingFramerate : GameFramerate);
        };
        
        _window.MouseButtonPressed += (_, args) =>
        {
            switch (args.Button)
            {
                case Mouse.Button.Left:
                    _leftMouseButtonPressed = true;
                    break;
                case Mouse.Button.Right:
                    _rightMouseButtonPressed = true;
                    break;
            }
        };
        
        _window.MouseButtonReleased += (_, args) =>
        {
            switch (args.Button)
            {
                case Mouse.Button.Left:
                    _leftMouseButtonPressed = false;
                    break;
                case Mouse.Button.Right:
                    _rightMouseButtonPressed = false;
                    break;
            }
        };
        
        _window.MouseMoved += (_, args) =>
        {
            if (!_editing || args.X > WindowWidth || args.Y > WindowHeight) return;
            var x = (uint)(args.X / PixelScale);
            var y = (uint)(args.Y / PixelScale);
            if (_leftMouseButtonPressed)
                _new[x, y] = Alive;
            else if (_rightMouseButtonPressed)
                _new[x, y] = Dead;
        };
    }

    public void Update(float frameTime)
    {
        if (_editing) return;
        
        for (uint i = 0; i < Height; i++)
        for (uint j = 0; j < Width; j++)
        {
            var neighbours = new[]
            {
                i >= 1 ? _old[j, i - 1] : Dead, // up
                i + 1 < Height ? _old[j, i + 1] : Dead, // down
                j + 1 < Width ? _old[j + 1, i] : Dead, // right
                j >= 1 ? _old[j - 1, i] : Dead, // left

                j >= 1 && i >= 1 ? _old[j - 1, i - 1] : Dead, // upLeft
                j + 1 < Width && i >= 1 ? _old[j + 1, i - 1] : Dead, // upRight
                j >= 1 && i + 1 < Height ? _old[j - 1, i + 1] : Dead, // downLeft
                j + 1 < Width && i + 1 < Height ? _old[j + 1, i + 1] : Dead, // downRight
            };

            var aliveNeighbours = neighbours.Count(neighbour => neighbour.Item1 == 255);

            var currentState = _old[j, i];
            var newState = aliveNeighbours switch
            {
                < 2 => Dead,
                2 => currentState,
                <= 3 => Alive,
                > 3 => Dead,
            };

            _new[j, i] = newState;
        }
    }

    public void FixedUpdate(float timeStep) { }

    public void Render(PixelData pixelData, float frameTime)
    {
        if (!_window.HasFocus()) return;
        pixelData.RawData = _new.RawData;
        _old.RawData = _new.RawData;
    }
}