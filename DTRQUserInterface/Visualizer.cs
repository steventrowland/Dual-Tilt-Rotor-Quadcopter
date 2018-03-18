﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using ADRCVisualization.Class_Files;
using System.Drawing.Imaging;
using System.IO;
using ADRCVisualization.Class_Files.Mathematics;
using DTRQCSInterface;

namespace ADRCVisualization
{
    public partial class Visualizer : Form
    {
        private DateTime dateTime;
        private DTRQQuadcopter quadcopter;
        
        //FeedbackControllers
        //private double RunTime = 60;
        
        //Timers for alternate threads and asynchronous calculations
        private System.Timers.Timer t1;

        private SVector targetPosition;
        private SDirAngle targetRotation;
        private bool initialized = false;

        public Visualizer()
        {
            quadcopter = new DTRQQuadcopter(true, 0.3, 55, 0.05);
            SQuad quad = quadcopter.GetQuadcopter();

            quadcopter.SimulateCurrent(new SVector(0, -9.81, 0));

            targetPosition = new SVector(0, 0, 0);
            targetRotation = new SDirAngle(0, 0, 1, 0);

            quadcopter.SetTarget(targetPosition, targetRotation);
            Console.WriteLine("Initializing UI components.");

            Thread.Sleep(2500);

            InitializeComponent();

            Console.WriteLine("UI components initialized.");

            //Start3DViewer();

            dateTime = DateTime.Now;

            StartTimers();
            //StopTimers();

            //Set current

            SetMultiple();

            //DisableMotor();

            initialized = true;
        }

        private void Start3DViewer()
        {
            //3DViewer viewer = new 3D
            Thread viewerThread = new Thread(delegate ()
            {
                System.Windows.Window window = new System.Windows.Window
                {
                    Title = "Quadcopter 3D Visualizer",
                    Content = new QuadViewer()
                };

                window.Height = 600;
                window.Width = 600;

                window.Icon = null;

                window.ShowDialog();

                System.Windows.Threading.Dispatcher.Run();
            });

            viewerThread.SetApartmentState(ApartmentState.STA); // needs to be STA or throws exception
            viewerThread.Start();
        }
        
        public SQuad GetQuadcopter()
        {
            if (!initialized)
            {
                Console.WriteLine("Not initialized");

                throw new Exception("Quadcopter not initialized before request.");
            }

            return quadcopter.GetQuadcopter();
        }

        private async void DisableMotor()
        {
            await Task.Delay(4000);

            //quad.ThrusterB.Disable = true;

            //await Task.Delay(500);

            //quad.ThrusterD.Disable = true;
        }

