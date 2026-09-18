using eT3_assignment;
using System.IO;

string filePath = "C:\\Users\\shosho\\Desktop\\test.txt";

List<Delivery> deliveries = ReadDeliveriesFromFile(filePath);

Trip planner = new Trip();

List<Trip> trips = planner.Create(deliveries);

for (int i = 0; i < trips.Count; i++)
{
    trips[i].id = i;
    Delivery delivery = trips[i].GetDeliveries().MinBy(d => d.id);
    trips[i].priority = delivery.priority;
    trips[i].area = delivery.area;
    
}
Dictionary<int, Trip> newTrips = trips
    .Select((trip, index) => (trip, index))
    .ToDictionary(x => x.index, x => x.trip);

var freeWeights = new Dictionary<int, double>(trips.Count);
for (int i = 0; i < trips.Count; i++)
{
    freeWeights[i] = trips[i].FreeWeight();
}


PrintTrips(trips);

Console.WriteLine("\n \n \n \n Newwww \n\n\n\n" );

Delivery d = new Delivery()
{
    id = 8,
    area = "Nasr City",
    priority = 1,
    packageWeight = 4
};


//check of id

Dictionary<int, Trip> trips2 = planner.AddNewDelivery(d, newTrips, freeWeights);

PrintTrips2(trips2);


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

static void PrintTrips2(Dictionary<int, Trip> trips)
{
    foreach (var (index, trip) in trips.OrderBy(kvp => kvp.Key))
    {
        Console.WriteLine($"Trip {index + 1}");
        Console.WriteLine("----------------");

        foreach (Delivery delivery in trip.GetDeliveries())
        {
            Console.WriteLine(
                $"ID: {delivery.id}, " +
                $"Area: {delivery.area}, " +
                $"Priority: {delivery.priority}, " +
                $"Weight: {delivery.packageWeight} kg");
        }

        Console.WriteLine($"Total Weight: {trip.TotalWeight()} kg");
        Console.WriteLine($"Free Weight: {trip.FreeWeight()} kg");
        Console.WriteLine();
    }
}