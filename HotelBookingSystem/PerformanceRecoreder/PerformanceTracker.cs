using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace PerformanceRecorder
{
    class TestTracked
    {
        private PerformanceTracker tracker; 
        public TestTracked(PerformanceTracker tracker){
            this.tracker = tracker;
        }

        public PerformanceTracker gettracker() { 
            return this.tracker;
        }
    }
    class PerformanceTracker
    {

        private Dictionary<int,PerformanceData> travalAgencies;

        static void Main(string[] args)
        {
            PerformanceTracker tracker = new PerformanceTracker();
            TestTracked[] testTracked = new TestTracked[4];
            for(int i=0;i<4;i++){
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

        public PerformanceTracker() {
            travalAgencies = new Dictionary<int,PerformanceData>();
        }

        public void addTravelAgency(int travelAgencyID){
            PerformanceData pd = new PerformanceData(travelAgencyID);
            travalAgencies.Add(travelAgencyID, pd);
        }

        public void startClock(int travelAgencyID) {
            PerformanceData pd;
            if (travalAgencies.TryGetValue(travelAgencyID, out pd))
            {
                pd.getStopWatch().Start();
            }
            else {
                throw new ArgumentException();
            }
        }

        public void stopClock(int travelAgencyID)
        {
            PerformanceData pd;
            if (travalAgencies.TryGetValue(travelAgencyID, out pd))
            {
               Stopwatch sw = pd.getStopWatch();
               sw.Stop();
                pd.addTimeSpan(sw.Elapsed);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void printPerfromancnceData() {
            PerformanceData pd;
            Console.WriteLine("{0,-10}\t {1,-11}\t {2,-11}", "Travel Agency ID","Average Response Time","Total Runtime");
                
            foreach (KeyValuePair<int, PerformanceData> entry in travalAgencies)
            {
                pd= entry.Value;
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.WriteLine("{0,-20}\t {1,-21}\t {2,-21}", pd.getTravelAgencyID(), pd.getAverageResponseTime(), pd.getTotalTime());
            }
        
        }
    }
}
