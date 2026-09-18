namespace eT3_assignment;

internal class Delivery
{
    public int Id { get; init; }
    public string Area { get; init; } = string.Empty;
    public int Priority { get; init; }
    public double PackageWeight { get; init; }

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
}