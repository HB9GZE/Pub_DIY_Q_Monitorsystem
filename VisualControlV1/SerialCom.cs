//-----------------------------------------------------------------------
// <copyright file="SerialCom.cs" company="Stefan Meyre">
//     Copyright (c) 2016 Stefan Meyre. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;

namespace VisualControlV3
{
    /// <summary>
    /// Establishes the serial communication and receives the send data
    /// </summary>
    public class SerialCom : IObservable<ReceivedRawData>
    {
        private const int DATABYTESBEFOREREAD = 60;
        private static SerialPort mySerialPort;
        private readonly List<IObserver<ReceivedRawData>> observers;
        private byte[] data;


        /// <summary>
        /// Gets or sets the RecievedRawdata
        /// </summary>
        public ReceivedRawData MyReceivedRawData { get; set; }

        /// <summary>
        /// Gets or sets the status information to be displayed
        /// </summary>
        public string Status { get; set; }

        public SerialPort MySerialPort
        {
            get { return mySerialPort; }

            set { mySerialPort = value; }
        }

        /// <summary>
        /// Initializes a new instance of the SerialCom class
        /// </summary>
        /// <param name="comPort">serial com port</param>
        public SerialCom(string comPort)
        {
            observers = new List<IObserver<ReceivedRawData>>();
            MyReceivedRawData = new ReceivedRawData();

            try
            {
                MySerialPort = new SerialPort(comPort, 57600, Parity.None, 8, StopBits.One);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
                MessageBox.Show("Please choose a valid com port.");
                Status = "Connection couldn't be established. ";
            }

            if (MySerialPort != null)
            {
                Status = "Connection established with " + comPort;
                MySerialPort.DataReceived += DataReceivedHandler;
                StartSerialCommunication();
            }
        }

        /// <summary>
        /// Opens a serial port and starts the serial communication in a thread
        /// </summary>
        public void StartSerialCommunication()
        {
            var mySerialPortThread = new Thread(OpenPort);
            mySerialPortThread.Start();
        }

        /// <summary>
        /// Handles the data received event. Reads the data from the serial buffer and lets them parse with the parser 
        /// </summary>
        /// <param name="s">Object sender</param>
        /// <param name="e">Serial data received event arguments</param>
        public void DataReceivedHandler(object s, SerialDataReceivedEventArgs e)
        {
            // Exception handling: InvalidOperationException is handled before, ArgumentNullException
            // doesn't pass if condition, ArgumentOutOfRangeException and ArgumentException doesn't appear because of relative count, 
            // TimeoutException can't occure because just triggered on new received data

            int howManyBytes = MySerialPort.BytesToRead;
            //Debug.WriteLine("How many bytes to read: " + howManyBytes);
            if (howManyBytes > DATABYTESBEFOREREAD)
            {
                MySerialPort.DiscardInBuffer();

                howManyBytes = 0;
            }
         
            
            if (howManyBytes == DATABYTESBEFOREREAD) //......................UPDATE-RATE!!
            {
                data = new byte[howManyBytes];
                MySerialPort.Read(data, 0, data.Length);
                MyReceivedRawData = DataParser.ParseIncommingData(data);
                DataChanged();
            }
            else
            {
                //MySerialPort.DiscardInBuffer();
            }
        }

        /// <summary>
        /// Opens the serial port.
        /// </summary>
        private void OpenPort()
        {
            try
            {
                if (!MySerialPort.IsOpen)
                {
                    MySerialPort.Open();
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You dont have the rights to access this port. Log in with administrator privileges.");
                Status = "Connection couldn't be established. ";
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show(
                    "Please check the serial properties (baudrate, parity, databits) on your microcontroller");
                Status = "Connection couldn't be established. ";
            }
            catch (IOException)
            {
                MessageBox.Show("Please choose a valid com port.");
                Status = "Connection couldn't be established. ";
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "An unexceptet error was raised. Pleas contact your systemadministrator and show him the following error message: \n\n" +
                    e);
                Status = "Connection couldn't be established. ";
            }
        }

        /// <summary>
        /// Stops the timer and closes the com port
        /// </summary>
        public void ClosePort()
        {
            try
            {
                if (MySerialPort.IsOpen)
                {
                    MySerialPort.Close();
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Port is in an invalid state");
            }
        }

        public void WriteByte(byte data)
        {
            var dataArray = new byte[] {data};
            MySerialPort.Write(dataArray, 0, 1);
        }


        /// <summary>
        /// Gives a reference to an interface that allows observers to stop receiving notifications 
        /// before the provider has finished sending them.
        /// </summary>
        /// <param name="observer">IObserver observer</param>
        /// <returns>new instance of the Unsubscriber class</returns>
        public IDisposable Subscribe(IObserver<ReceivedRawData> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return new Unsubscriber(observers, observer);

            //    if (provider != null)
            //    {
            //        unsubscriber = provider.Subscribe(this);
            //    }
        }

        /// <summary>
        /// Triggers the OnNext Method from the subscribed observers.
        /// </summary>
        public void DataChanged()
        {
            foreach (var observer in observers)
            {
                observer.OnNext(MyReceivedRawData);
            }
        }
    }
}