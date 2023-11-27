namespace InspiredCalculator.IO;

public class CsvFile {

    public class Row {
        private Cell[] cells;
        public int Columns => cells.Length;
        public Cell this[int col] => col >= 0 && col < cells.Length ? cells[col] : Cell.Empty;
        public Row (Cell[] cells) {
            this.cells = cells;
        }
    }

    public class Cell {
        public static readonly Cell Empty = new Cell(string.Empty);
        public string Value {get; private set;}
        public Cell(string value) {
            this.Value = value;
        }
    }

    public int Rows => rows.Count;
    public int Columns {get; private set;}
    private List<Row> rows = new List<Row>();
    public Cell this[int row, int col] => this.rows[row][col];

    private void Add(Row row) {
        this.rows.Add(row);
        this.Columns = Math.Max(Columns, row.Columns);
    }

    public static CsvFile Parse(char separator, string content) {
        CsvFile file = new CsvFile();
        foreach (var line in content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) {
            var cells = line.Split(separator, StringSplitOptions.TrimEntries).Select(x => new Cell(x)).ToArray();
            var row = new Row(cells);
            file.Add(row);
        }
        return file;
    }

    public string[,] ToMatrix(bool hasHeaders = false) {
        var rows = hasHeaders ? this.Rows -1 : this.Rows;
        var start = hasHeaders ? 1 : 0;
        var str = new string[rows, this.Columns];
        for (var row = start; row < this.Rows; row++) {
            for (var col = 0; col < this.Columns; col++) {
                str[row - start, col] = this[row, col].Value;
            }
        }
        return str;
    }
}