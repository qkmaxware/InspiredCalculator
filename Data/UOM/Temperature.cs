using Qkmaxware.Cas;

namespace InspiredCalculator;

public static partial class UOM {
    public static readonly Quantity Temperature = new Quantity(
        "temp",
        new Unit("°K", new Conversion(
            fromBase: (k) => k,
            toBase: (k) => k
        )),
        new Unit("°C", new Conversion(
            toBase: (c) => new Addition(c, new Real(273.15)),
            fromBase: (k) => new Subtraction(k, new Real(273.15))
        )),
        new Unit("°F", new Conversion(
            toBase: (f) => (new Subtraction(f, new Real(32)) / 1.79999999) + 273.15,
            fromBase: (k) => 1.79999999 * new Subtraction(k, new Real(273.15)) + 32
        ))
    );
};