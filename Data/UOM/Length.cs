namespace InspiredCalculator;

public static partial class UOM {
    public static readonly Quantity Length = new Quantity(
        "length",
        // Standard metric units
        new Unit("Micrometres", new Conversion(0.000001 )),
        new Unit("Millimetres", new Conversion(0.001 )),
        new Unit("Centimetres", new Conversion(0.01 )),
        new Unit("Decimetres" , new Conversion(0.1 )),
        new Unit("Metres"     , new Conversion(1 )),
        new Unit("Dekametres" , new Conversion(10 )),
        new Unit("Hectometres", new Conversion(100 )),
        new Unit("Kilometres" , new Conversion(100 )),
        new Unit("Megametres" , new Conversion(100000 )),
        new Unit("Gigametres" , new Conversion(1E9 )),
        new Unit("Terametres" , new Conversion(1E12 )),

        // Imperial units
        new Unit("Inches"     , new Conversion(0.0254 )),
        new Unit("Feet"       , new Conversion(0.3048 )),
        new Unit("Yards"      , new Conversion(0.9144 )),
        new Unit("Miles"      , new Conversion(1609.34 ))
    );
};