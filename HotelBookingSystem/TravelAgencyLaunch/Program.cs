using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelSupplierModel;
using TravelAgencyModel;
using System.Threading;

namespace TravelAgencyLaunch

{
    class Program
    {
        static void Main(string[] args)
        {
            //references book page 101

            HotelSupplier myHotel = new HotelSupplier();

            Thread customer = new Thread(new ThreadStart(myHotel.PriceFunction));
            customer.Start();
            TravelAgency travelAgency = new TravelAgency(myHotel);
            HotelSupplier.PriceCut += new HotelSupplier.PriceCutEvent(travelAgency.PriceCutNotification);

            Thread[] travelAgencies = new Thread[4];

            for (int i = 0; i < 3; i++)
            {
                travelAgencies[i] = new Thread(new ThreadStart(travelAgency.PriceCutNotification));
                travelAgencies[i].Name = (i + 1).ToString();
                travelAgencies[i].Start();
            }

            for (int i = 0; i < 3; i++)
            {
                travelAgencies[i].Join();
            }
            
            Console.ReadLine();


        }
    }
}
