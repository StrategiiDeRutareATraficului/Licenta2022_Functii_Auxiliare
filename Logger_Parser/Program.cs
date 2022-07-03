using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Collections;
using Logger_Parser;
using System.Reflection;
using System.Xml.Linq;
using DocumentFormat.OpenXml.ExtendedProperties;



namespace ConsoleApp1
{


    static class Program
    {

        public const int CONTAINER_SIZE = 5;
        public const int CONTAINER_NUMBER = 14;

        // Aceasta functie creeaza grafice in excel
        // http://csharp.net-informations.com/excel/csharp-excel-chart.htm
        // https://stackoverflow.com/questions/22777112/how-to-generate-a-chart-from-an-excel-sheet
        private static void CreateGraph(Microsoft.Office.Interop.Excel._Worksheet oSheet, string intervalStart, string intervalEnd, string title, string ytitle, string xtitle)
        {
            Microsoft.Office.Interop.Excel.Range chartRange;
            object misValue = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Excel.ChartObjects xlCharts = (Microsoft.Office.Interop.Excel.ChartObjects)oSheet.ChartObjects(Type.Missing);
            Microsoft.Office.Interop.Excel.ChartObject myChart = (Microsoft.Office.Interop.Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);

            Microsoft.Office.Interop.Excel.Chart chartPage = myChart.Chart;
            chartRange = oSheet.get_Range(intervalStart, intervalEnd);

            chartPage.SetSourceData(chartRange, misValue);
            chartPage.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered;
            chartPage.HasTitle = true;
            chartPage.ChartTitle.Text = title;

            var yAxis = (Microsoft.Office.Interop.Excel.Axis)chartPage.Axes(Microsoft.Office.Interop.Excel.XlAxisType.xlValue,
                Microsoft.Office.Interop.Excel.XlAxisGroup.xlPrimary);
            yAxis.HasTitle = true;
            yAxis.AxisTitle.Text = ytitle;

            var xAxis = (Microsoft.Office.Interop.Excel.Axis)chartPage.Axes(Microsoft.Office.Interop.Excel.XlAxisType.xlCategory,
                Microsoft.Office.Interop.Excel.XlAxisGroup.xlPrimary);
            xAxis.HasTitle = true;
            xAxis.AxisTitle.Text = xtitle;
        }

        // Calcularea imbunatatirii in procente
        // https://www.calculatorsoup.com/calculators/algebra/percentage-increase-calculator.php
        public static double PercentageGrowth(double startValue, double endValue)
        {
            return ((endValue - startValue) / startValue) * 100;
        }

        // Se creeaza foile de calcul pentru fiecare versiune de sumo
        public static void InsertData(List<int> containarsSpeed, List<double> containarsDistance, List<double> containarsTime, 
            Microsoft.Office.Interop.Excel._Worksheet oSheet, Statistics statistics)
        {
            oSheet.Cells[1, 1] = "Container Number";
            oSheet.Cells[2, 1] = "Nr of cars";
            oSheet.Cells[3, 1] = "Distance";
            oSheet.Cells[4, 1] = "Time";

            int column = 2;
            int row = 1;

            for (int i = 0; i < 14; i++)
            {
                row = 1;
                oSheet.Cells[row, column].Value = i;
                row = 2;
                oSheet.Cells[row, column].Value = containarsSpeed[i];
                row = 3;
                oSheet.Cells[row, column].Value = containarsDistance[i];
                row = 4;
                oSheet.Cells[row, column].Value = containarsTime[i];
                column++;
            }

            oSheet.Cells[6, 1] = "Average Vehicule Speed (km/h)";
            oSheet.Cells[6, 2] = statistics.AvrSpeed;
            oSheet.Cells[7, 1] = "NrVehicules";
            oSheet.Cells[7, 2] = statistics.NrVehicules;
            oSheet.Cells[8, 1] = "Total Traveled Distance (km)";
            oSheet.Cells[8, 2] = statistics.Distance;
            oSheet.Cells[9, 1] = "Total Traveled Time (hours)";
            oSheet.Cells[9, 2] = statistics.TotalTime;
            oSheet.Cells[10, 1] = "Simulation Time (hours)";
            oSheet.Cells[10, 2] = statistics.SimulationTime;
            oSheet.Cells[11, 1] = "Total Fuel Consumption (l)";
            oSheet.Cells[11, 2] = statistics.Fuel;
            oSheet.Cells[12, 1] = "Total CO2 Emission (kg)";
            oSheet.Cells[12, 2] = statistics.TotalCo2Emission;
            oSheet.Cells[13, 1] = "Peak Number of Vehicles on Roads";
            oSheet.Cells[13, 2] = statistics.GlobalPeakNrCars;
            

            CreateGraph(oSheet, "B2", "O2", "Average Speed (km/h)", "Number of Vehicles", "Speed Clusters (km/h)");
            CreateGraph(oSheet, "B3", "O3", "Total Traveled Distance (km)", "Distance (km)", "Speed Clusters (km/h)");
            CreateGraph(oSheet, "B4", "O4", "Total Traveled Time (hours)", "Time (hours)", "Speed Clusters (km/h)");
        }

