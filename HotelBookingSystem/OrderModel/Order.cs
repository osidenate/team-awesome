using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderModel
{
    public class Order
    {
        public Order() 
        { 
        }
        
        public static string EncoderOrder(Order order)
        {
            return string.Empty;
        }
        
        public static Order DecoderOrder(string encodedOrder)
        {
            return new Order();
        }

    }
}
