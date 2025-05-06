using Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerController : MonoBehaviour, INetObj
{
    [Serializable]
    public class TestingClass
    {
        [NetVariable(0)] public TestingClass2 testInt = new();
    }
    [Serializable]
    public class TestingClass2
    {
        [NetVariable(0)] public TestingClass3 testInt = new();
    }
    [Serializable]
    public class TestingClass3
    {
        [NetVariable(0)] public int testInt = 0;
    }

    [Serializable]
    private class TestingClass4
    {
        [NetVariable(0)] public int testInt = 0;
        public TestingClass2 testIntClass2 = new();

        public TestingClass4(int testingInt, TestingClass2 testingClass2)
        {
            this.testInt = testingInt;
            this.testIntClass2 = testingClass2;
        }
    }

    public struct TestingStruct
    {
        [NetVariable(0)] public int testInt;
        [NetVariable(1)] public int testInt2;
        [NetVariable(2)] public int testInt3;
    }

    [Serializable]
    public class DictionaryTestClass
    {
        [NetVariable(0)] public Dictionary<int, string> testDictionary = new Dictionary<int, string>();

        public DictionaryTestClass()
        {
            testDictionary.Add(1, "abc");
            testDictionary.Add(2, "xyz");
        }
    }

    [Serializable]
    public class MultiDimArrayTestClass
    {
        [NetVariable(0)] public int[,] twoDArray = new int[3, 3];
        [NetVariable(1)] public int[,,,,] fiveDArray = new int[2, 2, 2, 2, 2];

        public MultiDimArrayTestClass()
        {
            int counter = 1;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    twoDArray[i, j] = counter++;
                }
            }

            for (int a = 0; a < 2; a++)
                for (int b = 0; b < 2; b++)
                    for (int c = 0; c < 2; c++)
                        for (int d = 0; d < 2; d++)
                            for (int e = 0; e < 2; e++)
                                fiveDArray[a, b, c, d, e] = (a * 10000) + (b * 1000) + (c * 100) + (d * 10) + e;
        }
    }

    [Serializable]
    public class CustomCollection<T>
    {
        [NetVariable(0)] private List<T> customCollectionItems = new List<T>();
        public int Count => customCollectionItems.Count;
        public T this[int index] => customCollectionItems[index];
        public void Add(T item) => customCollectionItems.Add(item);
        public void Insert(int index, T item) => customCollectionItems.Insert(index, item);
        public void RemoveAt(int index) => customCollectionItems.RemoveAt(index);
        public void Clear() => customCollectionItems.Clear();
        public void Reverse() => customCollectionItems.Reverse();
    }

    [Serializable]
    public class IntCollection : CustomCollection<int> { }

    public enum TestEnum
    {
        Default = 0,
        Modified = 1,
        Special = 255
    }

    [NetVariable(0)] public float health = 3;
    //[NetVariable(1)] public bool myBool = false;
    //[NetVariable(2)] public string myString = "pepe";
    //[NetVariable(3)] public char myChar = 'a';
    //[NetVariable(4)] public decimal myDecimal = 1;
    //[NetVariable(5)] public double myDouble = 1;
    //[NetVariable(6)] public short myShort = 1;
    //[NetVariable(7)] public ushort myUShort = 1;
    //[NetVariable(8)] public int myInt = 1;
    //[NetVariable(9)] public uint myUInt = 1;
    //[NetVariable(10)] public long myLong = 1;
    //[NetVariable(11)] public ulong myULong = 1;
    //[NetVariable(12)] public byte myByte = 1;
    //[NetVariable(13)] public sbyte mySByte = 1;
    //[NetVariable(14)] public List<int> testList;
    //[NetVariable(15)] public TestingClass testing = new();
    //[NetVariable(16)] public TestingStruct testingStruct = new() { testInt = 0, testInt2 = 0, testInt3 = 0 };
    //[NetVariable(17)] public int[] myArray = new int[2];
    //[NetVariable(18)] TestingClass4 testingClass4;
    //[NetVariable(19)] public List<TestingClass3> jajaxd = null;
    //[NetVariable(20)] public TestingClass3 testingclass3 = new();
    //[NetVariable(21)] public Vector2 MyVector2 = Vector2.zero;
    //[NetVariable(22)] public Vector3 MyVector3 = Vector3.zero;
    //[NetVariable(23)] public Vector4 MyVector4 = Vector4.zero;
    //[NetVariable(24)] public Quaternion MyQuaternion = new(0f, 0f, 0f, 1f);
    //[NetVariable(25)] public Color MyColor = new(1f,0.5f,0f,1f);
    //[NetVariable(26)] public Color32 MyColor32 = new(0, 0, 0, 255);
    //[NetVariable(27)] public Rect MyRect = new(0, 0, 1, 1);
    //[NetVariable(28)] public Bounds MyBounds = new(Vector3.zero, Vector3.one);
    //[NetVariable(29)] public Matrix4x4 MyMatrix4x4 = new(new Vector4(0,0,0,0), new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 0));
    //[NetVariable(30)] public Plane MyPlane = new(new Vector3(1,2,3), 0);
    //[NetVariable(31)] public Vector2Int MyVector2Int = new (0,0);
    //[NetVariable(32)] public Vector3Int MyVector3Int = new (0,0,0);
    [NetVariable(33)] private DictionaryTestClass dictionaryTest;
    //[NetVariable(34)] private MultiDimArrayTestClass arrayTest = new MultiDimArrayTestClass();
    //[NetVariable(35)] private IntCollection _customCollection = new IntCollection();
    //[NetVariable(36)] public TestEnum enumField;
    [SerializeField] TowerTurns towerTurns;
    [SerializeField] TankMovement movement;
    [SerializeField] Transform cameraPivot;

    public bool currentPlayer = false;
    public int clientID = -1;
    private int testing4ConstructorInt = 0;
    private TestingClass2 testing4ConstructorClass = new();
    NetObj netObj = new(-1, -1);

    NetworkManager nm;

    #region ENUM
    //[ContextMenu("Test Enum - Set Default")]
    //void SetEnumDefault() => enumField = TestEnum.Default;
    //
    //[ContextMenu("Test Enum - Set Modified")]
    //void SetEnumModified() => enumField = TestEnum.Modified;
    //
    //[ContextMenu("Test Enum - Set Special")]
    //void SetEnumSpecial() => enumField = TestEnum.Special;
    #endregion
    #region CLASS
    [ContextMenu("Create TestingClass4")]
    private void TestCreator()
    {
        //if (testingClass4 == null)
        //{
        //    testingClass4 = new TestingClass4(testing4ConstructorInt, testing4ConstructorClass);
        //}
    }

    [ContextMenu("Destroy TestingClass4")]
    private void TestDestroyer()
    {
        //if (testingClass4 != null)
        //{
        //    testingClass4 = null;
        //}
    }
    #endregion
    #region DECIMAL
    //[ContextMenu("Add Decimal")]
    //private void AddDecimal()
    //{
    //    myDecimal += 1;
    //}
    //
    //[ContextMenu("Substract Decimal")]
    //private void SubstractDecimal()
    //{
    //    myDecimal -= 1;
    //}
    #endregion
    #region DICTIONARY
    [ContextMenu("Initialize Dictionary")]
    private void InitializeDictionary()
    {
        dictionaryTest = new DictionaryTestClass();
        Debug.Log($"Client {clientID} Dictionary initialized with {dictionaryTest.testDictionary.Count} entries");
    }

    [ContextMenu("Add Dictionary Entry")]
    private void AddDictionaryEntry()
    {
        try
        {
            // Initialize if null
            if (dictionaryTest == null)
            {
                dictionaryTest = new DictionaryTestClass();
            }

            // Safe key generation
            int newKey = dictionaryTest.testDictionary.Count > 0 ?
                       dictionaryTest.testDictionary.Keys.Max() + 1 :
                       1;

            string newValue = $"Entry {newKey} (Client {clientID})";
            dictionaryTest.testDictionary.Add(newKey, newValue);

            Debug.Log($"Client {clientID} added dictionary entry: {newKey} = {newValue}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error adding dictionary entry: {ex}");

            // Fallback to key 1 if empty
            dictionaryTest.testDictionary.Add(1, $"Fallback Entry (Client {clientID})");
        }
    }

    [ContextMenu("Remove Last Dictionary Entry")]
    private void RemoveDictionaryEntry()
    {
        if (dictionaryTest?.testDictionary.Count > 0)
        {
            int lastKey = dictionaryTest.testDictionary.Keys.Max();
            string removedValue = dictionaryTest.testDictionary[lastKey];
            dictionaryTest.testDictionary.Remove(lastKey);
            Debug.Log($"Client {clientID} removed dictionary entry: {lastKey} = {removedValue}");
        }
    }

    [ContextMenu("Update Random Dictionary Value")]
    private void UpdateRandomDictionaryValue()
    {
        if (dictionaryTest?.testDictionary.Count > 0)
        {
            int randomKey = dictionaryTest.testDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, dictionaryTest.testDictionary.Count));
            //int randomKey2 = UnityEngine.Random.Range(0, 1000);
            string oldValue = dictionaryTest.testDictionary[randomKey];
            string newValue = $"Updated by Client {clientID} at {DateTime.Now:HH:mm:ss}";
            dictionaryTest.testDictionary[randomKey] = newValue;
            Debug.Log($"Client {clientID} updated {randomKey}: {oldValue} -> {newValue}");
        }
    }

    [ContextMenu("Clear Dictionary")]
    private void ClearDictionary()
    {
        if (dictionaryTest != null)
        {
            dictionaryTest.testDictionary.Clear();
            Debug.Log($"Client {clientID} cleared dictionary (now has {dictionaryTest.testDictionary.Count} entries)");
        }
    }

    [ContextMenu("Set Dictionary to Null")]
    private void NullDictionary()
    {
        dictionaryTest = null;
        Debug.Log($"Client {clientID} set dictionary to NULL");
    }
    #endregion
    #region MULTIDIMENSIONALARRAYS
    //[ContextMenu("Initialize Arrays")]
    //private void InitializeArrays()
    //{
    //    arrayTest = new MultiDimArrayTestClass();
    //    Debug.Log($"Client {clientID} Initialized both arrays (3x3 2D and 2x2x2x2x2 5D)");
    //}
    //
    //[ContextMenu("Null Arrays")]
    //private void NullArrays()
    //{
    //    arrayTest = null;
    //    Debug.Log($"Client {clientID} Set arrays to NULL");
    //}
    //
    //[ContextMenu("Update Random 2D Value")]
    //private void UpdateRandom2DValue()
    //{
    //    if (arrayTest != null)
    //    {
    //        int x = UnityEngine.Random.Range(0, arrayTest.twoDArray.GetLength(0));
    //        int y = UnityEngine.Random.Range(0, arrayTest.twoDArray.GetLength(1));
    //
    //        int oldValue = arrayTest.twoDArray[x, y];
    //        arrayTest.twoDArray[x, y] = UnityEngine.Random.Range(100, 1000);
    //
    //        Debug.Log($"<color=yellow>Client {clientID} Updated 2D[{x},{y}] {oldValue}→{arrayTest.twoDArray[x, y]}</color>\n" +
    //                 ArrayVisualizer.VisualizeArray(arrayTest.twoDArray, new int[] { x, y }));
    //    }
    //}
    //
    //[ContextMenu("Update Random 5D Value")]
    //private void UpdateRandom5DValue()
    //{
    //    if (arrayTest != null)
    //    {
    //        int[] indices = new int[5];
    //        for (int i = 0; i < 5; i++)
    //            indices[i] = UnityEngine.Random.Range(0, arrayTest.fiveDArray.GetLength(i));
    //
    //        int oldValue = (int)arrayTest.fiveDArray.GetValue(indices);
    //        int newValue = UnityEngine.Random.Range(10000, 20000);
    //        arrayTest.fiveDArray.SetValue(newValue, indices);
    //
    //        Debug.Log($"<color=yellow>Client {clientID} Updated 5D[{string.Join(",", indices)}] {oldValue}→{newValue}</color>\n" +
    //                 ArrayVisualizer.VisualizeMultiDimArray(arrayTest.fiveDArray, indices));
    //    }
    //}
    //
    //[ContextMenu("Reset 2D Array")]
    //private void Reset2DArray()
    //{
    //    if (arrayTest != null)
    //    {
    //        int counter = 1;
    //        for (int i = 0; i < 3; i++)
    //        {
    //            for (int j = 0; j < 3; j++)
    //            {
    //                arrayTest.twoDArray[i, j] = counter++;
    //            }
    //        }
    //        Debug.Log($"Client {clientID} Reset 2D array to sequential values");
    //    }
    //}
    //
    //[ContextMenu("Reset 5D Array")]
    //private void Reset5DArray()
    //{
    //    if (arrayTest != null)
    //    {
    //        for (int a = 0; a < 2; a++)
    //            for (int b = 0; b < 2; b++)
    //                for (int c = 0; c < 2; c++)
    //                    for (int d = 0; d < 2; d++)
    //                        for (int e = 0; e < 2; e++)
    //                            arrayTest.fiveDArray[a, b, c, d, e] = (a * 10000) + (b * 1000) + (c * 100) + (d * 10) + e;
    //        Debug.Log($"Client {clientID} Reset 5D array to pattern values");
    //    }
    //}
    //
    //[ContextMenu("Print Array Visualizations")]
    //private void PrintArrayVisualizations()
    //{
    //    if (arrayTest != null)
    //    {
    //        Debug.Log($"<color=cyan><b>Client {clientID} 2D Array:</b></color>\n" +
    //                 ArrayVisualizer.VisualizeMultiDimArray(arrayTest.twoDArray));
    //
    //        Debug.Log($"<color=cyan><b>Client {clientID} 5D Array:</b></color>\n" +
    //                 ArrayVisualizer.VisualizeMultiDimArray(arrayTest.fiveDArray));
    //    }
    //}
    #endregion
    #region CUSTOM COLLECTION
    //[ContextMenu("Initialize Collection")]
    //private void InitializeCollection()
    //{
    //    _customCollection = new IntCollection();
    //    _customCollection.Add(10);
    //    _customCollection.Add(20);
    //    _customCollection.Add(30);
    //    Debug.Log($"Client {clientID} Initialized collection with 3 values");
    //}
    //
    //[ContextMenu("Add Random Value")]
    //private void AddRandomValue()
    //{
    //    if (_customCollection == null)
    //    {
    //        _customCollection = new IntCollection();
    //    }
    //
    //    int newValue = UnityEngine.Random.Range(100, 1000);
    //    _customCollection.Add(newValue);
    //    Debug.Log($"Client {clientID} Added value: {newValue}");
    //}
    //
    //[ContextMenu("Insert Random Value")]
    //private void InsertRandomValue()
    //{
    //    if (_customCollection != null && _customCollection.Count > 0)
    //    {
    //        int index = UnityEngine.Random.Range(0, _customCollection.Count);
    //        int newValue = UnityEngine.Random.Range(100, 1000);
    //        _customCollection.Insert(index, newValue);
    //        Debug.Log($"Client {clientID} Inserted {newValue} at index {index}");
    //    }
    //}
    //
    //[ContextMenu("Remove Random Item")]
    //private void RemoveRandomItem()
    //{
    //    if (_customCollection != null && _customCollection.Count > 0)
    //    {
    //        int index = UnityEngine.Random.Range(0, _customCollection.Count);
    //        int removedValue = _customCollection[index];
    //        _customCollection.RemoveAt(index);
    //        Debug.Log($"Client {clientID} Removed value {removedValue} from index {index}");
    //    }
    //}
    //
    //[ContextMenu("Reverse Collection")]
    //private void ReverseCollection()
    //{
    //    if (_customCollection != null && _customCollection.Count > 1)
    //    {
    //        _customCollection.Reverse();
    //        Debug.Log($"Client {clientID} Reversed collection order");
    //    }
    //}
    //
    //[ContextMenu("Clear Collection")]
    //private void ClearCollection()
    //{
    //    if (_customCollection != null)
    //    {
    //        _customCollection.Clear();
    //        Debug.Log($"Client {clientID} Cleared collection");
    //    }
    //}
    //
    //[ContextMenu("Null Collection")]
    //private void NullCollection()
    //{
    //    _customCollection = null;
    //    Debug.Log($"Client {clientID} Set collection to NULL");
    //}
    //
    //[ContextMenu("Print Collection Info")]
    //private void PrintCollectionInfo()
    //{
    //    if (_customCollection != null)
    //    {
    //        Debug.Log($"Client {clientID} Collection Count: {_customCollection.Count}");
    //    }
    //    else
    //    {
    //        Debug.Log($"Client {clientID} Collection is NULL");
    //    }
    //}
    #endregion

    private void Start()
    {
        nm = NetworkManager.Instance;
        if (currentPlayer)
        {
            Camera.main.gameObject.GetComponent<CameraOrbit>().SetFollowObject(cameraPivot);
        }
        //Debug.Log($"Initial list values: {string.Join(", ", testList)}");
        //enumField = TestEnum.Special;
        //testList.Add(1);
        //testList.Add(2);
        //testList.Add(3);
        //myArray[0] = 1;
        //myArray[1] = 2;
        //
        //testingClass4 = null;
        //
        //testingclass3.testInt = 1;
        //
        //jajaxd = new List<TestingClass3>
        //{
        //    new() { testInt = 7 },
        //    new() { testInt = 17 }
        //};
    }

    private void Update()
    {
        #region LIST
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    testList = null;
        //}
        //else if (Input.GetKeyDown(KeyCode.A))
        //{
        //    if (testList == null)
        //    {
        //        testList = new();
        //        testList.Add(1);
        //        testList.Add(2);
        //        testList.Add(3);
        //    }
        //}
        #endregion
        #region CLASS
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    testing = null;
        //}
        //else if (Input.GetKeyDown(KeyCode.O))
        //{
        //    if (testing == null)
        //        testing = new();
        //}
        //if (testing != null)
        //{
        //    Debug.Log($"Client {clientID} testing class IS NOT NULL");
        //}
        //else
        //{
        //    Debug.Log($"Client {clientID} testing class IS NULL");
        //}
        #endregion
        #region PLANE
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    MyPlane.normal += new Vector3 (1,0,1);
        //    MyPlane.distance += 2.67f;
        //    Debug.Log("Normal:" + MyPlane.normal);
        //    Debug.Log("Distance:" + MyPlane.distance);
        //}
        #endregion
        //Debug.Log($"Client {clientID} myDecimal: " + myDecimal);
        #region DICTIONARY
        if (dictionaryTest != null)
        {
            string dictContents = $"Client {clientID} Dictionary Contents:\n";
            foreach (var kvp in dictionaryTest.testDictionary)
            {
                dictContents += $"[{kvp.Key}] = {kvp.Value}\n";
            }
            Debug.Log(dictContents);
        }
        else
        {
            Debug.Log($"Client {clientID} Dictionary is NULL");
        }
        #endregion
        #region MULTIDIMENSIONALARRAYS
        //if (arrayTest != null)
        //{
        //    Debug.Log($"Client {clientID} Arrays are NOT NULL");
        //}
        //else
        //{
        //    Debug.Log($"Client {clientID} Arrays are NULL");
        //}
        #endregion
        #region CUSTOM COLLECTION
        //if (_customCollection != null)
        //{
        //    Debug.Log($"Client {clientID} Collection Contents:");
        //    for (int i = 0; i < _customCollection.Count; i++)
        //    {
        //        Debug.Log($"[{i}] = {_customCollection[i]}");
        //    }
        //}
        //else if (_customCollection == null)
        //{
        //    Debug.Log($"Client {clientID} Collection is NULL");
        //}
        #endregion

        if (health <= 0)
        {
            Debug.Log(clientID + " died");
            NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
            nm.networkEntity.SendMessage(netDisconnection.Serialize());
            nm.networkEntity.RemoveClient(clientID);
        }
    }

    public void OnReciveDamage()
    {
        health--;
    }

    public int GetID()
    {
        return netObj.ID;
    }

    public int GetOwnerID()
    {
        return netObj.OwnerId;
    }

    public NetObj GetNetObj()
    {
        return netObj;
    }
}

