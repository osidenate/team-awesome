using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Order myOrder = new Order();
            Order myDecodedOrder = null;
            myOrder.SetID("100A10");

            string encodedOrder = Order.EncodeOrder(myOrder);
            myDecodedOrder = Order.DecodeOrder(encodedOrder);

            Console.WriteLine(myDecodedOrder.GetID());

        }
    }
    public class Order
    {

        private static EncryptDecryptServiceRef.ServiceClient encryptDecryptService = new EncryptDecryptServiceRef.ServiceClient();
        private string SenderId;
        private int CardNo; // CC number
        private int Amount; // Number of Rooms to order


       
        public Order() 
        {
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
            String encodedOrder = encryptDecryptService.Encrypt(order.GetID());
            return encodedOrder;
        }
        
        public static Order DecodeOrder(string encodedOrder)
        {
            Order myOrder = new Order();
            myOrder.SetID(encryptDecryptService.Decrypt(encodedOrder));
            return myOrder;
        }
    }
}
