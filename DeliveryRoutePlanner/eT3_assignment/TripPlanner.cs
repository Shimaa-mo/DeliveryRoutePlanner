using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eT3_assignment
{
    internal class TripPlanner
    {
        
        
        public List<Trip> PlanTrips(List<Delivery> deliveries)
        {
            if (deliveries is null || deliveries.Count == 0)
                throw new ArgumentException("Deliveries list cannot be null or empty.", nameof(deliveries));

            var trips = new List<Trip>();
            var skippedDeliveries = new List<Delivery>();
            var overweightDeliveries = new List<Delivery>();
            var currentTrip = new Trip();

            while (deliveries.Count > 0)
            {
                // if it's new trip , put highest priority first
                if (currentTrip.DeliveriesCount() == 0)
                {
                    var seed = deliveries.MinBy(d => d.Priority);
                    currentTrip.Area = seed.Area;
                    currentTrip.Priority = seed.Priority;
                }

                // put the next possible delivery in the trip 
                var delivery = deliveries
                    .Where(d => d.Area == currentTrip.Area && !skippedDeliveries.Contains(d))
                    .MinBy(d => d.Priority);

                // if there is no available delivery for this trip , close it and open another one
                if (delivery is null)
                {
                    currentTrip.Id = trips.Count + 1;
                    trips.Add(currentTrip);

                    currentTrip = new Trip();
                    skippedDeliveries.Clear();
                    continue;
                }

                
                // check the availability of the delivery
                if (delivery.PackageWeight > Trip.MaxCapacity)
                {
                    overweightDeliveries.Add(delivery);
                    deliveries.Remove(delivery);

                }
                else if (currentTrip.TryAdd(delivery))
                {
                    deliveries.Remove(delivery);
                }
                else
                {
                    skippedDeliveries.Add(delivery);
                }
            }

            // add the last trip to the trips
            if (currentTrip.DeliveriesCount() > 0)
            {
                currentTrip.Id = trips.Count + 1;
                trips.Add(currentTrip);
            }

            if (overweightDeliveries.Count > 0)
            {
                Console.WriteLine("\nWarning: The following deliveries could not be scheduled because they exceed the 10 kg capacity:");

                foreach (var delivery in overweightDeliveries)
                {
                    Console.WriteLine(
                        $"  -> ID: {delivery.Id} | Weight: {delivery.PackageWeight} kg"
                    );
                }
            }

            return trips;
        }


        public List<Trip> ScheduleDelivery(Delivery delivery, List<Trip> trips)
        {
            // First : if there is a higher priority trip and has space for the delivery
            var readyTrip = trips
                .Where(t => t.Area == delivery.Area
                         && t.Priority <= delivery.Priority
                         && t.FreeWeight() >= delivery.PackageWeight)
                .OrderBy(t => t.Priority)
                .FirstOrDefault();

            if (readyTrip != null)
            {
                readyTrip.AddDelivery(delivery);
                return trips;
            }

            // Seconde : get all trips that has lower priority
            var lowerTrips = trips
                .Where(t => t.Priority >= delivery.Priority)
                .ToList();

            //if the delivery is the lowest priority , put it at the end
            if (lowerTrips.Count == 0)
            {
                var endTrip = new Trip
                {
                    Id = trips.Count + 1,
                    Priority = delivery.Priority,
                    Area = delivery.Area
                };
                endTrip.AddDelivery(delivery);
                trips.Add(endTrip);
                return trips;
            }

            // get the 
            var lowerTrip = lowerTrips.MinBy(t => t.Priority);

            // if the first trip -that has lower priority than this delivery- has space for this delivery , add delivery to it and return
            if (lowerTrip.FreeWeight() >= delivery.PackageWeight && lowerTrip.Area == delivery.Area)
            {
                lowerTrip.AddDelivery(delivery);
                lowerTrip.Priority = delivery.Priority;
                return trips;
            }

            // else if no space for it, add a trip delivery before all lower prioity trips 
            var trip = new Trip
            {
                Priority = delivery.Priority,
                Area = delivery.Area
            };
            trip.AddDelivery(delivery);

            //shift the trips
            int insertIndex = trips.IndexOf(lowerTrip);
            trips.Insert(insertIndex, trip);

            // to be not 0 index
            for (int i = 0; i < trips.Count; i++)
            {
                trips[i].Id = i + 1;
            }

            // if the trip has no space for other deliveries , return
            if (delivery.PackageWeight >= Trip.MaxCapacity)
                return trips;

            // if the trip has space for other trips get from lower priority trips , and organize them same way
            double freeWeight = trip.FreeWeight();
            var sameAreaTrips = lowerTrips
                .Where(t => t.Area == delivery.Area)
                .OrderBy(t => t.Id)
                .ToList();

            RebalanceDeliveries(sameAreaTrips, trip, trips);
            return trips;
        }

        public void RebalanceDeliveries(List<Trip> sameAreaTrips, Trip initialTrip, List<Trip> trips)
        {
            // queue for trips that has space can be filled by deliveries from other trips
            var rebalanceQueue = new Queue<Trip>();
            rebalanceQueue.Enqueue(initialTrip);

            while (rebalanceQueue.Count > 0)
            {
                Trip targetTrip = rebalanceQueue.Dequeue();

                // if the target trip has no more space , or didnt removed from the trips
                if (targetTrip.FreeWeight() <= 0 || !trips.Contains(targetTrip))
                    continue;

                // lower priority trips in the same area
                var lowerTrips = sameAreaTrips
                    .Where(t => t != targetTrip && t.Priority >= targetTrip.Priority && trips.Contains(t))
                    .OrderBy(t => t.Id)
                    .ToList();

                foreach (Trip sourceTrip in lowerTrips)
                {
                    if (targetTrip.FreeWeight() <= 0)
                        break;

                    // from this sourceTrip take the deliveries that fits in the target trip , and put source trip in queue (if you took from it)
                    while (true)
                    {
                        Delivery bestDelivery = sourceTrip.Deliveries
                            .Where(d => d.PackageWeight <= targetTrip.FreeWeight())
                            .MinBy(d => d.Priority);

                        if (bestDelivery == null)
                            break;

                        // transefer trip
                        targetTrip.AddDelivery(bestDelivery);
                        sourceTrip.RemoveDelivery(bestDelivery);

                        // 2. if the sourceTrip has no more deliveries , remove it
                        if (sourceTrip.DeliveriesCount() == 0)
                        {
                            trips.Remove(sourceTrip);
                            sameAreaTrips.Remove(sourceTrip);
                            break;
                        }

                        // change the priority of the sourceTrip with its highest priority delivery
                        sourceTrip.Priority = sourceTrip.Deliveries.Min(d => d.Priority);

                        // sourceTrip now has empty space , then add it to the queue to be able to take deliveries from lower trips
                        if (!rebalanceQueue.Contains(sourceTrip))
                        {
                            rebalanceQueue.Enqueue(sourceTrip);
                        }

                        // if targetTrip is full , then go to the next trip in the queue
                        if (targetTrip.FreeWeight() <= 0)
                            break;
                    }
                }
            }

            // to be not 0 index
            for (int i = 0; i < trips.Count; i++)
            {
                trips[i].Id = i + 1;
            }
        }
        
    }
}