public static class ArrayVisualizer
{
    public static string VisualizeArray(Array array, int[] highlightIndices = null)
    {
        if (array == null) return "<color=red>NULL</color>";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<b>Array Type:</b> {array.GetType().GetElementType().Name}[{string.Join(",", GetDimensions(array))}]");

        // Handle 1D and 2D arrays with pretty formatting
        if (array.Rank <= 2)
        {
            sb.AppendLine(VisualizeLowDimensionArray(array, highlightIndices));
        }
        else // Handle 3D+ arrays with smart slicing
        {
            sb.AppendLine(VisualizeHighDimensionArray(array, highlightIndices));
        }

        return sb.ToString();
    }

    private static string VisualizeLowDimensionArray(Array array, int[] highlightIndices)
    {
        StringBuilder sb = new StringBuilder();

        if (array.Rank == 1)
        {
            sb.Append("[ ");
            for (int i = 0; i < array.GetLength(0); i++)
            {
                AppendValue(sb, array.GetValue(i), highlightIndices != null && highlightIndices[0] == i);
                if (i < array.GetLength(0) - 1) sb.Append(", ");
            }
            sb.Append(" ]");
        }
        else // Rank == 2
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                sb.Append("[ ");
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    bool isHighlighted = highlightIndices != null &&
                                       highlightIndices[0] == i &&
                                       highlightIndices[1] == j;
                    AppendValue(sb, array.GetValue(i, j), isHighlighted);
                    if (j < array.GetLength(1) - 1) sb.Append(", ");
                }
                sb.AppendLine(" ]");
            }
        }

        return sb.ToString();
    }

    private static string VisualizeHighDimensionArray(Array array, int[] highlightIndices)
    {
        StringBuilder sb = new StringBuilder();

        if (highlightIndices != null)
        {
            // Show 2D slice containing the highlighted value
            int fixedDims = array.Rank - 2;
            sb.AppendLine($"<b>Slice at {GetIndicesString(highlightIndices.Take(fixedDims).ToArray())}:</b>");

            int[] sliceIndices = new int[array.Rank];
            Array.Copy(highlightIndices, sliceIndices, fixedDims);

            for (int i = 0; i < array.GetLength(fixedDims); i++)
            {
                sb.Append("[ ");
                for (int j = 0; j < array.GetLength(fixedDims + 1); j++)
                {
                    sliceIndices[fixedDims] = i;
                    sliceIndices[fixedDims + 1] = j;

                    bool isHighlighted = highlightIndices[fixedDims] == i &&
                                       highlightIndices[fixedDims + 1] == j;
                    AppendValue(sb, array.GetValue(sliceIndices), isHighlighted);

                    if (j < array.GetLength(fixedDims + 1) - 1) sb.Append(", ");
                }
                sb.AppendLine(" ]");
            }
        }

        // Show metadata
        sb.AppendLine($"<b>Total Elements:</b> {array.Length}");
        sb.AppendLine($"<b>Dimensions:</b> {string.Join("×", GetDimensions(array))}");

        return sb.ToString();
    }

    private static void AppendValue(StringBuilder sb, object value, bool highlight)
    {
        if (highlight)
            sb.Append($"<color=green><b>{value}</b></color>");
        else
            sb.Append(value);
    }

    private static string GetIndicesString(int[] indices)
    {
        return string.Join(",", indices.Select((i, dim) => $"dim{dim + 1}={i}"));
    }

    private static int[] GetDimensions(Array array)
    {
        int[] dims = new int[array.Rank];
        for (int i = 0; i < array.Rank; i++)
            dims[i] = array.GetLength(i);
        return dims;
    }

    public static string VisualizeMultiDimArray(Array array, int[] highlightIndices = null)
    {
        if (array == null) return "Array is null";
        if (array.Rank < 2) return "Array must have at least 2 dimensions";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Array Dimensions: {string.Join("x", Enumerable.Range(0, array.Rank).Select(d => array.GetLength(d)))}");

        // Visualize 2D slices of the array
        int[] indices = new int[array.Rank];
        int sliceDims = Math.Min(2, array.Rank); // Show last 2 dimensions

        // Initialize indices to 0 except last two dimensions
        for (int i = 0; i < indices.Length - sliceDims; i++)
        {
            indices[i] = highlightIndices != null && i < highlightIndices.Length
                ? highlightIndices[i]
                : 0;
        }

        // Iterate through last two dimensions
        for (int d1 = 0; d1 < array.GetLength(array.Rank - 2); d1++)
        {
            indices[array.Rank - 2] = d1;
            for (int d2 = 0; d2 < array.GetLength(array.Rank - 1); d2++)
            {
                indices[array.Rank - 1] = d2;

                bool isHighlighted = highlightIndices != null &&
                                   highlightIndices.Length >= array.Rank &&
                                   highlightIndices[array.Rank - 2] == d1 &&
                                   highlightIndices[array.Rank - 1] == d2;

                object value = array.GetValue(indices);
                sb.Append(isHighlighted ? $"<color=yellow>{value}</color>\t" : $"{value}\t");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}