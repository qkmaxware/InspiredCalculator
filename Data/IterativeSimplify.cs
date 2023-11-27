using Qkmaxware.Cas;

namespace InspiredCalculator;

public static class ExpressionExtensions {
    public static readonly int MAX_ITERATIONS = 20;
    public static IExpression? IterativeSimplify(this IExpression? expr) {
        if (expr is null)
            return null;

        for (var i = 0; i < MAX_ITERATIONS; i++) {
            var next_expr = expr.Simplify();

            // Simplification failed, return the prior successful version
            if (next_expr is null)
                return expr;
            
            // Simplification made no changes, return prior successful version
            if (expr == next_expr)
                return expr;
            
            // Try again with a simpler expression
            expr = next_expr;
        }

        // Fallback, we have reached max iterations and haven't fully simplified, return the best one we've got
        return expr;
    }
}