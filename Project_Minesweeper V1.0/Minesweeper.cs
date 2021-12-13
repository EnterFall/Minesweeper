using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class Minesweeper
    {
        private GameField gameField;
        private bool firstOpen = true;
        private bool isLost;
        private bool isWon;

        public int Width { get; }
        public int Height { get; }
        public int MinesCount { get; }
        public bool IsAlive => !isLost && !isWon;
        public bool IsLost => isLost;
        public bool IsWon => isWon;
        public int bombsLeft;

        public event FieldChangedEventHandler FieldChanged;

        internal void OnFieldChanged(FieldChangedEventArgs e)
            => FieldChanged(this, e);

        public Minesweeper(int width, int height, int minesCount)
        {
            Validate(width, height, minesCount);

            Width = width;
            Height = height;
            MinesCount = minesCount;
            bombsLeft = minesCount;
            gameField = new GameField(this, width, height);
        }

        public IReadOnlyCollection<Tile> GetField() => gameField;

        public bool IsInsideField(int x, int y)
            => y >= 0 && y < Height && x >= 0 && x < Width;

        public Tile GetTile(int x, int y) => gameField[x, y];

        public static void Validate(int width, int height, int minesCount)
        {
            if (width < 1 || height < 1 || minesCount < 1)
            {
                throw new Exception("Значения не могут быть меньше 1");
            }

            if (minesCount > width * height - Math.Min(Math.Min(width, height), 3) * 3)
            {
                throw new Exception("Мин больше чем доступных клеток");
            }
        }

        public void OpenTile(int x, int y)
        {
            if (firstOpen && gameField[x, y] == Tile.HiddenSafe)
            {
                firstOpen = false;
                GenerateField(x, y);
            }

            if (gameField[x, y] == Tile.HiddenSafe)
            {
                OpenArea(x, y);
            }
            else if (gameField[x, y] >= Tile.BombAround1 && gameField[x, y] <= Tile.BombAround8)
            {
                OpenClosestTiles(x, y);
            }
            else if (gameField[x, y] == Tile.HiddenBomb)
            {
                Loose();
                return;
            }

            if (bombsLeft == 0 && gameField.Count(tile => tile == Tile.FlagOnSafe || tile == Tile.HiddenSafe) == 0)
            {
                Win();
            }
        }

        public void MarkTile(int x, int y)
        {
            switch (gameField[x, y])
            {
                case Tile.HiddenBomb: gameField[x, y] = Tile.FlagOnBomb; bombsLeft--; break;
                case Tile.FlagOnBomb: gameField[x, y] = Tile.HiddenBomb; bombsLeft++; break;
                case Tile.HiddenSafe: gameField[x, y] = Tile.FlagOnSafe; bombsLeft--; break;
                case Tile.FlagOnSafe: gameField[x, y] = Tile.HiddenSafe; bombsLeft++; break;
            }

            if (bombsLeft == 0 && gameField.Count(tile => tile == Tile.FlagOnSafe || tile == Tile.HiddenSafe) == 0)
            {
                Win();
            }
        }

        private void GenerateField(int playerX, int playerY)
        {
            var rand = new Random();
            for (int i = 0; i < MinesCount;)
            {
                int randW = rand.Next(0, Width);
                int randH = rand.Next(0, Height);

                if ((randW == playerX - 1 || randW == playerX || randW == playerX + 1)
                 && (randH == playerY - 1 || randH == playerY || randH == playerY + 1))
                {
                    continue;
                }
                if (gameField[randW, randH] == Tile.HiddenSafe)
                {
                    gameField[randW, randH] = Tile.HiddenBomb;
                    i++;
                }
                else if (gameField[randW, randH] == Tile.FlagOnSafe)
                {
                    gameField[randW, randH] = Tile.FlagOnBomb;
                    i++;
                }
            }
        }

        private void OpenArea(int pW, int pH)
        {
            var queue = new Queue<(int w, int h)>();
            queue.Enqueue((pW, pH));
            while (queue.Count != 0)
            {
                (pW, pH) = queue.Dequeue();

                if (gameField[pW, pH] == Tile.FlagOnSafe)
                {
                    bombsLeft++;
                }
                gameField[pW, pH] = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (IsInsideField(pW + i, pH + j) && (gameField[pW + i, pH + j] == Tile.HiddenBomb || gameField[pW + i, pH + j] == Tile.FlagOnBomb))
                        {
                            gameField[pW, pH]++;
                        }
                    }
                }

                if (gameField[pW, pH] == 0 || gameField[pW, pH] == Tile.FlagOnSafe)
                {
                    gameField[pW, pH] = Tile.Empty;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (IsInsideField(pW + i, pH + j) && (gameField[pW + i, pH + j] == Tile.HiddenSafe || gameField[pW + i, pH + j] == Tile.FlagOnSafe))
                            {
                                if (!queue.Contains((pW + i, pH + j)))
                                    queue.Enqueue((pW + i, pH + j));
                            }
                        }
                    }
                }
            }
        }

        private void OpenClosestTiles(int x, int y)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (IsInsideField(x + i, y + j) && gameField[x + i, y + j] == Tile.HiddenBomb)
                    {
                        Loose();
                        return;
                    }
                }
            }

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (IsInsideField(x + i, y + j) && gameField[x + i, y + j] == Tile.HiddenSafe)
                    {
                        OpenArea(x + i, y + j);
                    }
                }
            }
        }

        private void Loose()
        {
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    switch (gameField[i, j])
                    {
                        case Tile.HiddenBomb:
                        case Tile.FlagOnBomb:
                            gameField[i, j] = Tile.VisibleBomb; break;
                    }
                }
            }
            isLost = true;
        }

        private void Win()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (gameField[i, j] == Tile.HiddenBomb || gameField[i, j] == Tile.FlagOnBomb)
                    {
                        gameField[i, j] = Tile.VisibleBomb;
                    }
                }
            }
            isWon = true;
        }
    }
}
