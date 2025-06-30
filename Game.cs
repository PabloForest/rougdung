using System;
using System.Collections.Generic;

public class Game
{
    const int Width = 40;
    const int Height = 20;
    char[,] map = new char[Height, Width];
    Player player = new Player();
    Random rand = new Random();
    int health = 3;
    int startX = 2;
    int startY = 2;

    public void Run()
    {
        InitMap();
        InitPlayer();
        InitTraps();
        InitItems();
        InitDoor();

        Console.Clear();
        while (true)
        {
            Console.SetCursorPosition(0, 0);
            DrawMap();
            Console.SetCursorPosition(0, Height);
            DrawUI();
            HandleInput();
        }
    }

    void InitMap()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                map[y, x] = (y == 0 || y == Height - 1 || x == 0 || x == Width - 1) ? '#' : '.';
    }

    void InitPlayer()
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
                Console.ReadKey(true); // pauza
                return;
            }
        }

        if (map[newY, newX] == 'K') player.AddItem("Key");

        if (map[newY, newX] == 'D')
        {
            if (player.HasItem("Key"))
                EndGame();
            else
            {
                Console.WriteLine("Drzwi są zamknięte. Potrzebujesz klucza (K).");
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
        Console.WriteLine("Kobieta Goblin czeka i oświadcza Ci się!");
        Console.WriteLine("Gratulacje! Wygrałeś grę.");
        Environment.Exit(0);
    }
}
