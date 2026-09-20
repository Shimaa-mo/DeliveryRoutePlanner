using System;
using System.Collections.Generic;
using System.Linq;

namespace eT3_assignment
{
    internal class Trip
    {
        public const double MaxCapacity = 10.0;

        public static int TotalDeliveriesAllTrips { get; private set; }
        public List<Delivery> Deliveries { get; private set; }
        public double TotalWeight { get; set; }

        public int Id { get; set; }
        public int Priority { get; set; }
        public string Area { get; set; }

        public double FreeWeight => MaxCapacity - TotalWeight;

        public int DeliveriesCount => Deliveries.Count;

        public Trip()
        {
            Deliveries = new List<Delivery>();
        }

        public bool CanFit(Delivery delivery)
        => delivery != null && delivery.PackageWeight <= FreeWeight;

        public bool TryAdd(Delivery delivery)
        {
            if (delivery is null || delivery.PackageWeight + TotalWeight > MaxCapacity)
                return false;

            AddDelivery(delivery); 
            return true;
        }
        public void AddDelivery(Delivery delivery)
        {
            if (TotalWeight >= MaxCapacity)
                throw new TripFullException();

            if (delivery is null)
                throw new ArgumentNullException(nameof(delivery), "Delivery cannot be null.");

            if (delivery.PackageWeight > MaxCapacity)
                throw new PackageWeightExceededException();

            if (delivery.PackageWeight + TotalWeight <= MaxCapacity)
            {
                TotalWeight += delivery.PackageWeight;
                Deliveries.Add(delivery);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(TotalWeight), "Capacity of the trip exceeded 10 kg");
            }

            TotalDeliveriesAllTrips++;
        }

        public bool RemoveDelivery(Delivery delivery)
        {
            if (delivery is null)
                return false;

            bool removed = Deliveries.Remove(delivery);
            if (removed)
            {
                TotalWeight -= delivery.PackageWeight;
                TotalDeliveriesAllTrips--;
            }
            
            return removed;
        }


    }
}