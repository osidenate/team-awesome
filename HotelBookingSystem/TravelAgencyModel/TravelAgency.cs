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
       
        public int roomsNeeded { get; set; }
        public Order myOrder { get; set; }
        public readonly HotelSupplier myHotel;
        private string TravelAgencyId = null;
        private readonly AutoResetEvent orderLock = new AutoResetEvent(true);
        private DateTime OrderStart { get; set; }
        private DateTime OrderEnd { get; set; }
        private PerformanceTracker myPerformanceTracker;
        private Boolean isPriceCut = false;
        public static double CurrentPrice;
        object box = CurrentPrice;

        public static double OldPrice;
        object box2 = OldPrice;

        public double myCurrentPrice
        {
            get
            {
                return (double)box;
            }

            set
            {
                box = value;
            }
        }
        public double myOldPrice
        {
            get
            {
                return (double)box2;
            }

            set
            {
                box2 = value;
            }
        }
        public TravelAgency(HotelSupplier hotel, PerformanceTracker performanceTest)
        {
            myHotel = hotel;
            myCurrentPrice = myHotel.CurrentRoomPrice.UnitPrice;
            myPerformanceTracker = performanceTest;
        }

        public void InitializePerformanceTracker()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            TravelAgencyId = threadId.ToString();
            myPerformanceTracker.addTravelAgency(threadId);
        }

        public void InitializeOrder(int cardNo, int amount)
        {
            myOrder = new Order(TravelAgencyId, cardNo, amount);
        }

        public bool IsPriceCut()
        {
           
            if (myOldPrice <= myCurrentPrice) {
                isPriceCut = false;
            }
            return (isPriceCut);
        }

        public void PriceCutNotification()
        {
            lock (box) {
                box = myHotel.CurrentRoomPrice.UnitPrice;
            }
            isPriceCut = true;
           
        }

        public void OrderProcessedNotification(string senderId, double totalCost)
        {
            if (senderId == TravelAgencyId)
            {
                OrderEnd = DateTime.Now;
                myPerformanceTracker.stopClock(Int32.Parse(TravelAgencyId));
                Console.WriteLine("Travel Agency " + TravelAgencyId + " order has completed");

               
            }
        }

        // Generates orders when there is not a price cut
        public void OrderProducer()
        {
            InitializePerformanceTracker();
            while ((int)myHotel.box < myHotel.MaxNumberOfPriceCuts +1)
            {
                Thread.Sleep(100);
                SubmitOrder();
            }
           Console.WriteLine("thread {0} exited", TravelAgencyId);
            Thread.CurrentThread.Abort();
            
            
        }

        private void SubmitOrder()
        {
         
            // The travelagency should only be submitting one order at a time
          orderLock.WaitOne();
           orderLock.Reset();

            OrderStart = DateTime.Now;
            myPerformanceTracker.startClock(Int32.Parse(TravelAgencyId));

            // Order two rooms if there is a price cut, otherwise order one room
            if (IsPriceCut())
            {
                Console.WriteLine("NOTICE: Additional rooms are being ordered due to a price cut event");
                InitializeOrder(GenerateRandomCreditCardNumber(), 2);
            }
            else
            {
                InitializeOrder(GenerateRandomCreditCardNumber(), 1);
            }

            myMultiCellBufferService.setOneCell(Order.EncodeOrder(myOrder));
            lock (box2) {
                myOldPrice = myHotel.LastRoomPrice.UnitPrice;
            }
            orderLock.Set();
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