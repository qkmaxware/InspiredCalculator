using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using InspiredCalculator.Shared;
using Qkmaxware.Cas;
using Qkmaxware.Cas.Calculus;

namespace InspiredCalculator;

public abstract class InputMode {
    public abstract int SubExpressionCount();
    public abstract IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions);
}

public class AttachmentInputMode : InputMode {
    public string FileName {get; private set;}
    public IGraphics Attachment {get; private set;}
    public AttachmentInputMode(string name, IGraphics attachment) {
        this.FileName = name;
        this.Attachment = attachment;
    }
    public override int SubExpressionCount() => 0;
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        sideEffects.Graphics.Add(Attachment);
        return new Symbol(FileName);
    }
}

public abstract class DirectInputMode : InputMode {
    public abstract int InputLength {get;}
    public abstract void Clear();
    public abstract void DeleteFromBack();
    public abstract void AddDigit(int digit);
    public abstract void AddCharacter(char c);
}

public class ConstantInputMode : InputMode {
    public Constant Constant {get; private set;}
    public override int SubExpressionCount() => 0;
    public ConstantInputMode(Constant constant) {
        this.Constant = constant;
    }
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) => Constant.Value;
}

public class MatrixInputMode : InputMode {
    public int Rows {get; private set;}
    public int Columns {get; private set;}
    public override int SubExpressionCount() => Rows * Columns;
    public MatrixInputMode(int rows, int columns) {
        this.Rows = rows;
        this.Columns = columns;
    }
    public int Get1dIndex(int row, int column) {
        return (column + row * this.Columns);
    }
    public (int row, int column) Get2dIndex(int ind) {
        var y = ind / this.Columns;
        var x = ind % this.Columns;
        return (row: y, column: x);
    }
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != this.SubExpressionCount()) {
            throw new ArgumentException("Invalid number of matrix elements.");
        }
        foreach (var expr in subexpressions) 
            if (expr is null)
                throw new ArgumentException("Missing element expression.");

        Matrix m = new Matrix(this.Rows, this.Columns);
        for (var i = 0; i < m.Rows; i++) {
            for (var j = 0; j < m.Columns; j++) {
                var expr = subexpressions[Get1dIndex(i, j)];
                if (expr is null)
                    throw new ArgumentException("Missing element expression.");
                m[i, j] = expr;
            }
        }
        return m;
    }
}

public class DerivativeInputMode : InputMode {
    public override int SubExpressionCount() => 2;
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != this.SubExpressionCount()) {
            throw new ArgumentException("Invalid number of expressions.");
        }

        var symbol = subexpressions[0];
        var expr = subexpressions[1];

        if (symbol is not Symbol s) {
            throw new ArgumentException("Derivative symbol must not be an expression.");
        }
        if (expr is null)
            throw new ArgumentException("Missing expression to derivate.");
        
        return expr.Differentiate(s);
    }
}

public class IndefIntegralInputMode : InputMode {
    public override int SubExpressionCount() => 2;
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != this.SubExpressionCount()) {
            throw new ArgumentException("Invalid number of expressions.");
        }

        var symbol = subexpressions[1];
        var expr = subexpressions[0];

        if (symbol is not Symbol s) {
            throw new ArgumentException("Symbol of integration must not be an expression.");
        }
        if (expr is null)
            throw new ArgumentException("Missing expression to integrate.");
        
        return expr.Integrate(s);
    }
}

public class DefIntegralInputMode : InputMode {
    public override int SubExpressionCount() => 4;
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != this.SubExpressionCount()) {
            throw new ArgumentException("Invalid number of expressions.");
        }

        var symbol = subexpressions[3];
        var lower = subexpressions[0];
        var upper = subexpressions[1];
        var expr = subexpressions[2];

        if (symbol is not Symbol s) {
            throw new ArgumentException("Symbol of integration must not be an expression.");
        }
        if (lower is null)
            throw new ArgumentException("Missing integration lower bound.");
        if (upper is null)
            throw new ArgumentException("Missing integration upper bound.");
        if (expr is null)
            throw new ArgumentException("Missing expression to integrate.");
        
        return expr.Integrate(s, lower, upper);
    }
}

