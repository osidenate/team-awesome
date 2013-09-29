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
        public static int NumberOfPriceCuts = 0;
        public readonly int MaxNumberOfPriceCuts = 10;
       

        public delegate void PriceCutEvent();
        public delegate void OrderProcessedEvent(string senderId, double totalCost);
        public delegate void SupplierStoppedEvent();

        public event PriceCutEvent PriceCut;
        public event OrderProcessedEvent OrderProcessed;
        public event SupplierStoppedEvent SupplierStopped;

        private static int _numberOfRoomsAvailable = 100;

        private const int BaseRoomPrice = 50;

        public readonly double TaxRate = 0.07;
        public readonly double LocationCharge = 10.0;
        public Pricing LastRoomPrice = new Pricing(_numberOfRoomsAvailable);
        public Pricing CurrentRoomPrice = new Pricing(_numberOfRoomsAvailable);
       

        public  object box = NumberOfPriceCuts;
        private object box2 = _numberOfRoomsAvailable;

        public int NumberOfRoomsAvailable
        {
            get
            {
                return (int)box2;
            }

            set
            {
                box2 = value;
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
            Pricing price = CurrentRoomPrice;
            var orderProcessor = new OrderProcessor(orderModel, price.UnitPrice, price.TaxRate, price.LocationCharge, this);
            var threadStart = new ThreadStart(orderProcessor.process);
            var orderThread = new Thread(threadStart);
            orderThread.Start();

            lock (price)
            {
                
                lock (LastRoomPrice)
                {
                    LastRoomPrice = price;
                }
                lock (box2)
                {
                    box2 = (int)box2- orderModel.GetAmt();
                }
            }
        }

        
        public void PriceFunction()
        {
            using (var bufferService = new MultiCellBufferServiceClient())
            {

                while ((int)box < MaxNumberOfPriceCuts +1 )
                {
                    Thread.Sleep(100);
                    lock (CurrentRoomPrice)
                    {
                        CurrentRoomPrice = new Pricing((int)box2);

                    }
                    lock (box)
                    {
                        if (CurrentRoomPrice.UnitPrice < LastRoomPrice.UnitPrice)
                        {
                            EmitPriceCutEvent();
                            box = (int)box + 1;
                        }
                    }
                    string encodedOrder = bufferService.getOneCell();
                    SubmitOrder(encodedOrder);
                }
                while (true)
                {
                    string encodedOrder = bufferService.getOneCell();
                    SubmitOrder(encodedOrder);
                }
                   
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
