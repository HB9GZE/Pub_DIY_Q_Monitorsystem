//-----------------------------------------------------------------------
// <copyright file="ReceivedRawData.cs" company="Stefan Meyre>
//     Copyright (c) 2016 Stefan Meyre. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace VisualControlV1
{
    /// <summary>
    /// Holds a data set of raw sensor data
    /// </summary>
    public class ReceivedRawData
    {
        /// <summary>
        /// Gets or sets the MSB/LSB of the timer3 (timer three counter) counter value
        /// </summary>
        public byte TtcMsb { get; set; }
        public byte TtcLsb { get; set; }

        /// <summary>
        ///  Gets or sets the MSB/LSB of the current current value
        /// </summary>
        public byte CcuMsb { get; set; }
        public byte CcuLsb { get; set; }

        /// <summary>
        ///  Gets or sets the MSB/LSB of the mAh decharge value
        /// </summary>
        public byte MahMsb { get; set; }
        public byte MahLsb { get; set; }

        /// <summary>
        ///  Gets or sets the MSB/LSB of the mAh decharge value
        /// </summary>
        public byte YanMsb { get; set; }
        public byte YanLsb { get; set; }

        /// <summary>
        ///  Gets or sets the MSB/LSB of the mAh decharge value
        /// </summary>
        public byte RanMsb { get; set; }
        public byte RanLsb { get; set; }

        /// <summary>
        ///  Gets or sets the MSB/LSB of the mAh decharge value
        /// </summary>
        public byte PanMsb { get; set; }
        public byte PanLsb { get; set; }

        public byte LonB1 { get; set; }
        public byte LonB2 { get; set; }
        public byte LonB3 { get; set; }
        public byte LonB4 { get; set; }

        public byte LatB1 { get; set; }
        public byte LatB2 { get; set; }
        public byte LatB3 { get; set; }
        public byte LatB4 { get; set; }


    }
}