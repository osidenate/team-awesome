using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace PerformanceRecorder
{
    public class TestTracked
    {
        private PerformanceTracker tracker;
        public TestTracked(PerformanceTracker tracker)
        {
            this.tracker = tracker;
        }

        public PerformanceTracker gettracker()
        {
            return this.tracker;
        }
    }
    public class PerformanceTracker
    {

        private Dictionary<int, PerformanceData> travalAgencies = new Dictionary<int, PerformanceData>();

        static void Main(string[] args)
        {
            PerformanceTracker tracker = new PerformanceTracker();
            TestTracked[] testTracked = new TestTracked[4];
            for (int i = 0; i < 4; i++)
            {
                testTracked[i] = new TestTracked(tracker);
                tracker.addTravelAgency(i);
            }
            for (int i = 0; i < 4; i++)
            {
                testTracked[i].gettracker().startClock(i);
            }
            System.Threading.Thread.Sleep(5000);
            for (int i = 0; i < 4; i++)
            {
                testTracked[i].gettracker().stopClock(i);
            }
            for (int i = 0; i < 4; i++)
            {
                testTracked[i].gettracker().startClock(i);
            }
            System.Threading.Thread.Sleep(5000);
            for (int i = 0; i < 4; i++)
            {
                testTracked[i].gettracker().stopClock(i);
            }

            tracker.printPerfromancnceData();
        }

        public PerformanceTracker()
        {
            //travalAgencies = new Dictionary<int,PerformanceData>();
        }

        public void addTravelAgency(int travelAgencyID)
        {
            PerformanceData pd = new PerformanceData(travelAgencyID);
            lock (travalAgencies)
            {
                travalAgencies.Add(travelAgencyID, pd);
            }
        }

        /*Oder ID is the running total of the orders that are out. This is a way to identify which stopwwatch is getting started. 
        Without this if a travel agengcy start the clock and before the order i processed the start another order,
        they will over ride the previous stop watch time
        Order number EX:
         * First order processed orderID = 0;
         * Second order processed orderID = 1;
      
          */
        public void startClock(int travelAgencyID)
        {
            PerformanceData pd;
            if (travalAgencies.TryGetValue(travelAgencyID, out pd))
            {
                pd.getStopWatch().Start();
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void stopClock(int travelAgencyID)
        {
            PerformanceData pd;
            if (travalAgencies.TryGetValue(travelAgencyID, out pd))
            {
                pd.addTimeSpan(pd.getStopWatch().Elapsed);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void printPerfromancnceData()
        {
            PerformanceData pd;
            Console.WriteLine("{0,-10}\t {1,-11}\t {2,-11}", "Travel Agency ID", "Average Response Time", "Total Runtime");

            foreach (KeyValuePair<int, PerformanceData> entry in travalAgencies)
            {
                pd = entry.Value;
                pd.getStopWatch().Stop();
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.WriteLine("{0,-20}\t {1,-21}\t {2,-21}", pd.getTravelAgencyID(), pd.getAverageResponseTime(), pd.getTotalTime());
            }

        }
    }
}