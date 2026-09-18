using eT3_assignment;
using System.IO;

string filePath = "C:\\Users\\shosho\\Desktop\\test.txt";

List<Delivery> deliveries = ReadDeliveriesFromFile(filePath);

Trip planner = new Trip();

(Dictionary<int, Trip> newTrips, Dictionary<int, double> freeWeights) = planner.Create(deliveries);

//for (int i = 0; i < trips.Count; i++)
//{
//    trips[i].Id = i;
//    Delivery delivery = trips[i].GetDeliveries().MinBy(d => d.id);
//    trips[i].Priority = delivery.priority;
//    trips[i].Area = delivery.area;

//}
//Dictionary<int, Trip> newTrips = trips
//    .Select((trip, index) => (trip, index))
//    .ToDictionary(x => x.index, x => x.trip);

//var freeWeights = new Dictionary<int, double>(trips.Count);
//for (int i = 0; i < trips.Count; i++)
//{
//    freeWeights[i] = trips[i].FreeWeight();
//}


PrintTrips2(newTrips);

Console.WriteLine("\n \n \n \n Newwww \n\n\n\n");

int id = newTrips.Values.Sum(t => t.Deliveries.Count);
Delivery d = new Delivery(id, "Nasr City", 1, 4);


Dictionary<int, Trip> trips2 = planner.AddNewDelivery(d, newTrips, freeWeights);

PrintTrips2(trips2);


static List<Delivery> ReadDeliveriesFromFile(string filePath)
{
    List<Delivery> deliveries = new List<Delivery>();

    string[] lines = File.ReadAllLines(filePath);

    foreach (string line in lines)
    {
        string[] parts = line.Split('|');

        Delivery delivery = new Delivery(int.Parse(parts[0])
            , parts[1]
            , int.Parse(parts[2])
            , double.Parse(parts[3]));

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
                $"ID: {delivery.Id}, " +
                $"Area: {delivery.Area}, " +
                $"Priority: {delivery.Priority}, " +
                $"Weight: {delivery.PackageWeight} kg");
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
                $"ID: {delivery.Id}, " +
                $"Area: {delivery.Area}, " +
                $"Priority: {delivery.Priority}, " +
                $"Weight: {delivery.PackageWeight} kg");
        }

        Console.WriteLine($"Total Weight: {trip.TotalWeight()} kg");
        Console.WriteLine($"Free Weight: {trip.FreeWeight()} kg");
        Console.WriteLine();
    }
}