public class AtomInputMode : DirectInputMode {

    private StringBuilder input = new StringBuilder();
    public string String => input.ToString();
    public override int InputLength => input.Length;

    public override int SubExpressionCount() => 0;

    public AtomInputMode() {}
    public AtomInputMode(string str) {
        this.input.Append(str);
    }
    public AtomInputMode(int value) {
        this.input.Append(value);
    }

    public override void Clear() => input.Clear();
    public override void DeleteFromBack() {
        if (input.Length > 0 )
            input.Length -= 1;
    }
    public override void AddCharacter(char c) => input.Append(c);
    public override void AddDigit(int digit) => input.Append(digit);

    private static Regex number = new Regex(@"^[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)$");

    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        var value = input.ToString();
        var isVariable = !number.IsMatch(value);
        return isVariable switch {
            true => sideEffects.Variables.ContainsKey(value) ? sideEffects.Variables[value] : new Symbol(value),
            false => new Real(double.Parse(value)),
        };
    }
}

public class ParenthesizedInputMode : InputMode {
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != 1)
            throw new ArgumentException("Invalid number of parenthesized expressions");
        var lhs = subexpressions[0];
        if (lhs is null)
            throw new ArgumentException("Missing expression in brackets");
        return lhs;
    }
    public override int SubExpressionCount() => 1;
}

public class UnitConversionInputNode : InputMode {
    public Quantity Quantity {get; private set;}
    public string? FromUnit {get; set;}
    public string? ToUnit {get; set;}

    public UnitConversionInputNode(Quantity quantity) {
        this.Quantity = quantity;
    }

    private IExpression convert(IExpression from) {
        var fromUnits = Quantity.AllowedUnits.Where(unit => unit.Name == FromUnit).FirstOrDefault();
        var toUnits = Quantity.AllowedUnits.Where(unit => unit.Name == ToUnit).FirstOrDefault();
        if ((fromUnits is null)) {
            throw new ArgumentException($"Invalid unit '{FromUnit ?? "null"}' to convert from");
        }
        if ((toUnits is null)) {
            throw new ArgumentException($"Invalid unit '{ToUnit ?? "null"}' to convert to");
        }
        return toUnits.ConversionSet.FromBase(fromUnits.ConversionSet.ToBase(from));
    }

    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != 1)
            throw new ArgumentException("Invalid expressions count");
        var lhs = subexpressions[0];
        if (lhs is null)
            throw new ArgumentException("Missing expression");
        return convert(lhs);
    }
    public override int SubExpressionCount() => 1;
}

public abstract class BinaryOperatorInputMode : InputMode {
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != 2)
            throw new ArgumentException("Invalid number of operands");
        var lhs = subexpressions[0]; var rhs = subexpressions[1];
        if (lhs is null)
            throw new ArgumentException("Invalid left-hand side operand");
        if (rhs is null)
            throw new ArgumentException("Invalid right-hand side operand");
        return MakeOperator(lhs, rhs);
    }
    public abstract IExpression MakeOperator(IExpression lhs, IExpression rhs);
    public override int SubExpressionCount() => 2;
}

public class AssignmentInputMode : InputMode {
    public override int SubExpressionCount() => 2;
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
         if (subexpressions.Length != 2)
            throw new ArgumentException("Invalid number of operands");
        var lhs = subexpressions[0]; var rhs = subexpressions[1];
        if (lhs is not Symbol symbol)
            throw new ArgumentException("Invalid left-hand side operand");
        if (rhs is null)
            throw new ArgumentException("Invalid right-hand side operand"); 
        rhs = rhs.Simplify();
        sideEffects.Assignments[symbol.Identifier] = rhs;
        return rhs;
    }
}

public class FunctionInputMode : InputMode {
    public Func<IExpression, IExpression> transformer;
    public string FunctionName;

    public FunctionInputMode(string name, Func<IExpression, IExpression> transformer) {
        this.FunctionName = name;
        this.transformer = transformer;
    }

    public override int SubExpressionCount() => 1;
    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != 1)
            throw new ArgumentException("Invalid number of operands");
        var arg = subexpressions[0]; 
        if (arg is null)
            throw new ArgumentException("Invalid argument");
        return transformer(arg);
    }
}

