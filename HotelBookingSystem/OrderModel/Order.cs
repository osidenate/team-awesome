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
        
        public static string EncodeOrder(Order order)
        {
            throw new NotImplementedException();
            return string.Empty;
        }
        
        public static Order DecodeOrder(string encodedOrder)
        {
            throw new NotImplementedException();
            return new Order();
        }
    }
}
