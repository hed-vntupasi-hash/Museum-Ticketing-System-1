using System;
using System.IO;
using System.Threading;

public class PersistentCounter
{
    private readonly string _filePath;
    private long _current;
    private readonly object _lock = new object();

    public PersistentCounter(string filePath)
    {
        _filePath = filePath;
        _current = LoadCounter();
    }

    public long Next()
    {
        lock (_lock)
        {
            _current++;
            SaveCounter(_current);
            return _current;
        }
    }

    private long LoadCounter()
    {
        if (!File.Exists(_filePath))
            return 0;

        if (long.TryParse(File.ReadAllText(_filePath), out long value))
            return value;

        return 0;
    }

    private void SaveCounter(long value)
    {
        File.WriteAllText(_filePath, value.ToString());
    }
}