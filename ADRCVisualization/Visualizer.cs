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

namespace ADRCVisualization
{
    public partial class Visualizer : Form
    {
        private DateTime dateTime;
        
        //FeedbackControllers
        private double RunTime = 60;
        
        //Timers for alternate threads and asynchronous calculations
        private System.Timers.Timer t1;

        private Vector gravity =  new Vector(0, -9.81, 0);
        private Quadcopter quad = new Quadcopter(0.3, 45);
        private Turbulence turbulence = new Turbulence(10, 100);

        private Vector targetPosition;
        private Vector targetRotation;

        public Visualizer()
        {
            InitializeComponent();
            
            dateTime = DateTime.Now;

            StartTimers();
            StopTimers();
            
            //Set current
            quad.CalculateCurrent();

            SetTargets();
            //SetTargetCircle();
        }

        private async void SetTargetCircle()
        {
            targetPosition = new Vector(0, 0, 0);
            targetRotation = new Vector(0, 0, 0);

            quad.SetTarget(targetPosition, targetRotation);

            while (true)
            {
                for (double i = 0; i < 360; i += 0.01)
                {
                    targetPosition = new Vector(Math.Sin(i), 0, Math.Cos(i));
                    targetRotation = new Vector(i * 100, i * 100, -i * 100);

                    await Task.Delay(5);
                }
            }
        }

        private async void SetTargets()
        {
            targetPosition = new Vector(0, 0, 0);
            targetRotation = new Vector(0, 0, 0);

            quad.SetTarget(targetPosition, targetRotation);

            while (true)
            {
                targetPosition = new Vector(1, -1, 1.2);
                targetRotation = new Vector(0, 0, 0);
                Console.WriteLine("Target Set");

                await Task.Delay(3000);

                targetPosition = new Vector(-1, 0, 1.2);
                targetRotation = new Vector(60, 0, 0);
                Console.WriteLine("Target Set");

                await Task.Delay(3000);
                targetPosition = new Vector(1, 1, -1.2);
                targetRotation = new Vector(0, 60, 0);
                Console.WriteLine("Target Set");

                await Task.Delay(3500);

                targetPosition = new Vector(-1, -1, -1.2);
                targetRotation = new Vector(0, 0, 60);
                Console.WriteLine("Target Set");

                await Task.Delay(3000);

                targetPosition = new Vector(0, 0, 0);
                targetRotation = new Vector(30, 60, 90);
                Console.WriteLine("Target Set");

                await Task.Delay(3000);
            }
        }
        
        private void SetChartPositions(Quadcopter quadcopter)
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
        
            chart1.Series[1].MarkerColor = Color.BurlyWood;
            chart1.Series[2].MarkerColor = Color.BlueViolet;
            chart1.Series[3].MarkerColor = Color.ForestGreen;
            chart1.Series[4].MarkerColor = Color.MediumAquamarine;
            chart1.Series[5].MarkerColor = Color.BurlyWood;
            chart1.Series[6].MarkerColor = Color.BlueViolet;
            chart1.Series[7].MarkerColor = Color.ForestGreen;
            chart1.Series[8].MarkerColor = Color.MediumAquamarine;
            
            chart1.Series[0].Points.AddXY(quad.CurrentPosition.X, quad.CurrentPosition.Z);
            chart1.Series[1].Points.AddXY(quad.ThrusterB.CurrentPosition.X, quad.ThrusterB.CurrentPosition.Z);
            chart1.Series[2].Points.AddXY(quad.ThrusterC.CurrentPosition.X, quad.ThrusterC.CurrentPosition.Z);
            chart1.Series[3].Points.AddXY(quad.ThrusterD.CurrentPosition.X, quad.ThrusterD.CurrentPosition.Z);
            chart1.Series[4].Points.AddXY(quad.ThrusterE.CurrentPosition.X, quad.ThrusterE.CurrentPosition.Z);

            chart1.Series[0].MarkerSize = (int)quad.CurrentPosition.Y + 5 * 2;
            chart1.Series[1].MarkerSize = (int)quad.ThrusterB.CurrentPosition.Y + 5 * 2;
            chart1.Series[2].MarkerSize = (int)quad.ThrusterC.CurrentPosition.Y + 5 * 2;
            chart1.Series[3].MarkerSize = (int)quad.ThrusterD.CurrentPosition.Y + 5 * 2;
            chart1.Series[4].MarkerSize = (int)quad.ThrusterE.CurrentPosition.Y + 5 * 2;

            chart1.Series[5].Points.AddXY(quad.ThrusterB.TargetPosition.X, quad.ThrusterB.TargetPosition.Z);
            chart1.Series[6].Points.AddXY(quad.ThrusterC.TargetPosition.X, quad.ThrusterC.TargetPosition.Z);
            chart1.Series[7].Points.AddXY(quad.ThrusterD.TargetPosition.X, quad.ThrusterD.TargetPosition.Z);
            chart1.Series[8].Points.AddXY(quad.ThrusterE.TargetPosition.X, quad.ThrusterE.TargetPosition.Z);

            chart2.Series[0].Points.Clear();

            chart2.Series[0].Points.Add(quad.CurrentPosition.Y);
            chart2.Series[0].Points.Add(quad.TargetPosition.Y);
        }
        
        /// <summary>
        /// Starts alternate threads for calculation of the inverted pendulum and updating the display of the user interface for the FFTWs, pendulum, and graphs.
        /// </summary>
        private async void StartTimers()
        {
            await Task.Delay(50);

            this.BeginInvoke((Action)(() =>
            {
                t1 = new System.Timers.Timer
                {
                    Interval = 30, //In milliseconds here
                    AutoReset = true //Stops it from repeating
                };
                t1.Elapsed += new ElapsedEventHandler(Calculate);
                t1.Start();
            }));
        }

        /// <summary>
        /// Stops the secondary threads to end the calculation.
        /// </summary>
        private async void StopTimers()
        {
            await Task.Delay((int)RunTime * 1000);

            this.BeginInvoke((Action)(() =>
            {
                t1.Stop();
            }));
        }
        
        /// <summary>
        /// Updates the diplay of the quadcopter and the charts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Calculate(object sender, ElapsedEventArgs e)
        {
            if (!(DateTime.Now.Subtract(dateTime).TotalSeconds > RunTime))
            {
                this.BeginInvoke((Action)(() =>
                {
                    //Calculate
                    //quad.CalculateIndividualThrustVectors();//Initial Solver
                    quad.CalculateCombinedThrustVector();//Secondary Solver

                    quad.ApplyForce(gravity);
                    quad.SetTarget(targetPosition, targetRotation);
                    quad.CalculateCurrent();
                    //quad.SetCurrent(positionAndRotation.Item1, positionAndRotation.Item2);
                    
                    SetChartPositions(quad);

                    label1.Text = Vector.CalculateEuclideanDistance(quad.ThrusterB.CurrentPosition, quad.ThrusterB.TargetPosition).ToString();
                    label2.Text = Vector.CalculateEuclideanDistance(quad.ThrusterC.CurrentPosition, quad.ThrusterC.TargetPosition).ToString();
                    label3.Text = Vector.CalculateEuclideanDistance(quad.ThrusterD.CurrentPosition, quad.ThrusterD.TargetPosition).ToString();
                    label4.Text = Vector.CalculateEuclideanDistance(quad.ThrusterE.CurrentPosition, quad.ThrusterE.TargetPosition).ToString();

                    //Console.Write(motorPositions.Item1.ToString());
                    //Console.WriteLine();
                }));
            }
        }

        private void Visualizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //t1.Stop();
        }
    }
}
