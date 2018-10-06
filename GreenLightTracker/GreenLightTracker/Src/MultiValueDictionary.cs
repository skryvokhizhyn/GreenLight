using System.Collections.Generic;

public class MultiValueDictionary<TKey, TValue>
{
    private Dictionary<TKey, List<TValue>> m_dictionary = new Dictionary<TKey, List<TValue>>();

    public void Add(TKey key, TValue value)
    {
        List<TValue> values;
        if (m_dictionary.TryGetValue(key, out values))
        {
            values.Add(value);
        }
        else
        {
            m_dictionary.Add(key, new List<TValue> { value });
        }
    }

    public void AddRange(TKey key, IEnumerable<TValue> values)
    {
        List<TValue> v;
        if (m_dictionary.TryGetValue(key, out v))
        {
            v.AddRange(values);
        }
        else
        {
            m_dictionary.Add(key, new List<TValue> (values));
        }
    }

    public bool Remove(TKey key)
    {
        return m_dictionary.Remove(key);
    }

    public bool TryGetValues(TKey key, out IEnumerable<TValue> values)
    {
        List<TValue> result;
        var res = m_dictionary.TryGetValue(key, out result);
        values = result;

        return res;
    }
}