//-----------------------------------------------------------------------
// <copyright file="DataParser.cs" company="Stefan Meyre>
//     Copyright (c) 2016 Stefan Meyre. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;

namespace VisualControlV1
{
    /// <summary>
    /// Extracts from the received raw data the values
    /// </summary>
    /// <param name="incommingData">byte array with the data bytes from the serial com buffer</param>
    public class DataParser
    {
        /// <summary>
        /// Extracts from the received raw data the values  
        /// </summary>
        /// <param name="incommingData">byte array with the data bytes from the serial com buffer</param>
        /// <returns>no return value</returns>
        public static ReceivedRawData ParseIncommingData(byte[] incommingData)
        {
            var receivedRawData = new ReceivedRawData();
            var data = incommingData;
            byte[] nextRxByte = new byte[1];
            byte[] dataBoxByte = new byte[11];
            int commandIsReady = 0;
            int dataIsReady = 0;
            int receivedBytesCounter = 0;


            foreach (var abyte in data)
            {
                nextRxByte[0] = abyte;
                dataBoxByte[receivedBytesCounter] = nextRxByte[0];
                receivedBytesCounter++;

                if (receivedBytesCounter > 10)
                {
                    receivedBytesCounter = 1;
                }

                if (dataBoxByte[receivedBytesCounter - 1] == '#')
                {
                    receivedBytesCounter = 1;
                    commandIsReady = 0;
                    dataIsReady = 0;
                }

                if (commandIsReady == 1)
                {
                    if ((dataBoxByte[4] == '1') && (receivedBytesCounter == 7))
                    {
                        dataIsReady = 1; // only needed if a one byte sensor data value is used
                    }

                    if ((dataBoxByte[4] == '2') && (receivedBytesCounter == 8))
                    {
                        dataIsReady = 1;
                    }

                    if ((dataBoxByte[4] == '4') && (receivedBytesCounter == 10))
                    {
                        dataIsReady = 1;
                    }
                }

                if (dataBoxByte[receivedBytesCounter - 1] == '/')
                {
                    commandIsReady = 1;
                }

                if ((commandIsReady & dataIsReady) == 1) //used to be only if dataIsReady
                {
                    if (dataBoxByte[0] == '#')
                    {
                        if (dataBoxByte[1] == 't')
                        {
                            if (dataBoxByte[2] == 't')
                            {
                                if (dataBoxByte[3] == 'c')
                                {
                                    receivedRawData.TtcMsb = dataBoxByte[7];
                                    receivedRawData.TtcLsb = dataBoxByte[6];
                                }
                            }
                        }
                    }

                    if (dataBoxByte[0] == '#')
                    {
                        if (dataBoxByte[1] == 'c')
                        {
                            if (dataBoxByte[2] == 'c')
                            {
                                if (dataBoxByte[3] == 'u')
                                {
                                    receivedRawData.CcuMsb = dataBoxByte[7];
                                    receivedRawData.CcuLsb = dataBoxByte[6];
                                }
                            }
                        }
                    }

                    if (dataBoxByte[0] == '#')
                    {
                        if (dataBoxByte[1] == 'm')
                        {
                            if (dataBoxByte[2] == 'a')
                            {
                                if (dataBoxByte[3] == 'h')
                                {
                                    receivedRawData.MahMsb = dataBoxByte[7];
                                    receivedRawData.MahLsb = dataBoxByte[6];
                                }
                            }
                        }
                    }

                    if (dataBoxByte[0] == '#')
                    {
                        if (dataBoxByte[1] == 'y')
                        {
                            if (dataBoxByte[2] == 'a')
                            {
                                if (dataBoxByte[3] == 'n')
                                {
                                    receivedRawData.YanMsb = dataBoxByte[7];
                                    receivedRawData.YanLsb = dataBoxByte[6];
                                }
                            }
                        }
                    }

                    if (dataBoxByte[0] == '#')
                    {
                        if (dataBoxByte[1] == 'r')
                        {
                            if (dataBoxByte[2] == 'a')
                            {
                                if (dataBoxByte[3] == 'n')
                                {
                                    receivedRawData.RanMsb = dataBoxByte[7];
                                    receivedRawData.RanLsb = dataBoxByte[6];
                                }
                            }
                        }
                    }

                    if (dataBoxByte[0] == '#')
                    {
                        if (dataBoxByte[1] == 'p')
                        {
                            if (dataBoxByte[2] == 'a')
                            {
                                if (dataBoxByte[3] == 'n')
                                {
                                    receivedRawData.PanMsb = dataBoxByte[7];
                                    receivedRawData.PanLsb = dataBoxByte[6];
                                }
                            }
                        }
                    }

                    if (dataBoxByte[0] == '#')
                    {
                        if (dataBoxByte[1] == 'l')
                        {
                            if (dataBoxByte[2] == 'a')
                            {
                                if (dataBoxByte[3] == 't')
                                {
                                    receivedRawData.LatB1 = dataBoxByte[6];
                                    receivedRawData.LatB2 = dataBoxByte[7];
                                    receivedRawData.LatB3 = dataBoxByte[8];
                                    receivedRawData.LatB4 = dataBoxByte[9];
                                }
                            }
                        }
                    }

                    if (dataBoxByte[0] == '#')
                    {
                        if (dataBoxByte[1] == 'l')
                        {
                            if (dataBoxByte[2] == 'o')
                            {
                                if (dataBoxByte[3] == 'n')
                                {
                                    receivedRawData.LonB1 = dataBoxByte[6];
                                    receivedRawData.LonB2 = dataBoxByte[7];
                                    receivedRawData.LonB3 = dataBoxByte[8];
                                    receivedRawData.LonB4 = dataBoxByte[9];
                                }
                            }
                        }
                    }



                }

            }

            for (int i = 0; i < 11; i++)
                   {
                         dataBoxByte[i] = 0;
                   }
            commandIsReady = dataIsReady = receivedBytesCounter = 0;
              
            return receivedRawData;
        }
    }
 }
 
