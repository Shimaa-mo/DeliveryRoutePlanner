using eT3_assignment;

public class Delivery
{
    public int Id { get; set; }
    public string Area { get; set; }
    public int Priority { get; set; }
    public double PackageWeight { get; set; }

    public Delivery(int id, string area, int priority, double packageWeight)
    {
        if (string.IsNullOrWhiteSpace(area))
            throw new ArgumentException("Area cannot be empty.", nameof(area));

        if (packageWeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(packageWeight), "Package weight must be greater than zero.");

        Id = id;
        Area = area;
        Priority = priority;
        PackageWeight = packageWeight;
    }

    public Delivery(string area, int priority, double packageWeight)
        : this(Trip.TotalDeliveriesAllTrips, area, priority, packageWeight)
    {
    }
}