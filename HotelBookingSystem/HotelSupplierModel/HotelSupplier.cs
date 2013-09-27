using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OrderModel;

namespace HotelSupplierModel
{
    public class HotelSupplier
    {
        // REQ: "There is a counter p in the HotelSupplier.
        //       After p (e.g., p = 10) price cuts have been made, the HotelSupplier thread will terminate."
        private int NumberOfPriceCuts = 0;
        private readonly int MaxNumberOfPriceCuts = 10;
        private double LastRoomPrice = 0;

        public delegate void PriceCutEvent();

        public static event PriceCutEvent PriceCut;

        private int _numberOfRoomsAvailable = 100;

        private const int BaseRoomPrice = 100;

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
            //If the new price is lower than the previous price, 
            //it emits a (promotional) event and calls the event handlers in the travel agencies that have subscribed to the event.
            if (BaseRoomPrice < UnitPrice)
                EmitPriceCutEvent();
        }


        /// <summary>
        /// Subscribes the Travel Agency to the Price Cut Event 
        /// </summary>
        public void SubscribeToPriceCutEvent()
        {
            //PriceCut += travelAgency.NotifyPriceCut;
        }

        /// <summary>
        /// REQ: "It defines a price-cut event that can emit an event and call the event handlers in the TravelAgencys
        ///       if there is a price-cut according to the PricingModel"
        /// </summary>
        public void EmitPriceCutEvent()
        {
            //KN: PriceCut.Invoke();
            //KN: corrected syntax for raising the event

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
            //Pricing price = new Pricing(this);
            Pricing price = new Pricing(200);
            var orderProcessor = new OrderProcessor(orderModel, price.UnitPrice, price.TaxRate, price.LocationCharge);
            var threadStart = new ThreadStart(orderProcessor.process);
            var orderThread = new Thread(threadStart);
            orderThread.Start();

            if (price.UnitPrice < LastRoomPrice)
            {
                NumberOfPriceCuts++;

                if (NumberOfPriceCuts >= MaxNumberOfPriceCuts)
                    return;

                EmitPriceCutEvent();
            }

            LastRoomPrice = price.UnitPrice;
        }

        public double UnitPrice
        {
            get
            {
                double todayRate = GetTodaysRoomPrice();

                double multiplier = Math.Sqrt(NumberOfRoomsAvailable) * (1 / NumberOfRoomsAvailable);

                double calculatedRate = todayRate + (todayRate * multiplier);

                // Surprise price cut
                if ((calculatedRate % 10) < 5)
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

        //KN: added this event
        public void PriceFunction()
        {
            //KN: REQ: It receives the orders (in a string) from the MultiCellBuffer. 
            //KN:It calls the Decoder to convert the string into the order object. 
            //KN:For each order, you can use the existing thread or start a new thread (resulting in multiple threads for processing multiple orders [1]) from OrderProcessing class (or method) to process the order based on the current price. 
            //KN:There is a counter p in the HotelSupplier. After p (e.g., p = 10) price cuts have been made, the HotelSupplier thread will terminate. Before generating the first price, a time stamp must be saved [2]. Before the thread terminates, the total time used will be calculated and saved (or printed). 
            //KN: Would add code

            //KN: MulticellBufferService.MultiCellBufferServiceClient myMultiCellBufferService = new MulticellBufferService.MultiCellBufferServiceClient();
            //KN: myMultiCellBufferService.getOneCell(Order.DecodeOrder(myOrder));

        }

    }
}