        // Aceasta functie creeaza fisierul cu rezultate
        private static void CreateExcel(string excelLocation, Statistics CStatistics, 
            Statistics N4Statistics, Statistics N5Statistics, Statistics N6Statistics,
            IDictionary<int, TimeStep> CSimulationData, IDictionary<int, TimeStep> N4SimulationData, 
            IDictionary<int, TimeStep> N5SimulationData, IDictionary<int, TimeStep> N6SimulationData)
        {

            List<int> cContainarsSpeed = new List<int>();
            List<double> cContainarsDistance = new List<double>();
            List<double> cContainarsTime = new List<double>();
            List<int> n4ContainarsSpeed = new List<int>();
            List<double> n4ContainarsDistance = new List<double>();
            List<double> n4ContainarsTime = new List<double>();
            List<int> n5ContainarsSpeed = new List<int>();
            List<double> n5ContainarsDistance = new List<double>();
            List<double> n5ContainarsTime = new List<double>();
            List<int> n6ContainarsSpeed = new List<int>();
            List<double> n6ContainarsDistance = new List<double>();
            List<double> n6ContainarsTime = new List<double>();


            CreateContainers(CONTAINER_NUMBER, CONTAINER_SIZE, CSimulationData,
                cContainarsSpeed, cContainarsDistance, cContainarsTime);
            CreateContainers(CONTAINER_NUMBER, CONTAINER_SIZE, N4SimulationData,
                n4ContainarsSpeed, n4ContainarsDistance, n4ContainarsTime);
            CreateContainers(CONTAINER_NUMBER, CONTAINER_SIZE, N5SimulationData,
                n5ContainarsSpeed, n5ContainarsDistance, n5ContainarsTime);
            CreateContainers(CONTAINER_NUMBER, CONTAINER_SIZE, N6SimulationData,
                n6ContainarsSpeed, n6ContainarsDistance, n6ContainarsTime);

            Microsoft.Office.Interop.Excel.Application oXL = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
            Microsoft.Office.Interop.Excel._Worksheet oSheetC = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            Microsoft.Office.Interop.Excel._Worksheet oSheetN4 = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            Microsoft.Office.Interop.Excel._Worksheet oSheetN5 = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            Microsoft.Office.Interop.Excel._Worksheet oSheetN6 = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            Microsoft.Office.Interop.Excel._Worksheet oSheetComp = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            Microsoft.Office.Interop.Excel.Range oRng;

            object misvalue = System.Reflection.Missing.Value;
            object missing = Type.Missing;

            try
            {
                //Deschide Excel si obtine Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;

                //Obtine un nou workbook.
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));

                oSheetC = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
                oSheetN4 = (Microsoft.Office.Interop.Excel._Worksheet)oWB.Sheets.Add(missing, missing, 1, missing);
                oSheetN5 = (Microsoft.Office.Interop.Excel._Worksheet)oWB.Sheets.Add(missing, missing, 1, missing);
                oSheetN6 = (Microsoft.Office.Interop.Excel._Worksheet)oWB.Sheets.Add(missing, missing, 1, missing);
                oSheetComp = (Microsoft.Office.Interop.Excel._Worksheet)oWB.Sheets.Add(missing, missing, 2, missing);

