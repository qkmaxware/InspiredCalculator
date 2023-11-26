namespace InspiredCalculator;

public static partial class UOM {
    public static readonly Quantity Current = new Quantity(
        "current",
        // Standard metric units
        new Unit("Microampere", new Conversion(0.000001 )),
        new Unit("Milliampere", new Conversion(0.001 )),
        new Unit("Centiampere", new Conversion(0.01 )),
        new Unit("Deciampere" , new Conversion(0.1 )),
        new Unit("Ampere"     , new Conversion(1 )),
        new Unit("Dekaampere" , new Conversion(10 )),
        new Unit("Hectoampere", new Conversion(100 )),
        new Unit("Kiloampere" , new Conversion(100 )),
        new Unit("Megaampere" , new Conversion(100000 )),
        new Unit("Gigaampere" , new Conversion(1E9 )),
        new Unit("Teraampere" , new Conversion(1E12 ))
    );
};