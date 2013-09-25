using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelAgencyModel
{
    public class TravelAgency
    {
        
        public TravelAgency() 
        {
            
            
            OrderModel.Order myOrder = new OrderModel.Order();
            var orderId = myOrder.GetID();

            //evaluates the price
            //generates an OrderObject (consisting of multiple values)s
            //sends the order to the Encoder to convert the order object into a plain string.

        }

        public void NotifyPriceCut()
        {
            throw new NotImplementedException();
        }
    }
}
