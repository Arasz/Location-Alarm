﻿using CoreLibrary.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CoreLibrary.Data.Persistence
{
    [DataContract]
    public class IdentityGenerator : IIdentityGenerator
    {
        [DataMember]
        private int _lastIdentity;

        [DataMember]
        public Queue<int> IdentityQueue { get; private set; }

        public int LastIdentity
        {
            get { return _lastIdentity++; }
            private set { _lastIdentity = value; }
        }

        public void AssignUniqueIdentity<TEntity>(TEntity entity) where TEntity : IEntity
        {
            FillQueue();
            entity.Id = IdentityQueue.Dequeue();
        }

        private void FillQueue()
        {
            if (IdentityQueue.Count >= 1) return;
            foreach (var identity in Enumerable.Range(LastIdentity, 10))
                IdentityQueue.Enqueue(identity);
            LastIdentity = IdentityQueue.Peek();
        }
    }
}