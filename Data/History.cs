using System.Collections;
using Qkmaxware.Cas;

namespace InspiredCalculator;

public class History : IList<History.Record> {
    public class Record {
        public TreeBuilderNode? InputExpression;
        public object? Result;

        public string? RawText;

        public Record() {}
        public Record(string? text) {
            this.RawText = text;
        }
        public Record(TreeBuilderNode input, object? result) {
            this.InputExpression = input;
            this.Result = result;
        }
    }

    private List<Record> records = new List<Record>();

    public int Count => this.records.Count;

    public bool IsReadOnly => false;

    public Record this[int index] { get => this.records[index]; set => this.records[index] = value; }

    public void AddAll(params Record[] records) {
        foreach (var record in records)
            this.records.Add(record);
    }

    public void AddRange(IEnumerable<Record> records) {
        foreach (var record in records)
            this.records.Add(record);
    }

    public void Add(Record record) {
        this.records.Add(record);
    }

    public void Clear() {
        this.records.Clear();
    }

    public IEnumerator<Record> GetEnumerator() => this.records.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.records.GetEnumerator();

    public int IndexOf(Record item) => records.IndexOf(item);

    public void Insert(int index, Record item) => this.records.Insert(index, item);

    public void RemoveAt(int index) => this.records.RemoveAt(index);

    public bool Contains(Record item) => this.records.Contains(item);

    public void CopyTo(Record[] array, int arrayIndex) => this.records.CopyTo(array, arrayIndex);

    public bool Remove(Record item) => this.records.Remove(item);
}