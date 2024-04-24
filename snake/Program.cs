namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new SnakeGame();
            game.Initialize();
            game.Run();
        }
    }

    public class SnakeGame
    {
        public World GameWorld { get; private set; }
        public Snake Snake { get; private set; }
        public bool IsGameOver { get; private set; } = false;
        public TimeSpan UpdateInterval { get; } = TimeSpan.FromMilliseconds(500);
        private DateTime lastUpdateTime;

        public void Initialize()
        {
            GameWorld = new World(32, 16);
            Snake = new Snake(GameWorld.ScreenWidth / 2, GameWorld.ScreenHeight / 2);
            GameWorld.PlaceBerry();
        }

        public void Run()
        {
            lastUpdateTime = DateTime.Now;
            while (!IsGameOver)
            {
                if (Console.KeyAvailable)
                {
                    ProcessInput();
                }

                if (DateTime.Now - lastUpdateTime >= UpdateInterval)
                {
                    Snake.Move();
                    GameWorld.CheckBerryCollision(Snake);
                    DrawWorld();
                    IsGameOver = Snake.CheckCollision() || GameWorld.CheckWallCollision(Snake);
                    lastUpdateTime = DateTime.Now;
                }
            }
            EndGame();
        }

        private void ProcessInput()
        {
            var key = Console.ReadKey(true).Key;
            Snake.ChangeDirection(key);
        }

        private void DrawWorld()
        {
            Console.Clear();
            GameWorld.DrawBorders();
            GameWorld.DrawBerry();
            Snake.Draw();
        }

        private void EndGame()
        {
            Console.SetCursorPosition(GameWorld.ScreenWidth / 5, GameWorld.ScreenHeight / 2);
            Console.WriteLine($"Game over, Score: {Snake.Score}");
        }
    }

    public class World
    {
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int BerryX { get; private set; }
        public int BerryY { get; private set; }
        public Random RandomNumber { get; private set; } = new Random();
        public static char WallChar { get; } = '■';
        public static char BerryChar { get; } = '■';

        public World(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
        }

        public void PlaceBerry()
        {
            BerryX = RandomNumber.Next(1, ScreenWidth - 2);
            BerryY = RandomNumber.Next(1, ScreenHeight - 2);
        }

        public void CheckBerryCollision(Snake snake)
        {
            if (snake.HeadX == BerryX && snake.HeadY == BerryY)
            {
                snake.Score++;
                PlaceBerry();
            }
        }

        public bool CheckWallCollision(Snake snake)
        {
            return snake.HeadX == 0 || snake.HeadX == ScreenWidth - 1 ||
                   snake.HeadY == 0 || snake.HeadY == ScreenHeight - 1;
        }

        public void DrawBorders()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < ScreenWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write(WallChar);
                Console.SetCursorPosition(i, ScreenHeight - 1);
                Console.Write(WallChar);
            }

            for (int i = 0; i < ScreenHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(WallChar);
                Console.SetCursorPosition(ScreenWidth - 1, i);
                Console.Write(WallChar);
            }
        }

        public void DrawBerry()
        {
            Console.SetCursorPosition(BerryX, BerryY);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(BerryChar);
        }
    }

    public class Snake
    {
        public int HeadX { get; private set; }
        public int HeadY { get; private set; }
        public ConsoleColor BodyColor { get; } = ConsoleColor.Green;
        public ConsoleColor HeadColor { get; } = ConsoleColor.Red;
        public static char SnakeChar { get; } = '■';
        public int Score { get; set; } = 0;
        private string direction = "RIGHT";
        private List<(int, int)> body = new List<(int, int)>();

        public Snake(int startX, int startY)
        {
            HeadX = startX;
            HeadY = startY;
            body.Add((HeadX, HeadY));
        }

        public void ChangeDirection(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow when direction != "DOWN":
                    direction = "UP";
                    break;
                case ConsoleKey.DownArrow when direction != "UP":
                    direction = "DOWN";
                    break;
                case ConsoleKey.LeftArrow when direction != "RIGHT":
                    direction = "LEFT";
                    break;
                case ConsoleKey.RightArrow when direction != "LEFT":
                    direction = "RIGHT";
                    break;
            }
        }

        public void Move()
        {
            switch (direction)
            {
                case "UP": HeadY--; break;
                case "DOWN": HeadY++; break;
                case "LEFT": HeadX--; break;
                case "RIGHT": HeadX++; break;
            }

            body.Add((HeadX, HeadY));

            if (body.Count > Score + 1)
            {
                body.RemoveAt(0);
            }
        }

        public bool CheckCollision()
        {
            for (int i = 0; i < body.Count - 1; i++)
            {
                if (body[i].Item1 == HeadX && body[i].Item2 == HeadY)
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw()
        {
            Console.ForegroundColor = BodyColor;
            foreach (var (x, y) in body)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(SnakeChar);
            }
            Console.ForegroundColor = HeadColor;
            Console.SetCursorPosition(HeadX, HeadY);
            Console.Write(SnakeChar);
        }
    }
}