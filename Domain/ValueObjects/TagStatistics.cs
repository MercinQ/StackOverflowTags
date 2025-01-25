namespace Domain.ValueObjects
{
    public class TagStatistics
    {
        public string Name { get; }
        public double Percentage { get; }

        public TagStatistics(string name, double percentage)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            Name = name;
            Percentage = percentage;
        }
    }
}
