using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Qkmaxware.Cas;

namespace InspiredCalculator.Js;

public class Plotly : JSInterop {

    public class FontStyle {
        public string? color {get; set;} = "rgb(127,127,127)";
    }

    public class Layout {
        public string? title { get; set; }
        public string? paper_bgcolor{ get; set; } = "rgba(0,0,0,0)";
        public string? plot_bgcolor{ get; set; } = "rgba(0,0,0,0)";
        public FontStyle font {get; set;} = new FontStyle();
    }

    public abstract class Trace {
        public string? name {get; set;}
        public abstract string type {get;}
        public abstract string mode {get;}
        public abstract object? x {get;}
        public abstract object? y {get;}
        public abstract object? z {get;}
        public abstract bool IsEmpty();
    }

    public class Lines : Trace {
        public override string type => "scatter";
        public override string mode => "lines";
        public List<double>? _x {get; set;} = new List<double>();
        public List<double>? _y {get; set;} = new List<double>();
        public List<double>? _z {get; set;} = new List<double>();
        public override object? x => _x;
        public override object? y => _y;
        public override object? z => _z;
        public override bool IsEmpty() => (_x?.Count ?? 0) == 0;
        public void Add(double x, double y) {
            this._x?.Add(x);
            this._y?.Add(y);
            this._z?.Add(0);
        }
    }

    public class Surface : Trace {
        public override string type => "surface";
        public override string mode => string.Empty;

        public Surface(int xs, int ys) {
            this.xs = new double[xs][];
            this.ys = new double[xs][];
            this.zs = new double[xs][];
            for (var i = 0; i < xs; i++) {
                this.xs[i] = new double[ys];
                this.ys[i] = new double[ys];
                this.zs[i] = new double[ys];
            }
        }
        private double [][] xs;
        private double [][] ys;
        private double [][] zs;
        public override object? x => xs;
        public override object? y => ys;
        public override object? z => zs;
        public override bool IsEmpty() => false;

        public void Add(int cellX, int cellY, double x, double y, double z) {
            this.xs[cellX][cellY] = x;
            this.ys[cellX][cellY] = y;
            this.zs[cellX][cellY] = z;
        }
    }

    public Plotly(IJSRuntime js) : base(js) {}

    public async Task Draw(ElementReference div, Trace[]? traces, Layout? layout) {
        await JavaScript.InvokeVoidAsync("plotlyInit", div, traces ?? new Trace[0], layout ?? new Layout());
    }
}
