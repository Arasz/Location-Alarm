using AutoMapper;
using System.Collections.Generic;

namespace CoreLibrary.Data.Persistence.Mapping
{
    public class ActiveDaysResolver : ITypeConverter<List<WeekDay>, string>, ITypeConverter<string, List<WeekDay>>
    {
        public string Convert(List<WeekDay> source, string destination, ResolutionContext context)
        {
            throw new System.NotImplementedException();
        }

        public List<WeekDay> Convert(string source, List<WeekDay> destination, ResolutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}