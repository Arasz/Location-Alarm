using GalaSoft.MvvmLight.Messaging;

namespace ArrivalAlarm.Messages
{
    internal class MapMessage : MessageBase
    {
        public MapMessage()
        {
        }

        public MapMessage(object sender) : base(sender)
        {
        }

        public MapMessage(object sender, object target) : base(sender, target)
        {
        }
    }
}