                Parallel.Invoke(
                        () => InsertData(cContainarsSpeed, cContainarsDistance, cContainarsTime, oSheetC, CStatistics),
                        () => InsertData(n4ContainarsSpeed, n4ContainarsDistance, n4ContainarsTime, oSheetN4, N4Statistics),
                        () => InsertData(n5ContainarsSpeed, n5ContainarsDistance, n5ContainarsTime, oSheetN5, N5Statistics),
                        () => InsertData(n6ContainarsSpeed, n6ContainarsDistance, n6ContainarsTime, oSheetN6, N6Statistics)
                        );

                oSheetComp.Cells[1, 1] = "Number of Vehicles = " + N4Statistics.NrVehicules;
                oSheetComp.Cells[1, 2] = "Default SUMO";
                oSheetComp.Cells[1, 3] = "CA SUMO 4";
                oSheetComp.Cells[1, 4] = "Improvement %";
                oSheetComp.Cells[2, 1] = "Average Vehicule Speed (km/h)";
                oSheetComp.Cells[3, 1] = "Total Traveled Distance (km)";
                oSheetComp.Cells[4, 1] = "Total Traveled Time (hours)";
                oSheetComp.Cells[5, 1] = "Simulation Time (hours)";
                oSheetComp.Cells[6, 1] = "Total Fuel Consumption (l)";
                oSheetComp.Cells[7, 1] = "Total CO2 Emission (kg)";
                oSheetComp.Cells[8, 1] = "Peak Number of Vehicles on Roads";

                oSheetComp.Cells[2, 2] = Math.Round((Double)CStatistics.AvrSpeed, 2);
                oSheetComp.Cells[3, 2] = Math.Round((Double)CStatistics.Distance, 2);
                oSheetComp.Cells[4, 2] = Math.Round((Double)CStatistics.TotalTime, 2);
                oSheetComp.Cells[5, 2] = Math.Round((Double)CStatistics.SimulationTime, 2);
                oSheetComp.Cells[6, 2] = Math.Round((Double)CStatistics.Fuel, 2);
                oSheetComp.Cells[7, 2] = Math.Round((Double)CStatistics.TotalCo2Emission, 2);
                oSheetComp.Cells[8, 2] = Math.Round((Double)CStatistics.GlobalPeakNrCars, 2);

                oSheetComp.Cells[2, 3] = Math.Round((Double)N4Statistics.AvrSpeed, 2);
                oSheetComp.Cells[3, 3] = Math.Round((Double)N4Statistics.Distance, 2);
                oSheetComp.Cells[4, 3] = Math.Round((Double)N4Statistics.TotalTime, 2);
                oSheetComp.Cells[5, 3] = Math.Round((Double)N4Statistics.SimulationTime, 2);
                oSheetComp.Cells[6, 3] = Math.Round((Double)N4Statistics.Fuel, 2);
                oSheetComp.Cells[7, 3] = Math.Round((Double)N4Statistics.TotalCo2Emission, 2);
                oSheetComp.Cells[8, 3] = Math.Round((Double)N4Statistics.GlobalPeakNrCars, 2);

                oSheetComp.Cells[2, 4] = Math.Round((Double)PercentageGrowth(CStatistics.AvrSpeed, N4Statistics.AvrSpeed), 2);
                oSheetComp.Cells[3, 4] = Math.Round((Double)PercentageGrowth(CStatistics.Distance, N4Statistics.Distance), 2);
                oSheetComp.Cells[4, 4] = Math.Round((Double)PercentageGrowth(CStatistics.TotalTime, N4Statistics.TotalTime), 2);
                oSheetComp.Cells[5, 4] = Math.Round((Double)PercentageGrowth(CStatistics.SimulationTime, N4Statistics.SimulationTime), 2);
                oSheetComp.Cells[6, 4] = Math.Round((Double)PercentageGrowth(CStatistics.Fuel, N4Statistics.Fuel), 2);
                oSheetComp.Cells[7, 4] = Math.Round((Double)PercentageGrowth(CStatistics.TotalCo2Emission, N4Statistics.TotalCo2Emission), 2);
                oSheetComp.Cells[8, 4] = Math.Round((Double)PercentageGrowth(CStatistics.GlobalPeakNrCars, N4Statistics.GlobalPeakNrCars), 2);

