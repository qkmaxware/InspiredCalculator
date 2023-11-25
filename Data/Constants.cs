using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Qkmaxware.Cas;

namespace InspiredCalculator;

public class Constant {
    public static readonly ReadOnlyCollection<IGrouping<string, Constant>> ConstantByGroup = new List<Constant> {
        // https://en.wikipedia.org/wiki/List_of_mathematical_constants
        new Constant("Mathematical Constants", "0", (Real)0, string.Empty, "Zero"),
        new Constant("Mathematical Constants", "1", (Real)1, string.Empty, "One"),
        new Constant("Mathematical Constants", "-1", (Real)(-1), string.Empty, "Negative One"),
        new Constant("Mathematical Constants", "2", (Real)2, string.Empty, "Two"),
        new Constant("Mathematical Constants", "½", (Real)0.5, string.Empty, "One Half"),
        new Constant("Mathematical Constants", "π", (Real)Math.PI, string.Empty, "Pi") { UID = "Pi" },
        new Constant("Mathematical Constants", "τ", (Real)6.28318_53071_79586_47692, string.Empty, "Tau Ratio"),
        new Constant("Mathematical Constants", "√2", (Real)1.41421_35623_73095_04880, string.Empty, "Square Root of 2"),
        new Constant("Mathematical Constants", "√3", (Real)1.73205_08075_68877_29352, string.Empty, "Square Root of 3"),
        new Constant("Mathematical Constants", "√5", (Real)2.23606_79774_99789_69640, string.Empty, "Square Root of 5"),
        new Constant("Mathematical Constants", "φ", (Real)1.61803_39887_49894_84820, string.Empty, "Golden Ratio"),
        new Constant("Mathematical Constants", "δ", "s", (Real)2.41421_35623_73095_04880 , string.Empty, "Silver Ratio"),
        new Constant("Mathematical Constants", "i", new Complex(0, 1) , string.Empty, "Imaginary Unit") { UID = "i" },
        new Constant("Mathematical Constants", "K`", (Real)0.11494_20448_53296_20070 , string.Empty, "Kepler-Bouwkamp Constant"),
        new Constant("Mathematical Constants", "w", (Real)2.09455_14815_42326_59148 , string.Empty, "Wallis' Constant"),
        new Constant("Mathematical Constants", "e", (Real)Math.E , string.Empty, "Euler's Number") { UID = "e" },
        new Constant("Mathematical Constants", "ln2", (Real)(Math.Log(2)) , string.Empty, "Natural Logarithm of 2"),
        // https://en.wikipedia.org/wiki/List_of_physical_constants
        new Constant("Physical Constants", "c", (Real)(299792458) , "m⋅s⁻¹", "Speed of Light in a Vacuum"),
        new Constant("Physical Constants", "h", (Real)(6.62607015e-34) , "J⋅Hz⁻¹", "Planck Constant"),
        new Constant("Physical Constants", "k", "B", (Real)(1.380649e-23) , "J⋅K⁻¹", "Boltzmann Constant"),
        new Constant("Physical Constants", "G", (Real)(6.67430e-11) , "m³⋅kg⁻¹⋅s⁻²", "Newtonian Constant of Gravitation	"),
        new Constant("Physical Constants", "m", "α", (Real)6.6446573357e-27, "kg", "Alpha Particle Mass"),
        new Constant("Physical Constants", "m", "e", (Real)9.1093837015e-31, "kg", "Electron Particle Mass"),
        new Constant("Physical Constants", "m", "p", (Real)1.67262192369e-27, "kg", "Proton Particle Mass"),
        new Constant("Physical Constants", "m", "n", (Real)1.67492749804e-28, "kg", "Neutron Particle Mass"),
        new Constant("Physical Constants", "m", "μ", (Real)1.883531627e-28, "kg", "Muon Particle Mass"),
        new Constant("Physical Constants", "m", "τ", (Real)3.16754e-27, "kg", "Tau Particle Mass"),
        new Constant("Physical Constants", "m", "t", (Real)3.0784e-25, "kg", "Top Quark Mass"),
        new Constant("Physical Constants", "N", "A", (Real)6.02214076e23, "mol⁻¹", "Avogadro's Number"),
        new Constant("Physical Constants", "μ", (Real)1.66053906660e-27, "kg", "Atomic Mass Unit"),
        
    }.GroupBy(x => x.Group).ToList().AsReadOnly();
    public static IEnumerable<Constant> All => ConstantByGroup.SelectMany(x => x);

    public string Group {get; private set;}
    public string RootSymbol {get; private set;}
    public string SubscriptSymbol {get; private set;}
    public string Description {get; private set;}
    public Complex Value {get; private set;}
    public string UnitOfMeasure {get; private set;}

    public string? UID {get; set;}

    public Constant(string group, string symbol, Complex value, string units, string desc = "") {
        this.Group = group;
        this.RootSymbol = symbol;
        this.SubscriptSymbol = string.Empty;
        this.Value = value;
        this.UnitOfMeasure = units;
        this.Description = desc;
    }
    public Constant(string group, string symbol, string subscript, Complex value, string units, string desc = "") {
        this.Group = group;
        this.RootSymbol = symbol;
        this.SubscriptSymbol = subscript;
        this.Value = value;
        this.UnitOfMeasure = units;
        this.Description = desc;
    }
}