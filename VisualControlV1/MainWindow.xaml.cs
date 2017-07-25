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
using VisualControlV3.Annotations;
using OxyPlot;
using OxyPlot.Series;


namespace VisualControlV3
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
            //myMap.DataContext = this;

            cmbComSelect.ItemsSource = SerialPort.GetPortNames();
            cmbComSelect.Text = "COM6";

            Button dummyButton = new Button();

            CalcValues.SomethingHappened += new EventHandler(MainWindow_SomethingHappened);
        }


        void MainWindow_SomethingHappened(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate() { myPushpin.Location = myCalcValues.CurrentLocation; }));
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void addNewPolygon()
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

            double dLat = (lat2 - lat1) / 180 * Math.PI;
            double dLong = (long2 - long1) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                       + Math.Cos(lat1 / 180 * Math.PI) * Math.Cos(lat2 / 180 * Math.PI)
                       * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(lat1 / 180 * Math.PI), 2);
            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(lat1 / 180 * Math.PI), 2)
                        + Math.Pow(radiusP * Math.Sin(lat1 / 180 * Math.PI), 2);
            double radius = Math.Sqrt(nr / dr);

            //Calculate distance in meters.
            distance = radius * c;
            return distance; // distance in meters
        }

        private void currentPosition_Click(object sender, RoutedEventArgs e)
        {
            Pushpin thePushpin = new Pushpin();
            MapLayer.SetPosition(thePushpin, myCalcValues.CurrentLocation);
            myMap.Children.Add(thePushpin);
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
            try
            {
                mySerialCom.MySerialPort.Write("#mno2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
                myCalcValues.SetHeading = 0;
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_South(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#mso2/");
                mySerialCom.WriteByte(0x0);
                mySerialCom.WriteByte(0x0);
                myCalcValues.SetHeading = 180;
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_West(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#mwe2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
                myCalcValues.SetHeading = 270;
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_East(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#mea2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
                myCalcValues.SetHeading = 90;
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_Hold(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#hld2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_Landing(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#lnd2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
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

        private void button_Click_CalEuler(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#cal2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_NoCalEuler(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#caz2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_CalMagneto(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#cam2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_RelPressureZero(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#spz2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }

        private void button_Click_Reset(object sender, RoutedEventArgs e)
        {
            try
            {
                mySerialCom.MySerialPort.Write("#rst2/");
                mySerialCom.WriteByte(0x00);
                mySerialCom.WriteByte(0x00);
            }
            catch (Exception)
            {
                MessageBox.Show("Open com port first!.", "Important Message");
            }
        }


        private void sliderKP_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#kpp2/");
                    short dummy = (short) (sliderKP.Value * 100);
                    mySerialCom.WriteByte((byte) dummy); //low byte
                    mySerialCom.WriteByte((byte) (dummy >> 8)); //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void sliderKD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#kdp2/");
                    short dummy = (short) (sliderKD.Value * 100);
                    mySerialCom.WriteByte((byte) dummy); //low byte
                    mySerialCom.WriteByte((byte) (dummy >> 8)); //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void sliderKI_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#kip2/");
                    short dummy = (short) (sliderKI.Value * 100);
                    mySerialCom.WriteByte((byte) dummy); //low byte
                    mySerialCom.WriteByte((byte) (dummy >> 8)); //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void sliderKO_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#kop2/");
                    short dummy = (short) (sliderKO.Value * 100);
                    mySerialCom.WriteByte((byte) dummy); //low byte
                    mySerialCom.WriteByte((byte) (dummy >> 8)); //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void sliderK1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#k112/");
                    short dummy = (short)(sliderK1.Value * 100);
                    mySerialCom.WriteByte((byte)dummy); //low byte
                    mySerialCom.WriteByte((byte)(dummy >> 8)); //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void sliderK2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#k222/");
                    short dummy = (short)(sliderK2.Value * 100);
                    mySerialCom.WriteByte((byte)dummy); //low byte
                    mySerialCom.WriteByte((byte)(dummy >> 8)); //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void sliderKTOT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#kto2/");
                    short dummy = (short)(sliderKTOT.Value * 100);
                    mySerialCom.WriteByte((byte)dummy); //low byte
                    mySerialCom.WriteByte((byte)(dummy >> 8)); //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }
        private void sliderSF_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#sfm2/");
                    short dummy = (short)(sliderSF.Value);
                    mySerialCom.WriteByte((byte)dummy);   //low byte
                    mySerialCom.WriteByte((byte)(dummy >> 8));   //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void sliderBeta_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#sbe2/");
                    short dummy = (short)(sliderBeta.Value * 100);
                    mySerialCom.WriteByte((byte)dummy);   //low byte
                    mySerialCom.WriteByte((byte)(dummy >> 8));   //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void sliderOutputLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#mol2/");
                    short dummy = (short)(sliderOutputLimit.Value * 100);
                    mySerialCom.WriteByte((byte)dummy);   //low byte
                    mySerialCom.WriteByte((byte)(dummy >> 8));   //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void SetMahonyKp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#smp2/");
                    short dummy = (short)(sliderSetMahonyKp.Value * 100);
                    mySerialCom.WriteByte((byte)dummy);   //low byte
                    mySerialCom.WriteByte((byte)(dummy >> 8));   //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void SetMahonyKi_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (initializing == false)
            {
                try
                {
                    mySerialCom.MySerialPort.Write("#smi2/");
                    short dummy = (short)(sliderSetMahonyKi.Value * 100);
                    mySerialCom.WriteByte((byte)dummy);   //low byte
                    mySerialCom.WriteByte((byte)(dummy >> 8));   //high byte
                }
                catch (Exception)
                {
                    MessageBox.Show("Open com port first!.", "Important Message");
                }
            }
        }

        private void NotifyChangedStatus(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}