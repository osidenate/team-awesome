using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderModel
{
    public class Order
    {
        private string SenderId;
        private int CardNo; // CC number
        private int Amount; // Number of Rooms to order

        public Order() 
        { 
        }

        public void SetID(string senderId)
        {
            SenderId = senderId;
        }

        public string GetID()
        {
            return SenderId;
        }

        public void SetCardNo(int cardNo)
        {
            CardNo = cardNo;
        }

        public int GetCardNo()
        {
            return CardNo;
        }

        public void SetAmt(int amount)
        {
            Amount = amount;
        }

        public int GetAmt()
        {
            return Amount;
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
