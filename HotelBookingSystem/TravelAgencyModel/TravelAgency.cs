using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelSupplierModel;
using System.Threading;
using OrderModel;
using PerformanceRecorder;

namespace TravelAgencyModel
{
    public class TravelAgency
    {
        //REQ: The travel agency will calculate the number of rooms to order, 
        //     for example, based on the need and the difference between the previous price and the current price.
        MulticellBufferService.MultiCellBufferServiceClient myMultiCellBufferService = new MulticellBufferService.MultiCellBufferServiceClient();
        public double CurrentPrice { get; set; }
        public int roomsNeeded { get; set; }
        public Order myOrder { get; set; }
        public readonly HotelSupplier myHotel;
        private string TravelAgencyId = null;
        private readonly AutoResetEvent orderLock = new AutoResetEvent(true);
        private DateTime OrderStart { get; set; }
        private DateTime OrderEnd { get; set; }
        private TestTracked PerformanceTest;

        public TravelAgency(HotelSupplier hotel, TestTracked performanceTest)
        {
            myHotel = hotel;
            CurrentPrice = myHotel.UnitPrice;
            PerformanceTest = performanceTest;
        }

        public void InitializePerformanceTracker()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            TravelAgencyId = threadId.ToString();
            PerformanceTest.gettracker().addTravelAgency(threadId);
        }

        public void InitializeOrder(int cardNo, int amount)
        {
            myOrder = new Order(TravelAgencyId, cardNo, amount);
        }

        public bool IsPriceCut(double newPrice)
        {
            return (newPrice < CurrentPrice);
        }

        public void PriceCutNotification()
        {
            SubmitOrder();
        }

        public void OrderProcessedNotification(string senderId, double totalCost)
        {
            if (senderId == TravelAgencyId)
            {
                OrderEnd = DateTime.Now;
                PerformanceTest.gettracker().stopClock(Int32.Parse(TravelAgencyId));
                Console.WriteLine("Order " + senderId + " has completed");

                orderLock.Set();
            }
        }

        // Generates orders when there is not a price cut
        public void OrderProducer()
        {
            while (myHotel.NumberOfRoomsAvailable > 0)
            {
                Thread.Sleep(1000);
                SubmitOrder();
            }
        }

        private void SubmitOrder()
        {
            if (TravelAgencyId == null)
                InitializePerformanceTracker();

            // The travelagency should only be submitting one order at a time
            orderLock.WaitOne();
            orderLock.Reset();

            OrderStart = DateTime.Now;
            PerformanceTest.gettracker().startClock(Int32.Parse(TravelAgencyId));

            // Order two rooms if there is a price cut, otherwise order one room
            if (IsPriceCut(myHotel.UnitPrice))
                InitializeOrder(GenerateRandomCreditCardNumber(), 2);
            else
                InitializeOrder(GenerateRandomCreditCardNumber(), 1);

            myMultiCellBufferService.setOneCell(Order.EncodeOrder(myOrder));
        }

        private int GenerateRandomCreditCardNumber()
        {
            Random random = new Random();
            string cc = "";
            int i;
            cc += "1"; // should start with 1 so that we don't accidentally overflow Int32
            for (i = 0; i < 9; i++)
                cc += random.Next(0, 9).ToString();
            
            return Int32.Parse(cc);
        }
    }

}
