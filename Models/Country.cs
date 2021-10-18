namespace RedisDemo.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Capital { get; set; }

        public override string ToString() =>
            $"id: {Id}, name: {Name}, capital: {Capital}";
    }
}