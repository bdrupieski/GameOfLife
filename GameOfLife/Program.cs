using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace GameOfLife
{
    class Program
    {
        private const char Block = '\u2588'; // '█'
        private const char Empty = ' ';

        private static readonly int Width = Console.WindowWidth - 1;
        private static readonly int Height = Console.WindowHeight;

        // As (x, y) pairs
        private static readonly Tuple<int, int>[] DirectionOffsets =
        {
            new Tuple<int, int>(-1, -1), new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1),
            new Tuple<int, int>(-1, 0),                              new Tuple<int, int>(1, 0),
            new Tuple<int, int>(-1, 1),  new Tuple<int, int>(0, 1),  new Tuple<int, int>(1, 1),
        };

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Green;

            bool[][] boardState = InitBoard();
            SetInitialBoardState(boardState);

            while (true)
            {
                PrintBoard(boardState);
                Thread.Sleep(100);
                boardState = GetNextBoardState(boardState);
            }
        }

        public static bool[][] InitBoard()
        {
            bool[][] board = new bool[Height][];
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new bool[Width];
            }
            return board;
        }

        public static void PrintBoard(bool[][] board)
        {
            Console.Clear();
            Console.WriteLine(string.Join(Environment.NewLine, board.Select(x => string.Join(string.Empty, x.Select(y => y ? Block : Empty )))));
        }

        public static void SetInitialBoardState(bool[][] uninitializedBoard)
        {
            var initialBoardStateLines = File.ReadAllLines("InitialBoardState.txt").Select(x => x.ToCharArray()).ToArray();

            int heightToReadIn = Math.Min(Height, initialBoardStateLines.Length);
            int lengthToReadIn = Math.Min(Width, initialBoardStateLines.First().Length);

            for (int y = 0; y < heightToReadIn; y++)
            {
                for (int x = 0; x < lengthToReadIn; x++)
                {
                    uninitializedBoard[y][x] = initialBoardStateLines[y][x] != '.';
                }
            }
        }

        public static bool[][] GetNextBoardState(bool[][] currentBoardState)
        {
            var nextBoardState = InitBoard();

            for (int y = 0; y < currentBoardState.Length; y++)
            {
                for (int x = 0; x < currentBoardState[y].Length; x++)
                {
                    int numberOfAliveNeighbors = GetNumberOfAliveNeighbors(currentBoardState, x, y);
                    bool currentCellIsAlive = currentBoardState[y][x];
                    bool currentCellIsDead = !currentBoardState[y][x];
                    bool currentCellFate = false;

                    if (currentCellIsAlive && numberOfAliveNeighbors < 2)
                    {
                        currentCellFate = false;
                    }
                    else if (currentCellIsAlive && (numberOfAliveNeighbors == 2 || numberOfAliveNeighbors == 3))
                    {
                        currentCellFate = true;
                    }
                    else if (currentCellIsAlive && numberOfAliveNeighbors > 3)
                    {
                        currentCellFate = false;
                    }
                    else if (currentCellIsDead && numberOfAliveNeighbors == 3)
                    {
                        currentCellFate = true;
                    }

                    nextBoardState[y][x] = currentCellFate;
                }
            }

            return nextBoardState;
        }

        public static int GetNumberOfAliveNeighbors(bool[][] boardState, int x, int y)
        {
            int aliveNeighbors = 0;

            foreach (var offset in DirectionOffsets)
            {
                int neighborX = x + offset.Item1;
                int neighborY = y + offset.Item2;

                if (IsCoordinateOnBoard(neighborX, neighborY) && boardState[neighborY][neighborX])
                {
                    aliveNeighbors++;
                }
            }

            return aliveNeighbors;
        }

        public static bool IsCoordinateOnBoard(int x, int y)
        {
            bool xOnBoard = x >= 0 && x < Width;
            bool yOnBoard = y >= 0 && y < Height;
            return xOnBoard && yOnBoard;
        }
    }
}