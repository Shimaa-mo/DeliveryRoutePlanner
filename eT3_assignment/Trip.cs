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
        private List<Delivery> _deliveries;
        private double _totalWeight;
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
                        if (!SingleDeliveryTrip(trip, freeWeights, trips, relatedDelivery, deliveries))
                        {
                            trips.Add(trip);
                            freeWeights.Add(trip.FreeWeight()); //the remaining weight from the trip

                        }
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
                if (!SingleDeliveryTrip(trip, freeWeights, trips, null, deliveries))
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
                if(!SingleDeliveryTrip(trip , freeWeights , trips , delivery , deliveries))
                {
                    freeWeights.Add(trip.FreeWeight()); //the remaining weight from the trip
                    trips.Add(trip);
                }

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
