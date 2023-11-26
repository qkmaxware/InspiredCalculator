namespace InspiredCalculator;

public static partial class UOM {
    public static readonly Quantity Mass = new Quantity(
        "mass",
        // Standard metric units
        new Unit("Micrograms", new Conversion(0.000001 )),
        new Unit("Milligrams", new Conversion(0.001 )),
        new Unit("Centigrams", new Conversion(0.01 )),
        new Unit("Decigrams" , new Conversion(0.1 )),
        new Unit("Grams"     , new Conversion(1 )),
        new Unit("Dekagrams" , new Conversion(10 )),
        new Unit("Hectograms", new Conversion(100 )),
        new Unit("Kilograms" , new Conversion(100 )),
        new Unit("Megagrams" , new Conversion(100000 )),
        new Unit("Gigagrams" , new Conversion(1E9 )),
        new Unit("Teragrams" , new Conversion(1E12 )),

        // Imperial units
        new Unit("Pounds"     , new Conversion(453.592 ))
    );
};