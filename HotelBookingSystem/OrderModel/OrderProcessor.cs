using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OrderModel
{
    class OrderProcessor
    {
        static Regex _regex = new Regex(@"^\d{10}$");//card number must be 10 digts long

        /// <exception cref="ArgumentOutOfRangeException">Thrown when the Card number sent in does not match the expected card number format</exception>
        public void process(Order order, Double price,Double taxRate, double locationCharge ) { 
        
            Match match = _regex.Match(order.GetCardNo().ToString());//Card number must be 10 digts long to be valid
            if (match.Success)
            {
                Double totalCost;

                totalCost = (price * order.GetAmt()) * (1+taxRate) + locationCharge;

                Console.WriteLine("The cost of you order is ${0}", totalCost);

            }
            else {

                throw new ArgumentOutOfRangeException("The Card Number "+ order.GetCardNo()+ " is not a valid card nuber");
            
            }
        
        }
    }
}
