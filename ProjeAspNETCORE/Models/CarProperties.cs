namespace ProjeAspNETCORE.Models
{
    public class CarProperties
    {
        public static List<string> GetBrands()
        {
            return new List<string> {"Toyota", "BMW", "Honda","Mercedes","Bugatti","Nissan",
            "Mazda","Volkswagen","Audi","Ford","Porsche","Kia","Lamborghini", "Lexus" };
        }
        public static List<string> GetColors()
        {
            return new List<string> {
            "Black",
            "White",
            "Silver",
            "Gray",
            "Blue",
            "Red",
            "Green",
            "Brown",
            "Beige",
            "Yellow",
            "Orange",
            "Gold",
            "Burgundy",
            "Purple" };
        }

        public static List<string> GetType()
        {
            return new List<string> { "Petrol", "Diesel", "Electric", "Hybrid" };
        }
        public static List<string> GetTransmissions()
        {
            return new List<string> { "Automatic", "Manual", "CVT", "Robot" };
        }
    }
}
