using System;
using System.Collections.Generic;

public class Player
{
    public int X;
    public int Y;
    public bool HasKey = false;
    private List<string> inventory = new List<string>();

    public void AddItem(string item)
    {
        inventory.Add(item);
        Console.WriteLine($"Picked up: {item}");
    }

    public bool HasItem(string item)
    {
        return inventory.Contains(item);
    }

    public void PrintInventory()
    {
        Console.WriteLine("Inventory:");
        foreach (var item in inventory)
            Console.WriteLine(" - " + item);
    }
}
