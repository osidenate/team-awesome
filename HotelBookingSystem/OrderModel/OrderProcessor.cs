using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OrderModel
{
    public class OrderProcessor
    {
        private readonly Order order;
        private readonly double price;
        private readonly double taxRate;
        private readonly double locationCharge;
        private ICompletedOrderListener hotelSupplier;

        static Regex _regex = new Regex(@"^\d{10}$");//card number must be 10 digts long

        public OrderProcessor(Order order, double price, double taxRate, double locationCharge, ICompletedOrderListener hotelSupplier)
        {
            this.order = order;
            this.price = price;
            this.taxRate = taxRate;
            this.locationCharge = locationCharge;
            this.hotelSupplier = hotelSupplier;
        }

        /// <exception cref="ArgumentOutOfRangeException">Thrown when the Card number sent in does not match the expected card number format</exception>
        public void process()
        { 
            Match match = _regex.Match(order.GetCardNo().ToString());//Card number must be 10 digts long to be valid
            
            if (match.Success)
            {
                Double totalCost;

                totalCost = (price * order.GetAmt()) * (1+taxRate) + locationCharge;

                hotelSupplier.OrderComplete(order.GetID(), totalCost);

                Console.WriteLine("The cost of you order is ${0}", totalCost);
            }
            else
            {
                throw new ArgumentOutOfRangeException("The Card Number "+ order.GetCardNo()+ " is not a valid card nuber");
            }
        }
    }
}
