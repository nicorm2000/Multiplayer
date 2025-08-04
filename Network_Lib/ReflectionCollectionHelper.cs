using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System;

namespace Net
{
    public class ReflectionCollectionHelper
    {
        private readonly Reflection reflection;

        /// <summary>
        /// Initializes the inspector with a reference to the core Reflection system.
        /// </summary>
        /// <param name="reflection">The main Reflection instance.</param>
        public ReflectionCollectionHelper(Reflection reflection)
        {
            this.reflection = reflection;
        }

        #region Helper Methods
        /// <summary>
        /// Attempts to get a value from a collection at a specific index.
        /// </summary>
        /// <param name="collection">The collection to read from.</param>
        /// <param name="index">The index to read.</param>
        /// <returns>The value at the specified index, or null if not found.</returns>
        public object? TryGetCollectionIndexValue(object collection, int index)
        {
            if (collection == null || index < 0)
                return null;

            Type type = collection.GetType();
            PropertyInfo indexer = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (indexer != null && indexer.CanRead)
            {
                try
                {
                    return indexer.GetValue(collection, new object[] { index });
                }
                catch
                {
                    return null;
                }
            }

            if (collection is IList list && index < list.Count)
            {
                return list[index];
            }

            return null;
        }

        /// <summary>
        /// Attempts to set a value in a collection at a specific index.
        /// </summary>
        /// <param name="collection">The collection to modify.</param>
        /// <param name="index">The index to write to.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        public bool TrySetCollectionIndexValue(object collection, int index, object value)
        {
            if (collection == null || index < 0) return false;

            Type type = collection.GetType();

            PropertyInfo indexer = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (indexer != null && indexer.CanWrite)
            {
                try
                {
                    indexer.SetValue(collection, value, new object[] { index });
                    //debugger?.Log($"[TrySetCollectionIndexValue] Set via indexer [{index}] = {value} on {type.Name}");
                    return true;
                }
                catch (Exception ex)
                {
                    //debugger?.Log($"[TrySetCollectionIndexValue] Indexer set failed: {ex.Message}");
                }
            }

            if (collection is IList list && index < list.Count)
            {
                try
                {
                    list[index] = value;
                    //debugger?.Log($"[TrySetCollectionIndexValue] Set via IList at index {index} to {value}");
                    return true;
                }
                catch (Exception ex)
                {
                    //debugger?.Log($"[TrySetCollectionIndexValue] IList set failed: {ex.Message}");
                }
            }

            MethodInfo? insertMethod = type.GetMethod("Insert", new[] { typeof(int), typeof(object) });
            if (insertMethod != null)
            {
                try
                {
                    insertMethod.Invoke(collection, new object[] { index, value });
                    //debugger?.Log($"[TrySetCollectionIndexValue] Inserted value at index {index}");
                    return true;
                }
                catch (Exception ex)
                {
                    //debugger?.Log($"[TrySetCollectionIndexValue] Insert failed: {ex.Message}");
                }
            }

            //debugger?.Log($"[TrySetCollectionIndexValue] Failed to set index {index} on {type.Name}");
            return false;
        }
        #endregion

        #region Dictionary Operations
        /// <summary>
        /// Handles removal of dictionary entries based on key hash.
        /// </summary>
        /// <param name="info">Field information for the dictionary.</param>
        /// <param name="obj">Parent object containing the dictionary.</param>
        /// <param name="keyHash">Hash of the key to remove.</param>
        /// <returns>The modified object.</returns>
        public object HandleDictionaryRemove(FieldInfo info, object obj, int keyHash)
        {
            IDictionary dictionary = (IDictionary)info.GetValue(obj);
            if (dictionary == null)
            {
                //debugger?.Log("ERROR: Dictionary is null");
                return obj;
            }

            //debugger?.Log($"Target Dictionary: {info.Name} | Current Keys: {string.Join(",", dictionary.Keys.Cast<object>())}");
            //debugger?.Log($"Searching for key with hash: {keyHash}");

            bool found = false;
            foreach (object key in dictionary.Keys)
            {
                int currentHash = ReflectionHelperMethods.GetStableKeyHash(key);
                if (currentHash == keyHash)
                {
                    //debugger?.Log($"FOUND KEY: {key} (Hash: {currentHash}) - REMOVING");
                    dictionary.Remove(key);
                    found = true;

                    // Update previous state if tracking
                    if (reflection.previousDictionaryStates.TryGetValue(dictionary, out Dictionary<object, int>? state))
                    {
                        state.Remove(key);
                    }
                    break;
                }
            }

            //if (!found) debugger?.Log("WARNING: No matching key found");
            //debugger?.Log($"Final Keys: {string.Join(",", dictionary.Keys.Cast<object>())}");
            return obj;
        }

        /// <summary>
        /// Handles writing values to dictionary entries.
        /// </summary>
        /// <param name="info">Field information for the dictionary.</param>
        /// <param name="obj">Parent object containing the dictionary.</param>
        /// <param name="attribute">NetVariable attribute of the field.</param>
        /// <param name="idRoute">Route information for the value.</param>
        /// <param name="idToRead">Current position in the route.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The modified object.</returns>
        public object HandleDictionaryWrite(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "HandleDictionaryWrite - ";
            debug += $"Field: {info.Name}, Current Route Index: {idToRead}\n";

            RouteInfo currentRoute = idRoute[idToRead];
            debug += $"RouteInfo: {currentRoute}\n";
            Type fieldType = info.FieldType;
            Type[] genericArgs = fieldType.GetGenericArguments();
            Type valueType = genericArgs[1];
            IDictionary dictionary = (IDictionary)info.GetValue(obj);

            if (dictionary == null)
            {
                debug += "Creating new dictionary instance\n";
                dictionary = (IDictionary)Activator.CreateInstance(info.FieldType);
            }

            debug += $"Looking for key with hash: {currentRoute.collectionKey}\n";
            debug += $"Current dictionary keys: {string.Join(", ", dictionary.Keys.Cast<object>().Select(k => $"{k}(hash:{ReflectionHelperMethods.GetStableKeyHash(k)})"))}\n";

            object matchingKey = ReflectionHelperMethods.FindMatchingKey(dictionary, currentRoute.collectionKey);
            debug += matchingKey != null ? $"Found matching key: {matchingKey}\n" : "No matching key found!\n";

            if (idRoute.Count <= idToRead + 1)
            {
                debug += $"Directly setting value: {value}\n";
                if (matchingKey != null)
                {
                    dictionary[matchingKey] = value;
                }
                else
                {
                    dictionary[currentRoute.collectionKey] = value;
                }
            }
            else
            {
                debug += $"Nested inspection for value\n";
                object element = dictionary[matchingKey] ?? ReflectionHelperMethods.ConstructObject(valueType, reflection.bindingFlags);
                dictionary[matchingKey] = element;
                reflection.reflectionInspector.InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
            }

            info.SetValue(obj, dictionary);
            debug += $"Final dictionary state: {string.Join(", ", dictionary.Keys.Cast<object>().Select(k => $"{k}={dictionary[k]}"))}\n";

            //debugger?.Log(debug);
            return obj;
        }
        #endregion
    }
}