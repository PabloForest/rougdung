using System;
using System.Collections.Generic;

public class Game
{
    const int Width = 40;
    const int Height = 20;
    char[,] map = new char[Height, Width];
    Player player = new Player();
    List<NPC> npcs = new List<NPC>();
    Random rand = new Random();

    public void Run()
    {
        InitMap();
        InitPlayer();
        InitNPCs();
        InitItems();
        InitDoor();

        while (true)
        {
            Console.Clear();
            DrawMap();
            Console.SetCursorPosition(0, Height);
            HandleInput();
            UpdateNPCs();
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
        player.X = 2;
        player.Y = 2;
    }

    void InitNPCs()
    {
        // Goblin
        var goblin = new NPC { X = rand.Next(5, Width - 5), Y = rand.Next(5, Height - 5), IsGoblin = true };
        npcs.Add(goblin);
        map[goblin.Y, goblin.X] = 'N';

        // Quest NPC
        var questNPC = new NPC { X = rand.Next(5, Width - 5), Y = rand.Next(5, Height - 5), HasQuest = true };
        npcs.Add(questNPC);
        map[questNPC.Y, questNPC.X] = 'N';
    }

    void InitItems()
    {
        PlaceItem('S'); // Sword
        PlaceItem('H'); // Shield
    }

    void PlaceItem(char symbol)
    {
        int x = rand.Next(1, Width - 1);
        int y = rand.Next(1, Height - 1);
        if (map[y, x] == '.')
            map[y, x] = symbol;
    }

    void InitDoor()
    {
        int x = rand.Next(1, Width - 1);
        int y = rand.Next(1, Height - 1);
        if (map[y, x] == '.')
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

        if (map[newY, newX] == 'S') player.AddItem("Sword");
        if (map[newY, newX] == 'H') player.AddItem("Shield");
        if (map[newY, newX] == 'D')
        {
            if (player.HasKey) EndGame();
            else Console.WriteLine("Drzwi są zamknięte.");
        }

        foreach (var npc in npcs)
        {
            if (npc.X == newX && npc.Y == newY)
            {
                if (npc.IsGoblin) PlayMiniGame();
                else if (npc.HasQuest)
                {
                    if (player.HasItem("Shield"))
                        Console.WriteLine("Dzięki! Goblin ma klucz.");
                    else
                        Console.WriteLine("Zgubiłem tarczę!");
                }
            }
        }

        map[player.Y, player.X] = '.';
        player.X = newX;
        player.Y = newY;
    }

    void UpdateNPCs()
    {
        foreach (var npc in npcs)
        {
            if (npc.IsGoblin)
            {
                int move = rand.Next(2) == 0 ? -2 : 2;
                int newX = npc.X + move;
                if (newX > 0 && newX < Width - 1 && map[npc.Y, newX] == '.')
                {
                    map[npc.Y, npc.X] = '.';
                    npc.X = newX;
                    map[npc.Y, npc.X] = 'N';
                }
            }
        }
    }

    void PlayMiniGame()
    {
        Console.WriteLine("Mini-gra: kamień, papier, nożyce. Wpisz: rock/paper/scissors");
        string playerChoice = Console.ReadLine().ToLower();
        string[] options = { "rock", "paper", "scissors" };
        string npcChoice = options[rand.Next(3)];

        Console.WriteLine($"Goblin wybrał: {npcChoice}");
        if (playerChoice == npcChoice)
        {
            Console.WriteLine("Remis. Spróbuj ponownie.");
            PlayMiniGame();
        }
        else if ((playerChoice == "rock" && npcChoice == "scissors") ||
                 (playerChoice == "paper" && npcChoice == "rock") ||
                 (playerChoice == "scissors" && npcChoice == "paper"))
        {
            Console.WriteLine("Wygrałeś! Otrzymujesz klucz.");
            player.HasKey = true;
        }
        else
        {
            Console.WriteLine("Przegrałeś! Wracasz na start.");
            player.X = 2;
            player.Y = 2;
        }
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