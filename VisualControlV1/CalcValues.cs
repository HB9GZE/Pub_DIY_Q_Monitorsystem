//-----------------------------------------------------------------------
// <copyright file="CalcValues.cs" company="Stefan Meyre>
//     Copyright (c) 2016 Stefan Meyre. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF;
using OxyPlot;

namespace VisualControlV1
{
    /// <summary>
    /// Calculates all the angles from the raw sensor data. This is done by applying a 
    /// complementary filter to combine the accelerator and the gyro data.
    /// Properties are defined for databindig of the view.
    /// </summary>
    /// <remarks>
    /// roll and pitch angles are valid in the range of +- 90 deg
    /// </remarks>
    /// <includesource>yes</includesource>
    public class CalcValues : INotifyPropertyChanged, IObserver<ReceivedRawData>
    {
        //private ReceivedRawData theReceivedData;
        private IDisposable unsubscriber;
        private short timer3Count = 1333;
        private double deltaX, deltaY, oldDeltaX, oldDeltaY;
        private int touchPointCounter;
        private double currentCurrent = 8.4;
        private double[] currentCurrentArray = new double[20];
        int currentArrayCounter = 0;
        private double[] dechargeArray = new double[20];
        int dechargeArrayCounter = 0;
        private int dechargeMAH;
        private IList<DataPoint> points;
        private IList<DataPoint> points2;

        private double yawAngle;
        private double pitchAngle, pitchAngleScaled;
        private double rollAngle;

        private double longitude;
        private double latitude;

        private Location currentLocation;

        public Location CurrentLocation
        {
            get { return currentLocation; }
            set
            {
                currentLocation = value;
                NotifyChangedValue(new PropertyChangedEventArgs("CurrentLocation"));
            }
        }

