using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace eT3_assignment
{
    internal class Trip
    {
        public List<Delivery> _deliveries;
        private double _totalWeight;
        public int id;
        public int priority;
        public string area;
        public Trip()
        {
            _deliveries = new List<Delivery>();
        }
        public double FreeWeight()
        {
            return 10 - _totalWeight;
        }
        public int DelivariesCount()
        {
            return _deliveries.Count();
        }
        public void AddDelivery(Delivery delivery)
        {
            if (_totalWeight >= 10) throw new TripFullException();
            
            if (delivery == null) throw new ArgumentNullException(nameof(delivery), "Delivery can't be null.");

            if (delivery.packageWeight > 10)
            {
                throw new PackageWeightExceededException();
            }
            else if (delivery.packageWeight + _totalWeight <= 10)
            {
                _totalWeight += delivery.packageWeight;
                _deliveries.Add(delivery);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(_totalWeight) , "capacity of the trip exceeded 10 kg");
            }
        }

        public List<Trip> Create(List<Delivery> deliveries)
        {
            if (deliveries is null || deliveries.Count == 0)
                throw new ArgumentException("Deliveries list cannot be null or empty.", nameof(deliveries));

            List<Trip> trips = new List<Trip>();
            List<Delivery> skippedDeliveries = new List<Delivery>();
            List<double> freeWeights = new List<double>();

            Trip trip = new Trip();
            
            //Delivery urgentDelivery;
            //while (true)
            //{
            //    urgentDelivery = deliveries.MinBy(d => d.priority);
            //    var result = CheckTripCapcity(ref urgentDelivery, ref trip, trips, deliveries, skippedDeliveries, freeWeights);
            //    if (result is not null)
            //    {
            //        trip = result;
            //        break;
            //    }
            //}
            Delivery urgentDelivery = null;
            while (deliveries.Count > 0)
            {
                urgentDelivery = deliveries.MinBy(d => d.priority);
                var result = CheckTripCapcity(ref urgentDelivery, ref trip, trips, deliveries, skippedDeliveries, freeWeights);
                if (result is not null)
                {
                    trip = result;
                    break;
                }
            }

            while (deliveries.Count != 0)
            {
                Delivery relatedDelivery = deliveries
                                                 .Where(d => d.area == urgentDelivery.area)
                                                 .Where(d => !skippedDeliveries.Contains(d))
                                                 .MinBy(d => d.priority);


                //Delivery relatedDelivery = deliveries
                //                                 .Where(d => d.area == urgentDelivery.area)
                //                                 .Where(d => !skippedDeliveries.Contains(d))
                //                                 .MinBy(d => d.priority) ?? deliveries.MinBy(d => d.priority);

                //if (relatedDelivery is null)
                //{

                //    trip = new Trip();
                //    //skipCount = 0;
                //    skippedDeliveries.Clear();
                //    continue;
                //}

                if (relatedDelivery is null)
                {
                    if (trip.DelivariesCount() > 0 && !trips.Contains(trip))
                    {
                        //if (!SingleDeliveryTrip(trip, freeWeights, trips, relatedDelivery, deliveries))
                        //{
                        //    trips.Add(trip);
                        //    freeWeights.Add(trip.FreeWeight()); //the remaining weight from the trip

                        //}
                        trips.Add(trip);
                        freeWeights.Add(trip.FreeWeight()); //the remaining weight from the trip

                    }


                    trip = new Trip();
                    skippedDeliveries.Clear();

                    // نبدأ بأكثر delivery urgent من المتبقي
                    urgentDelivery = deliveries.MinBy(d => d.priority);

                    continue;
                }
                var result = CheckTripCapcity(ref relatedDelivery, ref trip, trips, deliveries, skippedDeliveries , freeWeights);
                if (result is not null)
                {
                    trip = result;
                    urgentDelivery = relatedDelivery;
                }
                    
            }
            if (!trips.Contains(trip))
            {
                //if (!SingleDeliveryTrip(trip, freeWeights, trips, null, deliveries))
                    trips.Add(trip);
            }


            


            return trips;



            //try
            //{
            //    trip.AddDelivery(relatedDelivery);
            //    deliveries.Remove(relatedDelivery);
            //}
            //catch (ArgumentOutOfRangeException argEx)
            //{
            //    skipCount += 1;
            //    continue
            //}
            //catch (Exception ex)
            //{
            //    trips.Add(trip);
            //    trip = new Trip();

            //}


        }

        static Trip CheckTripCapcity(ref Delivery delivery , ref Trip trip, List<Trip> trips, List<Delivery> deliveries, List<Delivery> skippedDeliveries , List<double> freeWeights)
        {
            try
            {
                trip.AddDelivery(delivery);
                deliveries.Remove(delivery);
                return trip;
            }
            catch(PackageWeightExceededException weightEx)
            {
                Console.WriteLine("this isn't valid delivery, delivery package weight should be less than 10 kg");
                deliveries.Remove(delivery);
                return null;
            }
            catch (ArgumentOutOfRangeException argEx)
            {
                //skipCount += 1;
                skippedDeliveries.Add(delivery);
                return null;
                
            }
            catch (TripFullException tFullEx)
            {
                skippedDeliveries.Clear();
                //if(!SingleDeliveryTrip(trip , freeWeights , trips , delivery , deliveries))
                //{
                //    freeWeights.Add(trip.FreeWeight()); //the remaining weight from the trip
                //    trips.Add(trip);
                //}
                freeWeights.Add(trip.FreeWeight()); //the remaining weight from the trip
                trips.Add(trip);
                trip = new Trip();
                return trip;
            }
        }

        static bool SingleDeliveryTrip(Trip trip , List<double> freeWeights, List<Trip> trips , Delivery delivery , List<Delivery> deliveries)
        {
            //if (trip.DelivariesCount() == 1)
            //{
            //    for (int i = 0; i < freeWeights.Count; i++)
            //    {
                    
            //        Console.WriteLine($"Free Weight: {freeWeights[i]} kg");
            //    }
            //    Delivery delivery1 = trip._deliveries[0];
            //    double deliveryWeight = trip._deliveries[0].packageWeight;
            //    Console.WriteLine($" Weight: {deliveryWeight} kg");
            //    int index = freeWeights.FindIndex(w => w >= deliveryWeight );
            //    Console.WriteLine("Truee");
            //    if (index != -1)
            //    {
            //        Console.WriteLine($"Trip free Weight: {trips[index].FreeWeight()} kg ,Trip total Weight: {trips[index]._totalWeight} kg,  Weight: {delivery1.packageWeight} kg");
            //        //if (trips[index] is null) Console.WriteLine("hello");
            //        trips[index].AddDelivery(delivery1);
            //        deliveries.Remove(delivery);
            //        freeWeights[index] = trips[index].FreeWeight();
            //        return true;
            //    }
                
            //}
            //return false;

            double sum = 0;
            trip._deliveries.ForEach(x => sum += x.packageWeight);
            int index = freeWeights.FindIndex(w => w >= sum);
            if (index != -1)
            {
                Console.WriteLine($"Trip free Weight: {trips[index].FreeWeight()} kg ,Trip total Weight: {trips[index]._totalWeight} kg,   kg");
                //if (trips[index] is null) Console.WriteLine("hello");
                trip._deliveries.ForEach(x => trips[index].AddDelivery(x));
                deliveries.Remove(delivery);
                freeWeights[index] = trips[index].FreeWeight();
                return true;
            }
            return false;

        }

        public Dictionary<int, Trip> AddNewDelivery(Delivery delivery , Dictionary<int, Trip> trips , Dictionary<int ,double> freeWeights)
        {
            Trip trip = new Trip();
            trip.priority = delivery.priority;
            trip.area = delivery.area;
            //trip._totalWeight = delivery.packageWeight;
            trip.AddDelivery(delivery);

            //if there is space for it in upper priority
            Dictionary<int, Trip> upperTrips = trips
                                                  .Where(t => t.Value.area == delivery.area).Where(kvp => kvp.Value.priority <= delivery.priority).Where(kvp => kvp.Value.FreeWeight() >= delivery.packageWeight).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (upperTrips.Count != 0)
            {
                Trip readyTrip = upperTrips.OrderBy(kvp => kvp.Value.priority).FirstOrDefault().Value;
                trips[readyTrip.id].AddDelivery(delivery);


            }

            Dictionary<int, Trip> lowerTrips = trips.Where(kvp => kvp.Value.priority >= delivery.priority).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (lowerTrips.Count == 0)
            {
                //if any has its same area and have space for it?
                KeyValuePair<int , Trip> freeTrip = trips
                                                        .FirstOrDefault(t => t.Value.FreeWeight() >= delivery.packageWeight && t.Value.area == delivery.area);
                if (freeTrip.Value != null) {
                    trips[freeTrip.Key].AddDelivery(delivery);
                    //trips[freeTrip.Key]._totalWeight += delivery.packageWeight;
                    freeWeights[freeTrip.Key] = freeWeights[freeTrip.Key] - delivery.packageWeight;

                }
                else
                {
                    trip.id = trips.Count + 1;
                    freeWeights[trip.id] = 10 - delivery.packageWeight;
                    trips.Add(trip.id, trip);
                }
                

                return trips;
            }
            KeyValuePair<int ,Trip> lowerTrip = lowerTrips.MinBy(d => d.Value.priority);
            //List<Trip> sameAreaTrips = lowerTrips.Where(t=> t.area == delivery.area).ToList();

            //if there is space for the delivery in the first trip
            if(freeWeights[lowerTrip.Key] >= delivery.packageWeight && lowerTrip.Value.area == delivery.area)
            {
                //change the priortiy of the trip
                trips[lowerTrip.Key].AddDelivery(delivery);
                trips[lowerTrip.Key].priority = delivery.priority;
                //trips[lowerTrip.Key]._totalWeight += delivery.packageWeight;
                freeWeights[lowerTrip.Key] = trips[lowerTrip.Key].FreeWeight();
                return trips;
            }
            //else if no space for it
            trip.id = lowerTrip.Key;
            //foreach(Trip t in lowerTrips.Values)
            //{
            //    //t.id++;
            //    ShiftTripsForward(trips, trip);
            //    ShiftTripsForward(lowerTrips, trip);
            //}
            ShiftTripsForward(trips, freeWeights, trip);
            //ShiftTripsForward(lowerTrips, trip);
            trips.Add(trip.id, trip);
            //if trip is ful then no need to put other deliveries in it
            if (delivery.packageWeight >= 10.0)
            {
                return trips;
            }
            //if it can take other deliveries
            double freeWeight = 10 - delivery.packageWeight;
            Dictionary<int, Trip> sameAreaTrips = lowerTrips
                                                    .Where(t => t.Value.area == delivery.area)
                                                    .OrderBy(d => d.Value.id)
                                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Search(sameAreaTrips, freeWeight ,trip , trips , freeWeights);

            return trips;
            //foreach(Trip t in sameAreaTrips.Values)
            //{
            //    List<Delivery> validDeliveries =  t.GetDeliveries().Where(d => d.packageWeight <= freeWeight).ToList();
            //    foreach(Delivery d in validDeliveries)
            //    {
            //        //check if it has only one delivery
            //        trip.AddDelivery(d);
            //        freeWeight += d.packageWeight;
            //        if(freeWeight >= 10)
            //        {
            //            //cahnge priority of the trip
            //            sameAreaTrips = sameAreaTrips
            //                                .Where(s => s.Value.id < t.id)
            //                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            //            //organize the rest
            //        }
            //    }

                
            //}

        }

        public void Search(Dictionary<int, Trip> sameAreaTrips ,double freeWeight ,Trip trip , Dictionary<int, Trip> trips , Dictionary<int, double> freeWeights)
        {
            Console.WriteLine($"\nnow trip {trip.id}\n");
            if (sameAreaTrips.Count == 0)
            {
                return;
            }
            foreach (Trip t in sameAreaTrips.Values)
            {
                //List<Delivery> validDeliveries = t.GetDeliveries().Where(d => d.packageWeight <= freeWeight).ToList();
                //if (validDeliveries.Count == 0)
                //{
                //    continue;
                //}
                //if(validDeliveries.Count == 1)
                //{

                //}
                //foreach (Delivery d in validDeliveries)
                while (true)
                {
                    Delivery d = t.GetDeliveries()
                                      .Where(d => d.packageWeight <= freeWeight)
                                      .MinBy(d => d.priority);
                    if (d == null)
                    {
                        break;
                    }
                    //check if it has only one delivery
                    trip.AddDelivery(d);
                    t._deliveries.Remove(d);
                    t._totalWeight -= d.packageWeight;
                    
                    freeWeight -= d.packageWeight;

                    freeWeights[trip.id] = freeWeight;
                    //trips[trip.id]._totalWeight += d.packageWeight;
                    freeWeights[t.id] += d.packageWeight;
                    //trips[t.id]._totalWeight -= d.packageWeight;



                    if (t.DelivariesCount() == 0)
                    {
                        trips.Remove(t.id);
                        ShiftTrips(trips, freeWeights, t);
                        //ShiftTrips(sameAreaTrips, t);
                        break;
                    }

                    //if (freeWeight <= 0)
                    //{
                    //    //cahnge priority of the trip
                    //    //t.priority = validDeliveries.FirstOrDefault(d2 => d2.priority >= d.priority ).priority;
                    //    t.priority = t.GetDeliveries()
                    //                  .MinBy(x => x.priority)
                    //                  .priority;
                    //    sameAreaTrips = sameAreaTrips
                    //                        .Where(s => s.Value.priority <= t.priority)
                    //                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    //    Search(sameAreaTrips, 10 - t.TotalWeight(), t, trips, freeWeights);
                    //    trips.Add(trip.id, trip);
                    //    return;
                    //    //organize the rest
                    //}


                    //cahnge priority of the trip
                    t.priority = t.GetDeliveries()
                                    .MinBy(x => x.priority)
                                    .priority;
                    sameAreaTrips = sameAreaTrips
                                        .Where(s => s.Value.priority >= t.priority)
                                        .Where(s => s.Value != t)
                                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    Console.WriteLine(sameAreaTrips.Count);
                    Search(sameAreaTrips, 10 - t.TotalWeight(), t, trips, freeWeights);
                    //trips.Add(trip.id, trip);
                    if (freeWeight <= 0)
                    {
                        return;
                    }

                }
            }
            return;
        }
        //public void ShiftTrips(Dictionary<int, Trip> trips , Trip t)
        //{
            
        //    var keysToShift = trips.Keys
        //        .Where(k => k > t.id)
        //        .OrderBy(k => k)
        //        .ToList();

        //    foreach (int key in keysToShift)
        //    {
        //        Trip trip = trips[key];
        //        trips.Remove(key);
        //        trip.id -= 1;
        //        trips[key - 1] = trip;   
        //    }
        //}
        //public void ShiftTripsForward(Dictionary<int, Trip> trips, Trip t)
        //{
        //    var shifted = trips
        //        .Select(kvp =>
        //        {
        //            if (kvp.Key >= t.id)
        //            {
        //                kvp.Value.id++;
        //            }

        //            return new KeyValuePair<int, Trip>(
        //                kvp.Key >= t.id ? kvp.Key + 1 : kvp.Key,
        //                kvp.Value);
        //        })
        //        .ToList();

        //    trips.Clear();

        //    foreach (var pair in shifted)
        //    {
        //        trips.Add(pair.Key, pair.Value);
        //    }
        //}


        public void ShiftTrips(
    Dictionary<int, Trip> trips,
    Dictionary<int, double> freeWeights,
    Trip t)
        {
            var keysToShift = trips.Keys
                .Where(k => k > t.id)
                .OrderBy(k => k)
                .ToList();

            foreach (int key in keysToShift)
            {
                Trip trip = trips[key];

                trips.Remove(key);
                trip.id -= 1;
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
                    int newKey = kvp.Key >= t.id
                        ? kvp.Key + 1
                        : kvp.Key;

                    if (kvp.Key >= t.id)
                        kvp.Value.id++;

                    return new KeyValuePair<int, Trip>(newKey, kvp.Value);
                })
                .ToList();

            var shiftedFreeWeights = freeWeights
                .Select(kvp =>
                {
                    int newKey = kvp.Key >= t.id
                        ? kvp.Key + 1
                        : kvp.Key;

                    return new KeyValuePair<int, double>(
                        newKey,
                        kvp.Value);
                })
                .ToList();

            trips.Clear();
            freeWeights.Clear();

            foreach (var pair in shiftedTrips)
                trips.Add(pair.Key, pair.Value);

            foreach (var pair in shiftedFreeWeights)
                freeWeights.Add(pair.Key, pair.Value);
        }
        public List<Delivery> GetDeliveries()
        {
            return _deliveries;
        }

        public double TotalWeight()
        {
            return _totalWeight;
        }
    }
}
