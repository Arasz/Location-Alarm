using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace CoreLibrary.Data.Persistence.Mapping
{
    internal class ListConverter : ITypeConverter<List<WeekDay>, string>, ITypeConverter<string, List<WeekDay>>
    {
        public string Convert(List<WeekDay> source, string destination, ResolutionContext context)
        {
            return source.Select(day => day.Name).Aggregate("", (accu, ele) => accu += $"{ele},").TrimEnd(',');
        }

        public List<WeekDay> Convert(string source, List<WeekDay> destination, ResolutionContext context)
        {
            return source.Split(',')
                .Select(day => new WeekDay(day))
                .ToList();
        }
    }
}