using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger_Parser
{
    class TimeStep
    {
        private double time;
        private double fuelConsumtion;
        private double co2Emission;
        private double distance;
        private double lastTimeStep;

        public double LastTimeStep { get => lastTimeStep; set => lastTimeStep = value; }

        public double FuelConsumtion { get => fuelConsumtion; set => fuelConsumtion = value; }
        public double Co2Emission { get => co2Emission; set => co2Emission = value; }

        public TimeStep(double time, double lastTimeStep, double fuelConsumtion, double co2Emission)
        {
            this.time = time;
            this.LastTimeStep = lastTimeStep;
            this.FuelConsumtion = fuelConsumtion;
            this.Co2Emission = co2Emission;
        }

        public double getTime()
        {
            return time;
        }

        public void setTime(double time)
        {
            this.time = time;
        }

        public double getDistance()
        {
            return distance;
        }

        public void setDistance(double distance)
        {
            this.distance = distance;
        }

        public double averageSpeed()
        {
            return distance / time;
        }
    }
}
