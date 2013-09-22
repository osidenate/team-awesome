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
        private bool[] isWritable = new bool[bufferSize];
        
        public void setOneCell(string encodedOrder)
        {
           // _sem.WaitOne();
            for (int i = 0; i < bufferSize; i++)
            {
                while (!isWritable[i])
                {
                    try
                    {
                        Monitor.Wait(this);
                    }
                    catch { }
                }
                multiCellBuffer[i] = encodedOrder;
                isWritable[i] = false;
                Monitor.PulseAll(this);
            }
            //_sem.Release();
        }

        public string getOneCell()
        {
            //_sem.WaitOne();
            string returnValue = "";
            for (int i = 0; i < bufferSize; i++)
            {
                while (isWritable[i])
                {
                    try
                    {
                        Monitor.Wait(this);
                    }
                    catch { }
                }
                returnValue = multiCellBuffer[i];
                isWritable[i] = true;
                Monitor.PulseAll(this);
            }
           // _sem.Release();
            return returnValue;
        }
    }
}
