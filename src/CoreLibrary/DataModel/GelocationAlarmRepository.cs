using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreLibrary.DataModel
{
    public class GelocationAlarmRepository
    {
        public int Create(GeolocationAlarm alarm)
        {
            return 0;
        }

        public void Delete(GeolocationAlarm alarm)
        {
        }

        public void DeleteAll()
        {
        }

        public GeolocationAlarm Read(int index)
        {
            return null;
        }

        public IEnumerable<GeolocationAlarm> ReadAll()
        {
            return null;
        }

        public GeolocationAlarm ReadMatching(Expression<Func<GeolocationAlarm, bool>> criteria)
        {
            return null;
        }

        public bool Update(GeolocationAlarm alarm)
        {
            return false;
        }
    }
}