                oSheetComp.Cells[1, 6] = "Number of Vehicles = " + N5Statistics.NrVehicules;
                oSheetComp.Cells[1, 7] = "Default SUMO";
                oSheetComp.Cells[1, 8] = "CA SUMO 5";
                oSheetComp.Cells[1, 9] = "Improvement %";
                oSheetComp.Cells[2, 6] = "Average Vehicule Speed (km/h)";
                oSheetComp.Cells[3, 6] = "Total Traveled Distance (km)";
                oSheetComp.Cells[4, 6] = "Total Traveled Time (hours)";
                oSheetComp.Cells[5, 6] = "Simulation Time (hours)";
                oSheetComp.Cells[6, 6] = "Total Fuel Consumption (l)";
                oSheetComp.Cells[7, 6] = "Total CO2 Emission (kg)";
                oSheetComp.Cells[8, 6] = "Peak Number of Vehicles on Roads";

                oSheetComp.Cells[2, 7] = Math.Round((Double)CStatistics.AvrSpeed, 2);
                oSheetComp.Cells[3, 7] = Math.Round((Double)CStatistics.Distance, 2);
                oSheetComp.Cells[4, 7] = Math.Round((Double)CStatistics.TotalTime, 2);
                oSheetComp.Cells[5, 7] = Math.Round((Double)CStatistics.SimulationTime, 2);
                oSheetComp.Cells[6, 7] = Math.Round((Double)CStatistics.Fuel, 2);
                oSheetComp.Cells[7, 7] = Math.Round((Double)CStatistics.TotalCo2Emission, 2);
                oSheetComp.Cells[8, 7] = Math.Round((Double)CStatistics.GlobalPeakNrCars, 2);

                oSheetComp.Cells[2, 8] = Math.Round((Double)N5Statistics.AvrSpeed, 2);
                oSheetComp.Cells[3, 8] = Math.Round((Double)N5Statistics.Distance, 2);
                oSheetComp.Cells[4, 8] = Math.Round((Double)N5Statistics.TotalTime, 2);
                oSheetComp.Cells[5, 8] = Math.Round((Double)N5Statistics.SimulationTime, 2);
                oSheetComp.Cells[6, 8] = Math.Round((Double)N5Statistics.Fuel, 2);
                oSheetComp.Cells[7, 8] = Math.Round((Double)N5Statistics.TotalCo2Emission, 2);
                oSheetComp.Cells[8, 8] = Math.Round((Double)N5Statistics.GlobalPeakNrCars, 2);

                oSheetComp.Cells[2, 9] = Math.Round((Double)PercentageGrowth(CStatistics.AvrSpeed, N5Statistics.AvrSpeed), 2);
                oSheetComp.Cells[3, 9] = Math.Round((Double)PercentageGrowth(CStatistics.Distance, N5Statistics.Distance), 2);
                oSheetComp.Cells[4, 9] = Math.Round((Double)PercentageGrowth(CStatistics.TotalTime, N5Statistics.TotalTime), 2);
                oSheetComp.Cells[5, 9] = Math.Round((Double)PercentageGrowth(CStatistics.SimulationTime, N5Statistics.SimulationTime), 2);
                oSheetComp.Cells[6, 9] = Math.Round((Double)PercentageGrowth(CStatistics.Fuel, N5Statistics.Fuel), 2);
                oSheetComp.Cells[7, 9] = Math.Round((Double)PercentageGrowth(CStatistics.TotalCo2Emission, N5Statistics.TotalCo2Emission), 2);
                oSheetComp.Cells[8, 9] = Math.Round((Double)PercentageGrowth(CStatistics.GlobalPeakNrCars, N5Statistics.GlobalPeakNrCars), 2);

