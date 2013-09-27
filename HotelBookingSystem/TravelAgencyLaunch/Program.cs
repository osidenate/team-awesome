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
            HotelSupplier myHotel = new HotelSupplier();

            Thread customer = new Thread(new ThreadStart(myHotel.PriceFunction));
            customer.Start();  
            TravelAgency travelAgency = new TravelAgency();
            HotelSupplier.PriceCut += new HotelSupplier.PriceCutEvent(travelAgency.PriceCutNotification);

            Thread[] travelAgencies = new Thread[4];

            for (int i = 0; i < 3; i++)
            {
                travelAgencies[i] = new Thread(new ThreadStart(travelAgency.PriceCutNotification));
                travelAgencies[i].Name = (i + 1).ToString();
                travelAgencies[i].Start();
                //aborting now to avoid error until getting answer.
                travelAgencies[i].Abort();
            }

            //???The thread will terminate after the HotelSupplier thread has terminated. 

            for (int i = 0; i < 3; i++)
            {
                //travelAgencies[i].Abort();
                Console.WriteLine("Thread " + travelAgencies[i].Name + " needs to be stopped!");
            }
            
            Console.ReadLine();


        }
    }
}
