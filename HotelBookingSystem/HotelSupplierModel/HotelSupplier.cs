using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OrderModel;
using HotelSupplierModel.MulticellBufferService;

namespace HotelSupplierModel
{
    public class HotelSupplier : ICompletedOrderListener
    {
        // REQ: "There is a counter p in the HotelSupplier.
        //       After p (e.g., p = 10) price cuts have been made, the HotelSupplier thread will terminate."
        private int NumberOfPriceCuts = 0;
        private readonly int MaxNumberOfPriceCuts = 10;
        private double LastRoomPrice = 0;

        public delegate void PriceCutEvent();
        public delegate void OrderProcessedEvent(string senderId, double totalCost);
        public delegate void SupplierStoppedEvent();

        public event PriceCutEvent PriceCut;
        public event OrderProcessedEvent OrderProcessed;
        public event SupplierStoppedEvent SupplierStopped;

        private int _numberOfRoomsAvailable = 100;

        private const int BaseRoomPrice = 50;

        public readonly double TaxRate = 0.07;
        public readonly double LocationCharge = 10.0;

        public int NumberOfRoomsAvailable
        {
            get
            {
                return _numberOfRoomsAvailable;
            }

            set
            {
                _numberOfRoomsAvailable = value;
            }
        }

        public HotelSupplier()
        {

        }

        /// <summary>
        /// REQ: "It defines a price-cut event that can emit an event and call the event handlers in the TravelAgencys
        ///       if there is a price-cut according to the PricingModel"
        /// </summary>
        public void EmitPriceCutEvent()
        {
            if (PriceCut != null)
                PriceCut();
        }

        /// <summary>
        /// This method takes an encodedOrder string, decodes it to an Order object, and submits the Order for processing
        /// The MulticellBuffer should call this method to submit the order
        /// REQ: "It receives the orders (in a string) from the MultiCellBuffer."
        /// </summary>
        public void SubmitOrder(string encodedOrder)
        {
            Order orderModel = Order.DecodeOrder(encodedOrder);
            Pricing price = new Pricing(NumberOfRoomsAvailable);
            var orderProcessor = new OrderProcessor(orderModel, price.UnitPrice, price.TaxRate, price.LocationCharge, this);
            var threadStart = new ThreadStart(orderProcessor.process);
            var orderThread = new Thread(threadStart);
            orderThread.Start();

            lock (price)
            {
                if (price.UnitPrice < LastRoomPrice)
                {
                    if (NumberOfPriceCuts >= MaxNumberOfPriceCuts)
                    {
                        // REQ: The thread should stop when there are no more price cuts available
                        Console.WriteLine("Supplier is stopping because the max number of price cuts has been reached");
                        StopHotelSupplier();
                    }

                    EmitPriceCutEvent();
                    NumberOfPriceCuts++;
                }

                LastRoomPrice = price.UnitPrice;
            }
        }

        public double UnitPrice
        {
            get
            {
                double todayRate = GetTodaysRoomPrice();

                double multiplier = Math.Sqrt(NumberOfRoomsAvailable) * (1 / (double)NumberOfRoomsAvailable) * 10;

                double calculatedRate = todayRate + (todayRate * multiplier);

                // Surprise price cut
                if ((calculatedRate % 2) > 1)
                    calculatedRate = calculatedRate - 20;

                return Math.Round(calculatedRate, 2);
            }
        }

        private double GetTodaysRoomPrice()
        {
            var today = DateTime.Now;

            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return BaseRoomPrice;

                case DayOfWeek.Monday:
                    return BaseRoomPrice - 5;

                case DayOfWeek.Tuesday:
                    return BaseRoomPrice - 10;

                case DayOfWeek.Wednesday:
                    return BaseRoomPrice - 5;

                case DayOfWeek.Thursday:
                    return BaseRoomPrice + 5;

                case DayOfWeek.Friday:
                    return BaseRoomPrice + 15;

                case DayOfWeek.Saturday:
                    return BaseRoomPrice + 15;
            }

            return BaseRoomPrice;
        }

        public void PriceFunction()
        {
            using (var bufferService = new MultiCellBufferServiceClient())
            {
                while (NumberOfRoomsAvailable > 0)
                {
                    Thread.Sleep(250);

                    string encodedOrder = bufferService.getOneCell();
                    SubmitOrder(encodedOrder);
                    
                    NumberOfRoomsAvailable--;
                }

                Console.WriteLine("Supplier is stopping because there are no more rooms available.");
                StopHotelSupplier();
            }
        }

        public void OrderComplete(string senderId, double totalPrice)
        {
            if (OrderProcessed != null)
                OrderProcessed(senderId, totalPrice);
        }

        /// <summary>
        /// Call from the HotelSupplier thread. It will launch an event to stop the Travel Agency threads
        /// </summary>
        private void StopHotelSupplier()
        {
            NumberOfRoomsAvailable = 0;

            if (SupplierStopped != null)
                SupplierStopped();

            Thread.CurrentThread.Abort();
        }
    }
}
