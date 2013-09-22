using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderModel;
using TravelAgencyModel;

namespace HotelSupplierModel
{
    public class HotelSupplier
    {
        // REQ: "There is a counter p in the HotelSupplier.
        //       After p (e.g., p = 10) price cuts have been made, the HotelSupplier thread will terminate."
        private int NumberOfPriceCuts = 0;
        private readonly int MaxNumberOfPriceCuts = 10;

        private List<TravelAgency> TravelAgencies;

        public HotelSupplier()
        {
            TravelAgencies = new List<TravelAgency>();
        }

        /// <summary>
        /// Subscribes the Travel Agency to the Price Cut Event 
        /// </summary>
        public void SubscribeToPriceCutEvent(TravelAgency travelAgency)
        {
            TravelAgencies.Add(travelAgency);
        }

        /// <summary>
        /// REQ: "It defines a price-cut event that can emit an event and call the event handlers in the TravelAgencys
        ///       if there is a price-cut according to the PricingModel"
        /// </summary>
        public void EmitPriceCutEvent()
        {
            foreach (var travelAgency in TravelAgencies)
            {
                travelAgency.NotifyPriceCut();
            }
        }

        /// <summary>
        /// This method takes an encodedOrder string, decodes it to an Order object, and submits the Order for processing
        /// The MulticellBuffer should call this method to submit the order
        /// REQ: "It receives the orders (in a string) from the MultiCellBuffer."
        /// </summary>
        public void SubmitOrder(string encodedOrder)
        {
            Order orderModel = Order.DecodeOrder(encodedOrder);

            // TODO
        }
    }
}
