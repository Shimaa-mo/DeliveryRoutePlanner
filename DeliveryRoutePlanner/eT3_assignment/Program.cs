using eT3_assignment;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Delivery & Trip Management System ===\n");

        string filePath = GetValidFilePath();
        List<Delivery> deliveries = ReadDeliveriesFromFile(filePath);

        if (deliveries.Count == 0)
        {
            Console.WriteLine("The file is empty. Exiting program.");
            return;
        }

        TripPlanner planner = new TripPlanner();
        List<Trip> trips = planner.PlanTrips(deliveries);

        Console.WriteLine("\n[Initial Trips Planned Successfully]");
        PrintTrips(trips);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. Schedule a new delivery");
            Console.WriteLine("2. View current trips");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice (1-3): ");

            string choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    Delivery newDelivery = PromptForDelivery();
                    trips = planner.ScheduleDelivery(newDelivery, trips);
                    Console.WriteLine("\n[Delivery Scheduled & Trips Rebalanced]");
                    PrintTrips(trips);
                    break;

                case "2":
                    PrintTrips(trips);
                    break;

                case "3":
                    running = false;
                    Console.WriteLine("Exiting program. Goodbye!");
                    break;

                default:
                    Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
                    break;
            }
        }
    }

    static string GetValidFilePath()
    {
        while (true)
        {
            Console.Write("Enter file path (or press Enter for default 'test.txt'): ");
            string input = Console.ReadLine()?.Trim();

            //if (string.IsNullOrEmpty(input))
            //    input = @"C:\Users\shosho\Desktop\test.txt";
            if (string.IsNullOrEmpty(input))
            {
                // Walks up from bin\Debug\netX.0\ to the project root
                string basePath = AppContext.BaseDirectory;
                input = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\..\test.txt"));
            }

            if (File.Exists(input))
                return input;

            Console.WriteLine($"Error: File not found at '{input}'. Please try again.\n");
        }
    }

    static Delivery PromptForDelivery()
    {
        Console.WriteLine("\n--- Enter New Delivery Details ---");

        string area;
        while (true)
        {
            Console.Write("Enter Area: ");
            area = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(area))
                break;
            Console.WriteLine("Area cannot be empty.");
        }

        int priority;
        while (true)
        {
            Console.Write("Enter Priority (integer): ");
            if (int.TryParse(Console.ReadLine(), out priority) && priority > 0)
                break;
            Console.WriteLine("Priority must be a positive integer.");
        }

        double weight;
        while (true)
        {
            Console.Write($"Enter Package Weight in kg (Max {Trip.MaxCapacity}): ");
            if (double.TryParse(Console.ReadLine(), out weight) && weight > 0 && weight <= Trip.MaxCapacity)
                break;
            Console.WriteLine($"Invalid weight. Must be between 0 and {Trip.MaxCapacity} kg.");
        }

        return new Delivery(area, priority, weight);
    }

    static List<Delivery> ReadDeliveriesFromFile(string filePath)
    {
        List<Delivery> deliveries = new List<Delivery>();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length < 4)
                continue;

            if (int.TryParse(parts[0], out int id) &&
                int.TryParse(parts[2], out int priority) &&
                double.TryParse(parts[3], out double weight))
            {
                deliveries.Add(new Delivery(id, parts[1].Trim(), priority, weight));
            }
        }

        return deliveries;
    }

    static void PrintTrips(List<Trip> trips)
    {
        if (trips == null || trips.Count == 0)
        {
            Console.WriteLine("\nNo trips available.");
            return;
        }

        Console.WriteLine("\n================ Current Trips ================");
        for (int i = 0; i < trips.Count; i++)
        {
            Console.WriteLine($"Trip #{trips[i].Id} | Area: {trips[i].Area} | Priority: {trips[i].Priority}");
            Console.WriteLine(new string('-', 45));

            foreach (Delivery delivery in trips[i].Deliveries)
            {
                Console.WriteLine($"  -> ID: {delivery.Id,-4} | Priority: {delivery.Priority,-2} | Weight: {delivery.PackageWeight:0.0} kg");
            }

            Console.WriteLine($"Total Weight: {trips[i].TotalWeight:0.0} / {Trip.MaxCapacity} kg | Free: {trips[i].FreeWeight:0.0} kg");
            Console.WriteLine();
        }
        Console.WriteLine("===============================================");
    }
}