        private async void SetMultiple()
        {
            quadcopter.SetTarget(targetPosition, targetRotation);

            while (true)
            {
                /*
                for (double i = 0; i < 360; i += 1)
                {
                    targetPosition = new Vector(Math.Sin(MathE.DegreesToRadians(i)) * 1.5, 0, Math.Cos(MathE.DegreesToRadians(i)) * 1.5);
                    targetRotation = new Vector(i / 6, 0, 0);

                    await Task.Delay(15);
                }

                for (double i = 0; i < 360; i += 1)
                {
                    targetPosition = new Vector(Math.Sin(MathE.DegreesToRadians(i)) * 1.5, 0, Math.Cos(MathE.DegreesToRadians(i)) * 1.5);
                    targetRotation = new Vector(60 - i / 8, 0, 0);

                    await Task.Delay(15);
                }
                */
                
                targetPosition = new SVector(1, 0, 1.2);
                targetRotation = new SDirAngle(0, 0, 1, 0);
                Console.WriteLine("Target Set");

                await Task.Delay(3000);
                
                //////////////////////////////////////////////////
                targetPosition = new SVector(-1, 0, 1.2);
                targetRotation = new SDirAngle(0, 0, 0, -1);//90, 0, 0
                Console.WriteLine("Target Set");

                await Task.Delay(7500);

                //////////////////////////////////////////////////
                targetPosition = new SVector(-1, 0, -1.2);
                targetRotation = new SDirAngle(0, 0, -0.707, -0.707);//0, 45, 0
                Console.WriteLine("Target Set");

                await Task.Delay(7500);

                targetPosition = new SVector(-1, 0, -1.2);
                targetRotation = new SDirAngle(0, 0, 1, 0);//0, 0, 0
                Console.WriteLine("Target Set");

                await Task.Delay(5000);
                

                //////////////////////////////////////////////////
                targetPosition = new SVector(1, 0, -1.2);
                targetRotation = new SDirAngle(0, 1, 0, 0);//0, 0, 90
                Console.WriteLine("Target Set");

                await Task.Delay(10000);

                //////////////////////////////////////////////////
                targetPosition = new SVector(-1, 0, 1.2);
                targetRotation = new SDirAngle(0, 0.707, -0.707, 0);
                Console.WriteLine("Target Set");

                await Task.Delay(6000);

                //////////////////////////////////////////////////
                targetPosition = new SVector(1, 0, 1.2);
                targetRotation = new SDirAngle(0, 0, 1, 0);//0, 0, 0
                Console.WriteLine("Target Set");

                await Task.Delay(6000);
            }
        }
        
        
        private void SetChartPositions(SQuad q)
        {
            chart1.ChartAreas[0].AxisX.Maximum = 2;
            chart1.ChartAreas[0].AxisX.Minimum = -2;
            chart1.ChartAreas[0].AxisY.Maximum = 2;
            chart1.ChartAreas[0].AxisY.Minimum = -2;

            chart2.ChartAreas[0].AxisY.Maximum = 10;
            chart2.ChartAreas[0].AxisY.Minimum = -10;

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            chart1.Series[3].Points.Clear();
            chart1.Series[4].Points.Clear();
            chart1.Series[5].Points.Clear();
            chart1.Series[6].Points.Clear();
            chart1.Series[7].Points.Clear();
            chart1.Series[8].Points.Clear();

            chart1.Series[0].MarkerColor = Color.MediumSlateBlue;
            chart1.Series[1].MarkerColor = Color.BurlyWood;
            chart1.Series[2].MarkerColor = Color.BlueViolet;
            chart1.Series[3].MarkerColor = Color.ForestGreen;
            chart1.Series[4].MarkerColor = Color.MediumAquamarine;
            chart1.Series[5].MarkerColor = Color.BurlyWood;
            chart1.Series[6].MarkerColor = Color.BlueViolet;
            chart1.Series[7].MarkerColor = Color.ForestGreen;
            chart1.Series[8].MarkerColor = Color.MediumAquamarine;

            chart1.Series[9].MarkerColor = Color.LimeGreen;
            chart1.Series[10].MarkerColor = Color.LightPink;

            chart1.Series[9].Points.AddXY(q.CurrentPosition.X, q.CurrentPosition.Z);
            chart1.Series[10].Points.AddXY(q.TargetPosition.X, q.TargetPosition.Z);

            if (chart1.Series[9].Points.Count > 400) chart1.Series[9].Points.RemoveAt(0);
            if (chart1.Series[10].Points.Count > 400) chart1.Series[10].Points.RemoveAt(0);

            chart1.Series[0].Points.AddXY(q.CurrentPosition.X, q.CurrentPosition.Z);
            chart1.Series[1].Points.AddXY(q.ThrusterB.CurrentPosition.X, q.ThrusterB.CurrentPosition.Z);
            chart1.Series[2].Points.AddXY(q.ThrusterC.CurrentPosition.X, q.ThrusterC.CurrentPosition.Z);
            chart1.Series[3].Points.AddXY(q.ThrusterD.CurrentPosition.X, q.ThrusterD.CurrentPosition.Z);
            chart1.Series[4].Points.AddXY(q.ThrusterE.CurrentPosition.X, q.ThrusterE.CurrentPosition.Z);

            chart1.Series[0].MarkerSize = (int)MathE.Constrain(q.CurrentPosition.Y + 10, 2, 40);
            chart1.Series[1].MarkerSize = (int)MathE.Constrain(q.ThrusterB.CurrentPosition.Y + 10, 2, 40);
            chart1.Series[2].MarkerSize = (int)MathE.Constrain(q.ThrusterC.CurrentPosition.Y + 10, 2, 40);
            chart1.Series[3].MarkerSize = (int)MathE.Constrain(q.ThrusterD.CurrentPosition.Y + 10, 2, 40);
            chart1.Series[4].MarkerSize = (int)MathE.Constrain(q.ThrusterE.CurrentPosition.Y + 10, 2, 40);

            chart1.Series[5].Points.AddXY(q.ThrusterB.TargetPosition.X, q.ThrusterB.TargetPosition.Z);
            chart1.Series[6].Points.AddXY(q.ThrusterC.TargetPosition.X, q.ThrusterC.TargetPosition.Z);
            chart1.Series[7].Points.AddXY(q.ThrusterD.TargetPosition.X, q.ThrusterD.TargetPosition.Z);
            chart1.Series[8].Points.AddXY(q.ThrusterE.TargetPosition.X, q.ThrusterE.TargetPosition.Z);

            chart2.Series[0].Points.Clear();

            chart2.Series[0].Points.Add(q.CurrentPosition.Y);
            chart2.Series[0].Points.Add(q.TargetPosition.Y);
        }
        
