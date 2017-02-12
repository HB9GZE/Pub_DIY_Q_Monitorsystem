//-----------------------------------------------------------------------
// <copyright file="Unsubscriber.cs" company="Stefan Meyre>
//     Copyright (c) 2016 Stefan Meyre. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace VisualControlV3
{
    /// <summary>
    /// Holds the Observer and Observabels 
    /// </summary>
    public class Unsubscriber : IDisposable
    {
        private List<IObserver<ReceivedRawData>> observers;
        private IObserver<ReceivedRawData> observer;

        /// <summary>
        /// Initializes a new instance of the Unsubscriber class
        /// </summary>
        /// <param name="observers">the observers</param>
        /// <param name="observer">the observer</param>
        internal Unsubscriber(
            List<IObserver<ReceivedRawData>> observers,
            IObserver<ReceivedRawData> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }

        /// <summary>
        /// An observer can dispose from an observable
        /// </summary>
        public void Dispose()
        {
            if (observer != null && observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }
    }
}