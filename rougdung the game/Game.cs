// Game.cs
using System;
using System.Collections.Generic;

public class Game
{
    const int Width = 40;
    const int Height = 20;
    char[,] map = new char[Height, Width];
    Player player = new Player();
    List<NPC> goblinQuizzers = new List<NPC>();
    Random rand = new Random();
    int health = 3;
    int startX = 2;
    int startY = 2;
    int currentFloor = 1;
    const int maxFloor = 3;

    public void Run()
    {
        while (currentFloor <= maxFloor)
        {
            Console.Clear();
            InitMap();
            InitPlayerPosition();
            InitTraps();
            InitGoblinQuizzers();
            InitItems();
            InitDoor();

            while (true)
            {
                Console.SetCursorPosition(0, 0);
                MoveGoblinQuizzers();
                DrawMap();
                Console.SetCursorPosition(0, Height);
                DrawUI();
                HandleInput();

                if (player.NextFloorRequested)
                {
                    player.NextFloorRequested = false;
                    break;
                }
            }

            currentFloor++;
        }

        EndGame();
    }

    void InitMap()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                map[y, x] = (y == 0 || y == Height - 1 || x == 0 || x == Width - 1) ? '#' : '.';
    }

    void InitPlayerPosition()
    {
        player.X = startX;
        player.Y = startY;
    }

    void InitTraps()
    {
        for (int i = 0; i < 8; i++)
        {
            int x = rand.Next(1, Width - 6);
            int y = rand.Next(1, Height - 1);
            int length = rand.Next(3, 8);
            for (int j = 0; j < length; j++)
                if (map[y, x + j] == '.')
                    map[y, x + j] = 'X';
        }

        for (int i = 0; i < 6; i++)
        {
            int x = rand.Next(1, Width - 1);
            int y = rand.Next(1, Height - 6);
            int length = rand.Next(3, 8);
            for (int j = 0; j < length; j++)
                if (map[y + j, x] == '.')
                    map[y + j, x] = 'X';
        }

        for (int i = 0; i < 4; i++)
        {
            int x = rand.Next(1, Width - 6);
            int y = rand.Next(1, Height - 6);
            for (int dy = 0; dy < 3; dy++)
                for (int dx = 0; dx < 5; dx++)
                    if (map[y + dy, x + dx] == '.')
                        map[y + dy, x + dx] = 'X';
        }
    }

    void InitGoblinQuizzers()
    {
        goblinQuizzers.Clear();

        for (int i = 0; i < 5; i++)
        {
            int x, y;
            do
            {
                x = rand.Next(3, Width - 3);
                y = rand.Next(1, Height - 1);
            } while (map[y, x] != '.');

            map[y, x] = 'G';
            goblinQuizzers.Add(new NPC { X = x, Y = y, IsGoblin = true, Direction = rand.Next(2) == 0 ? -1 : 1 });
        }
    }

    void MoveGoblinQuizzers()
    {
        foreach (var goblin in goblinQuizzers)
        {
            map[goblin.Y, goblin.X] = '.';
            int newX = goblin.X + goblin.Direction;
            if (newX <= 0 || newX >= Width - 1 || map[goblin.Y, newX] != '.')
            {
                goblin.Direction *= -1;
                newX = goblin.X + goblin.Direction;
            }
            goblin.X = newX;
            map[goblin.Y, goblin.X] = 'G';
        }
    }

    void InitItems()
    {
        PlaceSafeKey('K');
    }

    void PlaceSafeKey(char symbol)
    {
        bool placed = false;
        while (!placed)
        {
            int x = rand.Next(3, Width - 3);
            int y = rand.Next(3, Height - 3);
            bool safe = true;

            for (int dy = -3; dy <= 3; dy++)
            {
                for (int dx = -3; dx <= 3; dx++)
                {
                    int checkX = x + dx;
                    int checkY = y + dy;
                    if (checkX > 0 && checkX < Width && checkY > 0 && checkY < Height)
                    {
                        if (map[checkY, checkX] == 'X')
                        {
                            safe = false;
                            break;
                        }
                    }
                }
                if (!safe) break;
            }

            if (safe && map[y, x] == '.')
            {
                map[y, x] = symbol;
                placed = true;
            }
        }
    }

    void InitDoor()
    {
        int x = Width / 2;
        int y = Height - 2;
        map[y, x] = 'D';
    }

    void DrawMap()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (x == player.X && y == player.Y)
                    Console.Write('@');
                else
                    Console.Write(map[y, x]);
            }
            Console.WriteLine();
        }
    }

    void DrawUI()
    {
        Console.Write("Zdrowie: ");
        for (int i = 0; i < health; i++)
        {
            Console.Write("|");
            if (i < health - 1) Console.Write(" ");
        }
        Console.Write("    Piętro: " + currentFloor + "/" + maxFloor);
        Console.WriteLine();
    }

    void HandleInput()
    {
        var key = Console.ReadKey(true).Key;
        int newX = player.X;
        int newY = player.Y;

        if (key == ConsoleKey.UpArrow) newY--;
        else if (key == ConsoleKey.DownArrow) newY++;
        else if (key == ConsoleKey.LeftArrow) newX--;
        else if (key == ConsoleKey.RightArrow) newX++;

        if (map[newY, newX] == '#') return;

        if (map[newY, newX] == 'X')
        {
            health--;
            if (health <= 0)
            {
                Console.Clear();
                DrawMap();
                DrawUI();
                Console.WriteLine("Wszedłeś na pułapkę i straciłeś ostatnie życie! Przegrywasz.");
                Environment.Exit(0);
            }
            else
            {
                player.X = startX;
                player.Y = startY;
                Console.Clear();
                DrawMap();
                DrawUI();
                Console.WriteLine("Auu! Straciłeś punkt życia i wracasz na start.");
                Console.ReadKey(true);
                return;
            }
        }

        if (map[newY, newX] == 'G')
        {
            Console.Clear();
            Console.WriteLine("Witaj człowieku! Ile widzisz palców!?");

            int correct = rand.Next(1, 6);
            var options = new List<int> { correct };
            while (options.Count < 3)
            {
                int n = rand.Next(1, 6);
                if (!options.Contains(n)) options.Add(n);
            }
            options.Sort();

            Console.WriteLine($"(widzisz palców: {correct})");
            Console.WriteLine($"A) {options[0]}  B) {options[1]}  C) {options[2]}");
            Console.Write("Twój wybór: ");
            var input = Console.ReadLine();

            int selected = input.ToUpper() switch
            {
                "A" => options[0],
                "B" => options[1],
                "C" => options[2],
                _ => -1
            };

            if (selected != correct)
            {
                Console.WriteLine("Źle! Goblin się śmieje i odpycha Cię.");
                Console.ReadKey();
                player.X = startX;
                player.Y = startY;
                return;
            }

            int f1 = rand.Next(1, 6);
            int f2 = rand.Next(1, 6);
            int sum = f1 + f2;
            var options2 = new List<int> { sum };
            while (options2.Count < 3)
            {
                int n = rand.Next(2, 11);
                if (!options2.Contains(n)) options2.Add(n);
            }
            options2.Sort();
            Console.WriteLine("Gratulacje człowieku! Teraz pora na drugą rundę!");
            Console.WriteLine($"Ile widzisz palców!? (w jednej ręce goblin pokazuje {f1}, a w drugiej {f2})");
            Console.WriteLine($"A) {options2[0]}  B) {options2[1]}  C) {options2[2]}");
            Console.Write("Twój wybór: ");
            input = Console.ReadLine();

            selected = input.ToUpper() switch
            {
                "A" => options2[0],
                "B" => options2[1],
                "C" => options2[2],
                _ => -1
            };

            if (selected == sum)
            {
                Console.WriteLine("Zwycięstwo! Goblin znika w kłębie dymu.");
                goblinQuizzers.RemoveAll(g => g.X == newX && g.Y == newY);
                map[newY, newX] = '.';
            }
            else
            {
                Console.WriteLine("Zła odpowiedź! Goblin się obraża i odsyła Cię.");
                player.X = startX;
                player.Y = startY;
            }

            Console.ReadKey();
            return;
        }

        if (map[newY, newX] == 'K') player.AddItem("Key");

        if (map[newY, newX] == 'D')
        {
            if (player.HasItem("Key"))
            {
                player.NextFloorRequested = true;
                return;
            }
            else
            {
                Console.WriteLine("Drzwi są zamknięte. Potrzebujesz klucza (K).");
                Console.ReadKey();
                return;
            }
        }

        map[player.Y, player.X] = '.';
        player.X = newX;
        player.Y = newY;
    }

    void EndGame()
    {
        Console.Clear();
        Console.WriteLine("Wchodzisz do finałowego pokoju...");
        Console.WriteLine("Królewna goblinów czeka na Ciebie i mówi: 'Brawo bohaterze!'");
        Console.WriteLine("Gratulacje! Ukończyłeś wszystkie 3 poziomy!");
        Environment.Exit(0);
    }
}