        /// <summary>
        /// Starts alternate threads for calculation of the inverted pendulum and updating the display of the user interface for the FFTWs, pendulum, and graphs.
        /// </summary>
        private async void StartTimers()
        {
            await Task.Delay(50);
            
            t1 = new System.Timers.Timer
            {
                Interval = 30, //In milliseconds here
                AutoReset = true //Stops it from repeating
            };
            t1.Elapsed += new ElapsedEventHandler(Calculate);
            t1.Start();
        }

        /// <summary>
        /// Stops the secondary threads to end the calculation.
        /// </summary>
        private async void StopTimers()
        {
            await Task.Delay(60000);
            
            t1.Stop();
        }
        
        /// <summary>
        /// Updates the diplay of the quadcopter and the charts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Calculate(object sender, ElapsedEventArgs e)
        {
            this.BeginInvoke((Action)(() =>
            {
                quadcopter.CalculateCombinedThrustVector();//Secondary Solver
                
                quadcopter.SetTarget(targetPosition, targetRotation);
                quadcopter.SimulateCurrent(new SVector(0, -9.81, 0));
                    
                SetChartPositions(quadcopter.GetQuadcopter());

                //label1.Text = Vector.CalculateEuclideanDistance(quad.ThrusterB.CurrentPosition, quad.ThrusterB.TargetPosition).ToString();
                //label2.Text = Vector.CalculateEuclideanDistance(quad.ThrusterC.CurrentPosition, quad.ThrusterC.TargetPosition).ToString();
                //label3.Text = Vector.CalculateEuclideanDistance(quad.ThrusterD.CurrentPosition, quad.ThrusterD.TargetPosition).ToString();
                //label4.Text = Vector.CalculateEuclideanDistance(quad.ThrusterE.CurrentPosition, quad.ThrusterE.TargetPosition).ToString();
            }));
        }
        
        private void Visualizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            t1.Stop();
        }

        private void SendXYZ_Click(object sender, EventArgs e)
        {
            Double.TryParse(xPositionTB.Text, out double x);
            Double.TryParse(yPositionTB.Text, out double y);
            Double.TryParse(zPositionTB.Text, out double z);

            //targetPosition = new SVector(x, y, z);
        }

        private void SendHPB_Click(object sender, EventArgs e)
        {
            Double.TryParse(xRotationTB.Text, out double x);
            Double.TryParse(yRotationTB.Text, out double y);
            Double.TryParse(zRotationTB.Text, out double z);
            Double.TryParse(rRotationTB.Text, out double r);

            //targetRotation = new SDirAngle(r, x, y, z);
        }
    }
}
