using Domain.Primitives;

namespace Domain.Entities
{
    public class Tag : Entity
    {
        public string Name { get; private set; }
        public int Count { get; private set; }

        protected Tag() { } 

        public Tag(string name, int count)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            Name = name;
            Count = count;
            Id = Guid.NewGuid();
        }
    }
}
