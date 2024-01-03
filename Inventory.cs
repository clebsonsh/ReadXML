namespace ReadXML
{
    public record Header(
        string BAC,
        string DMS
    );
    public record Inventory(
        Header Header,
        List<Vehicle> Vehicles
    );
}
