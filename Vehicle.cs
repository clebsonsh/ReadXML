namespace ReadXML
{
    public class Vehicle
    {
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Color { get; set; } = "";
        public string Vin { get; set; } = "";
        public string Condition { get; set; } = "";
        public string Plate { get; set; } = "";
        public int Year { get; set; }
        public int Doors { get; set; }
        public string Transmission { get; set; } = "";
        public string Engine { get; set; } = "";
        public string FuelType { get; set; } = "";
        public string Description { get; set; } = "";
        public int Mileage { get; set; }
        public string UnitCode { get; set; } = "";
        public string PriceMSRP { get; set; } = "";
        public string PriceListed { get; set; } = "";
        public string Currency { get; set; } = "";
        public string Notes { get; set; } = "";
        public List<string> Images { get; set; } = new List<string>();

    }
}