                oSheetComp.Cells[10, 1] = "Number of Vehicles = " + N6Statistics.NrVehicules;
                oSheetComp.Cells[10, 2] = "Default SUMO";
                oSheetComp.Cells[10, 3] = "CA SUMO 6";
                oSheetComp.Cells[10, 4] = "Improvement %";
                oSheetComp.Cells[11, 1] = "Average Vehicule Speed (km/h)";
                oSheetComp.Cells[12, 1] = "Total Traveled Distance (km)";
                oSheetComp.Cells[13, 1] = "Total Traveled Time (hours)";
                oSheetComp.Cells[14, 1] = "Simulation Time (hours)";
                oSheetComp.Cells[15, 1] = "Total Fuel Consumption (l)";
                oSheetComp.Cells[16, 1] = "Total CO2 Emission (kg)";
                oSheetComp.Cells[17, 1] = "Peak Number of Vehicles on Roads";

                oSheetComp.Cells[11, 2] = Math.Round((Double)CStatistics.AvrSpeed, 2);
                oSheetComp.Cells[12, 2] = Math.Round((Double)CStatistics.Distance, 2);
                oSheetComp.Cells[13, 2] = Math.Round((Double)CStatistics.TotalTime, 2);
                oSheetComp.Cells[14, 2] = Math.Round((Double)CStatistics.SimulationTime, 2);
                oSheetComp.Cells[15, 2] = Math.Round((Double)CStatistics.Fuel, 2);
                oSheetComp.Cells[16, 2] = Math.Round((Double)CStatistics.TotalCo2Emission, 2);
                oSheetComp.Cells[17, 2] = Math.Round((Double)CStatistics.GlobalPeakNrCars, 2);

                oSheetComp.Cells[11, 3] = Math.Round((Double)N6Statistics.AvrSpeed, 2);
                oSheetComp.Cells[12, 3] = Math.Round((Double)N6Statistics.Distance, 2);
                oSheetComp.Cells[13, 3] = Math.Round((Double)N6Statistics.TotalTime, 2);
                oSheetComp.Cells[14, 3] = Math.Round((Double)N6Statistics.SimulationTime, 2);
                oSheetComp.Cells[15, 3] = Math.Round((Double)N6Statistics.Fuel, 2);
                oSheetComp.Cells[16, 3] = Math.Round((Double)N6Statistics.TotalCo2Emission, 2);
                oSheetComp.Cells[17, 3] = Math.Round((Double)N6Statistics.GlobalPeakNrCars, 2);

                oSheetComp.Cells[11, 4] = Math.Round((Double)PercentageGrowth(CStatistics.AvrSpeed, N6Statistics.AvrSpeed), 2);
                oSheetComp.Cells[12, 4] = Math.Round((Double)PercentageGrowth(CStatistics.Distance, N6Statistics.Distance), 2);
                oSheetComp.Cells[13, 4] = Math.Round((Double)PercentageGrowth(CStatistics.TotalTime, N6Statistics.TotalTime), 2);
                oSheetComp.Cells[14, 4] = Math.Round((Double)PercentageGrowth(CStatistics.SimulationTime, N6Statistics.SimulationTime), 2);
                oSheetComp.Cells[15, 4] = Math.Round((Double)PercentageGrowth(CStatistics.Fuel, N6Statistics.Fuel), 2);
                oSheetComp.Cells[16, 4] = Math.Round((Double)PercentageGrowth(CStatistics.TotalCo2Emission, N6Statistics.TotalCo2Emission), 2);
                oSheetComp.Cells[17, 4] = Math.Round((Double)PercentageGrowth(CStatistics.GlobalPeakNrCars, N6Statistics.GlobalPeakNrCars), 2);

                oXL.Visible = false;
                oXL.UserControl = false;
                oWB.SaveAs(excelLocation, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                oWB.Close(Type.Missing, Type.Missing, Type.Missing);
                oXL.Quit();
            }
            catch
            {
                Console.WriteLine("Error");
            }

        }

