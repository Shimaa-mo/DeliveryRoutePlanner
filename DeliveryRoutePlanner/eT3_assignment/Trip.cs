using System;
using System.Collections.Generic;
using System.Linq;

namespace eT3_assignment
{
    internal class Trip
    {
        private const double MaxCapacity = 10.0;

        public List<Delivery> Deliveries { get; private set; }
        private double _totalWeight;

        public int Id { get; set; }
        public int Priority { get; set; }
        public string Area { get; set; }

        public Trip()
        {
            Deliveries = new List<Delivery>();
        }

        public double FreeWeight() => MaxCapacity - _totalWeight;

        public int DeliveriesCount() => Deliveries.Count;

        public double TotalWeight() => _totalWeight;

        public List<Delivery> GetDeliveries() => Deliveries;

        public void AddDelivery(Delivery delivery)
        {
            if (_totalWeight >= MaxCapacity)
                throw new TripFullException();

            if (delivery is null)
                throw new ArgumentNullException(nameof(delivery), "Delivery cannot be null.");

            if (delivery.PackageWeight > MaxCapacity)
                throw new PackageWeightExceededException();

            if (delivery.PackageWeight + _totalWeight <= MaxCapacity)
            {
                _totalWeight += delivery.PackageWeight;
                Deliveries.Add(delivery);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(_totalWeight), "Capacity of the trip exceeded 10 kg");
            }
        }

        public (Dictionary<int, Trip>, Dictionary<int, double>) Create(List<Delivery> deliveries)
        {
            if (deliveries is null || deliveries.Count == 0)
                throw new ArgumentException("Deliveries list cannot be null or empty.", nameof(deliveries));

            List<Trip> trips = new List<Trip>();
            List<Delivery> skippedDeliveries = new List<Delivery>();
            List<double> freeWeights = new List<double>();

            Trip trip = new Trip();
            Delivery urgentDelivery = null;

            while (deliveries.Count > 0)
            {
                urgentDelivery = deliveries.MinBy(d => d.Priority);
                Trip result = CheckTripCapacity(ref urgentDelivery, ref trip, trips, deliveries, skippedDeliveries, freeWeights);
                if (result is not null)
                {
                    trip = result;
                    trip.Priority = urgentDelivery.Priority;
                    trip.Area = urgentDelivery.Area;
                    break;
                }
            }

            while (deliveries.Count != 0)
            {
                Delivery relatedDelivery = deliveries
                    .Where(d => d.Area == urgentDelivery.Area)
                    .Where(d => !skippedDeliveries.Contains(d))
                    .MinBy(d => d.Priority);

                if (relatedDelivery is null)
                {
                    if (trip.DeliveriesCount() > 0 && !trips.Contains(trip))
                    {
                        trip.Id = trips.Count();
                        trips.Add(trip);
                        freeWeights.Add(trip.FreeWeight());
                    }

                    trip = new Trip();
                    skippedDeliveries.Clear();
                    urgentDelivery = deliveries.MinBy(d => d.Priority);
                    trip.Priority = urgentDelivery.Priority;
                    trip.Area = urgentDelivery.Area;
                    continue;
                }

                Trip result = CheckTripCapacity(ref relatedDelivery, ref trip, trips, deliveries, skippedDeliveries, freeWeights);
                if (result is not null)
                {
                    trip = result;
                    urgentDelivery = relatedDelivery;

                }
            }

            if (!trips.Contains(trip))
            {
                trip.Id = trips.Count();
                trips.Add(trip);
                freeWeights.Add(trip.FreeWeight());
            }
            var dict = freeWeights
                .Index()
                .ToDictionary(x => x.Index, x => x.Item);
            return (trips.ToDictionary(t => t.Id, t => t), dict);
        }

        private static Trip CheckTripCapacity(
            ref Delivery delivery,
            ref Trip trip,
            List<Trip> trips,
            List<Delivery> deliveries,
            List<Delivery> skippedDeliveries,
            List<double> freeWeights)
        {
            try
            {
                trip.AddDelivery(delivery);
                deliveries.Remove(delivery);
                return trip;
            }
            catch (PackageWeightExceededException)
            {
                Console.WriteLine("This is not a valid delivery, package weight must be <= 10 kg.");
                deliveries.Remove(delivery);
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                skippedDeliveries.Add(delivery);
                return null;
            }
            catch (TripFullException)
            {
                skippedDeliveries.Clear();
                freeWeights.Add(trip.FreeWeight());
                trip.Id = trips.Count();
                trips.Add(trip);
                trip = new Trip();
                return trip;
            }
        }

