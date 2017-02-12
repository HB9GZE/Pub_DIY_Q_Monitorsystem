//-----------------------------------------------------------------------
// <copyright file="MathHelper.cs" company="Stefan Meyre>
//     Copyright (c) 2016 Stefan Meyre. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace VisualControlV3
{
    /// <summary>
    /// Provides some mathematical equations to calculate the angles
    /// </summary>
    public class MathHelper
    {
        /// <summary>
        /// Converts a msb and a lsb byte to one short number (signed)
        /// </summary>
        /// <param name="msb">most significant byte</param>
        /// <param name="lsb">lowest significant byte</param>
        /// <returns>short made out of tho bytes</returns>
        public static short MakeInt(byte msb, byte lsb)
        {
            return (short)((msb << 8) + lsb);
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="radAngle">angle in radians</param>
        /// <returns>angle in degrees</returns>
        public static double ConvertRadToDeg(double radAngle)
        {
            return radAngle * (180.0 / Math.PI);
        }

 
    }
}