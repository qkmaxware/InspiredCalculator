namespace InspiredCalculator;

public static partial class UOM {
    public static readonly Quantity Time = new Quantity(
        "time",
        // Standard metric units
        new Unit("Seconds"     , new Conversion(1 )),
        new Unit("Minutes"     , new Conversion(60)),
        new Unit("Hours"     , new Conversion(3600))
    );
};