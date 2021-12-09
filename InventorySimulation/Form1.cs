using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryTesting;
using InventoryModels;
using System.IO;

namespace InventorySimulation
{
    public partial class Form1 : Form
    {
        public string[] lines = File.ReadAllLines(@"TestCase1.txt");
        public SimulationSystem SimSys = new SimulationSystem();
        public SimulationCase sc = new SimulationCase();
        public List<int> DaysUntilOrderArrives = new List<int>();
        decimal DemDistCummCounter = 0;
        Random rand = new Random();
        int DayWithinCycle = 1, DayCounter = 0, OrderQ = 0;

        public Form1()
        {
            InitializeComponent();
            for(int i = 0;i < lines.Length;i++)
            {
                if(lines[i] == "OrderUpTo")
                    SimSys.OrderUpTo = int.Parse(lines[i + 1]);
                else if (lines[i] == "ReviewPeriod")
                    SimSys.ReviewPeriod = int.Parse(lines[i + 1]);
                else if (lines[i] == "StartInventoryQuantity")
                    SimSys.StartInventoryQuantity = int.Parse(lines[i + 1]);
                else if (lines[i] == "StartLeadDays")
                    SimSys.StartLeadDays = int.Parse(lines[i + 1]); 
                else if (lines[i] == "StartOrderQuantity")
                    SimSys.StartOrderQuantity = int.Parse(lines[i + 1]);
                else if (lines[i] == "NumberOfDays")
                    SimSys.NumberOfDays = int.Parse(lines[i + 1]); 
                else if (lines[i] == "DemandDistribution")
                {
                    List<Distribution> DemDist = new List<Distribution>();
                    for(int j = 0;j < 5;j++)
                    { 
                        i++;
                        Distribution DemDistTemp = new Distribution();
                        string[] DemRow = lines[i].Trim().Replace(" ",String.Empty).Split(',');
                        if(lines[i-1] == "DemandDistribution")
                        {
                            decimal prob = decimal.Parse(DemRow[1]) * 100;
                            DemDistTemp.Value = int.Parse(DemRow[0]);
                            DemDistTemp.Probability = prob;
                            DemDistTemp.CummProbability = (int)prob;
                            DemDistCummCounter += DemDistTemp.CummProbability;
                            DemDistTemp.MinRange = 1;
                            DemDistTemp.MaxRange = (int)prob;
                        }
                        else
                        {
                            decimal prob = decimal.Parse(DemRow[1]) * 100;
                            DemDistTemp.Value = int.Parse(DemRow[0]);
                            DemDistTemp.Probability = prob;
                            DemDistCummCounter = DemDist[j - 1].CummProbability + (int)prob;
                            DemDistTemp.CummProbability = DemDistCummCounter;
                            DemDistTemp.MinRange = DemDist[j - 1].MaxRange + 1;
                            DemDistTemp.MaxRange = (int)DemDistTemp.CummProbability;
                        }
                        DemDist.Add(DemDistTemp);
                    }
                    SimSys.DemandDistribution = DemDist;
                }
                else if (lines[i] == "LeadDaysDistribution")
                {
                    List<Distribution> LeadDaysDist = new List<Distribution>();
                    for (int j = 0; j < 3; j++)
                    {
                        i++;
                        Distribution LeadDaysDistTemp = new Distribution();
                        string[] LeadDaysRow = lines[i].Trim().Replace(" ", String.Empty).Split(',');
                        if (lines[i - 1] == "LeadDaysDistribution")
                        {
                            decimal prob = decimal.Parse(LeadDaysRow[1]) * 100;
                            LeadDaysDistTemp.Value = int.Parse(LeadDaysRow[0]);
                            LeadDaysDistTemp.Probability = prob;
                            LeadDaysDistTemp.CummProbability = (int)prob;
                            DemDistCummCounter += LeadDaysDistTemp.CummProbability;
                            LeadDaysDistTemp.MinRange = 1;
                            LeadDaysDistTemp.MaxRange = (int)prob;
                        }
                        else
                        {
                            decimal prob = decimal.Parse(LeadDaysRow[1]) * 100;
                            LeadDaysDistTemp.Value = int.Parse(LeadDaysRow[0]);
                            LeadDaysDistTemp.Probability = prob;
                            DemDistCummCounter = LeadDaysDist[j - 1].CummProbability + (int)prob;
                            LeadDaysDistTemp.CummProbability = DemDistCummCounter;
                            LeadDaysDistTemp.MinRange = LeadDaysDist[j - 1].MaxRange + 1;
                            LeadDaysDistTemp.MaxRange = (int)LeadDaysDistTemp.CummProbability;

                        }
                        LeadDaysDist.Add(LeadDaysDistTemp);
                    }
                    SimSys.LeadDaysDistribution = LeadDaysDist;
                }
                
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Initial Helper Variables
            int ShortQ = 0, EndingQ = 0;
            DaysUntilOrderArrives.Add(SimSys.StartLeadDays-1);
            while (DayCounter < SimSys.NumberOfDays)
            {
                SimulationCase SimulationCasesTemp = new SimulationCase();
                // Initial Static Values Assignment  
                SimulationCasesTemp.Day = DayCounter + 1;
                SimulationCasesTemp.Cycle = GetCycle(SimulationCasesTemp.Day);
                SimulationCasesTemp.RandomDemand = Generate("Hundreds");
                SimulationCasesTemp.Demand = GetDemand(SimulationCasesTemp.RandomDemand);
                SimulationCasesTemp.LeadDays = 0;
                SimulationCasesTemp.ShortageQuantity = 0;
                if(DayWithinCycle != SimSys.ReviewPeriod)
                    DayWithinCycle = DayWithinCycle % SimSys.ReviewPeriod;
                SimulationCasesTemp.DayWithinCycle = DayWithinCycle;
                    ;
                System.Threading.Thread.Sleep(20);
                // Beginning Inventory Control
                if (DayCounter == 0)
                {
                    SimulationCasesTemp.BeginningInventory = SimSys.StartInventoryQuantity;
                    SimulationCasesTemp.EndingInventory = SimulationCasesTemp.BeginningInventory - SimulationCasesTemp.Demand;
                    if (SimulationCasesTemp.EndingInventory < 0)
                    {
                        SimulationCasesTemp.ShortageQuantity = Math.Abs(SimulationCasesTemp.EndingInventory);
                        SimulationCasesTemp.EndingInventory = 0;
                    }
                }
                else 
                {
                    if (DaysUntilOrderArrives[DayCounter] == -1 && DayCounter / SimSys.ReviewPeriod == 0)
                        SimulationCasesTemp.BeginningInventory = SimSys.SimulationCases[DayCounter - 1].EndingInventory
                            + SimSys.StartOrderQuantity;
                    else if (DaysUntilOrderArrives[DayCounter] == -1 && DayCounter / SimSys.ReviewPeriod != 0 && DayWithinCycle != SimSys.ReviewPeriod)
                        SimulationCasesTemp.BeginningInventory = SimSys.SimulationCases[DayCounter - 1].EndingInventory + OrderQ;
                    else
                        SimulationCasesTemp.BeginningInventory = SimSys.SimulationCases[DayCounter - 1].EndingInventory;
                    SimulationCasesTemp.EndingInventory = EndingQ = SimulationCasesTemp.BeginningInventory - (SimulationCasesTemp.Demand + SimSys.SimulationCases[DayCounter - 1].ShortageQuantity);
                    if (SimulationCasesTemp.EndingInventory <= 0)
                    {
                        SimulationCasesTemp.ShortageQuantity = ShortQ = Math.Abs(SimulationCasesTemp.EndingInventory);
                        SimulationCasesTemp.EndingInventory = EndingQ = 0;
                    }
                    else
                        ShortQ = 0;
                }
                System.Threading.Thread.Sleep(20);

                // Cycle Order Control
                if (SimulationCasesTemp.DayWithinCycle == 5)
                {
                    SimulationCasesTemp.OrderQuantity = OrderQ = GetOrderQuantity(
                       EndingQ,
                       ShortQ);
                    SimulationCasesTemp.RandomLeadDays = Generate("Hundreds");
                    SimulationCasesTemp.LeadDays = GetLeadDays(SimulationCasesTemp.RandomLeadDays);
                    DaysUntilOrderArrives[DayCounter] = SimulationCasesTemp.LeadDays;
                }
                else
                {
                    SimulationCasesTemp.OrderQuantity = 0;
                    SimulationCasesTemp.RandomLeadDays = 0;
                }
                System.Threading.Thread.Sleep(20);
                // Lead Days Control
                if (DaysUntilOrderArrives[DayCounter] >= -1)
                    DaysUntilOrderArrives.Add(DaysUntilOrderArrives[DayCounter] - 1);
                else
                    DaysUntilOrderArrives.Add(0);
                // Counters Control
                SimSys.SimulationCases.Add(SimulationCasesTemp);
                DayWithinCycle++;
                DayCounter++;
            }

            for (int i = 0; i < DaysUntilOrderArrives.Count; i++)
                if (DaysUntilOrderArrives[i] < 0)
                    DaysUntilOrderArrives[i] = 0;
            System.Threading.Thread.Sleep(20);
            DataTable dt = FinishFilling();
            decimal ShortSum = 0, EndinSum = 0;
            for (int i = 0; i < SimSys.SimulationCases.Count; i++)
            { 
                dt.Rows.Add(
                 SimSys.SimulationCases[i].Day,
                 SimSys.SimulationCases[i].Cycle,
                 SimSys.SimulationCases[i].DayWithinCycle,
                 SimSys.SimulationCases[i].BeginningInventory,
                 SimSys.SimulationCases[i].RandomDemand,
                 SimSys.SimulationCases[i].Demand,
                 SimSys.SimulationCases[i].EndingInventory,
                 SimSys.SimulationCases[i].ShortageQuantity,
                 SimSys.SimulationCases[i].OrderQuantity,
                 SimSys.SimulationCases[i].RandomLeadDays,
                 SimSys.SimulationCases[i].LeadDays,
                 DaysUntilOrderArrives[i]
                 );
                ShortSum += SimSys.SimulationCases[i].ShortageQuantity;
                EndinSum += SimSys.SimulationCases[i].EndingInventory;
            }
            PerformanceMeasures PM = new PerformanceMeasures();
            PM.ShortageQuantityAverage = ShortSum/SimSys.SimulationCases.Count;
            PM.EndingInventoryAverage = EndinSum/SimSys.SimulationCases.Count;
            SimSys.PerformanceMeasures = PM;
            string TM = TestingManager.Test(SimSys, Constants.FileNames.TestCase1);
            MessageBox.Show(TM);
        }
        public int Generate(string type)
        {
            if (type == "Tens")
                return rand.Next(1, 10);
            else if (type == "Hundreds")
                return rand.Next(1, 101);
            else
                return 0;
        }
        public int GetDemand(int Random)
        {
            for (int i = 0; i < SimSys.DemandDistribution.Count; i++)
                if (Random>=SimSys.DemandDistribution[i].MinRange &&Random<=SimSys.DemandDistribution[i].MaxRange)
                    return SimSys.DemandDistribution[i].Value;
            return -1;
        }
        public int GetLeadDays(int Random)
        {
            for (int i = 0; i < SimSys.LeadDaysDistribution.Count; i++)
                if (Random >= SimSys.LeadDaysDistribution[i].MinRange && Random <= SimSys.LeadDaysDistribution[i].MaxRange)
                    return SimSys.LeadDaysDistribution[i].Value;
            return -1;
        }
        public int GetCycle(int Day)
        {
            if (Day % SimSys.ReviewPeriod == 0)
                return (Day / SimSys.ReviewPeriod);
            return (Day / SimSys.ReviewPeriod) + 1;
        }
        public int GetOrderQuantity(int EndingE,int ShortQ)
        {
            return OrderQ = SimSys.OrderUpTo - EndingE + ShortQ;
        }
        public DataTable FinishFilling()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Day");
            dt.Columns.Add("Cycle");
            dt.Columns.Add("Day Within Cycle");
            dt.Columns.Add("Beginning Inventory");
            dt.Columns.Add("Random Digits For Demand");
            dt.Columns.Add("Demand");
            dt.Columns.Add("Ending Inventory");
            dt.Columns.Add("Shortage Quantity");
            dt.Columns.Add("Order Quantity");
            dt.Columns.Add("Random Digits For Lead Time");
            dt.Columns.Add("Lead Time");
            dt.Columns.Add("Days Until Order Arrives");
            dataGridView1.DataSource = dt;
            textBox1.Text = SimSys.OrderUpTo.ToString();
            textBox2.Text = SimSys.ReviewPeriod.ToString();
            textBox3.Text = SimSys.StartInventoryQuantity.ToString();
            textBox4.Text = SimSys.StartLeadDays.ToString();
            textBox5.Text = SimSys.StartOrderQuantity.ToString();
            textBox6.Text = SimSys.NumberOfDays.ToString();
            return dt;
        }
    }
}
