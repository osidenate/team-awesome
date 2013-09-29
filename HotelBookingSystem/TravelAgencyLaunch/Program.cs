using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelSupplierModel;
using TravelAgencyModel;
using System.Threading;
using PerformanceRecorder;

namespace TravelAgencyLaunch
{
    class Program
    {
        static void Main(string[] args)
        {
            HotelSupplier myHotel = new HotelSupplier();
            Thread hotelSupplierThread = new Thread(new ThreadStart(myHotel.PriceFunction));
            hotelSupplierThread.Start();
            

            List<Thread> travelAgencyThreads = new List<Thread>();

            PerformanceTracker tracker = new PerformanceTracker();

            // Create four travel agencies and subscribe them to the events
            for (int i = 0; i < 10; i++)
            {
                var travelAgency = new TravelAgency(myHotel, tracker);
                myHotel.PriceCut       += new HotelSupplier.PriceCutEvent(travelAgency.PriceCutNotification);
                myHotel.OrderProcessed += new HotelSupplier.OrderProcessedEvent(travelAgency.OrderProcessedNotification);

                var agencyThread = new Thread(new ThreadStart(travelAgency.OrderProducer));
                travelAgencyThreads.Add(agencyThread);
                agencyThread.Start();
            }

            try
            {
                foreach (var agencyThread in travelAgencyThreads)
                {
                    agencyThread.Join();
                }
            }
            finally
            {
                hotelSupplierThread.Abort();
                tracker.printPerfromancnceData();
                Console.WriteLine("All Done");
                Console.ReadLine();
            }

            
        }
    }
}
