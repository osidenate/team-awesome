using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderModel
{
    public interface ICompletedOrderListener
    {
        void OrderComplete(string senderId, double totalPrice);
    }
}
