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
        public event PriceCutEvent PriceCut;

        private int _numberOfRoomsAvailable = 100;
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
            PriceCut.Invoke();
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
    }
}
