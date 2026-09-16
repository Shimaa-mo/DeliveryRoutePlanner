using eT3_assignment;
using System.IO;

string filePath = "C:\\Users\\shosho\\Desktop\\test.txt";

List<Delivery> deliveries = ReadDeliveriesFromFile(filePath);

Trip planner = new Trip();

List<Trip> trips = planner.Create(deliveries);

PrintTrips(trips);

static List<Delivery> ReadDeliveriesFromFile(string filePath)
{
    List<Delivery> deliveries = new List<Delivery>();

    string[] lines = File.ReadAllLines(filePath);

    foreach (string line in lines)
    {
        string[] parts = line.Split('|');

        Delivery delivery = new Delivery
        {
            id = int.Parse(parts[0]),
            area = parts[1],
            priority = int.Parse(parts[2]),
            packageWeight = double.Parse(parts[3])
        };

        deliveries.Add(delivery);
    }

    return deliveries;
}

static void PrintTrips(List<Trip> trips)
{
    for (int i = 0; i < trips.Count; i++)
    {
        Console.WriteLine($"Trip {i + 1}");
        Console.WriteLine("----------------");

        foreach (Delivery delivery in trips[i].GetDeliveries())
        {
            Console.WriteLine(
                $"ID: {delivery.id}, " +
                $"Area: {delivery.area}, " +
                $"Priority: {delivery.priority}, " +
                $"Weight: {delivery.packageWeight} kg");
        }

        Console.WriteLine($"Total Weight: {trips[i].TotalWeight()} kg");
        Console.WriteLine($"Free Weight: {trips[i].FreeWeight()} kg");
        Console.WriteLine();
    }
}