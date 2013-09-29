using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderModel
{
    
    public class Order
    {
        private static EncryptDecryptServiceRef.ServiceClient encryptDecryptService = new EncryptDecryptServiceRef.ServiceClient();
        private string SenderId;
        private int CardNo; // CC number
        private int Amount; // Number of Rooms to order
       
        public Order() 
        {
        }

        public Order(string senderId, int cardNo, int amount)
        {
            this.SenderId = senderId;
            this.CardNo = cardNo;
            this.Amount = amount;
        }

        public void SetID(string senderId)
        {
            this.SenderId = senderId;
        }

        public string GetID()
        {
            return this.SenderId;
        }

        public void SetCardNo(int cardNo)
        {
            this.CardNo = cardNo;
        }

        public int GetCardNo()
        {
            return this.CardNo;
        }

        public void SetAmt(int amount)
        {
            this.Amount = amount;
        }

        public int GetAmt()
        {
            return this.Amount;
        }

        public static string EncodeOrder(Order order)
        {
            String encodedOrder = encryptDecryptService.Encrypt(Order.orderToString(order));
            return encodedOrder;
        }
        
        public static Order DecodeOrder(string encodedOrder)
        {
            Order myOrder = Order.orderFromString(encryptDecryptService.Decrypt(encodedOrder));
            return myOrder;
        }

        private static string orderToString(Order order)
        {
            string orderString = order.GetID() + "#" + order.GetCardNo() + "#" + order.GetAmt();

            return orderString;
        }

        private static Order orderFromString(string orderString)
        {
            Order myOrder = new Order();
            string[] orderObj = orderString.Split('#');
            myOrder.SetID(orderObj[0]);
            myOrder.SetCardNo(int.Parse(orderObj[1]));
            myOrder.SetAmt(int.Parse(orderObj[2]));
            return myOrder;
        }
    }
}
