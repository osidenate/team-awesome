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

        public event PriceCutEvent PriceCut;
        public event OrderProcessedEvent OrderProcessed;

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
    }
}