        // Se calculeaza datele care sunt folosite pentru a crea graficele
        private static void CreateContainers(int containerNumber,int containerSize, IDictionary<int, TimeStep> SimulationData, 
            List<int> containarsSpeed, List<double> containarsDistance, List<double> containarsTime)
        {
            
            for (int i = 0; i < containerNumber; i++)
            {
                containarsSpeed.Add(0);
            }
            for (int i = 0; i < containerNumber; i++)
            {
                containarsDistance.Add(0);
            }
            for (int i = 0; i < containerNumber; i++)
            {
                containarsTime.Add(0);
            }
            
            // Parcurgem datele rezultate din urma procesarii log-urilor 
            foreach (KeyValuePair<int, TimeStep> entry in SimulationData)
            {
                int key = entry.Key;
                TimeStep value = entry.Value;

                if (SimulationData[key].getDistance() > 0)
                {
                    double containrNr = SimulationData[key].averageSpeed();
                    containrNr = ((int)containrNr) / containerSize;

                    if (containrNr > 12)
                    {
                        containrNr = 13;
                    }

                    // Numarul de vehicule din container-ul de viteza medie.
                    containarsSpeed[(int)containrNr]++; 
                    // Distanta totala parcursa de masinile din fiecare container
                    containarsDistance[(int)containrNr] += SimulationData[key].getDistance();
                    // Timpul total petrecut in trafic de masinile din fiecare container
                    containarsTime[(int)containrNr] += SimulationData[key].getTime();
                }
            }
        }

        // In aceasta functie extragem datele statistice din log-ere
        private static Statistics CalculateStatistics(IDictionary<int, TimeStep> simulationData, IDictionary<int, Double> carDistance, int globalPeakNrCars)
        {
            int nr_v = 0;
            double total_time = 0.0;
            double distance = 0.0;
            int errors = 0;
            double simulationTime = -1.0;
            double totalConsumtion = 0.0;
            double totalCo2Emission = 0.0;

            // Parcurgem datele tuturor masinilor si calculam valorile totale
            foreach (KeyValuePair<int, TimeStep> entry in simulationData)
            {
                int key = entry.Key;
                TimeStep value = entry.Value;
                // Ne asiguram ca toate masinile au reusit sa fie simulate
                if (carDistance.ContainsKey(key))
                {
                    nr_v++;
                    value.setDistance(carDistance[key] / 1000);
                    value.setTime(value.getTime() / 3600);
                    distance += value.getDistance();
                    total_time += value.getTime();
                    totalConsumtion += value.FuelConsumtion;
                    totalCo2Emission += value.Co2Emission;
                    if (simulationTime < value.LastTimeStep)
                    {
                        simulationTime = value.LastTimeStep;
                    }
                }
                else
                {
                    errors++;
                    simulationData[key].setDistance(-1);
                }

            }
            return new Statistics((distance / total_time), nr_v, distance, total_time, simulationTime / 3600, totalConsumtion / 1000, totalCo2Emission / 1000000, globalPeakNrCars);
        }

        // Extragem distantele parcurse din fisierul generat in timpul crearii rutelor
        static public void GetRealDistance(string length, IDictionary<int, Double> carDistance)
        {
            String id = "0";
            double realDistance = 0.0;

            string[] lines = File.ReadAllLines(length);

            foreach (string line in lines)
            {
                string[] localLine = line.Split(':');
                carDistance[Int32.Parse(localLine[0])] = double.Parse(localLine[1].Replace('.', ','));
            }
        }

