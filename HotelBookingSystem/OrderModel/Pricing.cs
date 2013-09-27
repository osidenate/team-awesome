using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderModel
{
    public class Pricing
    {
        private const int BaseRoomPrice = 100;

        public readonly double TaxRate = 0.07;
        public readonly double LocationCharge = 10.0;
        public double NumberOfRooms;

        public double UnitPrice
        {
            get
            {
                double todayRate = GetTodaysRoomPrice();

                double multiplier = Math.Sqrt(NumberOfRooms) * (1 / NumberOfRooms);

                double calculatedRate = todayRate + (todayRate * multiplier);

                // Surprise price cut
                if ((calculatedRate % 10) < 5)
                    calculatedRate = calculatedRate - 20;

                return Math.Round(calculatedRate, 2);
            }
        }

        public Pricing(int numberOfRoomsAvailable)
        {
            NumberOfRooms = numberOfRoomsAvailable;
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