public class AddOperatorInputMode : BinaryOperatorInputMode {
    public override IExpression MakeOperator(IExpression lhs, IExpression rhs) => new Addition(lhs, rhs);
}
public class SubOperatorInputMode : BinaryOperatorInputMode {
    public override IExpression MakeOperator(IExpression lhs, IExpression rhs) => new Subtraction(lhs, rhs);
}
public class MulOperatorInputMode : BinaryOperatorInputMode {
    public override IExpression MakeOperator(IExpression lhs, IExpression rhs) => new Multiplication(lhs, rhs);
}
public class DivOperatorInputMode : BinaryOperatorInputMode {
    public override IExpression MakeOperator(IExpression lhs, IExpression rhs) => new Division(lhs, rhs);
}
public class PowOperatorInputMode : BinaryOperatorInputMode {
    public override IExpression MakeOperator(IExpression lhs, IExpression rhs) => new Exponentiation(lhs, rhs);
}
public class RootOperatorInputMode : BinaryOperatorInputMode {
    public override IExpression MakeOperator(IExpression lhs, IExpression rhs) => new NthRoot(lhs, rhs);
}


public class Plot2dInputMode : InputMode {

    public double XMax {get; set;} = 10;
    public double XMin {get; set;} = -10;
    public double XStep {get; set;} = 0.1;

    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != this.SubExpressionCount()) {
            throw new ArgumentException("Invalid number of expressions.");
        }

        var equation = subexpressions[0];
        if (equation is null) {
            throw new ArgumentException("Equation cannot be null");
        }

        // TODO actually create the plot
        var min = Math.Min(XMin, XMax);
        var max = Math.Max(XMin, XMax);
        var step = Math.Abs(XStep);
        var x = new Symbol("x");
        var trace = new Js.Plotly.Lines();
        for (var i = min; i <= max; i+= step) {
            var y = equation.When(x == i).IterativeSimplify();
            if (y is Complex complex && complex.IsPurelyReal) {
                trace.Add(i, complex.Real);
            }
        }
        if (!trace.IsEmpty()) {
            sideEffects.Graphics.Add(new PlotGraphic(new Js.Plotly.Layout(), trace));
        }

        return equation;
    }

    public override int SubExpressionCount() => 1;
}

public class Plot3dInputMode : InputMode {

    public double XMax {get; set;} = 10;
    public double XMin {get; set;} = -10;
    public int XSubDivisions = 40;

    public double YMax {get; set;} = 10;
    public double YMin {get; set;} = -10;
    public int YSubDivisions = 40;

    public override IExpression MakeExpression(SideEffects sideEffects, IExpression?[] subexpressions) {
        if (subexpressions.Length != this.SubExpressionCount()) {
            throw new ArgumentException("Invalid number of expressions.");
        }

        var equation = subexpressions[0];
        if (equation is null) {
            throw new ArgumentException("Equation cannot be null");
        }

        // TODO actually create the plot
        var xmin = Math.Min(XMin, XMax);
        var xmax = Math.Max(XMin, XMax);
        var xsubs = Math.Max(1, this.XSubDivisions);
        var ymin = Math.Min(XMin, XMax);
        var ymax = Math.Max(XMin, XMax);
        var ysubs = Math.Max(1, this.YSubDivisions);
        var xstep = (xmax-xmin) / xsubs;
        var ystep = (ymax-ymin) / ysubs;
        var x = new Symbol("x"); var y = new Symbol("y");

        var trace = new Js.Plotly.Surface(xsubs + 1, ysubs + 1);
        for (var xp = 0; xp <= xsubs; xp++) {
            for (var yp = 0; yp <= ysubs; yp++) {
                var i = xmin + xp * xstep;
                var j = ymin + yp * ystep;
                var z = equation.When(x == i, y == j).IterativeSimplify();
                if (z is Complex complex && complex.IsPurelyReal) {
                    trace.Add(xp, yp, i, j, complex.Real);
                }
            }
        }
        if (!trace.IsEmpty()) {
            sideEffects.Graphics.Add(new PlotGraphic(new Js.Plotly.Layout(), trace));
        }

        return equation;
    }

    public override int SubExpressionCount() => 1;
}