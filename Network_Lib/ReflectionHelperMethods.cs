using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;

namespace Net
{
    public static class ReflectionHelperMethods
    {
        #region Helper Methods
        /// <summary>
        /// Determines if a type is a simple type (value type, string, or enum).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is simple, false otherwise.</returns>
        public static bool IsSimpleType(Type type)
        {
            return (type.IsValueType && type.IsPrimitive) || type == typeof(string) || type.IsEnum;
        }

        /// <summary>
        /// Gets all indices for a multi-dimensional array.
        /// </summary>
        /// <param name="array">The array to process.</param>
        /// <returns>An enumerable of index arrays.</returns>
        public static IEnumerable<int[]> GetArrayIndices(Array array)
        {
            int[] indices = new int[array.Rank];
            yield return indices;

            while (IncrementIndices(array, indices))
            {
                yield return (int[])indices.Clone();
            }
        }

        /// <summary>
        /// Increments multi-dimensional array indices.
        /// </summary>
        /// <param name="array">The array being processed.</param>
        /// <param name="indices">The current indices to increment.</param>
        /// <returns>True if indices were successfully incremented, false if at end of array.</returns>
        public static bool IncrementIndices(Array array, int[] indices)
        {
            for (int dim = array.Rank - 1; dim >= 0; dim--)
            {
                indices[dim]++;
                if (indices[dim] < array.GetLength(dim))
                {
                    return true;
                }
                indices[dim] = 0;
            }
            return false;
        }

        /// <summary>
        /// Gets the element type of a collection or array.
        /// </summary>
        /// <param name="type">The collection/array type.</param>
        /// <returns>The element type, or typeof(object) if not determinable.</returns>
        public static Type GetElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            else if (type.IsGenericType)
                return type.GetGenericArguments()[0];
            return typeof(object);
        }

        /// <summary>
        /// Constructs an instance of the specified type.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <returns>A new instance of the type.</returns>
        public static object ConstructObject(Type type, BindingFlags bindingFlags)
        {
            ConstructorInfo[] constructors = type.GetConstructors(bindingFlags);
            ConstructorInfo constructorInfo = null;

            foreach (ConstructorInfo constructor in constructors)
            {
                if (constructor.GetParameters().Length == 0)
                {
                    return constructor.Invoke(new object[0]);
                }
                else
                {
                    foreach (ParameterInfo parametersInfo in constructor.GetParameters())
                    {
                        if (parametersInfo.ParameterType == type)
                        {
                            continue;
                        }
                    }
                    constructorInfo = constructor;
                }
            }

            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
            object[] parameters = new object[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                if (parameterInfos[i].ParameterType.IsValueType || parameterInfos[i].ParameterType == typeof(string) || parameterInfos[i].ParameterType.IsEnum)
                {
                    parameters[i] = default;
                }
                else
                {
                    parameters[i] = ConstructObject(parameterInfos[i].ParameterType, bindingFlags);
                }
            }
            return constructorInfo.Invoke(parameters);
        }

        /// <summary>
        /// Gets all fields of a type, including inherited fields.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>An enumerable of FieldInfo objects.</returns>
        public static IEnumerable<FieldInfo> GetAllFields(Type type, BindingFlags bindingFlags)
        {
            while (type != null)
            {
                foreach (FieldInfo? field in type.GetFields(bindingFlags))
                    yield return field;

                type = type.BaseType;
            }
        }

        /// <summary>
        /// Finds a dictionary key that matches the specified hash value.
        /// </summary>
        /// <param name="dictionary">The dictionary to search.</param>
        /// <param name="keyHash">The hash value to match.</param>
        /// <returns>The matching key, or null if not found.</returns>
        public static object FindMatchingKey(IDictionary dictionary, int keyHash)
        {
            foreach (object key in dictionary.Keys)
            {
                if (GetStableKeyHash(key) == keyHash) return key;
            }
            return null;
        }

        /// <summary>
        /// Gets a stable hash code for a dictionary key.
        /// </summary>
        /// <param name="key">The key to hash.</param>
        /// <returns>A stable hash code for the key.</returns>
        public static int GetStableKeyHash(object key)
        {
            // For types that can be reliably hashed
            if (key is string strKey) return strKey.GetHashCode();
            if (key is int intKey) return intKey;
            if (key is float floatKey) return floatKey.GetHashCode();
            if (key.GetType().IsEnum) return (int)key;

            // Fallback to standard hash code
            return key.GetHashCode();
        }
        #endregion
    }
}