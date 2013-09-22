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
            myOrder.SetCardNo(2000000000);
            myOrder.SetAmt(100);

            string encodedOrder = Order.EncodeOrder(myOrder);
            myDecodedOrder = Order.DecodeOrder(encodedOrder);

            Console.WriteLine(myDecodedOrder.GetID());
            Console.WriteLine(myDecodedOrder.GetCardNo());
            Console.WriteLine(myDecodedOrder.GetAmt());

            OrderProcessor op = new OrderProcessor();
            try
            {
                //This function will throw and exception if the card number is not vaild
                op.process(myDecodedOrder, 300.00,.80,1.00);
            }
            catch (ArgumentOutOfRangeException e) {

                Console.WriteLine(e.Message);
            }
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