        // In aceasta functie parcurgem fisierul cu log-uri si extragem datele din el.
        static public Statistics Route_Parser(string loggs, string length,
            IDictionary<int, TimeStep> simulationData, IDictionary<int, Double> carDistance)
        {
            XmlTextReader reader = null;
            string timestep = "0.0";
            string edge;
            string lane;
            int id;
            double fuel;
            double co2Emission;
            int peakNrCars = 0;
            int globalPeakNrCars = 0;
            Statistics statistics = new Statistics();
            try
            {
                // Incarcam datele din fisier
                reader = new XmlTextReader(loggs);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            // Cautam cuvinte cheie in fisierul de tipul xml. Procesam doar acele noduri care sunt de interes
                            switch (reader.Name)
                            {
                                
                                case "timestep":
                                    // Actualizam timpul curent 
                                    timestep = reader.GetAttribute(0);
                                    // Cautam numarul maxim de masini prezente in simulare la fiecare secunda
                                    if(peakNrCars > globalPeakNrCars)
                                    {
                                        globalPeakNrCars = peakNrCars;
                                    }
                                    peakNrCars = 0;
                                    break;

                                case "vehicle":
                                    peakNrCars++;
                                    id = int.Parse(reader.GetAttribute(0).Replace('.', ','));
                                    fuel = double.Parse(reader.GetAttribute(7).Replace('.', ','));
                                    co2Emission = double.Parse(reader.GetAttribute(2).Replace('.', ','));

                                    TimeStep newTimeStep = new TimeStep(1, double.Parse(timestep.Replace('.', ',')), fuel, co2Emission);
                                    // Verificam daca masina curent a mai aparut inainte
                                    if (simulationData.ContainsKey(id))
                                    {
                                        // Daca masina exista deja atunci actualizam valorile
                                        TimeStep oldTimeStep = simulationData[id];
                                        newTimeStep.setTime(newTimeStep.getTime() + oldTimeStep.getTime());
                                        newTimeStep.FuelConsumtion = oldTimeStep.FuelConsumtion + newTimeStep.FuelConsumtion;
                                        newTimeStep.Co2Emission = oldTimeStep.Co2Emission + newTimeStep.Co2Emission;
                                    }
                                    // Salvam informatiile despre masina
                                    simulationData[id] = newTimeStep;

                                    break;

                            }
                            break;
                    }
                }
            }
            finally
            {
                // Verificam daca am avut erori
                if (reader != null)
                {
                    // Daca nu am avut erori inchidem fisierul de log-ere si procesam informatiile extrase
                    reader.Close();
                    GetRealDistance(length, carDistance);
                    statistics = CalculateStatistics(simulationData, carDistance, globalPeakNrCars);
                }
                else
                {
                }
            }

            return statistics;
        }

        // Cu aceasta functie putem sterge toate semafoarele din harta
        static public void Map_Parser_Delete(string mapLocation)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(mapLocation);
            foreach (XmlNode xNode in xDoc.SelectNodes("net/tlLogic"))
            {
                xNode.RemoveAll();
            }
            xDoc.Save(mapLocation);
        }

        // Cu aceasta functie modificam programul semafoarelor
        static public void Map_Parser_Edit(string mapLocation)
        {
            XDocument xmlFile = XDocument.Load(mapLocation);

            var query = from c in xmlFile.Elements("net").Elements("tlLogic").Elements("phase")
                        select c;

            foreach (XElement book in query)
            {
                String s = book.Attribute("state").Value;
                String rez = "";
                for (int i = 0; i < s.Length; i++)
                {
                    rez += "G";
                }
                book.Attribute("state").Value = rez;
            }

            xmlFile.Save(mapLocation);
        }

        // Cu aceasta functie modificam timpul de aparitie al masinilor. Acestea aparand in primele 10000 de secunde
        static public void Trips_Modify(string tripsPath, int numberOfCars, int timeSpan)
        {
            List<int> depart_times = new List<int>();

            Random rand = new Random();

            for (int i = 0; i < numberOfCars; i++)
            {
                depart_times.Add(rand.Next() % timeSpan);
            }

            depart_times.Sort();

            XDocument xmlFile = XDocument.Load(tripsPath);

            var query = from c in xmlFile.Elements("routes").Elements("trip")
                        select c;

            int j = 0;

            foreach (XElement book in query)
            {
                book.Attribute("depart").Value = depart_times[j].ToString() + ".00";
                j++;
            }

            xmlFile.Save(tripsPath);

        }

        // Cu aceasta functie putem genera rute manual
        static public void Generate_Routes(string fixedTripsPath, List<string> edgeIdStart, string edgeIdDestination)
        {
            List<string> route = new List<string>();

            route.Add("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n <!-- generated on 2021-12-06 19:16:31.696661 by randomTrips.py v1_11_0+0348-ecb31501c71\n options: -n NewYork.net.xml -r NewYork.rou.xml -e 1000 -l\n -->\n <routes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://sumo.dlr.de/xsd/routes_file.xsd\">\n");

            int edgeNumber = edgeIdStart.Count();

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < edgeNumber; j++)
                {
                    route.Add("<trip id=\"" + (i + (j * 1000)) + "\" depart=\"" + i * 1 + ".00\" from=\"" + edgeIdStart[j] + "\" to=\"" + edgeIdDestination + "\"/>\n");
                }
            }

            route.Add("</routes>");

            try
            {
                using (FileStream fs = File.Create(fixedTripsPath))
                {
                    foreach (string line in route)
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(line);
                        fs.Write(info, 0, info.Length);
                    }
                }

                using (StreamReader sr = File.OpenText(fixedTripsPath))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Main(string[] args)
        {

            const String folderName = "Barcelona\\100K";
            const String folderName2 = "Barcelona_Corect";
            const String CLoggs = "F:\\Licenta\\" + folderName + "\\C\\loggs.xml";
            const String N4Loggs = "F:\\Licenta\\" + folderName + "\\N1_Corect\\loggs.xml";
            const String N5Loggs = "F:\\Licenta\\" + folderName + "\\N\\loggs.xml";
            const String N6Loggs = "F:\\Licenta\\" + folderName + "\\N6\\loggs.xml";
            const String CLength = "F:\\Licenta\\" + folderName + "\\C\\length.txt";
            const String N4Length = "F:\\Licenta\\" + folderName + "\\N1_Corect\\length.txt";
            const String N5Length = "F:\\Licenta\\" + folderName + "\\N\\length.txt";
            const String N6Length = "F:\\Licenta\\" + folderName + "\\N\\length.txt";

            const String excelLocation = "F:\\Licenta\\" + folderName + "\\" + folderName2 + ".xls";
            const String mapLocation = "F:\\Licenta\\NewYork\\50K\\TLS_OFF\\C\\NewYork.net.xml";
            const String tripsPath = "F:\\Licenta\\Sumo\\cmake-build\\src\\duarouter\\trips.trips.xml";
            const String FixedTripsPath = "F:\\Licenta\\Sumo\\cmake-build\\src\\duarouter\\trips.trips.xml";

            string edgeIdDestination = "172219128#2";
            List<string> edgeIdStart = new List<string>();

            edgeIdStart.Add("-29548010");
            edgeIdStart.Add("-293394055");
            edgeIdStart.Add("-195418555");
            edgeIdStart.Add("-256968789#0");
            edgeIdStart.Add("195430959#2");
            edgeIdStart.Add("-195430959#0");

            int numberOfCars = 32000;
            int timeSpan = 10000;

            IDictionary<int, TimeStep> CSimulationData = new Dictionary<int, TimeStep>();
            IDictionary<int, TimeStep> N4SimulationData = new Dictionary<int, TimeStep>();
            IDictionary<int, TimeStep> N5SimulationData = new Dictionary<int, TimeStep>();
            IDictionary<int, TimeStep> N6SimulationData = new Dictionary<int, TimeStep>();
            IDictionary<int, Double> CCarDistance = new Dictionary<int, Double>();
            IDictionary<int, Double> N4CarDistance = new Dictionary<int, Double>();
            IDictionary<int, Double> N5CarDistance = new Dictionary<int, Double>();
            IDictionary<int, Double> N6CarDistance = new Dictionary<int, Double>();

            Statistics CStatistics = new Statistics();
            Statistics N4Statistics = new Statistics();
            Statistics N5Statistics = new Statistics();
            Statistics N6Statistics = new Statistics();

            int operation = 1;

            if (operation == 1) // parsare logg-uri
            {
                Parallel.Invoke(
                        () => CStatistics = Route_Parser(CLoggs, CLength, CSimulationData, CCarDistance),
                        () => N4Statistics = Route_Parser(N4Loggs, N4Length, N4SimulationData, N4CarDistance),
                        () => N5Statistics = Route_Parser(N5Loggs, N5Length, N5SimulationData, N5CarDistance),
                        () => N6Statistics = Route_Parser(N6Loggs, N6Length, N6SimulationData, N6CarDistance)
                        );
                CreateExcel(excelLocation, CStatistics, N4Statistics, N5Statistics, N6Statistics, CSimulationData, N4SimulationData, N5SimulationData, N6SimulationData);
            }
            else if (operation == 2) // Stergere elemente din harta
            {
                Map_Parser_Delete(mapLocation);
            }
            else if (operation == 3) // Modificare elemente din harta
            {
                Map_Parser_Edit(mapLocation);
            }
            else if (operation == 4) // Modificare trips
            {
                Trips_Modify(tripsPath, numberOfCars, timeSpan);
            }
            else if (operation == 5) // Generare rute
            {
                Generate_Routes(FixedTripsPath, edgeIdStart, edgeIdDestination);
            }
        }


    }
}
