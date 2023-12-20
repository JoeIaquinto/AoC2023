using System.Collections;

namespace Common;

/// <summary>
/// Represents a collection of keys and values. Querying a value using a key which is not in the collection
/// returns a default value. This default value can be static, or generated using a delegate specified during
/// construction.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
