using System;

namespace Project_Minesweeper_V1._0
{

    class Program
    {
        enum FieldType
        {
            ClosedSafe,
            BombAround1,
            BombAround2,
            BombAround3,
            BombAround4,
            BombAround5,
            BombAround6,
            BombAround7,
            BombAround8,
            Empty,
            Bomb,
            EdgeOfGame = 12,
            ImageOfBomb,
            FlagOnSafe,
            FlagOnBomb

        }
        static public void DrawField(int[,] gamePlex)
        {
            int WIDTH = gamePlex.GetLength(0);
            int HEIGHT = gamePlex.GetLength(1);
            Console.Clear();
            Console.WriteLine("Введите 3 значение через Enter.\nПервое - номер клетки по ширине\nВторое - номер клетки по высоте\nТретье - 'false/true', открыть клетку/пометить клетку");
            Console.WriteLine();
            Console.Write("\t");
            for (int i = 1; i < WIDTH - 1; i++)
            {
                if (i > 9)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(i + "  ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(i + "   ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 1; i < HEIGHT - 1; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(i + "\t");
                Console.ForegroundColor = ConsoleColor.Gray;

                for (int j = 1; j < WIDTH - 1; j++)
                {
                    if (gamePlex[j, i] >= 1 && gamePlex[j, i] <= 8)
                        Console.Write(gamePlex[j, i] + " ");
                    else if (gamePlex[j, i] == (int)FieldType.ClosedSafe || gamePlex[j, i] == (int)FieldType.Bomb)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("██");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (gamePlex[j, i] == (int)FieldType.Empty)
                    {
                        Console.Write("▒▒");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (gamePlex[j, i] == (int)FieldType.FlagOnSafe || gamePlex[j, i] == (int)FieldType.FlagOnBomb)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("██");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (gamePlex[j, i] == (int)FieldType.ImageOfBomb)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("██");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    Console.Write("  ");
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }
        static public void Recursive(ref int[,] gamePlex, int pW, int pH)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {

                    if (gamePlex[pW + i, pH + j] == (int)FieldType.Bomb || gamePlex[pW + i, pH + j] == (int)FieldType.FlagOnBomb)
                    {
                        gamePlex[pW, pH]++;
                    }

                }

            }

            if (gamePlex[pW, pH] == (int)FieldType.ClosedSafe)
            {
                gamePlex[pW, pH] = (int)FieldType.Empty;


                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (gamePlex[pW + i, pH + j] == (int)FieldType.ClosedSafe)
                        {
                            Recursive(ref gamePlex, pW + i, pH + j);
                        }
                    }
                }

            }

        }
        static void Main(string[] args)
        {
            Console.Title = "Сапёр (тот самый, который лучший в мире)";
            Random rand = new Random();

            int WIDTH = 1;
            int HEIGHT = 1;
            int MINES = 1;
            bool wrongCycle;

            Console.WriteLine("Вы играете в сапёр. Введите 3 значение через Enter.\nПервое - ширина поля\nВторое - высота поля\nТретье - кол-во мин\nПоле не может быть меньше и равно 3x3\n(не жалуемся, в windows варианте 9x9 минимум)\nНи одно из значений не должно равняться нулю");

            do
            {
                if (!int.TryParse(Console.ReadLine(), out WIDTH) || !int.TryParse(Console.ReadLine(), out HEIGHT) || !int.TryParse(Console.ReadLine(), out MINES))
                {
                    Console.WriteLine("Вы ввели не число или дробное значение. Напиши всё правильно, это не сложно\u0007");
                    wrongCycle = true;
                }
                else if (WIDTH <= 3 || HEIGHT <= 3)
                {
                    Console.WriteLine("НЕ МЕНЬШЕ ИЛИ РАВНО 3x3 ПОЦ!\u0007");
                    wrongCycle = true;
                }
                else
                    wrongCycle = false;
            }
            while (wrongCycle);
            
            if (MINES > WIDTH * HEIGHT - 9 || MINES <= 0)
            {
                wrongCycle = true;
                while (wrongCycle == true)
                {
                    Console.WriteLine("Ошибка: Мин слишком много/мало/равно 0/больше или равно кол-во клеток. Выпей аскорбинку и напиши кол-во мин снова\u0007");
                    MINES = int.Parse(Console.ReadLine());
                    if (MINES > WIDTH * HEIGHT - 9 || MINES <= 0)
                    {

                    }
                    else
                        wrongCycle = false;
                }
            }
            
            WIDTH += 2;
            HEIGHT += 2;

            int randW;
            int randH;
            int pW = 1;
            int pH = 1;
            int leftBombs = 0;

            bool firstCheck = true;
            bool checkOrMark = true;
            bool bombAct = false;

            int[,] gamePlex = new int[WIDTH, HEIGHT];





            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (i == 0 || i == WIDTH - 1 || j == 0 || j == HEIGHT - 1)
                    {
                        gamePlex[i, j] = (int)FieldType.EdgeOfGame;
                    }

                }

            }

            

