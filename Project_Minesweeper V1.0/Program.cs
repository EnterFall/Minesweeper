using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Minesweeper
{

    public class Program
    {
        private static Minesweeper game;

        private static int Width => game.Width;
        private static int Height => game.Height;

        private static int pX;
        private static int pY;

        public static void Main(string[] args)
        {
            Console.Title = "Сапёр (тот самый, который лучший в мире)";
            Console.SetWindowSize(70, 20);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            int width = 1;
            int height = 1;
            int minesCount = 1;

            Console.WriteLine($"Как так вышло - незнаю, но вы теперь - Сапёр.\n" +
                $"Введите 3 значения через Enter:\n" +
                $"Первое - ширина поля\n" +
                $"Второе - высота поля\n" +
                $"Третье - кол-во мин\n" +
                $"Кол-во клеток не может быть меньше 10\n" +
                $"\n" +
                $"Управление:\n" +
                $"WASD    ВАСД\n" +
                $"Пробел  Вскрыть кубач\n" +
                $"Enter   Пометить кубач\n" +
                $"Нажав пробелом по числу можно вскрыть ближайшие кубачи (мины тоже!)");

            do
            {
                if (!int.TryParse(Console.ReadLine().Trim(), out width) ||
                    !int.TryParse(Console.ReadLine().Trim(), out height) ||
                    !int.TryParse(Console.ReadLine().Trim(), out minesCount))
                {
                    Console.WriteLine("Это не целое число, но я верю в тебя!");
                }
                else if (width < 1 || height < 1 || minesCount < 1)
                {
                    Console.WriteLine("Значения не могут быть меньше 1");
                }
                else if (minesCount > width * height - Math.Min(Math.Min(width, height), 3) * 3)
                {
                    Console.WriteLine("Мин больше чем доступных клеток");
                }
                else
                {
                    if (width * 2 + 1 > Console.LargestWindowWidth)
                    {
                        width = (Console.LargestWindowWidth - 1) / 2;
                    }
                    if (height + 3 > Console.LargestWindowHeight)
                    {
                        height = Console.LargestWindowHeight - 3;
                    }
                    Console.SetWindowSize(width * 2 + 1, height + 3);
                    if (width > 7 && height > 7)
                    {
                        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
                    }
                    break;
                }
            } while (true);

            game = new Minesweeper(width, height, minesCount);
            game.FieldChanged += GameFieldChanged;
            pX = width / 2;
            pY = height / 2;

            Console.CursorVisible = false;

            DrawField();
            while (game.IsAlive)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                var key = Console.ReadKey().Key;
                Console.SetCursorPosition(0, Height);

                if (key == ConsoleKey.Spacebar)
                    game.OpenTile(pX, pY);
                else if (key == ConsoleKey.Enter)
                    game.MarkTile(pX, pY);
                else
                {
                    switch (key)
                    {
                        case ConsoleKey.W: if (pY != 0) { pY--; Move(pX, pY, key); } break;
                        case ConsoleKey.S: if (pY != Height - 1) { pY++; Move(pX, pY, key); } break;
                        case ConsoleKey.A: if (pX != 0) { pX--; Move(pX, pY, key); } break;
                        case ConsoleKey.D: if (pX != Width - 1) { pX++; Move(pX, pY, key); } break;
                    }
                }
            }
            if (game.IsWon)
            {
                Console.WriteLine("Вы выиграли жизнь, поздравляю!");
            }
            else if (game.IsLost)
            {
                Console.WriteLine("Ты... потерялся");
            }

            Console.ReadLine();
        }

        private static void WriteTile(int x, int y, Tile tile)
        {
            Console.SetCursorPosition(x * 2, y);
            if (pX == x && pY == y)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("██");
            }
            else if (tile >= Tile.BombAround1 && tile <= Tile.BombAround8)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write((int)tile + " ");
            }
            else
            {
                Console.ForegroundColor = GetColorFromTile(tile);
                Console.Write("██");
            }

            Console.ResetColor();
            Console.SetCursorPosition(0, Height);
        }

        private static ConsoleColor GetColorFromTile(Tile tile)
        {
            switch (tile)
            {
                case Tile.HiddenSafe:  return ConsoleColor.White;
                case Tile.HiddenBomb:  return ConsoleColor.White;
                case Tile.FlagOnSafe:  return ConsoleColor.Green;
                case Tile.FlagOnBomb:  return ConsoleColor.Green;
                case Tile.Empty:       return ConsoleColor.DarkGray;
                case Tile.VisibleBomb: return ConsoleColor.Red;
                default:               return ConsoleColor.Gray;
            };
        }

        private static void Move(int pW, int pH, ConsoleKey key)
        {
            int dW = 0, dH = 0;
            switch (key)
            {
                case ConsoleKey.W: dH++; break;
                case ConsoleKey.S: dH--; break;
                case ConsoleKey.A: dW++; break;
                case ConsoleKey.D: dW--; break;
            }
            WriteTile(pW, pH, game.GetTile(pW, pH));
            WriteTile(pW + dW, pH + dH, game.GetTile(pW + dW, pH + dH));
        }

        private static void DrawField()
        {
            Console.Clear();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    WriteTile(j, i, game.GetTile(j, i));
                }
            }
        }

        private static void GameFieldChanged(Minesweeper sender, FieldChangedEventArgs e)
        {
            WriteTile(e.X, e.Y, e.NewValue);
        }
    }
}