        public Dictionary<int, Trip> AddNewDelivery(
            Delivery delivery,
            Dictionary<int, Trip> trips,
            Dictionary<int, double> freeWeights)
        {
            
            Trip trip = new Trip
            {
                Priority = delivery.Priority,
                Area = delivery.Area
            };
            trip.AddDelivery(delivery);

            // if there is any higher priority trips has space for this delivery
            var upperTrips = trips
                .Where(t => t.Value.Area == delivery.Area
                         && t.Value.Priority <= delivery.Priority
                         && t.Value.FreeWeight() >= delivery.PackageWeight)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (upperTrips.Count != 0)
            {
                Trip readyTrip = upperTrips.OrderBy(kvp => kvp.Value.Priority).First().Value;
                trips[readyTrip.Id].AddDelivery(delivery);
                freeWeights[readyTrip.Id] -= delivery.PackageWeight;
                return trips;
            }

            var lowerTrips = trips
                .Where(kvp => kvp.Value.Priority >= delivery.Priority)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            //if there it is the lowest priority trip , and no space in any trip above it
            if (lowerTrips.Count == 0)
            {
                trip.Id = trips.Count + 1;
                freeWeights[trip.Id] = MaxCapacity - delivery.PackageWeight;
                trips.Add(trip.Id, trip);
                return trips;
            }

            var lowerTrip = lowerTrips.MinBy(d => d.Value.Priority);
            // if the first trip -that has lower priority than this delivery- has space for it
            if (freeWeights[lowerTrip.Key] >= delivery.PackageWeight && lowerTrip.Value.Area == delivery.Area)
            {
                trips[lowerTrip.Key].AddDelivery(delivery);
                trips[lowerTrip.Key].Priority = delivery.Priority;
                freeWeights[lowerTrip.Key] = trips[lowerTrip.Key].FreeWeight();
                return trips;
            }

            // puting a trip ahead of all the lower priority trips
            trip.Id = lowerTrip.Key;
            ShiftTripsForward(trips, freeWeights, trip); //pushing the index of all trips that bellow that trip
            trips.Add(trip.Id, trip);

            //if the delivery took all the trip , then we don't need to organize the rest of the trips
            if (delivery.PackageWeight >= MaxCapacity)
                return trips;

            // if there is space in this trip , then we can move deliveries from bellow trips to this trip
            double freeWeight = MaxCapacity - delivery.PackageWeight;
            var sameAreaTrips = lowerTrips
                .Where(t => t.Value.Area == delivery.Area)
                .OrderBy(d => d.Value.Id)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Search(sameAreaTrips, freeWeight, trip, trips, freeWeights);
            return trips;
        }

        public void Search(
            Dictionary<int, Trip> sameAreaTrips,
            double freeWeight,
            Trip trip,
            Dictionary<int, Trip> trips,
            Dictionary<int, double> freeWeights)
        {
            if (sameAreaTrips.Count == 0)
                return;

            //see all trips bellow this trip and in the  same area
            foreach (Trip t in sameAreaTrips.Values)
            {
                //see all deliveries in this trip that can be moved to the base trip
                while (true)
                {

                    Delivery d = t.GetDeliveries()
                        .Where(item => item.PackageWeight <= freeWeight)
                        .MinBy(item => item.Priority);

                    if (d == null)
                        break;

                    //if delivery fits , add it to base trip , remove it from old trip 
                    trip.AddDelivery(d);
                    t.Deliveries.Remove(d);
                    t._totalWeight -= d.PackageWeight;

                    freeWeight -= d.PackageWeight;
                    freeWeights[trip.Id] = freeWeight;
                    freeWeights[t.Id] += d.PackageWeight;

                    //if current trip has no more deliveries , then remove the whole trip
                    if (t.DeliveriesCount() == 0)
                    {
                        trips.Remove(t.Id);
                        freeWeights.Remove(t.Id);
                        ShiftTrips(trips, freeWeights, t);
                        break;
                    }

                    //if we removed a delivery from current trip , then change the trip priority by its highest priority delivery
                    t.Priority = t.GetDeliveries().MinBy(x => x.Priority).Priority;

                    //if we took from current trip a delivery then we need to reorganize this current trip (take deliveries from bellow trips)
                    sameAreaTrips = sameAreaTrips
                        .Where(s => s.Value.Priority >= t.Priority && s.Value != t)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    Search(sameAreaTrips, MaxCapacity - t.TotalWeight(), t, trips, freeWeights);

                    //if there is no other space in the base trip , then break
                    if (freeWeight <= 0)
                        return;
                }
            }
        }

        public void ShiftTrips(
            Dictionary<int, Trip> trips,
            Dictionary<int, double> freeWeights,
            Trip t)
        {
            var keysToShift = trips.Keys
                .Where(k => k > t.Id)
                .OrderBy(k => k)
                .ToList();

            foreach (int key in keysToShift)
            {
                Trip trip = trips[key];
                trips.Remove(key);
                trip.Id -= 1;
                trips[key - 1] = trip;

                double freeWeight = freeWeights[key];
                freeWeights.Remove(key);
                freeWeights[key - 1] = freeWeight;
            }
        }

        public void ShiftTripsForward(
            Dictionary<int, Trip> trips,
            Dictionary<int, double> freeWeights,
            Trip t)
        {
            var shiftedTrips = trips
                .Select(kvp =>
                {
                    int newKey = kvp.Key >= t.Id ? kvp.Key + 1 : kvp.Key;
                    if (kvp.Key >= t.Id)
                        kvp.Value.Id++;

                    return new KeyValuePair<int, Trip>(newKey, kvp.Value);
                })
                .ToList();

            var shiftedFreeWeights = freeWeights
                .Select(kvp =>
                {
                    int newKey = kvp.Key >= t.Id ? kvp.Key + 1 : kvp.Key;
                    return new KeyValuePair<int, double>(newKey, kvp.Value);
                })
                .ToList();

            trips.Clear();
            freeWeights.Clear();

            foreach (var pair in shiftedTrips)
                trips.Add(pair.Key, pair.Value);

            foreach (var pair in shiftedFreeWeights)
                freeWeights.Add(pair.Key, pair.Value);
        }
    }
}