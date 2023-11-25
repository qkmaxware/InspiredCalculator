using Qkmaxware.Cas;

namespace InspiredCalculator;

public class Conversion {
    public Func<IExpression, IExpression> ToBase {get; private set;}
    public Func<IExpression, IExpression> FromBase {get; private set;}
    public Conversion(Func<IExpression, IExpression> toBase, Func<IExpression, IExpression> fromBase) {
        this.ToBase = toBase;
        this.FromBase = fromBase;
    }
}

public class Unit {
    public string Name {get; private set;}

    public Conversion ConversionSet {get; private set;}

    public Unit(string name, Conversion conversions) {
        this.Name = name;
        this.ConversionSet = conversions;
    }
}

public class Quantity {
    public string Name {get; private set;}
    private List<Unit> allowed_units = new List<Unit>();
    public IEnumerable<Unit> AllowedUnits => allowed_units.AsReadOnly();

    public Unit BaseUnit => allowed_units[0];

    public Quantity(string name, params Unit[] allowed) {
        this.Name = name;
        this.allowed_units.AddRange(allowed);
    }
}

public static partial class UOM {
    public static readonly Quantity Angles = new Quantity(
        "angle",
        new Unit("Degrees", new Conversion(
            toBase: (deg) => deg,
            fromBase: (deg) => deg 
        )),
        new Unit("Radians", new Conversion(
            toBase: (rad) =>  new Multiplication(rad, new Real(180/Math.PI)),
            fromBase: (deg) => new Multiplication(deg, new Real(Math.PI/180))
        ))
    );
}