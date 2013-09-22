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
        private int bufferSize = 3;
        private static Semaphore _sem = new Semaphore(0,3);

        public void setOneCell(string encodedOrder)
        {
        }

        public string getOneCell()
        {
            return "";
        }
    }
}
