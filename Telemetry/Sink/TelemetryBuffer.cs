using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CCLCC.Telemetry.Sink
{
    using Interfaces;    

    public class TelemetryBuffer : ITelemetryBuffer
    {
        private const int DEFAULT_CAPACITY = 100;
        private const int DEFAUL_BACKLOG_LIMIT = 100000;
        private const int MINIMUM_BACKLOG_LIMIT = 1001;

        private readonly object lockObj = new object();
        private int capacity = DEFAULT_CAPACITY;
        private int backlogLimit = DEFAUL_BACKLOG_LIMIT;
        
        private List<ITelemetry> items;
        
        public Action OnFull { get; set; }

        public int Capacity
        {
            get
            {
                return this.capacity;
            }
            set
            {
                if (value < 1)
                {
                    this.capacity = DEFAUL_BACKLOG_LIMIT;
                    return;
                }

                if (value > this.backlogLimit)
                {
                    this.capacity = this.backlogLimit;
                    return;
                }

                this.capacity = value;
            }
        }
       
        public int BacklogLimit
        {
            get
            {
                return this.backlogLimit;
            }

            set
            {
                if (value < MINIMUM_BACKLOG_LIMIT)
                {
                    this.backlogLimit = MINIMUM_BACKLOG_LIMIT;
                    return;
                }

                if (value < this.capacity)
                {
                    this.backlogLimit = this.capacity;
                    return;
                }

                this.backlogLimit = value;
            }
        }

        internal TelemetryBuffer()
        {
            this.items = new List<ITelemetry>(this.Capacity);
        }

        public IEnumerable<ITelemetry> Dequeue()
        {
            List<ITelemetry> telemetryToFlush = null;

            if (this.items.Count > 0)
            {
                lock (this.lockObj)
                {
                    if (this.items.Count > 0)
                    {
                        telemetryToFlush = this.items;
                        this.items = new List<ITelemetry>(this.Capacity);                       
                    }
                }
            }

            return telemetryToFlush;
        }

        public void Enqueue(ITelemetry item)
        {
            if (item == null)
            {                
                return;
            }

            lock (this.lockObj)
            {
                if (this.items.Count >= this.BacklogLimit)
                {                    
                    return;
                }

                this.items.Add(item);
                if (this.items.Count >= this.Capacity)
                {
                    if (this.OnFull != null)
                    {
                        this.OnFull();
                    }
                }
            }
        }
    }
}
