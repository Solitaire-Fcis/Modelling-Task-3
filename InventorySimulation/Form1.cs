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
        decimal DemDistCummCounter = 0;
        Random rand = new Random();
        int CycleCounter = 1, DayWithinCycle = 1,ending;

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
                            decimal prob = decimal.Parse(LeadDaysRow[1]) * 10;
                            LeadDaysDistTemp.Value = int.Parse(LeadDaysRow[0]);
                            LeadDaysDistTemp.Probability = prob;
                            LeadDaysDistTemp.CummProbability = (int)prob;
                            DemDistCummCounter += LeadDaysDistTemp.CummProbability;
                            LeadDaysDistTemp.MinRange = 1;
                            LeadDaysDistTemp.MaxRange = (int)prob;
                        }
                        else
                        {
                            decimal prob = decimal.Parse(LeadDaysRow[1]) * 10;
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
            int DayCounter = 1,DaysUntilOrderArrives=SimSys.StartLeadDays;
            bool flag = false;
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
            List<SimulationCase> SimulationCases = new List<SimulationCase>();
            
            while (DayCounter <= SimSys.NumberOfDays)
            {
                SimulationCase SimulationCasesTemp = new SimulationCase();
                SimulationCasesTemp.Day = DayCounter;
                SimulationCasesTemp.Cycle = GetCycle(SimulationCasesTemp.Day);
                SimulationCasesTemp.DayWithinCycle = DayWithinCycle;
                DayWithinCycle++;
                if (DayWithinCycle>SimSys.ReviewPeriod)
                {
                    DayWithinCycle = 1;
                }
                //Begining inventory Assign Start
                if (DayCounter==1)
                {
                    SimulationCasesTemp.BeginningInventory = SimSys.StartInventoryQuantity;
                }
                else if (flag)
                {
                    SimulationCasesTemp.BeginningInventory += SimSys.StartOrderQuantity;
                    flag = false;
                }
                else
                {
                    SimulationCasesTemp.BeginningInventory = ending;
                }
                //Begining inventory Assign End

                SimulationCasesTemp.RandomDemand=Generate("Hundreds");
                SimulationCasesTemp.Demand = GetDemand(SimulationCasesTemp.RandomDemand);
                SimulationCasesTemp.EndingInventory = SimulationCasesTemp.BeginningInventory- SimulationCasesTemp.Demand;
                if (SimulationCasesTemp.EndingInventory<0)
                {
                    SimulationCasesTemp.ShortageQuantity = Math.Abs(SimulationCasesTemp.EndingInventory);
                    SimulationCasesTemp.EndingInventory = 0;
                    ending = SimulationCasesTemp.EndingInventory;
                }
                else
                {
                    SimulationCasesTemp.ShortageQuantity = 0;
                    ending=SimulationCasesTemp.EndingInventory;
                }
                if (SimulationCasesTemp.DayWithinCycle==5)
                {
                    SimulationCasesTemp.OrderQuantity = GetOrderQuantity(SimulationCasesTemp.EndingInventory, SimulationCasesTemp.ShortageQuantity);
                    SimulationCasesTemp.RandomLeadDays = Generate("Tens");
                    SimulationCasesTemp.LeadDays = GetLeadDays(SimulationCasesTemp.RandomLeadDays);
                }
                else
                {
                    SimulationCasesTemp.OrderQuantity = 0;
                    SimulationCasesTemp.RandomLeadDays =0;
                    SimulationCasesTemp.LeadDays = 0;
                }
                SimulationCases.Add(SimulationCasesTemp);
                DaysUntilOrderArrives--;
                if (DaysUntilOrderArrives==0)
                {
                    flag = true;
                    if (!(SimulationCasesTemp.DayWithinCycle< SimSys.ReviewPeriod))
                    {
                        DaysUntilOrderArrives = SimulationCasesTemp.LeadDays;
                    }
                }
                DayCounter++;
            }
            SimSys.SimulationCases = SimulationCases;

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
                    SimSys.SimulationCases[i].LeadDays
                    );
            }
        }
        public int Generate(string type)
        {
            if (type == "Tens")
            {
                int generated = rand.Next(1, 10);
                return generated;
            }
            else if (type == "Hundreds")
            {
                int generated = rand.Next(1, 101);
                return generated;
            }
            else
                return 0;
        }
        public int GetDemand(int Random)
        {
            for (int i = 0; i < SimSys.DemandDistribution.Count; i++)
            {
                if (Random>=SimSys.DemandDistribution[i].MinRange &&Random<=SimSys.DemandDistribution[i].MaxRange)
                {
                    return SimSys.DemandDistribution[i].Value;
                }
            }
            return -1;
        }
        public int GetLeadDays(int Random)
        {
            for (int i = 0; i < SimSys.LeadDaysDistribution.Count; i++)
            {
                if (Random >= SimSys.LeadDaysDistribution[i].MinRange && Random <= SimSys.LeadDaysDistribution[i].MaxRange)
                {
                    return SimSys.LeadDaysDistribution[i].Value;
                }
            }
            return 0;
        }
        public int GetCycle(int Day)
        {
            int CycleCounterTemp;
            if (Day%SimSys.ReviewPeriod==0)
            {
                CycleCounterTemp = CycleCounter;
                CycleCounter++;
                return CycleCounterTemp;
            }
            else
            {
                return CycleCounter;
            }
        }
        public int GetOrderQuantity(int EndingE,int ShortQ)
        {
            return SimSys.OrderUpTo-EndingE+ShortQ;
        }
      
       
    }
}