        public double Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                NotifyChangedValue(new PropertyChangedEventArgs("Longitude"));
            }
        }

        public double Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                NotifyChangedValue(new PropertyChangedEventArgs("Latitude"));
            }
        }


        public double YawAngle
        {
            get { return yawAngle; }
            set
            {
                yawAngle = value;
                NotifyChangedValue(new PropertyChangedEventArgs("YawAngle"));
            }
        }

        public double PitchAngle
        {
            get { return pitchAngle; }
            set
            {
                pitchAngle = value;
                NotifyChangedValue(new PropertyChangedEventArgs("PitchAngle"));
            }
        }

        public double PitchAngleScaled
        {
            get { return pitchAngleScaled; }
            set
            {
                pitchAngleScaled = value;
                NotifyChangedValue(new PropertyChangedEventArgs("PitchAngleScaled"));
            }
        }

        public double RollAngle
        {
            get { return rollAngle; }
            set
            {
                rollAngle = value;
                NotifyChangedValue(new PropertyChangedEventArgs("RollAngle"));
            }
        }

        public IList<DataPoint> Points
        {
            get { return points; }
            set
            {
                points = value;
                NotifyChangedValue(new PropertyChangedEventArgs("Points"));
            }
        }

        public IList<DataPoint> Points2
        {
            get { return points2; }
            set
            {
                points2 = value;
                NotifyChangedValue(new PropertyChangedEventArgs("Points2"));
            }
        }

        public int DechargeMAH
        {
            get { return dechargeMAH; }
            set
            {
                dechargeMAH = value;
                NotifyChangedValue(new PropertyChangedEventArgs("DechargeMAH"));
            }
        }

        public double CurrentCurrent
        {
            get { return currentCurrent; }
            set
            {
                currentCurrent = value;
                NotifyChangedValue(new PropertyChangedEventArgs("CurrentCurrent"));
            }
        }

        public short Timer3Count
        {
            get { return timer3Count; }
            set
            {
                timer3Count = value;
                NotifyChangedValue(new PropertyChangedEventArgs("Timer3Count"));
            }
        }

        private int newAngle;

        public int NewAngle
        {
            get { return newAngle; }
            set
            {
                newAngle = value;
                NotifyChangedValue(new PropertyChangedEventArgs("NewAngle"));
            }
        }

        public void showLonLatInWaypoints(ReceivedRawData data)
        {
            Longitude = (double)(data.LonB4 + data.LonB3*256 + data.LonB2*256*256 + data.LonB1*256*256*256)/10000000;
            Latitude = (double)(data.LatB4 + data.LatB3 * 256 + data.LatB2 * 65536 + data.LatB1 * 16777216)/10000000;
            CurrentLocation = new Location(Latitude,Longitude);
            Console.Out.WriteLine("This is the Longitude: " + data.LonB4);

            //Location pinLocation = myMap.ViewportPointToLocation(CurrentLocation);

            //// The pushpin to add to the map.
            //Pushpin pin = new Pushpin();
            //pin.Location = pinLocation;

            //// Adds the pushpin to the map.
            //myMap.Children.Add(pin);



        }


        public void showAnglesInCockpit(ReceivedRawData data)
        {
            YawAngle = MathHelper.MakeInt(data.YanMsb, data.YanLsb);
            RollAngle = MathHelper.MakeInt(data.RanMsb, data.RanLsb);
            PitchAngle = MathHelper.MakeInt(data.PanMsb, data.PanLsb);
            PitchAngleScaled = MathHelper.MakeInt(data.PanMsb, data.PanLsb)*(-0.5/45) + 1;
        }

        public void calcNewAngle(double x, double y)
        {
            Debug.WriteLine("X:  " + x);
            Debug.WriteLine("Y:  " + y);

            var h = ((Panel) Application.Current.MainWindow.Content).ActualHeight;
            var w = ((Panel) Application.Current.MainWindow.Content).ActualWidth;


            Debug.WriteLine("hight:  " + x);
            Debug.WriteLine("width:  " + y);


            deltaX = x - 850;
            deltaY = y - 400;

            touchPointCounter++;

            if (touchPointCounter > 2)
            {
                touchPointCounter = 2;

                if (!(deltaX == 0) || !(deltaY == 0))
                {
                    NewAngle = NewAngle +
                               (int)
                                   ((Math.Atan2(deltaY, deltaX)*(180/Math.PI)) -
                                    (Math.Atan2(oldDeltaY, oldDeltaX)*(180/Math.PI)));
                }
            }

            Debug.WriteLine("NewAngle =:  " + NewAngle);

            oldDeltaX = deltaX;
            oldDeltaY = deltaY;
        }

        public void resetNewAngle()
        {
            touchPointCounter = 0;
        }

        public void calculateTimer3Count(ReceivedRawData data)
        {
            Timer3Count = MathHelper.MakeInt(data.TtcMsb, data.TtcLsb);
            //Console.Out.WriteLine("This is the Timer3Count: ", Timer3Count);
        }

        public void calculateDischargeValue(ReceivedRawData data)
        {
            DechargeMAH = MathHelper.MakeInt(data.MahMsb, data.MahLsb)/50;
            Console.Out.WriteLine("This is the discharge in mAh: " + DechargeMAH);

            int i = 0;
            while (i < 19)
            {
                dechargeArray[i] = dechargeArray[i + 1];
                i++;
            }
            dechargeArray[19] = DechargeMAH;

            List<DataPoint> myPoints = new List<DataPoint>();

            i = 0;
            while (i < 20)
            {
                myPoints.Add(new DataPoint(i + 1, dechargeArray[i]));
                i++;
            }

            Points2 = myPoints;
       
    }

        public void calculateCurrentCurrent(ReceivedRawData data)
        {
            CurrentCurrent = (MathHelper.MakeInt(data.CcuMsb, data.CcuLsb) - 2047)*0.0078;
            Console.Out.WriteLine("This is the current current: " + CurrentCurrent);

            int i = 0;
            while (i < 19)
            {
                currentCurrentArray[i] = currentCurrentArray[i + 1];
                i++;
            }
            currentCurrentArray[19] = CurrentCurrent;

            List<DataPoint> myPoints = new List<DataPoint>();

            i = 0;
            while (i < 20)
            {
                myPoints.Add(new DataPoint(i + 1, currentCurrentArray[i]));
                i++;
            }

            Points = myPoints;
        }

        #region methods for INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChangedValue(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion

        #region methodes for observer pattern

        /// <summary>
        /// allows to unsubscribe to notification
        /// </summary>
        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        /// <summary>
        /// Data has changed, now execute the following methods
        /// </summary>
        /// <param name="value">Received Data</param>
        public void OnNext(ReceivedRawData value)
        {
            calculateTimer3Count(value);
            calculateCurrentCurrent(value);
            calculateDischargeValue(value);
            showAnglesInCockpit(value);
            showLonLatInWaypoints(value);
        }

        /// <summary>
        /// empty exception method of the observer functionality
        /// </summary>
        /// <param name="error">Exception error</param>
        public void OnError(Exception error)
        {
        }

        /// <summary>
        /// empty method to call after completed notification of the observer functionality
        /// </summary>
        public void OnCompleted()
        {
        }

        #endregion
    }
}