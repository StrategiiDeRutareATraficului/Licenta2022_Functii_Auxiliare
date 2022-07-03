using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger_Parser
{
    class Statistics
    {
        private double avrSpeed;
        private int nrVehicules;
        private double distance;
        private double totalTime;
        private double simulationTime;
        private double fuel;
        private double totalCo2Emission;
        private int globalPeakNrCars;
        public Statistics(double avrSpeed, int nrVehicules, double distance, double totalTime, double simulationTime, double fuel, double totalCo2Emission, int globalPeakNrCars)
        {
            this.avrSpeed = avrSpeed;
            this.nrVehicules = nrVehicules;
            this.distance = distance;
            this.totalTime = totalTime;
            this.simulationTime = simulationTime;
            this.fuel = fuel;
            this.totalCo2Emission = totalCo2Emission;
            this.globalPeakNrCars = globalPeakNrCars;
        }

        public Statistics()
        {
        }

        public double AvrSpeed { get => avrSpeed; set => avrSpeed = value; }
        public int NrVehicules { get => nrVehicules; set => nrVehicules = value; }
        public double Distance { get => distance; set => distance = value; }
        public double TotalTime { get => totalTime; set => totalTime = value; }
        public double SimulationTime { get => simulationTime; set => simulationTime = value; }
        public double Fuel { get => fuel; set => fuel = value; }
        public double TotalCo2Emission { get => totalCo2Emission; set => totalCo2Emission = value; }
        public int GlobalPeakNrCars { get => globalPeakNrCars; set => globalPeakNrCars = value; }

        public string toString()
        {
            string str = "";

            str = "Average Vehicule Speed (Km/h) = " + avrSpeed + ",\n"
                + "Total Traveled Distance (Km) = " + distance + ",\n"
                + "Total Traveled Time (hours) = " + totalTime + ",\n"
                + "Simulation Time (hours) = " + simulationTime + ",\n"
                + "Total Fuel Consumption (Kg) = " + fuel + ",\n"
                + "Total CO2 Emission (Kg) = " + totalCo2Emission + ",\n"
                + "Peak Number of Vehicles on Roads = " + globalPeakNrCars;

            return str;
        }
    }
}
