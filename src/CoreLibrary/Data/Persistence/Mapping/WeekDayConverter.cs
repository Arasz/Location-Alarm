using AutoMapper;

namespace CoreLibrary.Data.Persistence.Mapping
{
    public class WeekDayConverter : ITypeConverter<string, WeekDay>, ITypeConverter<WeekDay, string>
    {
        public WeekDay Convert(string source, WeekDay destination, ResolutionContext context) => new WeekDay(source);

        public string Convert(WeekDay source, string destination, ResolutionContext context) => source.Name;
    }
}