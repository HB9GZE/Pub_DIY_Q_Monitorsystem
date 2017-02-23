//-----------------------------------------------------------------------
// <copyright file="ReceivedRawData.cs" company="Stefan Meyre>
//     Copyright (c) 2016 Stefan Meyre. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace VisualControlV3
{
    /// <summary>
    /// Holds a data set of raw sensor data
    /// </summary>
    public class ReceivedRawData
    {
        public byte AlpMsb { get; set; } //altitude pressure sensor 64523 = 645,23 m
        public byte AlpLsb { get; set; }

        public byte VltMsb { get; set; } //battery voltage
        public byte VltLsb { get; set; }

        public byte YanMsb { get; set; } //yaw
        public byte YanLsb { get; set; }

        public byte RanMsb { get; set; } //roll
        public byte RanLsb { get; set; }

        public byte PanMsb { get; set; } //pitch
        public byte PanLsb { get; set; }

        public byte LonB1 { get; set; }
        public byte LonB2 { get; set; }
        public byte LonB3 { get; set; }
        public byte LonB4 { get; set; }

        public byte LatB1 { get; set; }
        public byte LatB2 { get; set; }
        public byte LatB3 { get; set; }
        public byte LatB4 { get; set; }

        public byte CvgB1 { get; set; }
        public byte CvgB2 { get; set; }
        public byte CvgB3 { get; set; }
        public byte CvgB4 { get; set; }

        public byte AlgB1 { get; set; }
        public byte AlgB2 { get; set; }
        public byte AlgB3 { get; set; }
        public byte AlgB4 { get; set; }

        public byte HagB1 { get; set; }
        public byte HagB2 { get; set; }
        public byte HagB3 { get; set; }
        public byte HagB4 { get; set; }
    }
}