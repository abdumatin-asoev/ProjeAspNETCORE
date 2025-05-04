namespace ProjeAspNETCORE.Models
{
    public class SearchClass
    {
            public string Id { get; set; }
            public string Brand { get; set; }
            public string Type { get; set; }
            public string Color { get; set; }
            public string Transmission { get; set; }

            public List<Car> CarsFound { get; set; } = new List<Car>();
        
    }
}
