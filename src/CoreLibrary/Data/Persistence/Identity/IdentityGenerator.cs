using CoreLibrary.Data.DataModel.Base;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CoreLibrary.Data.Persistence.Identity
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class IdentityGenerator : IIdentityGenerator
    {
        [DataMember, JsonProperty]
        private int _lastIdentity;

        public int LastIdentity
        {
            get { return _lastIdentity++; }
            private set { _lastIdentity = value; }
        }

        [DataMember, JsonProperty]
        private Queue<int> IdentityQueue { get; } = new Queue<int>();

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