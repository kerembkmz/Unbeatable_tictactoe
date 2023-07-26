using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace TicTacToe
{
    static class Program
    {
        static int boardSize = 3;
        static int cellSize = 100;

        static int[,] board = new int[boardSize, boardSize];
        static int currentPlayer = 1; // 1 for player, 2 for bot

        static List<int> allLegalMoves(int[,] currentBoard)
        {
            List<int> legalMoves = new List<int>();

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (currentBoard[y, x] == 0)
                    {
                        int move = y * boardSize + x;
                        legalMoves.Add(move);
                    }
                }
            }

            return legalMoves;
        }

        public static void Main()
        {
            Vector2 loadedWindowSize = GetSavedWindowSize();
            int screenW = (int)loadedWindowSize.X;
            int screenH = (int)loadedWindowSize.Y;

            Raylib.InitWindow(screenW, screenH, "Tic Tac Toe Game");
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {


                List<int> dynamicBoard = allLegalMoves(board); 
                //Player's move
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    int mouseX = Raylib.GetMouseX();
                    int mouseY = Raylib.GetMouseY();

                    if (mouseX >= 0 && mouseY >= 0 && mouseX < boardSize * cellSize && mouseY < boardSize * cellSize)
                    {
                        int x = mouseX / cellSize;
                        int y = mouseY / cellSize;

                        if (board[y, x] == 0)
                        {
                            board[y, x] = currentPlayer;
                            currentPlayer = (currentPlayer == 1) ? 2 : 1;
                        }
                    }
                }

                //Bot's move
                if (currentPlayer == 2)
                {
                    
                    int botMove = botThink(dynamicBoard);
                    int botMoveX = botMove % boardSize;
                    int botMoveY = botMove / boardSize;

                    if (board[botMoveY, botMoveX] == 0)
                    {
                        board[botMoveY, botMoveX] = currentPlayer;
                        currentPlayer = (currentPlayer == 1) ? 2 : 1;
                    }
                }





                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(22, 22, 22, 255));

                DrawBoard();

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

        }

        static void DrawBoard()
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    Rectangle cellRect = new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize);

                    Raylib.DrawRectangleLinesEx(cellRect, 3, Color.BLACK);

                    if (board[y, x] == 1)
                    {
                        Raylib.DrawLineEx(new Vector2(cellRect.x, cellRect.y),
                            new Vector2(cellRect.x + cellSize, cellRect.y + cellSize),
                            3, Color.BLUE);
                        Raylib.DrawLineEx(new Vector2(cellRect.x, cellRect.y + cellSize),
                            new Vector2(cellRect.x + cellSize, cellRect.y),
                            3, Color.BLUE);
                    }
                    else if (board[y, x] == 2)
                    {
                        Raylib.DrawCircle((int)(cellRect.x + cellSize / 2), (int)(cellRect.y + cellSize / 2),
                            cellSize / 2 - 20, Color.RED);
                    }
                }
            }
        }

        static Vector2 GetSavedWindowSize()
        {
            if (File.Exists(FileHelper.PrefsFilePath))
            {
                string prefs = File.ReadAllText(FileHelper.PrefsFilePath);
                if (!string.IsNullOrEmpty(prefs))
                {
                    if (prefs[0] == '0')
                    {
                        return Settings.ScreenSizeSmall;
                    }
                    else if (prefs[0] == '1')
                    {
                        return Settings.ScreenSizeBig;
                    }
                }
            }
            return Settings.ScreenSizeSmall;
        }

        static void SaveWindowSize()
        {
            Directory.CreateDirectory(FileHelper.AppDataPath);
            bool isBigWindow = Raylib.GetScreenWidth() > Settings.ScreenSizeSmall.X;
            File.WriteAllText(FileHelper.PrefsFilePath, isBigWindow ? "1" : "0");
        }

        static int botThink(List<int> legalMoves)
        {
            Random random = new Random();
            int moveIndex = random.Next(legalMoves.Count);
            int move = legalMoves[moveIndex];
            return move;


        }

    }
}