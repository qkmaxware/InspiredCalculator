using Qkmaxware.Cas;
using System.Collections;
using System.Collections.Generic;

namespace InspiredCalculator;

public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable {
    #region IComparer<TKey> Members
    #nullable disable
    public int Compare(TKey x, TKey y) {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1; // Handle equality as being greater. Note: this will break Remove(key) or
        else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
            return result;
    }
    #nullable restore
    #endregion
}

public class DocumentStatistics {
    public int TotalLines {get; private set;}
    public int TextLines {get; private set;}
    public int TextCharacterCount {get; private set;}
    public int ExpressionLines {get; private set;}

    public int MetricsAbleExpressions {get; private set;}
    public double Sum {get; private set;} =0;
    public double Min {get; private set;} =0;
    public double Max {get; private set;} =0;
    public double Range => Max - Min;
    public double Median {get; private set;} =0;
    public double Mean => Sum / MetricsAbleExpressions;
    public double SquaredMean => Mean * Mean; 

    public DocumentStatistics(Document doc) {
        this.TotalLines = doc.History.Count;
        var values = new SortedList<double, double>(new DuplicateKeyComparer<double>());
        foreach (var record in doc.History) {
            if (record.RawText is not null) {
                this.TextLines++;
                this.TextCharacterCount += record.RawText.Length;
            }
            if (record.Result is not null && record.Result is IExpression expr) {
                this.ExpressionLines++;
                if (expr is Complex complex && complex.IsPurelyReal) {
                    this.MetricsAbleExpressions++;
                    this.Sum += complex.Real;
                    values.Add(complex.Real, complex.Real);
                    if (this.MetricsAbleExpressions == 1 || complex.Real < Min) {
                        this.Min = complex.Real;
                    }
                    if (this.MetricsAbleExpressions == 1 || complex.Real > Max) {
                        this.Max = complex.Real;
                    } 
                }
            }
        }
        if (values.Count % 2 == 0) {
            // Even 
            this.Median = (values[values.Count / 2] + values[values.Count/2 + 1]) / 2;
        } else {
            // Odd
            this.Median = values[(values.Count + 1) / 2];
        }
    }
}