namespace LocationAlarm.ViewModels.DataTemplates
{
    public class DayOfWeekDataTemplate
    {
        public bool IsChosen { get; set; }

        public string Name { get; set; }

        public DayOfWeekDataTemplate(string name)
        {
            Name = name;
        }
    }
}