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
        public readonly HotelSupplier myHotel;
        private readonly string TravelAgencyId;
        private readonly AutoResetEvent orderLock = new AutoResetEvent(true);

        public TravelAgency(HotelSupplier hotel)
        {
            myHotel = hotel;
            CurrentPrice = myHotel.UnitPrice;
            TravelAgencyId = Thread.CurrentThread.ManagedThreadId.ToString();
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
                // TODO stop the stopwatch and log order completed timestamp

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
            // The travelagency should only be submitting one order at a time
            orderLock.WaitOne();
            orderLock.Reset();

            // Order two rooms if there is a price cut, otherwise order one room
            if (IsPriceCut(myHotel.UnitPrice))
                InitializeOrder(GenerateRandomCreditCardNumber(), 2);
            else
                InitializeOrder(GenerateRandomCreditCardNumber(), 1);

            // TODO Set timestamp/stopwatch

            myMultiCellBufferService.setOneCell(Order.EncodeOrder(myOrder));
        }

        private int GenerateRandomCreditCardNumber()
        {
            Random random = new Random();
            string cc = "";
            int i;
            cc += random.Next(1, 9).ToString();
            for (i = 0; i < 9; i++)
                cc += random.Next(0, 9).ToString();
            
            return Int32.Parse(cc);
        }
    }
}
