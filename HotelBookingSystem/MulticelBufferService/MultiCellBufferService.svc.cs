﻿using System;
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
        private static Semaphore _sem = new Semaphore(3,3);
        private static Semaphore _sem2 = new Semaphore(0, 3);
        private static string[] multiCellBuffer = new string[bufferSize];
        private static bool[] isOccupied = {true,true,true};
        
        public void setOneCell(string encodedOrder)
        {
            _sem.WaitOne();
            lock (multiCellBuffer)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    if (isOccupied[i])
                    {
                        multiCellBuffer[i] = encodedOrder;
                        isOccupied[i] = false;
                        _sem2.Release();
                        break;
                    }
                }
            }
        }

        public string getOneCell()
        {
            _sem2.WaitOne();
            string returnValue = "";
            lock (multiCellBuffer)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    if (!isOccupied[i])
                    {
                        returnValue = multiCellBuffer[i].ToString();
                        isOccupied[i] = true;            
                        _sem.Release();
                        break;
                    }
                }
            }

            return returnValue;
        }
    }
}