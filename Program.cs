using System.Xml.Linq;
using ReadXML;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/", (HttpContext content) =>
{
    var reader = new StreamReader(content.Request.Body);
    var xml = reader.ReadToEndAsync().Result;

    XDocument doc = XDocument.Parse(xml);
    XNamespace ns = "http://www.starstandard.org/STAR/5";

    var vehicles = doc.Descendants(ns + "VehicleInventoryInvoice")
        .Select(v => new Vehicle
        {

            Brand = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "MakeString")
                ?.Value ?? "",

            Model = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "ModelDescription")
                ?.Value ?? "",

            Color = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "ColorGroup")
                ?.Elements(ns + "ColorItemCode")
                ?.Where(c => c.Value == "Exterior")
                ?.Select(c => c.Parent?.Element(ns + "ColorName")?.Value)
                ?.FirstOrDefault() ?? "",

            Vin = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "VehicleID")
                ?.Value ?? "",

            Condition = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "Condition")
                ?.Value ?? "",

            Plate = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "LicenseNumberString")
                ?.Value ?? "",

            Year = int.Parse(v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "ModelYear")
                ?.Value ?? "0"),

            Doors = int.Parse(v
                .Descendants(ns + "DoorsQuantityNumeric")
                .Single()
                ?.Value ?? "0"),

            Transmission = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "TransmissionGroup")
                ?.Element(ns + "TransmissionTypeName")
                ?.Value ?? "",

            Engine = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "Engine")
                ?.Element(ns + "GeneralEngineDescription")
                ?.Value ?? "",

            FuelType = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "Engine")
                ?.Element(ns + "FuelTypeCode")
                ?.Value ?? v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "FuelTypeCode")
                ?.Value ?? "",

            Description = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "VehicleDescription")
                ?.Value ?? "",

            Mileage = int.Parse(v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "DeliveryDistanceMeasure")
                ?.Value ?? "0"),

            UnitCode = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "DeliveryDistanceMeasure")
                ?.Attribute("unitCode")
                ?.Value ?? "",

            PriceMSRP = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Pricing")
                ?.Elements(ns + "Price")
                ?.Elements(ns + "PriceCode")
                ?.Where(p => p.Value == "Base MSRP")
                ?.Select(p => p.Parent?.Element(ns + "ChargeAmount")?.Value)
                .FirstOrDefault() ?? "",

            PriceListed = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Pricing")
                ?.Elements(ns + "Price")
                ?.Elements(ns + "PriceCode")
                ?.Where(p => p.Value == "List")
                ?.Select(p => p.Parent?.Element(ns + "ChargeAmount")?.Value)
                .FirstOrDefault() ?? "",

            Currency = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Pricing")
                ?.Elements(ns + "Price")
                ?.Elements(ns + "PriceCode")
                ?.Where(p => p.Value == "Base MSRP")
                ?.Select(p => p.Parent?.Element(ns + "ChargeAmount")?.Attribute("currencyID")?.Value)
                .FirstOrDefault() ?? v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Pricing")
                ?.Elements(ns + "Price")
                ?.Elements(ns + "PriceCode")
                ?.Where(p => p.Value == "List")
                ?.Select(p => p.Parent?.Element(ns + "ChargeAmount")?.Attribute("currencyID")?.Value)
                .FirstOrDefault() ?? "",

            Notes = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "VehicleNote")
                ?.Value ?? "",

            Images = v
                .Element(ns + "VehicleInventoryVehicleLineItem")
                ?.Element(ns + "Vehicle")
                ?.Element(ns + "VehicleIdentificationGroup")
                ?.Elements(ns + "VehicleID").Select(i => i.Value.Trim()).ToList() ?? new List<string>(),
        });

    string bac = doc
    .Element(ns + "ShowVehicleInventory")
    ?.Element(ns + "ApplicationArea")
    ?.Element(ns + "Sender")
    ?.Element(ns + "DealerNumberID")
    ?.Value ?? "";

    string dms = doc
        .Element(ns + "ShowVehicleInventory")
        ?.Element(ns + "ApplicationArea")
        ?.Element(ns + "Sender")
        ?.Element(ns + "SenderNameCode")
        ?.Value ?? "";


    Inventory inventory = new(
        Header: new(bac, dms),
        Vehicles: vehicles.ToList()
    );

    return inventory;
}).Accepts<HttpRequest>("application/xml");
app.Run();
