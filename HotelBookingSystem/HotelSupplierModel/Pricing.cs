using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelSupplierModel
{
    class Pricing
    {
        private const int BaseRoomPrice = 100;

        public readonly double TaxRate = 0.07;
        public readonly double LocationCharge = 10.0;

        public double UnitPrice
        {
            get
            {
                double todayRate = GetTodaysRoomPrice();
                double rooms = Hotel.NumberOfRoomsAvailable;
                double multiplier = Math.Sqrt(rooms) * (1 / rooms);

                double calculatedRate = todayRate + (todayRate * multiplier);

                // Surprise price cut
                if ((calculatedRate % 10) < 5)
                    calculatedRate = calculatedRate - 20;

                return Math.Round(calculatedRate, 2);
            }
        }

        private readonly HotelSupplier Hotel;

        public Pricing(HotelSupplier hotelSupplier)
        {
            Hotel = hotelSupplier;
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
    }
}