            try
            {
                if (WIDTH * 2 + 9 <= 240 && WIDTH > 23)
                    Console.WindowWidth = WIDTH * 2 + 9;
                if (HEIGHT * 2 + 8 <= 85)
                    Console.WindowHeight = HEIGHT * 2 + 8;
            }
            catch
            {

            }

            while (!bombAct)
            {
                

                DrawField(gamePlex);
                do
                {
                    if (!int.TryParse(Console.ReadLine(), out pW) || !int.TryParse(Console.ReadLine(), out pH))
                    {
                        Console.WriteLine("Вы ввели не число или дробное значение. Напиши всё правильно, это не сложно\u0007");
                        wrongCycle = true;
                    }
                    else if (pW <= 0 || pW >= WIDTH - 1 || pH <= 0 || pH >= HEIGHT - 1)
                    {
                        Console.WriteLine("Слишком большие или маленькие значения. Вводи все заново\u0007");
                        wrongCycle = true;
                    }
                    else if (!bool.TryParse(Console.ReadLine(),out checkOrMark))
                    {
                        Console.WriteLine("Вы ввели не true или false, ну пытайся снова\u0007");
                        wrongCycle = true;
                    }
                    else
                        wrongCycle = false;

                }
                while (wrongCycle);
                

                

                
                

                if (checkOrMark == false && firstCheck == true)
                {
                    firstCheck = false;
                    
                    
                    

                        for (int i = 0; i < MINES;)
                        {
                            randW = rand.Next(1, WIDTH - 1);
                            randH = rand.Next(1, HEIGHT - 1);

                            if ( (randW == pW - 1 || randW == pW || randW == pW + 1) && (randH == pH - 1 || randH == pH || randH == pH + 1) )
                            {
                                continue;
                            }
                            if (gamePlex[randW, randH] == (int)FieldType.ClosedSafe)
                            {
                                gamePlex[randW, randH] = (int)FieldType.Bomb;
                                i++;
                            }

                        }
                        
                }



                if (checkOrMark == false && gamePlex[pW, pH] == (int)FieldType.ClosedSafe)
                {
                    Recursive(ref gamePlex, pW, pH);
                }
                else if (checkOrMark == true && gamePlex[pW, pH] == (int)FieldType.Bomb)
                {
                    gamePlex[pW, pH] = (int)FieldType.FlagOnBomb;
                }
                else if (checkOrMark == true && gamePlex[pW, pH] == (int)FieldType.ClosedSafe)
                {
                    gamePlex[pW, pH] = (int)FieldType.FlagOnSafe;
                }
                else if (checkOrMark == true && gamePlex[pW, pH] == (int)FieldType.FlagOnSafe)
                {
                    gamePlex[pW, pH] = (int)FieldType.ClosedSafe;
                }
                else if (checkOrMark == true && gamePlex[pW, pH] == (int)FieldType.FlagOnBomb)
                {
                    gamePlex[pW, pH] = (int)FieldType.Bomb;
                }
                else if (checkOrMark == false && gamePlex[pW, pH] == 10)
                {
                    for (int i = 1; i < WIDTH - 1; i++)
                    {
                        for (int j = 1; j < HEIGHT - 1; j++)
                        {
                            if (gamePlex[i, j] == (int)FieldType.Bomb || gamePlex[i, j] == (int)FieldType.FlagOnBomb)
                            {
                                gamePlex[i, j] = (int)FieldType.ImageOfBomb;
                            }
                        }
                    }
                    DrawField(gamePlex);
                    Console.WriteLine("You are maybe debil\u0007");
                    bombAct = true;
                    break;
                }

                for (int i = 1; i < WIDTH - 1; i++)
                {
                    for (int j = 1; j < HEIGHT - 1; j++)
                    {
                        if (gamePlex[i, j] == (int)FieldType.Bomb 
                            || gamePlex[i, j] == (int)FieldType.ClosedSafe 
                            || gamePlex[i, j] == (int)FieldType.FlagOnSafe 
                            || gamePlex[i, j] == (int)FieldType.FlagOnBomb)
                        {
                            leftBombs++;
                        }
                    }
                }
                if (leftBombs == MINES)
                {

                    for (int i = 1; i < WIDTH - 1; i++)
                    {
                        for (int j = 1; j < HEIGHT - 1; j++)
                        {
                            if (gamePlex[i, j] == (int)FieldType.Bomb || gamePlex[i, j] == (int)FieldType.FlagOnBomb)
                            {
                                gamePlex[i, j] = (int)FieldType.ImageOfBomb;
                            }
                        }
                    }
                    DrawField(gamePlex);
                    Console.WriteLine("Вы выйграли жизнь, поздравляю!");
                    break;
                }
                else
                {
                    leftBombs = 0;
                }
            }
            Console.ReadLine();
        }
    }

}
/*
* 0 - "█"
* 1-8 - Цифра от 1 до 8 
* 9 - ничего
* 10 - "█" БОМБАРДА!!11111
* 11 - .....
* 12 - край игры
* 13 - "☼" Изображение бомбы после победы/поражения
* 
* 14 - Флаг на пустую клетку
* 15 - Флаг на бомбу
* ╔═╗
* ║ ║
* ╚═╝
* 
*/
