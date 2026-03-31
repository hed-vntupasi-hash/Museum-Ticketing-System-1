using System.Text;

public class CodeGenerator
{
    private readonly PersistentCounter _counter;
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const long ObfuscationKey = 0x5A5A5A5A5A5A5A5A;
    private const long Multiplier = 6364136223846793005;

    public CodeGenerator(string counterFilePath)
    {
        _counter = new PersistentCounter(counterFilePath);
    }

    public long GetNextId()
    {
        return _counter.Next();
    }

    public string GenerateCode(long id)
    {
        long obfuscated = (id ^ ObfuscationKey) * Multiplier;
        string encoded = EncodeBase62(obfuscated);
        return encoded.PadLeft(12, '0');
    }

    private static string EncodeBase62(long value)
    {
        if (value == 0) return "0";

        var result = new StringBuilder();
        ulong v = (ulong)value;

        while (v > 0)
        {
            result.Insert(0, Base62Chars[(int)(v % 62)]);
            v /= 62;
        }

        return result.ToString();
    }
}