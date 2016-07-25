using CoreLibrary.DataModel;
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

        public void AssignUniqueIdentity(IEntity entity)
        {
            FillQueue();
            entity.Id = IdentityQueue.Dequeue();
        }

        public void RecycleIdentity(IEntity entity) => IdentityQueue.Enqueue(entity.Id);

        public void RecycleIdentity(int id) => IdentityQueue.Enqueue(id);

        private void FillQueue()
        {
            if (IdentityQueue.Count >= 1) return;
            foreach (var identity in Enumerable.Range(LastIdentity, 10))
                IdentityQueue.Enqueue(identity);
            LastIdentity = IdentityQueue.Peek();
        }
    }
}