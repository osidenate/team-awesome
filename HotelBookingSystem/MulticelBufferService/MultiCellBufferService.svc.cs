using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace MulticelBufferService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MulticelBufferService" in code, svc and config file together.
    public class MulticelBufferService : IMultiCellBufferService
    {
        const int bufferSize = 3;
        private static Semaphore _sem = new Semaphore(0,3);
        private string[] multiCellBuffer = new string[bufferSize];

        public void setOneCell(string encodedOrder)
        {
            for (int i = 0; i < bufferSize; i++)
            {
                Monitor.Enter(multiCellBuffer[i]);
                try
                {
                    if (multiCellBuffer[i].Length == 0)
                        multiCellBuffer[i] = encodedOrder;
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    Monitor.Exit(multiCellBuffer[i]);
                }

                //_sem.Release(3);
            }

        }

        public string getOneCell()
        {
            return "";
        }
    }
}
