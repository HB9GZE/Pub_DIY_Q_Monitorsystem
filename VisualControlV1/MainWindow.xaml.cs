//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Stefan Meyre>
//     Copyright (c) 2016 Stefan Meyre. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using VisualControlV1.Annotations;
using OxyPlot;
using OxyPlot.Series;



namespace VisualControlV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CalcValues myCalcValues;
        private SerialCom mySerialCom;
        private string status;
        private bool initializing = false;


        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyChangedStatus(new PropertyChangedEventArgs("Status"));
            }
        }

        private void NotifyChangedStatus(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public MainWindow()
        {
            initializing = true;
            InitializeComponent();
            initializing = false;

            myMap.Focus();
            //Set map to Aerial mode with labels
            myMap.Mode = new AerialMode(true);
            //addNewPolygon();

            myCalcValues = new CalcValues();
            myGrid.DataContext = myCalcValues;
     
            lblStatus.DataContext = this;

            cmbComSelect.ItemsSource = SerialPort.GetPortNames();
            cmbComSelect.Text = "COM3";
        }


        private void BtnPortOpen_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialCom == null)
            {
                mySerialCom = new SerialCom(cmbComSelect.Text);
            }
            else
            {
                MessageBox.Show("Serial port is already open!");
            }

            if (mySerialCom != null)
            {
                Status = mySerialCom.Status;
                mySerialCom.Subscribe(myCalcValues);
            }
        }

        private void BtnPortClose_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialCom != null)
            {
                mySerialCom.ClosePort();
                mySerialCom = null;
                Status = cmbComSelect.Text + " has been closed.";
            }
        }

        private void button_Click_North(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click_South(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click_West(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click_East(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click_Hold(object sender, RoutedEventArgs e)
        {

        }
        private void button_Click_Landing(object sender, RoutedEventArgs e)
        {

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if (initializing) return;
        //    byte myDummyByte;
        //    myDummyByte = Convert.ToByte(slider1.Value);
        //    try
        //    {
        //        mySerialCom.MySerialPort.Write("#yaw2/");
        //        mySerialCom.WriteByte(myDummyByte);
        //        mySerialCom.WriteByte(myDummyByte);
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Open com port first!.", "Important Message");
        //    }
        //}

        //private void MyTouchMove(object sender, TouchEventArgs e)
        //{
        //    Point myTouchPoint = e.GetTouchPoint(this).Position;
        //    myCalcValues.calcNewAngle(myTouchPoint.X, myTouchPoint.Y);

        //    //GeneralTransform generalTransform = image1.TransformToVisual(this);
        //    //Point point = generalTransform.Transform(new Point());
        //    //Debug.WriteLine(point.X + " " + point.Y);
        //}

        //private void MyTouchDownAndUp(object sender, TouchEventArgs e)
        //{
        //    myCalcValues.resetNewAngle();
        //}

        void addNewPolygon()
        {
            MapPolygon polygon = new MapPolygon();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            polygon.StrokeThickness = 5;
            polygon.Opacity = 0.7;
            polygon.Locations = new LocationCollection()
            {
                new Location(47.3392, 8.4432),
                new Location(47.34, 8.4442),
                new Location(47.3402, 8.4332),
                new Location(47.3394, 8.4435)
            };

            myMap.Children.Add(polygon);
        }

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            // The pushpin to add to the map.
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;

            // Adds the pushpin to the map.
            myMap.Children.Add(pin);
        }

        public double GetDistanceBetweenPoints(double lat1, double long1, double lat2, double long2)
        {
            double distance = 0;

            double dLat = (lat2 - lat1)/180*Math.PI;
            double dLong = (long2 - long1)/180*Math.PI;

            double a = Math.Sin(dLat/2)*Math.Sin(dLat/2)
                       + Math.Cos(lat1/180*Math.PI)*Math.Cos(lat2/180*Math.PI)
                       *Math.Sin(dLong/2)*Math.Sin(dLong/2);
            double c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE*radiusP*Math.Cos(lat1/180*Math.PI), 2);
            //Denominator part of the function
            double dr = Math.Pow(radiusE*Math.Cos(lat1/180*Math.PI), 2)
                        + Math.Pow(radiusP*Math.Sin(lat1/180*Math.PI), 2);
            double radius = Math.Sqrt(nr/dr);

            //Calculate distance in meters.
            distance = radius*c;
            return distance; // distance in meters
        }

        private void button_Click_Start(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#sta2/");
                mySerialCom.WriteByte(0x10);
                mySerialCom.WriteByte(0x10);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_Stop(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#sto2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }
    }
}