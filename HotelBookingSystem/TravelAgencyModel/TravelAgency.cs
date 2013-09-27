using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelSupplierModel;
using System.Threading;
using OrderModel;

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
        public HotelSupplier myHotel = new HotelSupplier();
        public TravelAgency(HotelSupplier hotel)
        {
            myHotel = hotel;
            CurrentPrice = myHotel.UnitPrice;
        }

        public void InitializeOrder(string senderId, int cardNo, int amount)
        {
            myOrder = new Order(senderId, cardNo, amount);
        }

        public void CalculateRoomsToOrder(int roomsNeed)
        {

        }

        public bool PriceCut(double newPrice)
        {
            return (newPrice < CurrentPrice);
        }

        //Each order is an OrderClass object. 
        //The object is sent to the Encoder for encoding. 
        //The encoded string is sent back to the travel agency. 
        //Then, the travel agency will send the order in String format to the MultiCellBuffer. 
        //Before sending the order to the MultiCellBuffer, a time stamp must be saved. 


        public void PriceCutNotification()
        {
            if (PriceCut(myHotel.UnitPrice))
                return;
            myMultiCellBufferService.setOneCell(Order.EncodeOrder(myOrder));
            //???When the confirmation of order completion is received, the time of the order will be calculated and saved (or printed).
            Console.WriteLine("Order is Complete");
        }
    }
}
