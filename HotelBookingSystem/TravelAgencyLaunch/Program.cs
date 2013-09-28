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


//Cameron will create the performance function?
//            (5) Performance tables (The performance table is required for group projects).
//            Please measure and list the following performance values. When you are recording the performance data, you must close all other applications before you run the experiment.
//            The table must contain:
//            N = 1, 2, 3, …, 10, where N is the TravelAgency threads’ numbers (ID).
//            ARTOC: Average Response Time between placing Order and receiving Confirmation
//            TTC: Total Time between the first price release and completion measured by the HotelSupplier


//Victoria will create the
//            10. Performance Table and User Manual: You must submit a document in WORD, PDF, or a web page. You must give: [10 points]
//(1) Name(s) of developer(s) of the system and the percentage of contribution of each member if the project is done as a group project. The percentage will be used to scale the score.
//(2) Class diagram: Each class block should include the class name, attributes and methods, and the relations between the classes. For the definition of class diagram, see http://www.objectmentor.com/resources/articles/umlClassDiagrams.pdf
//You can use the Visual Studio to generate the class diagram automatically.
//(3) A guide on testing the system: development environment used, loading the program, input, and expected output;
//(4) A screenshot of your execution of the program with input and output;

            HotelSupplier myHotel = new HotelSupplier();
            Thread hotelSupplierThread = new Thread(new ThreadStart(myHotel.PriceFunction));
            hotelSupplierThread.Start();

            TravelAgency travelAgency = new TravelAgency(myHotel);
            myHotel.PriceCut       += new HotelSupplier.PriceCutEvent(travelAgency.PriceCutNotification);
            myHotel.OrderProcessed += new HotelSupplier.OrderProcessedEvent(travelAgency.OrderProcessedNotification);

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
