using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace PerformanceRecorder
{
    class PerformanceData
    {
        private Stopwatch stopWatch;
        private int travelAgengyID;
        private List<TimeSpan> elsapsedTime;
        public PerformanceData(int travelAgengyID ) {
            this.stopWatch =new Stopwatch() ;
            this.travelAgengyID = travelAgengyID;
            this.elsapsedTime = new List<TimeSpan>();
        }

        public Stopwatch getStopWatch() {
            
            return this.stopWatch;
        }

        public int getTravelAgencyID() {
            return this.travelAgengyID;
        }

        public void addTimeSpan(TimeSpan elsapsedTime) { 
            this.elsapsedTime.Add((elsapsedTime));
        }

        public double getAverageResponseTime(){
        
            int timesServed = elsapsedTime.Count;
            TimeSpan totalTime = new TimeSpan();
            foreach (TimeSpan ts in elsapsedTime){
                   totalTime = totalTime.Add(ts);
            }
            double average = totalTime.TotalMilliseconds / timesServed;

            return average;
        }

        public double getTotalTime()
        {
            TimeSpan totalTime = new TimeSpan();
            foreach (TimeSpan ts in elsapsedTime)
            {
                totalTime = totalTime.Add(ts);
            }
            double totalTimeMills = totalTime.TotalMilliseconds;

            return totalTimeMills;
        }
    }
}
