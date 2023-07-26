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
        static int scoreOfPlayer = 0;
        static int scoreOfBot = 0;
        static int scoreOfDraw = 0;
        static bool resetGame = false;

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
                string playerScoreS = "Player Score: " + scoreOfPlayer;
                string botScoreS = "Bot Score: " + scoreOfBot;
                string drawScoreS = "Draws: " + scoreOfDraw;

                Raylib.DrawText(playerScoreS, 50, 360, 20, Color.GREEN);
                Raylib.DrawText(botScoreS, 50, 380, 20, Color.RED);
                Raylib.DrawText(drawScoreS, 50, 400, 20, Color.SKYBLUE);

                if (boardIsDraw(board) || boardIsWon(board))
                {
                    string resultText = "";
                    if (boardIsDraw(board))
                    {
                        resultText = "Game is draw";
                    }
                    else
                    {
                        int det2 = boardIsWonBy(board);
                        if (det2 == 1)
                        {
                            resultText = "Game is won by player";
                        }
                        else if (det2 == 2) {
                            resultText = "Game is won by bot";
                        }
                    }

                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                    {

                        // Setting the pressing on the coordinates of the reset button.
                        int buttonX = 300;
                        int buttonY = 300;
                        int buttonWidth = 150;
                        int buttonHeight = 50;

                        int mouseX = Raylib.GetMouseX();
                        int mouseY = Raylib.GetMouseY();

                        if (mouseX >= buttonX && mouseX <= buttonX + buttonWidth && mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
                        {
                            resetGame = true;
                        }

                        if (resetGame)
                        {
                            int determineWinner = boardIsWonBy(board);
                            if (determineWinner == 1)
                            {
                                scoreOfPlayer += 1;
                            }
                            else if (determineWinner == 2)
                            {
                                scoreOfBot += 1;
                            }
                            else if (determineWinner == 0) {
                                scoreOfDraw += 1;
                            }

                            ResetBoard(board, boardSize);

                            resetGame = false;
                        }
                    }


                    Raylib.DrawText(resultText, 400, 400, 20, Color.YELLOW);

                }




                else
                {
                    
                    // Check for player's move with the mouse
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

                    // Check for bot's move
                    if (currentPlayer == 2 && !boardIsDraw(board) && !boardIsWon(board)) //Added the Draw and Won functions here so that bot won't try an illegal move, and we wont get an out of index error.
                    {
                     
                        int bestMove = botThink(board);
                        int botMoveX = bestMove % boardSize;
                        int botMoveY = bestMove / boardSize;

                        board[botMoveY, botMoveX] = 2;
                        currentPlayer = 1;
                    }
                }


                Raylib.BeginDrawing();

                Raylib.ClearBackground(new Color(22, 22, 22, 255));

                Raylib.DrawRectangle(300, 300, 150, 50, Color.GRAY);
                Raylib.DrawText("Reset Game", 320, 310, 20, Color.WHITE);

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

        public static bool boardIsDraw(int[,] currentBoard)
        {
            // Check if there is any empty cell on the board
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (currentBoard[y, x] == 0)
                    {
                        // If there is an empty cell, the board is not a draw
                        return false;
                    }
                }
            }

            // All cells are filled, check if there is a winner
            return !boardIsWon(currentBoard);
        }


        public static bool boardIsWon(int[,] currentBoard)
        {
            //rows check
            for (int row = 0; row < 3; row++)
            {
                if (board[row, 0] != 0 && board[row, 0] == board[row, 1] && board[row, 1] == board[row, 2])
                {
                    return true;
                }
            }

            //columns check
            for (int col = 0; col < 3; col++)
            {
                if (board[0, col] != 0 && board[0, col] == board[1, col] && board[1, col] == board[2, col])
                {
                    return true;
                }
            }

            //main diagonal check
            if (board[0, 0] != 0 && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
            {
                return true;
            }

            //anti-diagonal check
            if (board[0, 2] != 0 && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
            {
                return true;
            }

            // If no winning condition is met, return false
            return false;
        }

        public static int boardIsWonBy(int[,] currentBoard)
        {
            // Rows check
            for (int row = 0; row < 3; row++)
            {
                if (currentBoard[row, 0] != 0 && currentBoard[row, 0] == currentBoard[row, 1] && currentBoard[row, 1] == currentBoard[row, 2])
                {
                    return currentBoard[row, 0]; // Return the winning player number (1 or 2)
                }
            }

            // Columns check
            for (int col = 0; col < 3; col++)
            {
                if (currentBoard[0, col] != 0 && currentBoard[0, col] == currentBoard[1, col] && currentBoard[1, col] == currentBoard[2, col])
                {
                    return currentBoard[0, col]; // Return the winning player number (1 or 2)
                }
            }

            // Main diagonal check
            if (currentBoard[0, 0] != 0 && currentBoard[0, 0] == currentBoard[1, 1] && currentBoard[1, 1] == currentBoard[2, 2])
            {
                return currentBoard[0, 0]; // Return the winning player number (1 or 2)
            }

            // Anti-diagonal check
            if (currentBoard[0, 2] != 0 && currentBoard[0, 2] == currentBoard[1, 1] && currentBoard[1, 1] == currentBoard[2, 0])
            {
                return currentBoard[0, 2]; // Return the winning player number (1 or 2)
            }

            // If no winning condition is met, return 0 ~ draw
            return 0;
        }

        public static void ResetBoard(int[,] board, int boardSize)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    board[y, x] = 0;
                }
            }
        }

        static int botThink(int[,] currentBoard)
        {
            int bestMove = -1;
            int bestScore = int.MinValue;
            List<int> legalMoves = allLegalMoves(currentBoard);

            foreach (int move in legalMoves)
            {
                int botMoveX = move % boardSize;
                int botMoveY = move / boardSize;

                // Simulate the move
                currentBoard[botMoveY, botMoveX] = 2;

                // Call the MiniMax function with Alpha-Beta Pruning to get the score
                int score = MiniMax(currentBoard, 10, false, int.MinValue, int.MaxValue);

                // Undo the move
                currentBoard[botMoveY, botMoveX] = 0;

                // If the current score is greater than the best score, update the best move and best score
                if (score > bestScore)
                {
                    bestMove = move;
                    bestScore = score;
                }
            }

            return bestMove;
        }

        static int MiniMax(int[,] currentBoard, int depth, bool isMaximizing, int alpha, int beta)
        {
            
            if (boardIsWon(currentBoard) || boardIsDraw(currentBoard) || depth == 0)
            {
            
                return evaluateBoard(currentBoard, depth);
            }

            
            if (isMaximizing)
            {
                int maxScore = int.MinValue; 

                List<int> legalMoves = allLegalMoves(currentBoard);
                foreach (int move in legalMoves)
                {
                    int botMoveX = move % boardSize;
                    int botMoveY = move / boardSize;

                    
                    currentBoard[botMoveY, botMoveX] = 2;

                    
                    int score = MiniMax(currentBoard, depth - 1, false, alpha, beta);

                    
                    currentBoard[botMoveY, botMoveX] = 0;

                    
                    maxScore = Math.Max(maxScore, score);
                    alpha = Math.Max(alpha, maxScore);

                    
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return alpha; 
            }
            else 
            {
                int minScore = int.MaxValue; 

                List<int> legalMoves = allLegalMoves(currentBoard);
                foreach (int move in legalMoves)
                {
                    int playerMoveX = move % boardSize;
                    int playerMoveY = move / boardSize;

                    
                    currentBoard[playerMoveY, playerMoveX] = 1;

                    
                    int score = MiniMax(currentBoard, depth - 1, true, alpha, beta);

                    
                    currentBoard[playerMoveY, playerMoveX] = 0;

                    
                    minScore = Math.Min(minScore, score);
                    beta = Math.Min(beta, minScore);

                    
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return beta; 
            }
        }



        //https://stackoverflow.com/questions/35357419/tic-tac-toe-rate-a-board-algorithm
        public static int evaluateBoard(int[,] board, int depth) {
            int x1 = 0, x2 = 0, x3 = 0, o1 = 0, o2 = 0, o3 = 0;
            //The following correspond to
            // x1 -> number of rowns with N x's and no o's.
            // x2 -> number of columns with N x's and no o's.
            // x3 -> number of diagonals with N x's and no o's.
            // o1 -> number of rowns with N o's and no x's.
            // o2 -> number of columns with N o's and no x's.
            // o3 -> number of diagonals with N o's and no x's.

            int N = 3; //board is 3x3

            for (int i = 0; i < N; i++)
            {
                int rowXCount = 0, rowOCount = 0;
                int colXCount = 0, colOCount = 0;

                for (int j = 0; j < N; j++)
                {
                    if (board[i, j] == 1) 
                        rowXCount++;
                    else if (board[i, j] == 2) 
                        rowOCount++;

                    if (board[j, i] == 1)
                        colXCount++;
                    else if (board[j, i] == 2)
                        colOCount++;
                }

                if (rowXCount == N && rowOCount == 0)
                    x1++;
                if (colXCount == N && colOCount == 0)
                    x2++;

                if (rowOCount == N && rowXCount == 0)
                    o1++;
                if (colOCount == N && colXCount == 0)
                    o2++;
            }

            
            int diag1XCount = 0, diag1OCount = 0;
            int diag2XCount = 0, diag2OCount = 0;

            for (int i = 0; i < N; i++)
            {
                if (board[i, i] == 1)
                    diag1XCount++;
                else if (board[i, i] == 2)
                    diag1OCount++;

                if (board[i, N - 1 - i] == 1)
                    diag2XCount++;
                else if (board[i, N - 1 - i] == 2)
                    diag2OCount++;
            }

            if (diag1XCount == N && diag1OCount == 0)
                x3++;
            if (diag2XCount == N && diag2OCount == 0)
                x3++;

            if (diag1OCount == N && diag1XCount == 0)
                o3++;
            if (diag2OCount == N && diag2XCount == 0)
                o3++;

            int score = (10 * o3 + 3 * o2 + o1) - (10 * x3 + 3 * x2 + x1); // We should change the original pointing system to the opposite, since in our case, bot is playing O.

            return score;
        }

    }

    }
