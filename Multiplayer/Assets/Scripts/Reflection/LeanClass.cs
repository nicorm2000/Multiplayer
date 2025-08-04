using UnityEngine;
using System;
using System.Collections.Generic;
using Random = System.Random;

public class LeanClass : BaseTestClass
{
	#region C#Fields
	public bool publicBool;
	private bool privateBool;
	protected bool protectedBool;
	internal bool internalBool;

	public byte publicByte;
	private byte privateByte;
	protected byte protectedByte;
	internal byte internalByte;

	public sbyte publicSByte;
	private sbyte privateSByte;
	protected sbyte protectedSByte;
	internal sbyte internalSByte;

	public short publicShort;
	private short privateShort;
	protected short protectedShort;
	internal short internalShort;

	public ushort publicUShort;
	private ushort privateUShort;
	protected ushort protectedUShort;
	internal ushort internalUShort;

	public int publicInt;
	private int privateInt;
	protected int protectedInt;
	internal int internalInt;

	public uint publicUInt;
	private uint privateUInt;
	protected uint protectedUInt;
	internal uint internalUInt;

	public long publicLong;
	private long privateLong;
	protected long protectedLong;
	internal long internalLong;

	public ulong publicULong;
	private ulong privateULong;
	protected ulong protectedULong;
	internal ulong internalULong;

	public float publicFloat;
	private float privateFloat;
	protected float protectedFloat;
	internal float internalFloat;

	public double publicDouble;
	private double privateDouble;
	protected double protectedDouble;
	internal double internalDouble;

	public decimal publicDecimal;
	private decimal privateDecimal;
	protected decimal protectedDecimal;
	internal decimal internalDecimal;

	public char publicChar;
	private char privateChar;
	protected char protectedChar;
	internal char internalChar;

	public string publicString;
	private string privateString;
	protected string protectedString;
	internal string internalString;
	#endregion

	#region C#Collections
	#region BoolCollections
	public bool[] publicBoolArray;
	private bool[] privateBoolArray;
	protected bool[] protectedBoolArray;
	internal bool[] internalBoolArray;

	public bool[,] publicBoolMatrix;
	private bool[,] privateBoolMatrix;
	protected bool[,] protectedBoolMatrix;
	internal bool[,] internalBoolMatrix;

	public List<bool> publicBoolList;
	private List<bool> privateBoolList;
	protected List<bool> protectedBoolList;
	internal List<bool> internalBoolList;

	public Stack<bool> publicBoolStack;
	private Stack<bool> privateBoolStack;
	protected Stack<bool> protectedBoolStack;
	internal Stack<bool> internalBoolStack;

	public Queue<bool> publicBoolQueue;
	private Queue<bool> privateBoolQueue;
	protected Queue<bool> protectedBoolQueue;
	internal Queue<bool> internalBoolQueue;

	public Dictionary<string, bool> publicBoolDict;
	private Dictionary<string, bool> privateBoolDict;
	protected Dictionary<string, bool> protectedBoolDict;
	internal Dictionary<string, bool> internalBoolDict;

	public List<bool>[] publicBoolListArray;
	private List<bool>[] privateBoolListArray;
	protected List<bool>[] protectedBoolListArray;
	internal List<bool>[] internalBoolListArray;
	#endregion

	#region ByteCollections
	public byte[] publicByteArray;
	private byte[] privateByteArray;
	protected byte[] protectedByteArray;
	internal byte[] internalByteArray;

	public byte[,] publicByteMatrix;
	private byte[,] privateByteMatrix;
	protected byte[,] protectedByteMatrix;
	internal byte[,] internalByteMatrix;

	public List<byte> publicByteList;
	private List<byte> privateByteList;
	protected List<byte> protectedByteList;
	internal List<byte> internalByteList;

	public Stack<byte> publicByteStack;
	private Stack<byte> privateByteStack;
	protected Stack<byte> protectedByteStack;
	internal Stack<byte> internalByteStack;

	public Queue<byte> publicByteQueue;
	private Queue<byte> privateByteQueue;
	protected Queue<byte> protectedByteQueue;
	internal Queue<byte> internalByteQueue;

	public Dictionary<string, byte> publicByteDict;
	private Dictionary<string, byte> privateByteDict;
	protected Dictionary<string, byte> protectedByteDict;
	internal Dictionary<string, byte> internalByteDict;

	public List<byte>[] publicByteListArray;
	private List<byte>[] privateByteListArray;
	protected List<byte>[] protectedByteListArray;
	internal List<byte>[] internalByteListArray;
	#endregion

	#region SbyteCollections
	public sbyte[] publicSByteArray;
	private sbyte[] privateSByteArray;
	protected sbyte[] protectedSByteArray;
	internal sbyte[] internalSByteArray;

	public sbyte[,] publicSByteMatrix;
	private sbyte[,] privateSByteMatrix;
	protected sbyte[,] protectedSByteMatrix;
	internal sbyte[,] internalSByteMatrix;

	public List<sbyte> publicSByteList;
	private List<sbyte> privateSByteList;
	protected List<sbyte> protectedSByteList;
	internal List<sbyte> internalSByteList;

	public Stack<sbyte> publicSByteStack;
	private Stack<sbyte> privateSByteStack;
	protected Stack<sbyte> protectedSByteStack;
	internal Stack<sbyte> internalSByteStack;

	public Queue<sbyte> publicSByteQueue;
	private Queue<sbyte> privateSByteQueue;
	protected Queue<sbyte> protectedSByteQueue;
	internal Queue<sbyte> internalSByteQueue;

	public Dictionary<string, sbyte> publicSByteDict;
	private Dictionary<string, sbyte> privateSByteDict;
	protected Dictionary<string, sbyte> protectedSByteDict;
	internal Dictionary<string, sbyte> internalSByteDict;

	public List<sbyte>[] publicSByteListArray;
	private List<sbyte>[] privateSByteListArray;
	protected List<sbyte>[] protectedSByteListArray;
	internal List<sbyte>[] internalSByteListArray;
	#endregion

	#region ShortCollections
	public short[] publicShortArray;
	private short[] privateShortArray;
	protected short[] protectedShortArray;
	internal short[] internalShortArray;

	public short[,] publicShortMatrix;
	private short[,] privateShortMatrix;
	protected short[,] protectedShortMatrix;
	internal short[,] internalShortMatrix;

	public List<short> publicShortList;
	private List<short> privateShortList;
	protected List<short> protectedShortList;
	internal List<short> internalShortList;

	public Stack<short> publicShortStack;
	private Stack<short> privateShortStack;
	protected Stack<short> protectedShortStack;
	internal Stack<short> internalShortStack;

	public Queue<short> publicShortQueue;
	private Queue<short> privateShortQueue;
	protected Queue<short> protectedShortQueue;
	internal Queue<short> internalShortQueue;

	public Dictionary<string, short> publicShortDict;
	private Dictionary<string, short> privateShortDict;
	protected Dictionary<string, short> protectedShortDict;
	internal Dictionary<string, short> internalShortDict;

	public List<short>[] publicShortListArray;
	private List<short>[] privateShortListArray;
	protected List<short>[] protectedShortListArray;
	internal List<short>[] internalShortListArray;
	#endregion

	#region UshortCollections
	public ushort[] publicUShortArray;
	private ushort[] privateUShortArray;
	protected ushort[] protectedUShortArray;
	internal ushort[] internalUShortArray;

	public ushort[,] publicUShortMatrix;
	private ushort[,] privateUShortMatrix;
	protected ushort[,] protectedUShortMatrix;
	internal ushort[,] internalUShortMatrix;

	public List<ushort> publicUShortList;
	private List<ushort> privateUShortList;
	protected List<ushort> protectedUShortList;
	internal List<ushort> internalUShortList;

	public Stack<ushort> publicUShortStack;
	private Stack<ushort> privateUShortStack;
	protected Stack<ushort> protectedUShortStack;
	internal Stack<ushort> internalUShortStack;

	public Queue<ushort> publicUShortQueue;
	private Queue<ushort> privateUShortQueue;
	protected Queue<ushort> protectedUShortQueue;
	internal Queue<ushort> internalUShortQueue;

	public Dictionary<string, ushort> publicUShortDict;
	private Dictionary<string, ushort> privateUShortDict;
	protected Dictionary<string, ushort> protectedUShortDict;
	internal Dictionary<string, ushort> internalUShortDict;

	public List<ushort>[] publicUShortListArray;
	private List<ushort>[] privateUShortListArray;
	protected List<ushort>[] protectedUShortListArray;
	internal List<ushort>[] internalUShortListArray;
	#endregion

	#region IntCollections
	public int[] publicIntArray;
	private int[] privateIntArray;
	protected int[] protectedIntArray;
	internal int[] internalIntArray;

	public int[,] publicIntMatrix;
	private int[,] privateIntMatrix;
	protected int[,] protectedIntMatrix;
	internal int[,] internalIntMatrix;

	public List<int> publicIntList;
	private List<int> privateIntList;
	protected List<int> protectedIntList;
	internal List<int> internalIntList;

	public Stack<int> publicIntStack;
	private Stack<int> privateIntStack;
	protected Stack<int> protectedIntStack;
	internal Stack<int> internalIntStack;

	public Queue<int> publicIntQueue;
	private Queue<int> privateIntQueue;
	protected Queue<int> protectedIntQueue;
	internal Queue<int> internalIntQueue;

	public Dictionary<string, int> publicIntDict;
	private Dictionary<string, int> privateIntDict;
	protected Dictionary<string, int> protectedIntDict;
	internal Dictionary<string, int> internalIntDict;

	public List<int>[] publicIntListArray;
	private List<int>[] privateIntListArray;
	protected List<int>[] protectedIntListArray;
	internal List<int>[] internalIntListArray;
	#endregion

	#region UintCollections
	public uint[] publicUIntArray;
	private uint[] privateUIntArray;
	protected uint[] protectedUIntArray;
	internal uint[] internalUIntArray;

	public uint[,] publicUIntMatrix;
	private uint[,] privateUIntMatrix;
	protected uint[,] protectedUIntMatrix;
	internal uint[,] internalUIntMatrix;

	public List<uint> publicUIntList;
	private List<uint> privateUIntList;
	protected List<uint> protectedUIntList;
	internal List<uint> internalUIntList;

	public Stack<uint> publicUIntStack;
	private Stack<uint> privateUIntStack;
	protected Stack<uint> protectedUIntStack;
	internal Stack<uint> internalUIntStack;

	public Queue<uint> publicUIntQueue;
	private Queue<uint> privateUIntQueue;
	protected Queue<uint> protectedUIntQueue;
	internal Queue<uint> internalUIntQueue;

	public Dictionary<string, uint> publicUIntDict;
	private Dictionary<string, uint> privateUIntDict;
	protected Dictionary<string, uint> protectedUIntDict;
	internal Dictionary<string, uint> internalUIntDict;

	public List<uint>[] publicUIntListArray;
	private List<uint>[] privateUIntListArray;
	protected List<uint>[] protectedUIntListArray;
	internal List<uint>[] internalUIntListArray;
	#endregion

	#region LongCollections
	public long[] publicLongArray;
	private long[] privateLongArray;
	protected long[] protectedLongArray;
	internal long[] internalLongArray;

	public long[,] publicLongMatrix;
	private long[,] privateLongMatrix;
	protected long[,] protectedLongMatrix;
	internal long[,] internalLongMatrix;

	public List<long> publicLongList;
	private List<long> privateLongList;
	protected List<long> protectedLongList;
	internal List<long> internalLongList;

	public Stack<long> publicLongStack;
	private Stack<long> privateLongStack;
	protected Stack<long> protectedLongStack;
	internal Stack<long> internalLongStack;

	public Queue<long> publicLongQueue;
	private Queue<long> privateLongQueue;
	protected Queue<long> protectedLongQueue;
	internal Queue<long> internalLongQueue;

	public Dictionary<string, long> publicLongDict;
	private Dictionary<string, long> privateLongDict;
	protected Dictionary<string, long> protectedLongDict;
	internal Dictionary<string, long> internalLongDict;

	public List<long>[] publicLongListArray;
	private List<long>[] privateLongListArray;
	protected List<long>[] protectedLongListArray;
	internal List<long>[] internalLongListArray;
	#endregion

	#region UlongCollections
	public ulong[] publicULongArray;
	private ulong[] privateULongArray;
	protected ulong[] protectedULongArray;
	internal ulong[] internalULongArray;

	public ulong[,] publicULongMatrix;
	private ulong[,] privateULongMatrix;
	protected ulong[,] protectedULongMatrix;
	internal ulong[,] internalULongMatrix;

	public List<ulong> publicULongList;
	private List<ulong> privateULongList;
	protected List<ulong> protectedULongList;
	internal List<ulong> internalULongList;

	public Stack<ulong> publicULongStack;
	private Stack<ulong> privateULongStack;
	protected Stack<ulong> protectedULongStack;
	internal Stack<ulong> internalULongStack;

	public Queue<ulong> publicULongQueue;
	private Queue<ulong> privateULongQueue;
	protected Queue<ulong> protectedULongQueue;
	internal Queue<ulong> internalULongQueue;

	public Dictionary<string, ulong> publicULongDict;
	private Dictionary<string, ulong> privateULongDict;
	protected Dictionary<string, ulong> protectedULongDict;
	internal Dictionary<string, ulong> internalULongDict;

	public List<ulong>[] publicULongListArray;
	private List<ulong>[] privateULongListArray;
	protected List<ulong>[] protectedULongListArray;
	internal List<ulong>[] internalULongListArray;
	#endregion

	#region FloatCollections
	public float[] publicFloatArray;
	private float[] privateFloatArray;
	protected float[] protectedFloatArray;
	internal float[] internalFloatArray;

	public float[,] publicFloatMatrix;
	private float[,] privateFloatMatrix;
	protected float[,] protectedFloatMatrix;
	internal float[,] internalFloatMatrix;

	public List<float> publicFloatList;
	private List<float> privateFloatList;
	protected List<float> protectedFloatList;
	internal List<float> internalFloatList;

	public Stack<float> publicFloatStack;
	private Stack<float> privateFloatStack;
	protected Stack<float> protectedFloatStack;
	internal Stack<float> internalFloatStack;

	public Queue<float> publicFloatQueue;
	private Queue<float> privateFloatQueue;
	protected Queue<float> protectedFloatQueue;
	internal Queue<float> internalFloatQueue;

	public Dictionary<string, float> publicFloatDict;
	private Dictionary<string, float> privateFloatDict;
	protected Dictionary<string, float> protectedFloatDict;
	internal Dictionary<string, float> internalFloatDict;

	public List<float>[] publicFloatListArray;
	private List<float>[] privateFloatListArray;
	protected List<float>[] protectedFloatListArray;
	internal List<float>[] internalFloatListArray;
	#endregion

	#region DoubleCollections
	public double[] publicDoubleArray;
	private double[] privateDoubleArray;
	protected double[] protectedDoubleArray;
	internal double[] internalDoubleArray;

	public double[,] publicDoubleMatrix;
	private double[,] privateDoubleMatrix;
	protected double[,] protectedDoubleMatrix;
	internal double[,] internalDoubleMatrix;

	public List<double> publicDoubleList;
	private List<double> privateDoubleList;
	protected List<double> protectedDoubleList;
	internal List<double> internalDoubleList;

	public Stack<double> publicDoubleStack;
	private Stack<double> privateDoubleStack;
	protected Stack<double> protectedDoubleStack;
	internal Stack<double> internalDoubleStack;

	public Queue<double> publicDoubleQueue;
	private Queue<double> privateDoubleQueue;
	protected Queue<double> protectedDoubleQueue;
	internal Queue<double> internalDoubleQueue;

	public Dictionary<string, double> publicDoubleDict;
	private Dictionary<string, double> privateDoubleDict;
	protected Dictionary<string, double> protectedDoubleDict;
	internal Dictionary<string, double> internalDoubleDict;

	public List<double>[] publicDoubleListArray;
	private List<double>[] privateDoubleListArray;
	protected List<double>[] protectedDoubleListArray;
	internal List<double>[] internalDoubleListArray;
	#endregion

	#region DecimalCollections
	public decimal[] publicDecimalArray;
	private decimal[] privateDecimalArray;
	protected decimal[] protectedDecimalArray;
	internal decimal[] internalDecimalArray;

	public decimal[,] publicDecimalMatrix;
	private decimal[,] privateDecimalMatrix;
	protected decimal[,] protectedDecimalMatrix;
	internal decimal[,] internalDecimalMatrix;

	public List<decimal> publicDecimalList;
	private List<decimal> privateDecimalList;
	protected List<decimal> protectedDecimalList;
	internal List<decimal> internalDecimalList;

	public Stack<decimal> publicDecimalStack;
	private Stack<decimal> privateDecimalStack;
	protected Stack<decimal> protectedDecimalStack;
	internal Stack<decimal> internalDecimalStack;

	public Queue<decimal> publicDecimalQueue;
	private Queue<decimal> privateDecimalQueue;
	protected Queue<decimal> protectedDecimalQueue;
	internal Queue<decimal> internalDecimalQueue;

	public Dictionary<string, decimal> publicDecimalDict;
	private Dictionary<string, decimal> privateDecimalDict;
	protected Dictionary<string, decimal> protectedDecimalDict;
	internal Dictionary<string, decimal> internalDecimalDict;

	public List<decimal>[] publicDecimalListArray;
	private List<decimal>[] privateDecimalListArray;
	protected List<decimal>[] protectedDecimalListArray;
	internal List<decimal>[] internalDecimalListArray;
	#endregion

	#region CharCollections
	public char[] publicCharArray;
	private char[] privateCharArray;
	protected char[] protectedCharArray;
	internal char[] internalCharArray;

	public char[,] publicCharMatrix;
	private char[,] privateCharMatrix;
	protected char[,] protectedCharMatrix;
	internal char[,] internalCharMatrix;

	public List<char> publicCharList;
	private List<char> privateCharList;
	protected List<char> protectedCharList;
	internal List<char> internalCharList;

	public Stack<char> publicCharStack;
	private Stack<char> privateCharStack;
	protected Stack<char> protectedCharStack;
	internal Stack<char> internalCharStack;

	public Queue<char> publicCharQueue;
	private Queue<char> privateCharQueue;
	protected Queue<char> protectedCharQueue;
	internal Queue<char> internalCharQueue;

	public Dictionary<string, char> publicCharDict;
	private Dictionary<string, char> privateCharDict;
	protected Dictionary<string, char> protectedCharDict;
	internal Dictionary<string, char> internalCharDict;

	public List<char>[] publicCharListArray;
	private List<char>[] privateCharListArray;
	protected List<char>[] protectedCharListArray;
	internal List<char>[] internalCharListArray;
	#endregion

	#region StringCollections
	public string[] publicStringArray;
	private string[] privateStringArray;
	protected string[] protectedStringArray;
	internal string[] internalStringArray;

	public string[,] publicStringMatrix;
	private string[,] privateStringMatrix;
	protected string[,] protectedStringMatrix;
	internal string[,] internalStringMatrix;

	public List<string> publicStringList;
	private List<string> privateStringList;
	protected List<string> protectedStringList;
	internal List<string> internalStringList;

	public Stack<string> publicStringStack;
	private Stack<string> privateStringStack;
	protected Stack<string> protectedStringStack;
	internal Stack<string> internalStringStack;

	public Queue<string> publicStringQueue;
	private Queue<string> privateStringQueue;
	protected Queue<string> protectedStringQueue;
	internal Queue<string> internalStringQueue;

	public Dictionary<string, string> publicStringDict;
	private Dictionary<string, string> privateStringDict;
	protected Dictionary<string, string> protectedStringDict;
	internal Dictionary<string, string> internalStringDict;

	public List<string>[] publicStringListArray;
	private List<string>[] privateStringListArray;
	protected List<string>[] protectedStringListArray;
	internal List<string>[] internalStringListArray;
	#endregion
	#endregion

	#region UnityFields
	public Vector2 publicVector2;
	private Vector2 privateVector2;
	protected Vector2 protectedVector2;
	internal Vector2 internalVector2;

	public Vector3 publicVector3;
	private Vector3 privateVector3;
	protected Vector3 protectedVector3;
	internal Vector3 internalVector3;

	public Vector2Int publicVector2Int;
	private Vector2Int privateVector2Int;
	protected Vector2Int protectedVector2Int;
	internal Vector2Int internalVector2Int;

	public Vector3Int publicVector3Int;
	private Vector3Int privateVector3Int;
	protected Vector3Int protectedVector3Int;
	internal Vector3Int internalVector3Int;

	public Color publicColor;
	private Color privateColor;
	protected Color protectedColor;
	internal Color internalColor;

	public Bounds publicBounds;
	private Bounds privateBounds;
	protected Bounds protectedBounds;
	internal Bounds internalBounds;

	public BoundsInt publicBoundsInt;
	private BoundsInt privateBoundsInt;
	protected BoundsInt protectedBoundsInt;
	internal BoundsInt internalBoundsInt;
	#endregion

	#region UnityCollections
	#region Vector2Collections
	public Vector2[] publicVector2Array;
	private Vector2[] privateVector2Array;
	protected Vector2[] protectedVector2Array;
	internal Vector2[] internalVector2Array;

	public Vector2[,] publicVector2Matrix;
	private Vector2[,] privateVector2Matrix;
	protected Vector2[,] protectedVector2Matrix;
	internal Vector2[,] internalVector2Matrix;

	public List<Vector2> publicVector2List;
	private List<Vector2> privateVector2List;
	protected List<Vector2> protectedVector2List;
	internal List<Vector2> internalVector2List;

	public Stack<Vector2> publicVector2Stack;
	private Stack<Vector2> privateVector2Stack;
	protected Stack<Vector2> protectedVector2Stack;
	internal Stack<Vector2> internalVector2Stack;

	public Queue<Vector2> publicVector2Queue;
	private Queue<Vector2> privateVector2Queue;
	protected Queue<Vector2> protectedVector2Queue;
	internal Queue<Vector2> internalVector2Queue;

	public Dictionary<string, Vector2> publicVector2Dict;
	private Dictionary<string, Vector2> privateVector2Dict;
	protected Dictionary<string, Vector2> protectedVector2Dict;
	internal Dictionary<string, Vector2> internalVector2Dict;

	public List<Vector2>[] publicVector2ListArray;
	private List<Vector2>[] privateVector2ListArray;
	protected List<Vector2>[] protectedVector2ListArray;
	internal List<Vector2>[] internalVector2ListArray;
	#endregion

	#region Vector3Collections
	public Vector3[] publicVector3Array;
	private Vector3[] privateVector3Array;
	protected Vector3[] protectedVector3Array;
	internal Vector3[] internalVector3Array;

	public Vector3[,] publicVector3Matrix;
	private Vector3[,] privateVector3Matrix;
	protected Vector3[,] protectedVector3Matrix;
	internal Vector3[,] internalVector3Matrix;

	public List<Vector3> publicVector3List;
	private List<Vector3> privateVector3List;
	protected List<Vector3> protectedVector3List;
	internal List<Vector3> internalVector3List;

	public Stack<Vector3> publicVector3Stack;
	private Stack<Vector3> privateVector3Stack;
	protected Stack<Vector3> protectedVector3Stack;
	internal Stack<Vector3> internalVector3Stack;

	public Queue<Vector3> publicVector3Queue;
	private Queue<Vector3> privateVector3Queue;
	protected Queue<Vector3> protectedVector3Queue;
	internal Queue<Vector3> internalVector3Queue;

	public Dictionary<string, Vector3> publicVector3Dict;
	private Dictionary<string, Vector3> privateVector3Dict;
	protected Dictionary<string, Vector3> protectedVector3Dict;
	internal Dictionary<string, Vector3> internalVector3Dict;

	public List<Vector3>[] publicVector3ListArray;
	private List<Vector3>[] privateVector3ListArray;
	protected List<Vector3>[] protectedVector3ListArray;
	internal List<Vector3>[] internalVector3ListArray;
	#endregion

	#region Vector2IntCollections
	public Vector2Int[] publicVector2IntArray;
	private Vector2Int[] privateVector2IntArray;
	protected Vector2Int[] protectedVector2IntArray;
	internal Vector2Int[] internalVector2IntArray;

	public Vector2Int[,] publicVector2IntMatrix;
	private Vector2Int[,] privateVector2IntMatrix;
	protected Vector2Int[,] protectedVector2IntMatrix;
	internal Vector2Int[,] internalVector2IntMatrix;

	public List<Vector2Int> publicVector2IntList;
	private List<Vector2Int> privateVector2IntList;
	protected List<Vector2Int> protectedVector2IntList;
	internal List<Vector2Int> internalVector2IntList;

	public Stack<Vector2Int> publicVector2IntStack;
	private Stack<Vector2Int> privateVector2IntStack;
	protected Stack<Vector2Int> protectedVector2IntStack;
	internal Stack<Vector2Int> internalVector2IntStack;

	public Queue<Vector2Int> publicVector2IntQueue;
	private Queue<Vector2Int> privateVector2IntQueue;
	protected Queue<Vector2Int> protectedVector2IntQueue;
	internal Queue<Vector2Int> internalVector2IntQueue;

	public Dictionary<string, Vector2Int> publicVector2IntDict;
	private Dictionary<string, Vector2Int> privateVector2IntDict;
	protected Dictionary<string, Vector2Int> protectedVector2IntDict;
	internal Dictionary<string, Vector2Int> internalVector2IntDict;

	public List<Vector2Int>[] publicVector2IntListArray;
	private List<Vector2Int>[] privateVector2IntListArray;
	protected List<Vector2Int>[] protectedVector2IntListArray;
	internal List<Vector2Int>[] internalVector2IntListArray;
	#endregion

	#region Vector3IntCollections
	public Vector3Int[] publicVector3IntArray;
	private Vector3Int[] privateVector3IntArray;
	protected Vector3Int[] protectedVector3IntArray;
	internal Vector3Int[] internalVector3IntArray;

	public Vector3Int[,] publicVector3IntMatrix;
	private Vector3Int[,] privateVector3IntMatrix;
	protected Vector3Int[,] protectedVector3IntMatrix;
	internal Vector3Int[,] internalVector3IntMatrix;

	public List<Vector3Int> publicVector3IntList;
	private List<Vector3Int> privateVector3IntList;
	protected List<Vector3Int> protectedVector3IntList;
	internal List<Vector3Int> internalVector3IntList;

	public Stack<Vector3Int> publicVector3IntStack;
	private Stack<Vector3Int> privateVector3IntStack;
	protected Stack<Vector3Int> protectedVector3IntStack;
	internal Stack<Vector3Int> internalVector3IntStack;

	public Queue<Vector3Int> publicVector3IntQueue;
	private Queue<Vector3Int> privateVector3IntQueue;
	protected Queue<Vector3Int> protectedVector3IntQueue;
	internal Queue<Vector3Int> internalVector3IntQueue;

	public Dictionary<string, Vector3Int> publicVector3IntDict;
	private Dictionary<string, Vector3Int> privateVector3IntDict;
	protected Dictionary<string, Vector3Int> protectedVector3IntDict;
	internal Dictionary<string, Vector3Int> internalVector3IntDict;

	public List<Vector3Int>[] publicVector3IntListArray;
	private List<Vector3Int>[] privateVector3IntListArray;
	protected List<Vector3Int>[] protectedVector3IntListArray;
	internal List<Vector3Int>[] internalVector3IntListArray;
	#endregion

	#region ColorCollections
	public Color[] publicColorArray;
	private Color[] privateColorArray;
	protected Color[] protectedColorArray;
	internal Color[] internalColorArray;

	public Color[,] publicColorMatrix;
	private Color[,] privateColorMatrix;
	protected Color[,] protectedColorMatrix;
	internal Color[,] internalColorMatrix;

	public List<Color> publicColorList;
	private List<Color> privateColorList;
	protected List<Color> protectedColorList;
	internal List<Color> internalColorList;

	public Stack<Color> publicColorStack;
	private Stack<Color> privateColorStack;
	protected Stack<Color> protectedColorStack;
	internal Stack<Color> internalColorStack;

	public Queue<Color> publicColorQueue;
	private Queue<Color> privateColorQueue;
	protected Queue<Color> protectedColorQueue;
	internal Queue<Color> internalColorQueue;

	public Dictionary<string, Color> publicColorDict;
	private Dictionary<string, Color> privateColorDict;
	protected Dictionary<string, Color> protectedColorDict;
	internal Dictionary<string, Color> internalColorDict;

	public List<Color>[] publicColorListArray;
	private List<Color>[] privateColorListArray;
	protected List<Color>[] protectedColorListArray;
	internal List<Color>[] internalColorListArray;
	#endregion

	#region BoundsCollections
	public Bounds[] publicBoundsArray;
	private Bounds[] privateBoundsArray;
	protected Bounds[] protectedBoundsArray;
	internal Bounds[] internalBoundsArray;

	public Bounds[,] publicBoundsMatrix;
	private Bounds[,] privateBoundsMatrix;
	protected Bounds[,] protectedBoundsMatrix;
	internal Bounds[,] internalBoundsMatrix;

	public List<Bounds> publicBoundsList;
	private List<Bounds> privateBoundsList;
	protected List<Bounds> protectedBoundsList;
	internal List<Bounds> internalBoundsList;

	public Stack<Bounds> publicBoundsStack;
	private Stack<Bounds> privateBoundsStack;
	protected Stack<Bounds> protectedBoundsStack;
	internal Stack<Bounds> internalBoundsStack;

	public Queue<Bounds> publicBoundsQueue;
	private Queue<Bounds> privateBoundsQueue;
	protected Queue<Bounds> protectedBoundsQueue;
	internal Queue<Bounds> internalBoundsQueue;

	public Dictionary<string, Bounds> publicBoundsDict;
	private Dictionary<string, Bounds> privateBoundsDict;
	protected Dictionary<string, Bounds> protectedBoundsDict;
	internal Dictionary<string, Bounds> internalBoundsDict;

	public List<Bounds>[] publicBoundsListArray;
	private List<Bounds>[] privateBoundsListArray;
	protected List<Bounds>[] protectedBoundsListArray;
	internal List<Bounds>[] internalBoundsListArray;
	#endregion

	#region BoundsIntCollections
	public BoundsInt[] publicBoundsIntArray;
	private BoundsInt[] privateBoundsIntArray;
	protected BoundsInt[] protectedBoundsIntArray;
	internal BoundsInt[] internalBoundsIntArray;

	public BoundsInt[,] publicBoundsIntMatrix;
	private BoundsInt[,] privateBoundsIntMatrix;
	protected BoundsInt[,] protectedBoundsIntMatrix;
	internal BoundsInt[,] internalBoundsIntMatrix;

	public List<BoundsInt> publicBoundsIntList;
	private List<BoundsInt> privateBoundsIntList;
	protected List<BoundsInt> protectedBoundsIntList;
	internal List<BoundsInt> internalBoundsIntList;

	public Stack<BoundsInt> publicBoundsIntStack;
	private Stack<BoundsInt> privateBoundsIntStack;
	protected Stack<BoundsInt> protectedBoundsIntStack;
	internal Stack<BoundsInt> internalBoundsIntStack;

	public Queue<BoundsInt> publicBoundsIntQueue;
	private Queue<BoundsInt> privateBoundsIntQueue;
	protected Queue<BoundsInt> protectedBoundsIntQueue;
	internal Queue<BoundsInt> internalBoundsIntQueue;

	public Dictionary<string, BoundsInt> publicBoundsIntDict;
	private Dictionary<string, BoundsInt> privateBoundsIntDict;
	protected Dictionary<string, BoundsInt> protectedBoundsIntDict;
	internal Dictionary<string, BoundsInt> internalBoundsIntDict;

	public List<BoundsInt>[] publicBoundsIntListArray;
	private List<BoundsInt>[] privateBoundsIntListArray;
	protected List<BoundsInt>[] protectedBoundsIntListArray;
	internal List<BoundsInt>[] internalBoundsIntListArray;
	#endregion
	#endregion

	#region Enums
	public ShortEnum publicShortEnum;
	private ShortEnum privateShortEnum;
	protected ShortEnum protectedShortEnum;
	internal ShortEnum internalShortEnum;

	public UShortEnum publicUShortEnum;
	private UShortEnum privateUShortEnum;
	protected UShortEnum protectedUShortEnum;
	internal UShortEnum internalUShortEnum;

	public IntEnum publicIntEnum;
	private IntEnum privateIntEnum;
	protected IntEnum protectedIntEnum;
	internal IntEnum internalIntEnum;

	public UIntEnum publicUIntEnum;
	private UIntEnum privateUIntEnum;
	protected UIntEnum protectedUIntEnum;
	internal UIntEnum internalUIntEnum;

	public LongEnum publicLongEnum;
	private LongEnum privateLongEnum;
	protected LongEnum protectedLongEnum;
	internal LongEnum internalLongEnum;

	public ULongEnum publicULongEnum;
	private ULongEnum privateULongEnum;
	protected ULongEnum protectedULongEnum;
	internal ULongEnum internalULongEnum;
	#endregion

	#region EnumsCollections
	#region ShortEnumCollections
	public ShortEnum[] publicShortEnumArray;
	private ShortEnum[] privateShortEnumArray;
	protected ShortEnum[] protectedShortEnumArray;
	internal ShortEnum[] internalShortEnumArray;

	public ShortEnum[,] publicShortEnumMatrix;
	private ShortEnum[,] privateShortEnumMatrix;
	protected ShortEnum[,] protectedShortEnumMatrix;
	internal ShortEnum[,] internalShortEnumMatrix;

	public List<ShortEnum> publicShortEnumList;
	private List<ShortEnum> privateShortEnumList;
	protected List<ShortEnum> protectedShortEnumList;
	internal List<ShortEnum> internalShortEnumList;

	public Stack<ShortEnum> publicShortEnumStack;
	private Stack<ShortEnum> privateShortEnumStack;
	protected Stack<ShortEnum> protectedShortEnumStack;
	internal Stack<ShortEnum> internalShortEnumStack;

	public Queue<ShortEnum> publicShortEnumQueue;
	private Queue<ShortEnum> privateShortEnumQueue;
	protected Queue<ShortEnum> protectedShortEnumQueue;
	internal Queue<ShortEnum> internalShortEnumQueue;

	public Dictionary<string, ShortEnum> publicShortEnumDict;
	private Dictionary<string, ShortEnum> privateShortEnumDict;
	protected Dictionary<string, ShortEnum> protectedShortEnumDict;
	internal Dictionary<string, ShortEnum> internalShortEnumDict;

	public List<ShortEnum>[] publicShortEnumListArray;
	private List<ShortEnum>[] privateShortEnumListArray;
	protected List<ShortEnum>[] protectedShortEnumListArray;
	internal List<ShortEnum>[] internalShortEnumListArray;
	#endregion

	#region UShortEnumCollections
	public UShortEnum[] publicUShortEnumArray;
	private UShortEnum[] privateUShortEnumArray;
	protected UShortEnum[] protectedUShortEnumArray;
	internal UShortEnum[] internalUShortEnumArray;

	public UShortEnum[,] publicUShortEnumMatrix;
	private UShortEnum[,] privateUShortEnumMatrix;
	protected UShortEnum[,] protectedUShortEnumMatrix;
	internal UShortEnum[,] internalUShortEnumMatrix;

	public List<UShortEnum> publicUShortEnumList;
	private List<UShortEnum> privateUShortEnumList;
	protected List<UShortEnum> protectedUShortEnumList;
	internal List<UShortEnum> internalUShortEnumList;

	public Stack<UShortEnum> publicUShortEnumStack;
	private Stack<UShortEnum> privateUShortEnumStack;
	protected Stack<UShortEnum> protectedUShortEnumStack;
	internal Stack<UShortEnum> internalUShortEnumStack;

	public Queue<UShortEnum> publicUShortEnumQueue;
	private Queue<UShortEnum> privateUShortEnumQueue;
	protected Queue<UShortEnum> protectedUShortEnumQueue;
	internal Queue<UShortEnum> internalUShortEnumQueue;

	public Dictionary<string, UShortEnum> publicUShortEnumDict;
	private Dictionary<string, UShortEnum> privateUShortEnumDict;
	protected Dictionary<string, UShortEnum> protectedUShortEnumDict;
	internal Dictionary<string, UShortEnum> internalUShortEnumDict;

	public List<UShortEnum>[] publicUShortEnumListArray;
	private List<UShortEnum>[] privateUShortEnumListArray;
	protected List<UShortEnum>[] protectedUShortEnumListArray;
	internal List<UShortEnum>[] internalUShortEnumListArray;
	#endregion

	#region IntEnumCollections
	public IntEnum[] publicIntEnumArray;
	private IntEnum[] privateIntEnumArray;
	protected IntEnum[] protectedIntEnumArray;
	internal IntEnum[] internalIntEnumArray;

	public IntEnum[,] publicIntEnumMatrix;
	private IntEnum[,] privateIntEnumMatrix;
	protected IntEnum[,] protectedIntEnumMatrix;
	internal IntEnum[,] internalIntEnumMatrix;

	public List<IntEnum> publicIntEnumList;
	private List<IntEnum> privateIntEnumList;
	protected List<IntEnum> protectedIntEnumList;
	internal List<IntEnum> internalIntEnumList;

	public Stack<IntEnum> publicIntEnumStack;
	private Stack<IntEnum> privateIntEnumStack;
	protected Stack<IntEnum> protectedIntEnumStack;
	internal Stack<IntEnum> internalIntEnumStack;

	public Queue<IntEnum> publicIntEnumQueue;
	private Queue<IntEnum> privateIntEnumQueue;
	protected Queue<IntEnum> protectedIntEnumQueue;
	internal Queue<IntEnum> internalIntEnumQueue;

	public Dictionary<string, IntEnum> publicIntEnumDict;
	private Dictionary<string, IntEnum> privateIntEnumDict;
	protected Dictionary<string, IntEnum> protectedIntEnumDict;
	internal Dictionary<string, IntEnum> internalIntEnumDict;

	public List<IntEnum>[] publicIntEnumListArray;
	private List<IntEnum>[] privateIntEnumListArray;
	protected List<IntEnum>[] protectedIntEnumListArray;
	internal List<IntEnum>[] internalIntEnumListArray;
	#endregion

	#region UIntEnumCollections
	public UIntEnum[] publicUIntEnumArray;
	private UIntEnum[] privateUIntEnumArray;
	protected UIntEnum[] protectedUIntEnumArray;
	internal UIntEnum[] internalUIntEnumArray;

	public UIntEnum[,] publicUIntEnumMatrix;
	private UIntEnum[,] privateUIntEnumMatrix;
	protected UIntEnum[,] protectedUIntEnumMatrix;
	internal UIntEnum[,] internalUIntEnumMatrix;

	public List<UIntEnum> publicUIntEnumList;
	private List<UIntEnum> privateUIntEnumList;
	protected List<UIntEnum> protectedUIntEnumList;
	internal List<UIntEnum> internalUIntEnumList;

	public Stack<UIntEnum> publicUIntEnumStack;
	private Stack<UIntEnum> privateUIntEnumStack;
	protected Stack<UIntEnum> protectedUIntEnumStack;
	internal Stack<UIntEnum> internalUIntEnumStack;

	public Queue<UIntEnum> publicUIntEnumQueue;
	private Queue<UIntEnum> privateUIntEnumQueue;
	protected Queue<UIntEnum> protectedUIntEnumQueue;
	internal Queue<UIntEnum> internalUIntEnumQueue;

	public Dictionary<string, UIntEnum> publicUIntEnumDict;
	private Dictionary<string, UIntEnum> privateUIntEnumDict;
	protected Dictionary<string, UIntEnum> protectedUIntEnumDict;
	internal Dictionary<string, UIntEnum> internalUIntEnumDict;

	public List<UIntEnum>[] publicUIntEnumListArray;
	private List<UIntEnum>[] privateUIntEnumListArray;
	protected List<UIntEnum>[] protectedUIntEnumListArray;
	internal List<UIntEnum>[] internalUIntEnumListArray;
	#endregion

	#region LongEnumCollections
	public LongEnum[] publicLongEnumArray;
	private LongEnum[] privateLongEnumArray;
	protected LongEnum[] protectedLongEnumArray;
	internal LongEnum[] internalLongEnumArray;

	public LongEnum[,] publicLongEnumMatrix;
	private LongEnum[,] privateLongEnumMatrix;
	protected LongEnum[,] protectedLongEnumMatrix;
	internal LongEnum[,] internalLongEnumMatrix;

	public List<LongEnum> publicLongEnumList;
	private List<LongEnum> privateLongEnumList;
	protected List<LongEnum> protectedLongEnumList;
	internal List<LongEnum> internalLongEnumList;

	public Stack<LongEnum> publicLongEnumStack;
	private Stack<LongEnum> privateLongEnumStack;
	protected Stack<LongEnum> protectedLongEnumStack;
	internal Stack<LongEnum> internalLongEnumStack;

	public Queue<LongEnum> publicLongEnumQueue;
	private Queue<LongEnum> privateLongEnumQueue;
	protected Queue<LongEnum> protectedLongEnumQueue;
	internal Queue<LongEnum> internalLongEnumQueue;

	public Dictionary<string, LongEnum> publicLongEnumDict;
	private Dictionary<string, LongEnum> privateLongEnumDict;
	protected Dictionary<string, LongEnum> protectedLongEnumDict;
	internal Dictionary<string, LongEnum> internalLongEnumDict;

	public List<LongEnum>[] publicLongEnumListArray;
	private List<LongEnum>[] privateLongEnumListArray;
	protected List<LongEnum>[] protectedLongEnumListArray;
	internal List<LongEnum>[] internalLongEnumListArray;
	#endregion

	#region ULongEnumCollections
	public ULongEnum[] publicULongEnumArray;
	private ULongEnum[] privateULongEnumArray;
	protected ULongEnum[] protectedULongEnumArray;
	internal ULongEnum[] internalULongEnumArray;

	public ULongEnum[,] publicULongEnumMatrix;
	private ULongEnum[,] privateULongEnumMatrix;
	protected ULongEnum[,] protectedULongEnumMatrix;
	internal ULongEnum[,] internalULongEnumMatrix;

	public List<ULongEnum> publicULongEnumList;
	private List<ULongEnum> privateULongEnumList;
	protected List<ULongEnum> protectedULongEnumList;
	internal List<ULongEnum> internalULongEnumList;

	public Stack<ULongEnum> publicULongEnumStack;
	private Stack<ULongEnum> privateULongEnumStack;
	protected Stack<ULongEnum> protectedULongEnumStack;
	internal Stack<ULongEnum> internalULongEnumStack;

	public Queue<ULongEnum> publicULongEnumQueue;
	private Queue<ULongEnum> privateULongEnumQueue;
	protected Queue<ULongEnum> protectedULongEnumQueue;
	internal Queue<ULongEnum> internalULongEnumQueue;

	public Dictionary<string, ULongEnum> publicULongEnumDict;
	private Dictionary<string, ULongEnum> privateULongEnumDict;
	protected Dictionary<string, ULongEnum> protectedULongEnumDict;
	internal Dictionary<string, ULongEnum> internalULongEnumDict;

	public List<ULongEnum>[] publicULongEnumListArray;
	private List<ULongEnum>[] privateULongEnumListArray;
	protected List<ULongEnum>[] protectedULongEnumListArray;
	internal List<ULongEnum>[] internalULongEnumListArray;
	#endregion
	#endregion

	#region CustomStruct
	public TestStruct publicTestStruct;
	private TestStruct privateTestStruct;
	protected TestStruct protectedTestStruct;
	internal TestStruct internalTestStruct;
	#endregion

	#region CustomStructCollection
	public TestStruct[] publicTestStructArray;
	private TestStruct[] privateTestStructArray;
	protected TestStruct[] protectedTestStructArray;
	internal TestStruct[] internalTestStructArray;

	public TestStruct[,] publicTestStructMatrix;
	private TestStruct[,] privateTestStructMatrix;
	protected TestStruct[,] protectedTestStructMatrix;
	internal TestStruct[,] internalTestStructMatrix;

	public List<TestStruct> publicTestStructList;
	private List<TestStruct> privateTestStructList;
	protected List<TestStruct> protectedTestStructList;
	internal List<TestStruct> internalTestStructList;

	public Stack<TestStruct> publicTestStructStack;
	private Stack<TestStruct> privateTestStructStack;
	protected Stack<TestStruct> protectedTestStructStack;
	internal Stack<TestStruct> internalTestStructStack;

	public Queue<TestStruct> publicTestStructQueue;
	private Queue<TestStruct> privateTestStructQueue;
	protected Queue<TestStruct> protectedTestStructQueue;
	internal Queue<TestStruct> internalTestStructQueue;

	public Dictionary<string, TestStruct> publicTestStructDict;
	private Dictionary<string, TestStruct> privateTestStructDict;
	protected Dictionary<string, TestStruct> protectedTestStructDict;
	internal Dictionary<string, TestStruct> internalTestStructDict;

	public List<TestStruct>[] publicTestStructListArray;
	private List<TestStruct>[] privateTestStructListArray;
	protected List<TestStruct>[] protectedTestStructListArray;
	internal List<TestStruct>[] internalTestStructListArray;
	#endregion

	public LeanClass() : base()
	{
		Random rnd = new Random();

		#region C#Fields
		publicBool = rnd.Next(2) == 0;
		privateBool = rnd.Next(2) == 0;
		protectedBool = rnd.Next(2) == 0;
		internalBool = rnd.Next(2) == 0;

		publicByte = (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1);
		privateByte = (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1);
		protectedByte = (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1);
		internalByte = (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1);

		publicSByte = (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1);
		privateSByte = (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1);
		protectedSByte = (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1);
		internalSByte = (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1);

		publicShort = (short)rnd.Next(short.MinValue, short.MaxValue + 1);
		privateShort = (short)rnd.Next(short.MinValue, short.MaxValue + 1);
		protectedShort = (short)rnd.Next(short.MinValue, short.MaxValue + 1);
		internalShort = (short)rnd.Next(short.MinValue, short.MaxValue + 1);

		publicUShort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1);
		privateUShort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1);
		protectedUShort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1);
		internalUShort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1);

		publicInt = rnd.Next();
		privateInt = rnd.Next();
		protectedInt = rnd.Next();
		internalInt = rnd.Next();

		publicUInt = (uint)rnd.Next();
		privateUInt = (uint)rnd.Next();
		protectedUInt = (uint)rnd.Next();
		internalUInt = (uint)rnd.Next();

		publicLong = ((long)rnd.Next() << 32) | (uint)rnd.Next();
		privateLong = ((long)rnd.Next() << 32) | (uint)rnd.Next();
		protectedLong = ((long)rnd.Next() << 32) | (uint)rnd.Next();
		internalLong = ((long)rnd.Next() << 32) | (uint)rnd.Next();

		publicULong = (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next());
		privateULong = (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next());
		protectedULong = (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next());
		internalULong = (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next());

		publicFloat = (float)(rnd.NextDouble() * 1000);
		privateFloat = (float)(rnd.NextDouble() * 1000);
		protectedFloat = (float)(rnd.NextDouble() * 1000);
		internalFloat = (float)(rnd.NextDouble() * 1000);

		publicDouble = rnd.NextDouble() * 1000;
		privateDouble = rnd.NextDouble() * 1000;
		protectedDouble = rnd.NextDouble() * 1000;
		internalDouble = rnd.NextDouble() * 1000;

		publicDecimal = (decimal)(rnd.NextDouble() * 1000);
		privateDecimal = (decimal)(rnd.NextDouble() * 1000);
		protectedDecimal = (decimal)(rnd.NextDouble() * 1000);
		internalDecimal = (decimal)(rnd.NextDouble() * 1000);

		publicChar = (char)rnd.Next(65, 91);
		privateChar = (char)rnd.Next(65, 91);
		protectedChar = (char)rnd.Next(65, 91);
		internalChar = (char)rnd.Next(65, 91);

		publicString = RandomString(rnd, 8);
		privateString = RandomString(rnd, 8);
		protectedString = RandomString(rnd, 8);
		internalString = RandomString(rnd, 8);
		#endregion

		#region C#Collections
		#region BoolCollections
		publicBoolArray = new bool[3] { rnd.Next(2) == 0,
			rnd.Next(2) == 0,
			rnd.Next(2) == 0 };

		publicBoolMatrix = new bool[1, 3] { { rnd.Next(2) == 0,
				rnd.Next(2) == 0,
				rnd.Next(2) == 0 } };

		publicBoolList = new List<bool>() { rnd.Next(2) == 0,
			rnd.Next(2) == 0,
			rnd.Next(2) == 0 };

		publicBoolStack = new Stack<bool>();
		publicBoolStack.Push(rnd.Next(2) == 0);
		publicBoolStack.Push(rnd.Next(2) == 0);
		publicBoolStack.Push(rnd.Next(2) == 0);

		publicBoolQueue = new Queue<bool>();
		publicBoolQueue.Enqueue(rnd.Next(2) == 0);
		publicBoolQueue.Enqueue(rnd.Next(2) == 0);
		publicBoolQueue.Enqueue(rnd.Next(2) == 0);

		publicBoolDict = new Dictionary<string, bool>()
		{
			{ "key1", rnd.Next(2) == 0 },
			{ "key2", rnd.Next(2) == 0 },
			{ "key3", rnd.Next(2) == 0 }
		};

		publicBoolListArray = new List<bool>[3];
		for (int i = 0; i < 3; i++)
			publicBoolListArray[i] = new List<bool>() { rnd.Next(2) == 0,
				rnd.Next(2) == 0,
				rnd.Next(2) == 0 };

		privateBoolArray = new bool[3] { rnd.Next(2) == 0,
			rnd.Next(2) == 0,
			rnd.Next(2) == 0 };

		privateBoolMatrix = new bool[1, 3] { { rnd.Next(2) == 0,
				rnd.Next(2) == 0,
				rnd.Next(2) == 0 } };

		privateBoolList = new List<bool>() { rnd.Next(2) == 0,
			rnd.Next(2) == 0,
			rnd.Next(2) == 0 };

		privateBoolStack = new Stack<bool>();
		privateBoolStack.Push(rnd.Next(2) == 0);
		privateBoolStack.Push(rnd.Next(2) == 0);
		privateBoolStack.Push(rnd.Next(2) == 0);

		privateBoolQueue = new Queue<bool>();
		privateBoolQueue.Enqueue(rnd.Next(2) == 0);
		privateBoolQueue.Enqueue(rnd.Next(2) == 0);
		privateBoolQueue.Enqueue(rnd.Next(2) == 0);

		privateBoolDict = new Dictionary<string, bool>()
		{
			{ "key1", rnd.Next(2) == 0 },
			{ "key2", rnd.Next(2) == 0 },
			{ "key3", rnd.Next(2) == 0 }
		};

		privateBoolListArray = new List<bool>[3];
		for (int i = 0; i < 3; i++)
			privateBoolListArray[i] = new List<bool>() { rnd.Next(2) == 0,
				rnd.Next(2) == 0,
				rnd.Next(2) == 0 };

		protectedBoolArray = new bool[3] { rnd.Next(2) == 0,
			rnd.Next(2) == 0,
			rnd.Next(2) == 0 };

		protectedBoolMatrix = new bool[1, 3] { { rnd.Next(2) == 0,
				rnd.Next(2) == 0,
				rnd.Next(2) == 0 } };

		protectedBoolList = new List<bool>() { rnd.Next(2) == 0,
			rnd.Next(2) == 0,
			rnd.Next(2) == 0 };

		protectedBoolStack = new Stack<bool>();
		protectedBoolStack.Push(rnd.Next(2) == 0);
		protectedBoolStack.Push(rnd.Next(2) == 0);
		protectedBoolStack.Push(rnd.Next(2) == 0);

		protectedBoolQueue = new Queue<bool>();
		protectedBoolQueue.Enqueue(rnd.Next(2) == 0);
		protectedBoolQueue.Enqueue(rnd.Next(2) == 0);
		protectedBoolQueue.Enqueue(rnd.Next(2) == 0);

		protectedBoolDict = new Dictionary<string, bool>()
		{
			{ "key1", rnd.Next(2) == 0 },
			{ "key2", rnd.Next(2) == 0 },
			{ "key3", rnd.Next(2) == 0 }
		};

		protectedBoolListArray = new List<bool>[3];
		for (int i = 0; i < 3; i++)
			protectedBoolListArray[i] = new List<bool>() { rnd.Next(2) == 0,
				rnd.Next(2) == 0,
				rnd.Next(2) == 0 };

		internalBoolArray = new bool[3] { rnd.Next(2) == 0,
			rnd.Next(2) == 0,
			rnd.Next(2) == 0 };

		internalBoolMatrix = new bool[1, 3] { { rnd.Next(2) == 0,
				rnd.Next(2) == 0,
				rnd.Next(2) == 0 } };

		internalBoolList = new List<bool>() { rnd.Next(2) == 0,
			rnd.Next(2) == 0,
			rnd.Next(2) == 0 };

		internalBoolStack = new Stack<bool>();
		internalBoolStack.Push(rnd.Next(2) == 0);
		internalBoolStack.Push(rnd.Next(2) == 0);
		internalBoolStack.Push(rnd.Next(2) == 0);

		internalBoolQueue = new Queue<bool>();
		internalBoolQueue.Enqueue(rnd.Next(2) == 0);
		internalBoolQueue.Enqueue(rnd.Next(2) == 0);
		internalBoolQueue.Enqueue(rnd.Next(2) == 0);

		internalBoolDict = new Dictionary<string, bool>()
		{
			{ "key1", rnd.Next(2) == 0 },
			{ "key2", rnd.Next(2) == 0 },
			{ "key3", rnd.Next(2) == 0 }
		};

		internalBoolListArray = new List<bool>[3];
		for (int i = 0; i < 3; i++)
			internalBoolListArray[i] = new List<bool>() { rnd.Next(2) == 0,
				rnd.Next(2) == 0,
				rnd.Next(2) == 0 };
		#endregion

		#region ByteCollections
		publicByteArray = new byte[3] { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		publicByteMatrix = new byte[1, 3] { { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) } };

		publicByteList = new List<byte>() { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		publicByteStack = new Stack<byte>();
		publicByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		publicByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		publicByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));

		publicByteQueue = new Queue<byte>();
		publicByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		publicByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		publicByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));

		publicByteDict = new Dictionary<string, byte>()
		{
			{ "key1", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) },
			{ "key2", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) },
			{ "key3", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) }
		};

		publicByteListArray = new List<byte>[3];
		for (int i = 0; i < 3; i++)
			publicByteListArray[i] = new List<byte>() { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		privateByteArray = new byte[3] { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		privateByteMatrix = new byte[1, 3] { { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) } };

		privateByteList = new List<byte>() { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		privateByteStack = new Stack<byte>();
		privateByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		privateByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		privateByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));

		privateByteQueue = new Queue<byte>();
		privateByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		privateByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		privateByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));

		privateByteDict = new Dictionary<string, byte>()
		{
			{ "key1", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) },
			{ "key2", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) },
			{ "key3", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) }
		};

		privateByteListArray = new List<byte>[3];
		for (int i = 0; i < 3; i++)
			privateByteListArray[i] = new List<byte>() { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		protectedByteArray = new byte[3] { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		protectedByteMatrix = new byte[1, 3] { { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) } };

		protectedByteList = new List<byte>() { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		protectedByteStack = new Stack<byte>();
		protectedByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		protectedByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		protectedByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));

		protectedByteQueue = new Queue<byte>();
		protectedByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		protectedByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		protectedByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));

		protectedByteDict = new Dictionary<string, byte>()
		{
			{ "key1", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) },
			{ "key2", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) },
			{ "key3", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) }
		};

		protectedByteListArray = new List<byte>[3];
		for (int i = 0; i < 3; i++)
			protectedByteListArray[i] = new List<byte>() { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		internalByteArray = new byte[3] { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		internalByteMatrix = new byte[1, 3] { { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) } };

		internalByteList = new List<byte>() { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
			(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };

		internalByteStack = new Stack<byte>();
		internalByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		internalByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		internalByteStack.Push((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));

		internalByteQueue = new Queue<byte>();
		internalByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		internalByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));
		internalByteQueue.Enqueue((byte)rnd.Next(byte.MinValue, byte.MaxValue + 1));

		internalByteDict = new Dictionary<string, byte>()
		{
			{ "key1", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) },
			{ "key2", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) },
			{ "key3", (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) }
		};

		internalByteListArray = new List<byte>[3];
		for (int i = 0; i < 3; i++)
			internalByteListArray[i] = new List<byte>() { (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1),
				(byte)rnd.Next(byte.MinValue, byte.MaxValue + 1) };
		#endregion

		#region SbyteCollections
		publicSByteArray = new sbyte[3] { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		publicSByteMatrix = new sbyte[1, 3] { { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) } };

		publicSByteList = new List<sbyte>() { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		publicSByteStack = new Stack<sbyte>();
		publicSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		publicSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		publicSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));

		publicSByteQueue = new Queue<sbyte>();
		publicSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		publicSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		publicSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));

		publicSByteDict = new Dictionary<string, sbyte>()
		{
			{ "key1", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) },
			{ "key2", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) },
			{ "key3", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) }
		};

		publicSByteListArray = new List<sbyte>[3];
		for (int i = 0; i < 3; i++)
			publicSByteListArray[i] = new List<sbyte>() { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		privateSByteArray = new sbyte[3] { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		privateSByteMatrix = new sbyte[1, 3] { { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) } };

		privateSByteList = new List<sbyte>() { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		privateSByteStack = new Stack<sbyte>();
		privateSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		privateSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		privateSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));

		privateSByteQueue = new Queue<sbyte>();
		privateSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		privateSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		privateSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));

		privateSByteDict = new Dictionary<string, sbyte>()
		{
			{ "key1", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) },
			{ "key2", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) },
			{ "key3", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) }
		};

		privateSByteListArray = new List<sbyte>[3];
		for (int i = 0; i < 3; i++)
			privateSByteListArray[i] = new List<sbyte>() { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		protectedSByteArray = new sbyte[3] { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		protectedSByteMatrix = new sbyte[1, 3] { { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) } };

		protectedSByteList = new List<sbyte>() { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		protectedSByteStack = new Stack<sbyte>();
		protectedSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		protectedSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		protectedSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));

		protectedSByteQueue = new Queue<sbyte>();
		protectedSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		protectedSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		protectedSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));

		protectedSByteDict = new Dictionary<string, sbyte>()
		{
			{ "key1", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) },
			{ "key2", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) },
			{ "key3", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) }
		};

		protectedSByteListArray = new List<sbyte>[3];
		for (int i = 0; i < 3; i++)
			protectedSByteListArray[i] = new List<sbyte>() { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		internalSByteArray = new sbyte[3] { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		internalSByteMatrix = new sbyte[1, 3] { { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) } };

		internalSByteList = new List<sbyte>() { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
			(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };

		internalSByteStack = new Stack<sbyte>();
		internalSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		internalSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		internalSByteStack.Push((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));

		internalSByteQueue = new Queue<sbyte>();
		internalSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		internalSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));
		internalSByteQueue.Enqueue((sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1));

		internalSByteDict = new Dictionary<string, sbyte>()
		{
			{ "key1", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) },
			{ "key2", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) },
			{ "key3", (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) }
		};

		internalSByteListArray = new List<sbyte>[3];
		for (int i = 0; i < 3; i++)
			internalSByteListArray[i] = new List<sbyte>() { (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1),
				(sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1) };
		#endregion

		#region ShortCollections
		publicShortArray = new short[3] { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		publicShortMatrix = new short[1, 3] { { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) } };

		publicShortList = new List<short>() { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		publicShortStack = new Stack<short>();
		publicShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		publicShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		publicShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));

		publicShortQueue = new Queue<short>();
		publicShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		publicShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		publicShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));

		publicShortDict = new Dictionary<string, short>()
		{
			{ "key1", (short)rnd.Next(short.MinValue, short.MaxValue + 1) },
			{ "key2", (short)rnd.Next(short.MinValue, short.MaxValue + 1) },
			{ "key3", (short)rnd.Next(short.MinValue, short.MaxValue + 1) }
		};

		publicShortListArray = new List<short>[3];
		for (int i = 0; i < 3; i++)
			publicShortListArray[i] = new List<short>() { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		privateShortArray = new short[3] { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		privateShortMatrix = new short[1, 3] { { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) } };

		privateShortList = new List<short>() { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		privateShortStack = new Stack<short>();
		privateShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		privateShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		privateShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));

		privateShortQueue = new Queue<short>();
		privateShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		privateShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		privateShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));

		privateShortDict = new Dictionary<string, short>()
		{
			{ "key1", (short)rnd.Next(short.MinValue, short.MaxValue + 1) },
			{ "key2", (short)rnd.Next(short.MinValue, short.MaxValue + 1) },
			{ "key3", (short)rnd.Next(short.MinValue, short.MaxValue + 1) }
		};

		privateShortListArray = new List<short>[3];
		for (int i = 0; i < 3; i++)
			privateShortListArray[i] = new List<short>() { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		protectedShortArray = new short[3] { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		protectedShortMatrix = new short[1, 3] { { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) } };

		protectedShortList = new List<short>() { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		protectedShortStack = new Stack<short>();
		protectedShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		protectedShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		protectedShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));

		protectedShortQueue = new Queue<short>();
		protectedShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		protectedShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		protectedShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));

		protectedShortDict = new Dictionary<string, short>()
		{
			{ "key1", (short)rnd.Next(short.MinValue, short.MaxValue + 1) },
			{ "key2", (short)rnd.Next(short.MinValue, short.MaxValue + 1) },
			{ "key3", (short)rnd.Next(short.MinValue, short.MaxValue + 1) }
		};

		protectedShortListArray = new List<short>[3];
		for (int i = 0; i < 3; i++)
			protectedShortListArray[i] = new List<short>() { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		internalShortArray = new short[3] { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		internalShortMatrix = new short[1, 3] { { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) } };

		internalShortList = new List<short>() { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };

		internalShortStack = new Stack<short>();
		internalShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		internalShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		internalShortStack.Push((short)rnd.Next(short.MinValue, short.MaxValue + 1));

		internalShortQueue = new Queue<short>();
		internalShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		internalShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));
		internalShortQueue.Enqueue((short)rnd.Next(short.MinValue, short.MaxValue + 1));

		internalShortDict = new Dictionary<string, short>()
		{
			{ "key1", (short)rnd.Next(short.MinValue, short.MaxValue + 1) },
			{ "key2", (short)rnd.Next(short.MinValue, short.MaxValue + 1) },
			{ "key3", (short)rnd.Next(short.MinValue, short.MaxValue + 1) }
		};

		internalShortListArray = new List<short>[3];
		for (int i = 0; i < 3; i++)
			internalShortListArray[i] = new List<short>() { (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1), (short)rnd.Next(short.MinValue, short.MaxValue + 1) };
		#endregion

		#region UShortCollections
		publicUShortArray = new ushort[3] { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		publicUShortMatrix = new ushort[1, 3] { { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) } };

		publicUShortList = new List<ushort>() { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		publicUShortStack = new Stack<ushort>();
		publicUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		publicUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		publicUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));

		publicUShortQueue = new Queue<ushort>();
		publicUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		publicUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		publicUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));

		publicUShortDict = new Dictionary<string, ushort>()
		{
			{ "key1", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) },
			{ "key2", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) },
			{ "key3", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) }
		};

		publicUShortListArray = new List<ushort>[3];
		for (int i = 0; i < 3; i++)
			publicUShortListArray[i] = new List<ushort>() { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		privateUShortArray = new ushort[3] { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		privateUShortMatrix = new ushort[1, 3] { { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) } };

		privateUShortList = new List<ushort>() { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		privateUShortStack = new Stack<ushort>();
		privateUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		privateUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		privateUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));

		privateUShortQueue = new Queue<ushort>();
		privateUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		privateUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		privateUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));

		privateUShortDict = new Dictionary<string, ushort>()
		{
			{ "key1", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) },
			{ "key2", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) },
			{ "key3", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) }
		};

		privateUShortListArray = new List<ushort>[3];
		for (int i = 0; i < 3; i++)
			privateUShortListArray[i] = new List<ushort>() { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		protectedUShortArray = new ushort[3] { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		protectedUShortMatrix = new ushort[1, 3] { { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) } };

		protectedUShortList = new List<ushort>() { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		protectedUShortStack = new Stack<ushort>();
		protectedUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		protectedUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		protectedUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));

		protectedUShortQueue = new Queue<ushort>();
		protectedUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		protectedUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		protectedUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));

		protectedUShortDict = new Dictionary<string, ushort>()
		{
			{ "key1", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) },
			{ "key2", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) },
			{ "key3", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) }
		};

		protectedUShortListArray = new List<ushort>[3];
		for (int i = 0; i < 3; i++)
			protectedUShortListArray[i] = new List<ushort>() { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		internalUShortArray = new ushort[3] { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		internalUShortMatrix = new ushort[1, 3] { { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) } };

		internalUShortList = new List<ushort>() { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
			(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };

		internalUShortStack = new Stack<ushort>();
		internalUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		internalUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		internalUShortStack.Push((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));

		internalUShortQueue = new Queue<ushort>();
		internalUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		internalUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));
		internalUShortQueue.Enqueue((ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1));

		internalUShortDict = new Dictionary<string, ushort>()
		{
			{ "key1", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) },
			{ "key2", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) },
			{ "key3", (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) }
		};

		internalUShortListArray = new List<ushort>[3];
		for (int i = 0; i < 3; i++)
			internalUShortListArray[i] = new List<ushort>() { (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1),
				(ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1) };
		#endregion

		#region IntCollections
		publicIntArray = new int[3] { rnd.Next(),
			rnd.Next(),
			rnd.Next() };

		publicIntMatrix = new int[1, 3] { { rnd.Next(),
				rnd.Next(),
				rnd.Next() } };

		publicIntList = new List<int>() { rnd.Next(),
			rnd.Next(),
			rnd.Next() };

		publicIntStack = new Stack<int>();
		publicIntStack.Push(rnd.Next());
		publicIntStack.Push(rnd.Next());
		publicIntStack.Push(rnd.Next());

		publicIntQueue = new Queue<int>();
		publicIntQueue.Enqueue(rnd.Next());
		publicIntQueue.Enqueue(rnd.Next());
		publicIntQueue.Enqueue(rnd.Next());

		publicIntDict = new Dictionary<string, int>()
		{
			{ "key1", rnd.Next() },
			{ "key2", rnd.Next() },
			{ "key3", rnd.Next() }
		};

		publicIntListArray = new List<int>[3];
		for (int i = 0; i < 3; i++)
			publicIntListArray[i] = new List<int>() { rnd.Next(),
				rnd.Next(),
				rnd.Next() };

		privateIntArray = new int[3] { rnd.Next(),
			rnd.Next(),
			rnd.Next() };

		privateIntMatrix = new int[1, 3] { { rnd.Next(),
				rnd.Next(),
				rnd.Next() } };

		privateIntList = new List<int>() { rnd.Next(),
			rnd.Next(),
			rnd.Next() };

		privateIntStack = new Stack<int>();
		privateIntStack.Push(rnd.Next());
		privateIntStack.Push(rnd.Next());
		privateIntStack.Push(rnd.Next());

		privateIntQueue = new Queue<int>();
		privateIntQueue.Enqueue(rnd.Next());
		privateIntQueue.Enqueue(rnd.Next());
		privateIntQueue.Enqueue(rnd.Next());

		privateIntDict = new Dictionary<string, int>()
		{
			{ "key1", rnd.Next() },
			{ "key2", rnd.Next() },
			{ "key3", rnd.Next() }
		};

		privateIntListArray = new List<int>[3];
		for (int i = 0; i < 3; i++)
			privateIntListArray[i] = new List<int>() { rnd.Next(),
				rnd.Next(),
				rnd.Next() };

		protectedIntArray = new int[3] { rnd.Next(),
			rnd.Next(),
			rnd.Next() };

		protectedIntMatrix = new int[1, 3] { { rnd.Next(),
				rnd.Next(),
				rnd.Next() } };

		protectedIntList = new List<int>() { rnd.Next(),
			rnd.Next(),
			rnd.Next() };

		protectedIntStack = new Stack<int>();
		protectedIntStack.Push(rnd.Next());
		protectedIntStack.Push(rnd.Next());
		protectedIntStack.Push(rnd.Next());

		protectedIntQueue = new Queue<int>();
		protectedIntQueue.Enqueue(rnd.Next());
		protectedIntQueue.Enqueue(rnd.Next());
		protectedIntQueue.Enqueue(rnd.Next());

		protectedIntDict = new Dictionary<string, int>()
		{
			{ "key1", rnd.Next() },
			{ "key2", rnd.Next() },
			{ "key3", rnd.Next() }
		};

		protectedIntListArray = new List<int>[3];
		for (int i = 0; i < 3; i++)
			protectedIntListArray[i] = new List<int>() { rnd.Next(),
				rnd.Next(),
				rnd.Next() };

		internalIntArray = new int[3] { rnd.Next(),
			rnd.Next(),
			rnd.Next() };

		internalIntMatrix = new int[1, 3] { { rnd.Next(),
				rnd.Next(),
				rnd.Next() } };

		internalIntList = new List<int>() { rnd.Next(),
			rnd.Next(),
			rnd.Next() };

		internalIntStack = new Stack<int>();
		internalIntStack.Push(rnd.Next());
		internalIntStack.Push(rnd.Next());
		internalIntStack.Push(rnd.Next());

		internalIntQueue = new Queue<int>();
		internalIntQueue.Enqueue(rnd.Next());
		internalIntQueue.Enqueue(rnd.Next());
		internalIntQueue.Enqueue(rnd.Next());

		internalIntDict = new Dictionary<string, int>()
		{
			{ "key1", rnd.Next() },
			{ "key2", rnd.Next() },
			{ "key3", rnd.Next() }
		};

		internalIntListArray = new List<int>[3];
		for (int i = 0; i < 3; i++)
			internalIntListArray[i] = new List<int>() { rnd.Next(),
				rnd.Next(),
				rnd.Next() };
		#endregion

		#region UIntCollections
		publicUIntArray = new uint[3] { (uint)rnd.Next(),
			(uint)rnd.Next(),
			(uint)rnd.Next() };

		publicUIntMatrix = new uint[1, 3] { { (uint)rnd.Next(),
				(uint)rnd.Next(),
				(uint)rnd.Next() } };

		publicUIntList = new List<uint>() { (uint)rnd.Next(),
			(uint)rnd.Next(),
			(uint)rnd.Next() };

		publicUIntStack = new Stack<uint>();
		publicUIntStack.Push((uint)rnd.Next());
		publicUIntStack.Push((uint)rnd.Next());
		publicUIntStack.Push((uint)rnd.Next());

		publicUIntQueue = new Queue<uint>();
		publicUIntQueue.Enqueue((uint)rnd.Next());
		publicUIntQueue.Enqueue((uint)rnd.Next());
		publicUIntQueue.Enqueue((uint)rnd.Next());

		publicUIntDict = new Dictionary<string, uint>()
		{
			{ "key1", (uint)rnd.Next() },
			{ "key2", (uint)rnd.Next() },
			{ "key3", (uint)rnd.Next() }
		};

		publicUIntListArray = new List<uint>[3];
		for (int i = 0; i < 3; i++)
			publicUIntListArray[i] = new List<uint>() { (uint)rnd.Next(),
				(uint)rnd.Next(),
				(uint)rnd.Next() };

		privateUIntArray = new uint[3] { (uint)rnd.Next(),
			(uint)rnd.Next(),
			(uint)rnd.Next() };

		privateUIntMatrix = new uint[1, 3] { { (uint)rnd.Next(),
				(uint)rnd.Next(),
				(uint)rnd.Next() } };

		privateUIntList = new List<uint>() { (uint)rnd.Next(),
			(uint)rnd.Next(),
			(uint)rnd.Next() };

		privateUIntStack = new Stack<uint>();
		privateUIntStack.Push((uint)rnd.Next());
		privateUIntStack.Push((uint)rnd.Next());
		privateUIntStack.Push((uint)rnd.Next());

		privateUIntQueue = new Queue<uint>();
		privateUIntQueue.Enqueue((uint)rnd.Next());
		privateUIntQueue.Enqueue((uint)rnd.Next());
		privateUIntQueue.Enqueue((uint)rnd.Next());

		privateUIntDict = new Dictionary<string, uint>()
		{
			{ "key1", (uint)rnd.Next() },
			{ "key2", (uint)rnd.Next() },
			{ "key3", (uint)rnd.Next() }
		};

		privateUIntListArray = new List<uint>[3];
		for (int i = 0; i < 3; i++)
			privateUIntListArray[i] = new List<uint>() { (uint)rnd.Next(),
				(uint)rnd.Next(),
				(uint)rnd.Next() };

		protectedUIntArray = new uint[3] { (uint)rnd.Next(),
			(uint)rnd.Next(),
			(uint)rnd.Next() };

		protectedUIntMatrix = new uint[1, 3] { { (uint)rnd.Next(),
				(uint)rnd.Next(),
				(uint)rnd.Next() } };

		protectedUIntList = new List<uint>() { (uint)rnd.Next(),
			(uint)rnd.Next(),
			(uint)rnd.Next() };

		protectedUIntStack = new Stack<uint>();
		protectedUIntStack.Push((uint)rnd.Next());
		protectedUIntStack.Push((uint)rnd.Next());
		protectedUIntStack.Push((uint)rnd.Next());

		protectedUIntQueue = new Queue<uint>();
		protectedUIntQueue.Enqueue((uint)rnd.Next());
		protectedUIntQueue.Enqueue((uint)rnd.Next());
		protectedUIntQueue.Enqueue((uint)rnd.Next());

		protectedUIntDict = new Dictionary<string, uint>()
		{
			{ "key1", (uint)rnd.Next() },
			{ "key2", (uint)rnd.Next() },
			{ "key3", (uint)rnd.Next() }
		};

		protectedUIntListArray = new List<uint>[3];
		for (int i = 0; i < 3; i++)
			protectedUIntListArray[i] = new List<uint>() { (uint)rnd.Next(),
				(uint)rnd.Next(),
				(uint)rnd.Next() };

		internalUIntArray = new uint[3] { (uint)rnd.Next(),
			(uint)rnd.Next(),
			(uint)rnd.Next() };

		internalUIntMatrix = new uint[1, 3] { { (uint)rnd.Next(),
				(uint)rnd.Next(),
				(uint)rnd.Next() } };

		internalUIntList = new List<uint>() { (uint)rnd.Next(),
			(uint)rnd.Next(),
			(uint)rnd.Next() };

		internalUIntStack = new Stack<uint>();
		internalUIntStack.Push((uint)rnd.Next());
		internalUIntStack.Push((uint)rnd.Next());
		internalUIntStack.Push((uint)rnd.Next());

		internalUIntQueue = new Queue<uint>();
		internalUIntQueue.Enqueue((uint)rnd.Next());
		internalUIntQueue.Enqueue((uint)rnd.Next());
		internalUIntQueue.Enqueue((uint)rnd.Next());

		internalUIntDict = new Dictionary<string, uint>()
		{
			{ "key1", (uint)rnd.Next() },
			{ "key2", (uint)rnd.Next() },
			{ "key3", (uint)rnd.Next() }
		};

		internalUIntListArray = new List<uint>[3];
		for (int i = 0; i < 3; i++)
			internalUIntListArray[i] = new List<uint>() { (uint)rnd.Next(),
				(uint)rnd.Next(),
				(uint)rnd.Next() };
		#endregion

		#region LongCollections
		publicLongArray = new long[3] { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next() };

		publicLongMatrix = new long[1, 3] { { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next() } };

		publicLongList = new List<long>() { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next() };

		publicLongStack = new Stack<long>();
		publicLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());
		publicLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());
		publicLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());

		publicLongQueue = new Queue<long>();
		publicLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());
		publicLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());
		publicLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());

		publicLongDict = new Dictionary<string, long>()
		{
			{ "key1", ((long)rnd.Next() << 32) | (uint)rnd.Next() },
			{ "key2", ((long)rnd.Next() << 32) | (uint)rnd.Next() },
			{ "key3", ((long)rnd.Next() << 32) | (uint)rnd.Next() }
		};

		publicLongListArray = new List<long>[3];
		for (int i = 0; i < 3; i++)
			publicLongListArray[i] = new List<long>() { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next() };

		privateLongArray = new long[3] { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next() };

		privateLongMatrix = new long[1, 3] { { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next() } };

		privateLongList = new List<long>() { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next() };

		privateLongStack = new Stack<long>();
		privateLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());
		privateLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());
		privateLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());

		privateLongQueue = new Queue<long>();
		privateLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());
		privateLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());
		privateLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());

		privateLongDict = new Dictionary<string, long>()
		{
			{ "key1", ((long)rnd.Next() << 32) | (uint)rnd.Next() },
			{ "key2", ((long)rnd.Next() << 32) | (uint)rnd.Next() },
			{ "key3", ((long)rnd.Next() << 32) | (uint)rnd.Next() }
		};

		privateLongListArray = new List<long>[3];
		for (int i = 0; i < 3; i++)
			privateLongListArray[i] = new List<long>() { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next() };

		protectedLongArray = new long[3] { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next() };

		protectedLongMatrix = new long[1, 3] { { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next() } };

		protectedLongList = new List<long>() { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next() };

		protectedLongStack = new Stack<long>();
		protectedLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());
		protectedLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());
		protectedLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());

		protectedLongQueue = new Queue<long>();
		protectedLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());
		protectedLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());
		protectedLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());

		protectedLongDict = new Dictionary<string, long>()
		{
			{ "key1", ((long)rnd.Next() << 32) | (uint)rnd.Next() },
			{ "key2", ((long)rnd.Next() << 32) | (uint)rnd.Next() },
			{ "key3", ((long)rnd.Next() << 32) | (uint)rnd.Next() }
		};

		protectedLongListArray = new List<long>[3];
		for (int i = 0; i < 3; i++)
			protectedLongListArray[i] = new List<long>() { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next() };

		internalLongArray = new long[3] { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next() };

		internalLongMatrix = new long[1, 3] { { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next() } };

		internalLongList = new List<long>() { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next(),
			((long)rnd.Next() << 32) | (uint)rnd.Next() };

		internalLongStack = new Stack<long>();
		internalLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());
		internalLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());
		internalLongStack.Push(((long)rnd.Next() << 32) | (uint)rnd.Next());

		internalLongQueue = new Queue<long>();
		internalLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());
		internalLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());
		internalLongQueue.Enqueue(((long)rnd.Next() << 32) | (uint)rnd.Next());

		internalLongDict = new Dictionary<string, long>()
		{
			{ "key1", ((long)rnd.Next() << 32) | (uint)rnd.Next() },
			{ "key2", ((long)rnd.Next() << 32) | (uint)rnd.Next() },
			{ "key3", ((long)rnd.Next() << 32) | (uint)rnd.Next() }
		};

		internalLongListArray = new List<long>[3];
		for (int i = 0; i < 3; i++)
			internalLongListArray[i] = new List<long>() { ((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next(),
				((long)rnd.Next() << 32) | (uint)rnd.Next() };
		#endregion

		#region ULongCollections
		publicULongArray = new ulong[3] { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		publicULongMatrix = new ulong[1, 3] { { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) } };

		publicULongList = new List<ulong>() { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		publicULongStack = new Stack<ulong>();
		publicULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		publicULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		publicULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));

		publicULongQueue = new Queue<ulong>();
		publicULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		publicULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		publicULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));

		publicULongDict = new Dictionary<string, ulong>()
		{
			{ "key1", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) },
			{ "key2", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) },
			{ "key3", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) }
		};

		publicULongListArray = new List<ulong>[3];
		for (int i = 0; i < 3; i++)
			publicULongListArray[i] = new List<ulong>() { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		privateULongArray = new ulong[3] { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		privateULongMatrix = new ulong[1, 3] { { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) } };

		privateULongList = new List<ulong>() { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		privateULongStack = new Stack<ulong>();
		privateULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		privateULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		privateULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));

		privateULongQueue = new Queue<ulong>();
		privateULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		privateULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		privateULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));

		privateULongDict = new Dictionary<string, ulong>()
		{
			{ "key1", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) },
			{ "key2", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) },
			{ "key3", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) }
		};

		privateULongListArray = new List<ulong>[3];
		for (int i = 0; i < 3; i++)
			privateULongListArray[i] = new List<ulong>() { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		protectedULongArray = new ulong[3] { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		protectedULongMatrix = new ulong[1, 3] { { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) } };

		protectedULongList = new List<ulong>() { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		protectedULongStack = new Stack<ulong>();
		protectedULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		protectedULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		protectedULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));

		protectedULongQueue = new Queue<ulong>();
		protectedULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		protectedULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		protectedULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));

		protectedULongDict = new Dictionary<string, ulong>()
		{
			{ "key1", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) },
			{ "key2", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) },
			{ "key3", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) }
		};

		protectedULongListArray = new List<ulong>[3];
		for (int i = 0; i < 3; i++)
			protectedULongListArray[i] = new List<ulong>() { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		internalULongArray = new ulong[3] { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		internalULongMatrix = new ulong[1, 3] { { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) } };

		internalULongList = new List<ulong>() { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
			(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };

		internalULongStack = new Stack<ulong>();
		internalULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		internalULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		internalULongStack.Push((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));

		internalULongQueue = new Queue<ulong>();
		internalULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		internalULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));
		internalULongQueue.Enqueue((ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()));

		internalULongDict = new Dictionary<string, ulong>()
		{
			{ "key1", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) },
			{ "key2", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) },
			{ "key3", (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) }
		};

		internalULongListArray = new List<ulong>[3];
		for (int i = 0; i < 3; i++)
			internalULongListArray[i] = new List<ulong>() { (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()),
				(ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next()) };
		#endregion

		#region FloatCollections
		publicFloatArray = new float[3] { (float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000) };

		publicFloatMatrix = new float[1, 3] { { (float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000) } };

		publicFloatList = new List<float>() { (float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000) };

		publicFloatStack = new Stack<float>();
		publicFloatStack.Push((float)(rnd.NextDouble() * 1000));
		publicFloatStack.Push((float)(rnd.NextDouble() * 1000));
		publicFloatStack.Push((float)(rnd.NextDouble() * 1000));

		publicFloatQueue = new Queue<float>();
		publicFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));
		publicFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));
		publicFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));

		publicFloatDict = new Dictionary<string, float>()
		{
			{ "key1", (float)(rnd.NextDouble() * 1000) },
			{ "key2", (float)(rnd.NextDouble() * 1000) },
			{ "key3", (float)(rnd.NextDouble() * 1000) }
		};

		publicFloatListArray = new List<float>[3];
		for (int i = 0; i < 3; i++)
			publicFloatListArray[i] = new List<float>() { (float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000) };

		privateFloatArray = new float[3] { (float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000) };

		privateFloatMatrix = new float[1, 3] { { (float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000) } };

		privateFloatList = new List<float>() { (float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000) };

		privateFloatStack = new Stack<float>();
		privateFloatStack.Push((float)(rnd.NextDouble() * 1000));
		privateFloatStack.Push((float)(rnd.NextDouble() * 1000));
		privateFloatStack.Push((float)(rnd.NextDouble() * 1000));

		privateFloatQueue = new Queue<float>();
		privateFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));
		privateFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));
		privateFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));

		privateFloatDict = new Dictionary<string, float>()
		{
			{ "key1", (float)(rnd.NextDouble() * 1000) },
			{ "key2", (float)(rnd.NextDouble() * 1000) },
			{ "key3", (float)(rnd.NextDouble() * 1000) }
		};

		privateFloatListArray = new List<float>[3];
		for (int i = 0; i < 3; i++)
			privateFloatListArray[i] = new List<float>() { (float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000) };

		protectedFloatArray = new float[3] { (float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000) };

		protectedFloatMatrix = new float[1, 3] { { (float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000) } };

		protectedFloatList = new List<float>() { (float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000) };

		protectedFloatStack = new Stack<float>();
		protectedFloatStack.Push((float)(rnd.NextDouble() * 1000));
		protectedFloatStack.Push((float)(rnd.NextDouble() * 1000));
		protectedFloatStack.Push((float)(rnd.NextDouble() * 1000));

		protectedFloatQueue = new Queue<float>();
		protectedFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));
		protectedFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));
		protectedFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));

		protectedFloatDict = new Dictionary<string, float>()
		{
			{ "key1", (float)(rnd.NextDouble() * 1000) },
			{ "key2", (float)(rnd.NextDouble() * 1000) },
			{ "key3", (float)(rnd.NextDouble() * 1000) }
		};

		protectedFloatListArray = new List<float>[3];
		for (int i = 0; i < 3; i++)
			protectedFloatListArray[i] = new List<float>() { (float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000) };

		internalFloatArray = new float[3] { (float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000) };

		internalFloatMatrix = new float[1, 3] { { (float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000) } };

		internalFloatList = new List<float>() { (float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000) };

		internalFloatStack = new Stack<float>();
		internalFloatStack.Push((float)(rnd.NextDouble() * 1000));
		internalFloatStack.Push((float)(rnd.NextDouble() * 1000));
		internalFloatStack.Push((float)(rnd.NextDouble() * 1000));

		internalFloatQueue = new Queue<float>();
		internalFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));
		internalFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));
		internalFloatQueue.Enqueue((float)(rnd.NextDouble() * 1000));

		internalFloatDict = new Dictionary<string, float>()
		{
			{ "key1", (float)(rnd.NextDouble() * 1000) },
			{ "key2", (float)(rnd.NextDouble() * 1000) },
			{ "key3", (float)(rnd.NextDouble() * 1000) }
		};

		internalFloatListArray = new List<float>[3];
		for (int i = 0; i < 3; i++)
			internalFloatListArray[i] = new List<float>() { (float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000),
				(float)(rnd.NextDouble() * 1000) };
		#endregion

		#region DoubleCollections
		publicDoubleArray = new double[3] { rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000 };

		publicDoubleMatrix = new double[1, 3] { { rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000 } };

		publicDoubleList = new List<double>() { rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000 };

		publicDoubleStack = new Stack<double>();
		publicDoubleStack.Push(rnd.NextDouble() * 1000);
		publicDoubleStack.Push(rnd.NextDouble() * 1000);
		publicDoubleStack.Push(rnd.NextDouble() * 1000);

		publicDoubleQueue = new Queue<double>();
		publicDoubleQueue.Enqueue(rnd.NextDouble() * 1000);
		publicDoubleQueue.Enqueue(rnd.NextDouble() * 1000);
		publicDoubleQueue.Enqueue(rnd.NextDouble() * 1000);

		publicDoubleDict = new Dictionary<string, double>()
		{
			{ "key1", rnd.NextDouble() * 1000 },
			{ "key2", rnd.NextDouble() * 1000 },
			{ "key3", rnd.NextDouble() * 1000 }
		};

		publicDoubleListArray = new List<double>[3];
		for (int i = 0; i < 3; i++)
			publicDoubleListArray[i] = new List<double>() { rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000 };

		privateDoubleArray = new double[3] { rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000 };

		privateDoubleMatrix = new double[1, 3] { { rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000 } };

		privateDoubleList = new List<double>() { rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000 };

		privateDoubleStack = new Stack<double>();
		privateDoubleStack.Push(rnd.NextDouble() * 1000);
		privateDoubleStack.Push(rnd.NextDouble() * 1000);
		privateDoubleStack.Push(rnd.NextDouble() * 1000);

		privateDoubleQueue = new Queue<double>();
		privateDoubleQueue.Enqueue(rnd.NextDouble() * 1000);
		privateDoubleQueue.Enqueue(rnd.NextDouble() * 1000);
		privateDoubleQueue.Enqueue(rnd.NextDouble() * 1000);

		privateDoubleDict = new Dictionary<string, double>()
		{
			{ "key1", rnd.NextDouble() * 1000 },
			{ "key2", rnd.NextDouble() * 1000 },
			{ "key3", rnd.NextDouble() * 1000 }
		};

		privateDoubleListArray = new List<double>[3];
		for (int i = 0; i < 3; i++)
			privateDoubleListArray[i] = new List<double>() { rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000 };

		protectedDoubleArray = new double[3] { rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000 };

		protectedDoubleMatrix = new double[1, 3] { { rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000 } };

		protectedDoubleList = new List<double>() { rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000 };

		protectedDoubleStack = new Stack<double>();
		protectedDoubleStack.Push(rnd.NextDouble() * 1000);
		protectedDoubleStack.Push(rnd.NextDouble() * 1000);
		protectedDoubleStack.Push(rnd.NextDouble() * 1000);

		protectedDoubleQueue = new Queue<double>();
		protectedDoubleQueue.Enqueue(rnd.NextDouble() * 1000);
		protectedDoubleQueue.Enqueue(rnd.NextDouble() * 1000);
		protectedDoubleQueue.Enqueue(rnd.NextDouble() * 1000);

		protectedDoubleDict = new Dictionary<string, double>()
		{
			{ "key1", rnd.NextDouble() * 1000 },
			{ "key2", rnd.NextDouble() * 1000 },
			{ "key3", rnd.NextDouble() * 1000 }
		};

		protectedDoubleListArray = new List<double>[3];
		for (int i = 0; i < 3; i++)
			protectedDoubleListArray[i] = new List<double>() { rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000 };

		internalDoubleArray = new double[3] { rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000 };

		internalDoubleMatrix = new double[1, 3] { { rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000 } };

		internalDoubleList = new List<double>() { rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000,
			rnd.NextDouble() * 1000 };

		internalDoubleStack = new Stack<double>();
		internalDoubleStack.Push(rnd.NextDouble() * 1000);
		internalDoubleStack.Push(rnd.NextDouble() * 1000);
		internalDoubleStack.Push(rnd.NextDouble() * 1000);

		internalDoubleQueue = new Queue<double>();
		internalDoubleQueue.Enqueue(rnd.NextDouble() * 1000);
		internalDoubleQueue.Enqueue(rnd.NextDouble() * 1000);
		internalDoubleQueue.Enqueue(rnd.NextDouble() * 1000);

		internalDoubleDict = new Dictionary<string, double>()
		{
			{ "key1", rnd.NextDouble() * 1000 },
			{ "key2", rnd.NextDouble() * 1000 },
			{ "key3", rnd.NextDouble() * 1000 }
		};

		internalDoubleListArray = new List<double>[3];
		for (int i = 0; i < 3; i++)
			internalDoubleListArray[i] = new List<double>() { rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000,
				rnd.NextDouble() * 1000 };
		#endregion

		#region DecimalCollections
		publicDecimalArray = new decimal[3] { (decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000) };

		publicDecimalMatrix = new decimal[1, 3] { { (decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000) } };

		publicDecimalList = new List<decimal>() { (decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000) };

		publicDecimalStack = new Stack<decimal>();
		publicDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));
		publicDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));
		publicDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));

		publicDecimalQueue = new Queue<decimal>();
		publicDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));
		publicDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));
		publicDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));

		publicDecimalDict = new Dictionary<string, decimal>()
		{
			{ "key1", (decimal)(rnd.NextDouble() * 1000) },
			{ "key2", (decimal)(rnd.NextDouble() * 1000) },
			{ "key3", (decimal)(rnd.NextDouble() * 1000) }
		};

		publicDecimalListArray = new List<decimal>[3];
		for (int i = 0; i < 3; i++)
			publicDecimalListArray[i] = new List<decimal>() { (decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000) };

		privateDecimalArray = new decimal[3] { (decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000) };

		privateDecimalMatrix = new decimal[1, 3] { { (decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000) } };

		privateDecimalList = new List<decimal>() { (decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000) };

		privateDecimalStack = new Stack<decimal>();
		privateDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));
		privateDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));
		privateDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));

		privateDecimalQueue = new Queue<decimal>();
		privateDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));
		privateDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));
		privateDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));

		privateDecimalDict = new Dictionary<string, decimal>()
		{
			{ "key1", (decimal)(rnd.NextDouble() * 1000) },
			{ "key2", (decimal)(rnd.NextDouble() * 1000) },
			{ "key3", (decimal)(rnd.NextDouble() * 1000) }
		};

		privateDecimalListArray = new List<decimal>[3];
		for (int i = 0; i < 3; i++)
			privateDecimalListArray[i] = new List<decimal>() { (decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000) };

		protectedDecimalArray = new decimal[3] { (decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000) };

		protectedDecimalMatrix = new decimal[1, 3] { { (decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000) } };

		protectedDecimalList = new List<decimal>() { (decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000) };

		protectedDecimalStack = new Stack<decimal>();
		protectedDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));
		protectedDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));
		protectedDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));

		protectedDecimalQueue = new Queue<decimal>();
		protectedDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));
		protectedDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));
		protectedDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));

		protectedDecimalDict = new Dictionary<string, decimal>()
		{
			{ "key1", (decimal)(rnd.NextDouble() * 1000) },
			{ "key2", (decimal)(rnd.NextDouble() * 1000) },
			{ "key3", (decimal)(rnd.NextDouble() * 1000) }
		};

		protectedDecimalListArray = new List<decimal>[3];
		for (int i = 0; i < 3; i++)
			protectedDecimalListArray[i] = new List<decimal>() { (decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000) };

		internalDecimalArray = new decimal[3] { (decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000) };

		internalDecimalMatrix = new decimal[1, 3] { { (decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000) } };

		internalDecimalList = new List<decimal>() { (decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000),
			(decimal)(rnd.NextDouble() * 1000) };

		internalDecimalStack = new Stack<decimal>();
		internalDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));
		internalDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));
		internalDecimalStack.Push((decimal)(rnd.NextDouble() * 1000));

		internalDecimalQueue = new Queue<decimal>();
		internalDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));
		internalDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));
		internalDecimalQueue.Enqueue((decimal)(rnd.NextDouble() * 1000));

		internalDecimalDict = new Dictionary<string, decimal>()
		{
			{ "key1", (decimal)(rnd.NextDouble() * 1000) },
			{ "key2", (decimal)(rnd.NextDouble() * 1000) },
			{ "key3", (decimal)(rnd.NextDouble() * 1000) }
		};

		internalDecimalListArray = new List<decimal>[3];
		for (int i = 0; i < 3; i++)
			internalDecimalListArray[i] = new List<decimal>() { (decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000),
				(decimal)(rnd.NextDouble() * 1000) };
		#endregion

		#region CharCollections
		publicCharArray = new char[3] { (char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91) };

		publicCharMatrix = new char[1, 3] { { (char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91) } };

		publicCharList = new List<char>() { (char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91) };

		publicCharStack = new Stack<char>();
		publicCharStack.Push((char)rnd.Next(65, 91));
		publicCharStack.Push((char)rnd.Next(65, 91));
		publicCharStack.Push((char)rnd.Next(65, 91));

		publicCharQueue = new Queue<char>();
		publicCharQueue.Enqueue((char)rnd.Next(65, 91));
		publicCharQueue.Enqueue((char)rnd.Next(65, 91));
		publicCharQueue.Enqueue((char)rnd.Next(65, 91));

		publicCharDict = new Dictionary<string, char>()
		{
			{ "key1", (char)rnd.Next(65, 91) },
			{ "key2", (char)rnd.Next(65, 91) },
			{ "key3", (char)rnd.Next(65, 91) }
		};

		publicCharListArray = new List<char>[3];
		for (int i = 0; i < 3; i++)
			publicCharListArray[i] = new List<char>() { (char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91) };

		privateCharArray = new char[3] { (char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91) };

		privateCharMatrix = new char[1, 3] { { (char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91) } };

		privateCharList = new List<char>() { (char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91) };

		privateCharStack = new Stack<char>();
		privateCharStack.Push((char)rnd.Next(65, 91));
		privateCharStack.Push((char)rnd.Next(65, 91));
		privateCharStack.Push((char)rnd.Next(65, 91));

		privateCharQueue = new Queue<char>();
		privateCharQueue.Enqueue((char)rnd.Next(65, 91));
		privateCharQueue.Enqueue((char)rnd.Next(65, 91));
		privateCharQueue.Enqueue((char)rnd.Next(65, 91));

		privateCharDict = new Dictionary<string, char>()
		{
			{ "key1", (char)rnd.Next(65, 91) },
			{ "key2", (char)rnd.Next(65, 91) },
			{ "key3", (char)rnd.Next(65, 91) }
		};

		privateCharListArray = new List<char>[3];
		for (int i = 0; i < 3; i++)
			privateCharListArray[i] = new List<char>() { (char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91) };

		protectedCharArray = new char[3] { (char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91) };

		protectedCharMatrix = new char[1, 3] { { (char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91) } };

		protectedCharList = new List<char>() { (char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91) };

		protectedCharStack = new Stack<char>();
		protectedCharStack.Push((char)rnd.Next(65, 91));
		protectedCharStack.Push((char)rnd.Next(65, 91));
		protectedCharStack.Push((char)rnd.Next(65, 91));

		protectedCharQueue = new Queue<char>();
		protectedCharQueue.Enqueue((char)rnd.Next(65, 91));
		protectedCharQueue.Enqueue((char)rnd.Next(65, 91));
		protectedCharQueue.Enqueue((char)rnd.Next(65, 91));

		protectedCharDict = new Dictionary<string, char>()
		{
			{ "key1", (char)rnd.Next(65, 91) },
			{ "key2", (char)rnd.Next(65, 91) },
			{ "key3", (char)rnd.Next(65, 91) }
		};

		protectedCharListArray = new List<char>[3];
		for (int i = 0; i < 3; i++)
			protectedCharListArray[i] = new List<char>() { (char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91) };

		internalCharArray = new char[3] { (char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91) };

		internalCharMatrix = new char[1, 3] { { (char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91) } };

		internalCharList = new List<char>() { (char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91),
			(char)rnd.Next(65, 91) };

		internalCharStack = new Stack<char>();
		internalCharStack.Push((char)rnd.Next(65, 91));
		internalCharStack.Push((char)rnd.Next(65, 91));
		internalCharStack.Push((char)rnd.Next(65, 91));

		internalCharQueue = new Queue<char>();
		internalCharQueue.Enqueue((char)rnd.Next(65, 91));
		internalCharQueue.Enqueue((char)rnd.Next(65, 91));
		internalCharQueue.Enqueue((char)rnd.Next(65, 91));

		internalCharDict = new Dictionary<string, char>()
		{
			{ "key1", (char)rnd.Next(65, 91) },
			{ "key2", (char)rnd.Next(65, 91) },
			{ "key3", (char)rnd.Next(65, 91) }
		};

		internalCharListArray = new List<char>[3];
		for (int i = 0; i < 3; i++)
			internalCharListArray[i] = new List<char>() { (char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91),
				(char)rnd.Next(65, 91) };
		#endregion

		#region StringCollections
		publicStringArray = new string[3] { RandomString(rnd, 8),
			RandomString(rnd, 8),
			RandomString(rnd, 8) };

		publicStringMatrix = new string[1, 3] { { RandomString(rnd, 8),
				RandomString(rnd, 8),
				RandomString(rnd, 8) } };

		publicStringList = new List<string>() { RandomString(rnd, 8),
			RandomString(rnd, 8),
			RandomString(rnd, 8) };

		publicStringStack = new Stack<string>();
		publicStringStack.Push(RandomString(rnd, 8));
		publicStringStack.Push(RandomString(rnd, 8));
		publicStringStack.Push(RandomString(rnd, 8));

		publicStringQueue = new Queue<string>();
		publicStringQueue.Enqueue(RandomString(rnd, 8));
		publicStringQueue.Enqueue(RandomString(rnd, 8));
		publicStringQueue.Enqueue(RandomString(rnd, 8));

		publicStringDict = new Dictionary<string, string>()
		{
			{ "key1", RandomString(rnd, 8) },
			{ "key2", RandomString(rnd, 8) },
			{ "key3", RandomString(rnd, 8) }
		};

		publicStringListArray = new List<string>[3];
		for (int i = 0; i < 3; i++)
			publicStringListArray[i] = new List<string>() { RandomString(rnd, 8),
				RandomString(rnd, 8),
				RandomString(rnd, 8) };

		privateStringArray = new string[3] { RandomString(rnd, 8),
			RandomString(rnd, 8),
			RandomString(rnd, 8) };

		privateStringMatrix = new string[1, 3] { { RandomString(rnd, 8),
				RandomString(rnd, 8),
				RandomString(rnd, 8) } };

		privateStringList = new List<string>() { RandomString(rnd, 8),
			RandomString(rnd, 8),
			RandomString(rnd, 8) };

		privateStringStack = new Stack<string>();
		privateStringStack.Push(RandomString(rnd, 8));
		privateStringStack.Push(RandomString(rnd, 8));
		privateStringStack.Push(RandomString(rnd, 8));

		privateStringQueue = new Queue<string>();
		privateStringQueue.Enqueue(RandomString(rnd, 8));
		privateStringQueue.Enqueue(RandomString(rnd, 8));
		privateStringQueue.Enqueue(RandomString(rnd, 8));

		privateStringDict = new Dictionary<string, string>()
		{
			{ "key1", RandomString(rnd, 8) },
			{ "key2", RandomString(rnd, 8) },
			{ "key3", RandomString(rnd, 8) }
		};

		privateStringListArray = new List<string>[3];
		for (int i = 0; i < 3; i++)
			privateStringListArray[i] = new List<string>() { RandomString(rnd, 8),
				RandomString(rnd, 8),
				RandomString(rnd, 8) };

		protectedStringArray = new string[3] { RandomString(rnd, 8),
			RandomString(rnd, 8),
			RandomString(rnd, 8) };

		protectedStringMatrix = new string[1, 3] { { RandomString(rnd, 8),
				RandomString(rnd, 8),
				RandomString(rnd, 8) } };

		protectedStringList = new List<string>() { RandomString(rnd, 8),
			RandomString(rnd, 8),
			RandomString(rnd, 8) };

		protectedStringStack = new Stack<string>();
		protectedStringStack.Push(RandomString(rnd, 8));
		protectedStringStack.Push(RandomString(rnd, 8));
		protectedStringStack.Push(RandomString(rnd, 8));

		protectedStringQueue = new Queue<string>();
		protectedStringQueue.Enqueue(RandomString(rnd, 8));
		protectedStringQueue.Enqueue(RandomString(rnd, 8));
		protectedStringQueue.Enqueue(RandomString(rnd, 8));

		protectedStringDict = new Dictionary<string, string>()
		{
			{ "key1", RandomString(rnd, 8) },
			{ "key2", RandomString(rnd, 8) },
			{ "key3", RandomString(rnd, 8) }
		};

		protectedStringListArray = new List<string>[3];
		for (int i = 0; i < 3; i++)
			protectedStringListArray[i] = new List<string>() { RandomString(rnd, 8),
				RandomString(rnd, 8),
				RandomString(rnd, 8) };

		internalStringArray = new string[3] { RandomString(rnd, 8),
			RandomString(rnd, 8),
			RandomString(rnd, 8) };

		internalStringMatrix = new string[1, 3] { { RandomString(rnd, 8),
				RandomString(rnd, 8),
				RandomString(rnd, 8) } };

		internalStringList = new List<string>() { RandomString(rnd, 8),
			RandomString(rnd, 8),
			RandomString(rnd, 8) };

		internalStringStack = new Stack<string>();
		internalStringStack.Push(RandomString(rnd, 8));
		internalStringStack.Push(RandomString(rnd, 8));
		internalStringStack.Push(RandomString(rnd, 8));

		internalStringQueue = new Queue<string>();
		internalStringQueue.Enqueue(RandomString(rnd, 8));
		internalStringQueue.Enqueue(RandomString(rnd, 8));
		internalStringQueue.Enqueue(RandomString(rnd, 8));

		internalStringDict = new Dictionary<string, string>()
		{
			{ "key1", RandomString(rnd, 8) },
			{ "key2", RandomString(rnd, 8) },
			{ "key3", RandomString(rnd, 8) }
		};

		internalStringListArray = new List<string>[3];
		for (int i = 0; i < 3; i++)
			internalStringListArray[i] = new List<string>() { RandomString(rnd, 8),
				RandomString(rnd, 8),
				RandomString(rnd, 8) };
		#endregion
		#endregion

		#region UnityFields
		publicVector2 = new Vector2((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		privateVector2 = new Vector2((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		protectedVector2 = new Vector2((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		internalVector2 = new Vector2((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));

		publicVector3 = new Vector3((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		privateVector3 = new Vector3((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		protectedVector3 = new Vector3((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		internalVector3 = new Vector3((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));

		publicVector2Int = new Vector2Int(rnd.Next(), rnd.Next());
		privateVector2Int = new Vector2Int(rnd.Next(), rnd.Next());
		protectedVector2Int = new Vector2Int(rnd.Next(), rnd.Next());
		internalVector2Int = new Vector2Int(rnd.Next(), rnd.Next());

		publicVector3Int = new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next());
		privateVector3Int = new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next());
		protectedVector3Int = new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next());
		internalVector3Int = new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next());

		publicColor = new Color((float)rnd.NextDouble(),
								(float)rnd.NextDouble(),
								(float)rnd.NextDouble(),
								(float)rnd.NextDouble());
		privateColor = new Color((float)rnd.NextDouble(),
								 (float)rnd.NextDouble(),
								 (float)rnd.NextDouble(),
								 (float)rnd.NextDouble());
		protectedColor = new Color((float)rnd.NextDouble(),
								   (float)rnd.NextDouble(),
								   (float)rnd.NextDouble(),
								   (float)rnd.NextDouble());
		internalColor = new Color((float)rnd.NextDouble(),
								  (float)rnd.NextDouble(),
								  (float)rnd.NextDouble(),
								  (float)rnd.NextDouble());

		publicBounds = new Bounds(
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)));
		privateBounds = new Bounds(
			new Vector3((float)(rnd.NextDouble() * 1000),
						   (float)(rnd.NextDouble() * 1000),
						   (float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)));
		protectedBounds = new Bounds(
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)));
		internalBounds = new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		publicBoundsInt = new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()));
		privateBoundsInt = new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()));
		protectedBoundsInt = new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()));
		internalBoundsInt = new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()));
		#endregion

		#region UnityCollections
		#region Vector2Collections
		publicVector2Array = new Vector2[3] { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		publicVector2Matrix = new Vector2[1, 3] { { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) } };

		publicVector2List = new List<Vector2>() { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		publicVector2Stack = new Stack<Vector2>();
		publicVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		publicVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		publicVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		publicVector2Queue = new Queue<Vector2>();
		publicVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		publicVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		publicVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		publicVector2Dict = new Dictionary<string, Vector2>()
		{
			{ "key1", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key2", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key3", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) }
		};

		publicVector2ListArray = new List<Vector2>[3];
		for (int i = 0; i < 3; i++)
			publicVector2ListArray[i] = new List<Vector2>() { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		privateVector2Array = new Vector2[3] { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		privateVector2Matrix = new Vector2[1, 3] { { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) } };

		privateVector2List = new List<Vector2>() { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		privateVector2Stack = new Stack<Vector2>();
		privateVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		privateVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		privateVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		privateVector2Queue = new Queue<Vector2>();
		privateVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		privateVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		privateVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		privateVector2Dict = new Dictionary<string, Vector2>()
		{
			{ "key1", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key2", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key3", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) }
		};

		privateVector2ListArray = new List<Vector2>[3];
		for (int i = 0; i < 3; i++)
			privateVector2ListArray[i] = new List<Vector2>() { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		protectedVector2Array = new Vector2[3] { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		protectedVector2Matrix = new Vector2[1, 3] { { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) } };

		protectedVector2List = new List<Vector2>() { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		protectedVector2Stack = new Stack<Vector2>();
		protectedVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		protectedVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		protectedVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		protectedVector2Queue = new Queue<Vector2>();
		protectedVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		protectedVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		protectedVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		protectedVector2Dict = new Dictionary<string, Vector2>()
		{
			{ "key1", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key2", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key3", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) }
		};

		protectedVector2ListArray = new List<Vector2>[3];
		for (int i = 0; i < 3; i++)
			protectedVector2ListArray[i] = new List<Vector2>() { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		internalVector2Array = new Vector2[3] { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		internalVector2Matrix = new Vector2[1, 3] { { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) } };

		internalVector2List = new List<Vector2>() { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		internalVector2Stack = new Stack<Vector2>();
		internalVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		internalVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		internalVector2Stack.Push(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		internalVector2Queue = new Queue<Vector2>();
		internalVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		internalVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		internalVector2Queue.Enqueue(new Vector2((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		internalVector2Dict = new Dictionary<string, Vector2>()
		{
			{ "key1", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key2", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key3", new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) }
		};

		internalVector2ListArray = new List<Vector2>[3];
		for (int i = 0; i < 3; i++)
			internalVector2ListArray[i] = new List<Vector2>() { new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector2((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };
		#endregion

		#region Vector3Collections
		publicVector3Array = new Vector3[3] { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		publicVector3Matrix = new Vector3[1, 3] { { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) } };

		publicVector3List = new List<Vector3>() { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		publicVector3Stack = new Stack<Vector3>();
		publicVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		publicVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		publicVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		publicVector3Queue = new Queue<Vector3>();
		publicVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		publicVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		publicVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		publicVector3Dict = new Dictionary<string, Vector3>()
		{
			{ "key1", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key2", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key3", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) }
		};

		publicVector3ListArray = new List<Vector3>[3];
		for (int i = 0; i < 3; i++)
			publicVector3ListArray[i] = new List<Vector3>() { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		privateVector3Array = new Vector3[3] { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		privateVector3Matrix = new Vector3[1, 3] { { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) } };

		privateVector3List = new List<Vector3>() { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		privateVector3Stack = new Stack<Vector3>();
		privateVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		privateVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		privateVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		privateVector3Queue = new Queue<Vector3>();
		privateVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		privateVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		privateVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		privateVector3Dict = new Dictionary<string, Vector3>()
		{
			{ "key1", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key2", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key3", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) }
		};

		privateVector3ListArray = new List<Vector3>[3];
		for (int i = 0; i < 3; i++)
			privateVector3ListArray[i] = new List<Vector3>() { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		protectedVector3Array = new Vector3[3] { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		protectedVector3Matrix = new Vector3[1, 3] { { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) } };

		protectedVector3List = new List<Vector3>() { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		protectedVector3Stack = new Stack<Vector3>();
		protectedVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		protectedVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		protectedVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		protectedVector3Queue = new Queue<Vector3>();
		protectedVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		protectedVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		protectedVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		protectedVector3Dict = new Dictionary<string, Vector3>()
		{
			{ "key1", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key2", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key3", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) }
		};

		protectedVector3ListArray = new List<Vector3>[3];
		for (int i = 0; i < 3; i++)
			protectedVector3ListArray[i] = new List<Vector3>() { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		internalVector3Array = new Vector3[3] { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		internalVector3Matrix = new Vector3[1, 3] { { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) } };

		internalVector3List = new List<Vector3>() { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };

		internalVector3Stack = new Stack<Vector3>();
		internalVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		internalVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		internalVector3Stack.Push(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		internalVector3Queue = new Queue<Vector3>();
		internalVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		internalVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));
		internalVector3Queue.Enqueue(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		internalVector3Dict = new Dictionary<string, Vector3>()
		{
			{ "key1", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key2", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) },
			{ "key3", new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) }
		};

		internalVector3ListArray = new List<Vector3>[3];
		for (int i = 0; i < 3; i++)
			internalVector3ListArray[i] = new List<Vector3>() { new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)),
				new Vector3((float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000),(float)(rnd.NextDouble() * 1000)) };
		#endregion

		#region Vector2IntCollections
		publicVector2IntArray = new Vector2Int[3] { new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()) };

		publicVector2IntMatrix = new Vector2Int[1, 3] { { new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()) } };

		publicVector2IntList = new List<Vector2Int>() { new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()) };

		publicVector2IntStack = new Stack<Vector2Int>();
		publicVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));
		publicVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));
		publicVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));

		publicVector2IntQueue = new Queue<Vector2Int>();
		publicVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));
		publicVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));
		publicVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));

		publicVector2IntDict = new Dictionary<string, Vector2Int>()
		{
			{ "key1", new Vector2Int(rnd.Next(),rnd.Next()) },
			{ "key2", new Vector2Int(rnd.Next(),rnd.Next()) },
			{ "key3", new Vector2Int(rnd.Next(),rnd.Next()) }
		};

		publicVector2IntListArray = new List<Vector2Int>[3];
		for (int i = 0; i < 3; i++)
			publicVector2IntListArray[i] = new List<Vector2Int>() { new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()) };

		privateVector2IntArray = new Vector2Int[3] { new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()) };

		privateVector2IntMatrix = new Vector2Int[1, 3] { { new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()) } };

		privateVector2IntList = new List<Vector2Int>() { new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()) };

		privateVector2IntStack = new Stack<Vector2Int>();
		privateVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));
		privateVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));
		privateVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));

		privateVector2IntQueue = new Queue<Vector2Int>();
		privateVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));
		privateVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));
		privateVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));

		privateVector2IntDict = new Dictionary<string, Vector2Int>()
		{
			{ "key1", new Vector2Int(rnd.Next(),rnd.Next()) },
			{ "key2", new Vector2Int(rnd.Next(),rnd.Next()) },
			{ "key3", new Vector2Int(rnd.Next(),rnd.Next()) }
		};

		privateVector2IntListArray = new List<Vector2Int>[3];
		for (int i = 0; i < 3; i++)
			privateVector2IntListArray[i] = new List<Vector2Int>() { new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()) };

		protectedVector2IntArray = new Vector2Int[3] { new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()) };

		protectedVector2IntMatrix = new Vector2Int[1, 3] { { new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()) } };

		protectedVector2IntList = new List<Vector2Int>() { new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()) };

		protectedVector2IntStack = new Stack<Vector2Int>();
		protectedVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));
		protectedVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));
		protectedVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));

		protectedVector2IntQueue = new Queue<Vector2Int>();
		protectedVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));
		protectedVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));
		protectedVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));

		protectedVector2IntDict = new Dictionary<string, Vector2Int>()
		{
			{ "key1", new Vector2Int(rnd.Next(),rnd.Next()) },
			{ "key2", new Vector2Int(rnd.Next(),rnd.Next()) },
			{ "key3", new Vector2Int(rnd.Next(),rnd.Next()) }
		};

		protectedVector2IntListArray = new List<Vector2Int>[3];
		for (int i = 0; i < 3; i++)
			protectedVector2IntListArray[i] = new List<Vector2Int>() { new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()) };

		internalVector2IntArray = new Vector2Int[3] { new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()) };

		internalVector2IntMatrix = new Vector2Int[1, 3] { { new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()) } };

		internalVector2IntList = new List<Vector2Int>() { new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()),
			new Vector2Int(rnd.Next(),rnd.Next()) };

		internalVector2IntStack = new Stack<Vector2Int>();
		internalVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));
		internalVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));
		internalVector2IntStack.Push(new Vector2Int(rnd.Next(), rnd.Next()));

		internalVector2IntQueue = new Queue<Vector2Int>();
		internalVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));
		internalVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));
		internalVector2IntQueue.Enqueue(new Vector2Int(rnd.Next(), rnd.Next()));

		internalVector2IntDict = new Dictionary<string, Vector2Int>()
		{
			{ "key1", new Vector2Int(rnd.Next(),rnd.Next()) },
			{ "key2", new Vector2Int(rnd.Next(),rnd.Next()) },
			{ "key3", new Vector2Int(rnd.Next(),rnd.Next()) }
		};

		internalVector2IntListArray = new List<Vector2Int>[3];
		for (int i = 0; i < 3; i++)
			internalVector2IntListArray[i] = new List<Vector2Int>() { new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()),
				new Vector2Int(rnd.Next(),rnd.Next()) };
		#endregion

		#region Vector3IntCollections
		publicVector3IntArray = new Vector3Int[3] { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		publicVector3IntMatrix = new Vector3Int[1, 3] { { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) } };

		publicVector3IntList = new List<Vector3Int>() { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		publicVector3IntStack = new Stack<Vector3Int>();
		publicVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));
		publicVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));
		publicVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));

		publicVector3IntQueue = new Queue<Vector3Int>();
		publicVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));
		publicVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));
		publicVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));

		publicVector3IntDict = new Dictionary<string, Vector3Int>()
		{
			{ "key1", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) },
			{ "key3", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) },
			{ "key3", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) }
		};

		publicVector3IntListArray = new List<Vector3Int>[3];
		for (int i = 0; i < 3; i++)
			publicVector3IntListArray[i] = new List<Vector3Int>() { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		privateVector3IntArray = new Vector3Int[3] { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		privateVector3IntMatrix = new Vector3Int[1, 3] { { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) } };

		privateVector3IntList = new List<Vector3Int>() { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		privateVector3IntStack = new Stack<Vector3Int>();
		privateVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));
		privateVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));
		privateVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));

		privateVector3IntQueue = new Queue<Vector3Int>();
		privateVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));
		privateVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));
		privateVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));

		privateVector3IntDict = new Dictionary<string, Vector3Int>()
		{
			{ "key1", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) },
			{ "key3", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) },
			{ "key3", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) }
		};

		privateVector3IntListArray = new List<Vector3Int>[3];
		for (int i = 0; i < 3; i++)
			privateVector3IntListArray[i] = new List<Vector3Int>() { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		protectedVector3IntArray = new Vector3Int[3] { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		protectedVector3IntMatrix = new Vector3Int[1, 3] { { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) } };

		protectedVector3IntList = new List<Vector3Int>() { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		protectedVector3IntStack = new Stack<Vector3Int>();
		protectedVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));
		protectedVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));
		protectedVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));

		protectedVector3IntQueue = new Queue<Vector3Int>();
		protectedVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));
		protectedVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));
		protectedVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));

		protectedVector3IntDict = new Dictionary<string, Vector3Int>()
		{
			{ "key1", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) },
			{ "key3", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) },
			{ "key3", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) }
		};

		protectedVector3IntListArray = new List<Vector3Int>[3];
		for (int i = 0; i < 3; i++)
			protectedVector3IntListArray[i] = new List<Vector3Int>() { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		internalVector3IntArray = new Vector3Int[3] { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		internalVector3IntMatrix = new Vector3Int[1, 3] { { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) } };

		internalVector3IntList = new List<Vector3Int>() { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
			new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };

		internalVector3IntStack = new Stack<Vector3Int>();
		internalVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));
		internalVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));
		internalVector3IntStack.Push(new Vector3Int(rnd.Next(), rnd.Next()));

		internalVector3IntQueue = new Queue<Vector3Int>();
		internalVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));
		internalVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));
		internalVector3IntQueue.Enqueue(new Vector3Int(rnd.Next(), rnd.Next()));

		internalVector3IntDict = new Dictionary<string, Vector3Int>()
		{
			{ "key1", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) },
			{ "key3", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) },
			{ "key3", new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) }
		};

		internalVector3IntListArray = new List<Vector3Int>[3];
		for (int i = 0; i < 3; i++)
			internalVector3IntListArray[i] = new List<Vector3Int>() { new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()),
				new Vector3Int(rnd.Next(),rnd.Next(),rnd.Next()) };
		#endregion

		#region ColorCollections
		publicColorArray = new Color[3] { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		publicColorMatrix = new Color[1, 3] { { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) } };

		publicColorList = new List<Color>() { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		publicColorStack = new Stack<Color>();
		publicColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		publicColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		publicColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

		publicColorQueue = new Queue<Color>();
		publicColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		publicColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		publicColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

		publicColorDict = new Dictionary<string, Color>()
		{
			{ "key1", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) },
			{ "key2", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) },
			{ "key3", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) }
		};

		publicColorListArray = new List<Color>[3];
		for (int i = 0; i < 3; i++)
			publicColorListArray[i] = new List<Color>() { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		privateColorArray = new Color[3] { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		privateColorMatrix = new Color[1, 3] { { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) } };

		privateColorList = new List<Color>() { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		privateColorStack = new Stack<Color>();
		privateColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		privateColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		privateColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

		privateColorQueue = new Queue<Color>();
		privateColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		privateColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		privateColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

		privateColorDict = new Dictionary<string, Color>()
		{
			{ "key1", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) },
			{ "key2", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) },
			{ "key3", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) }
		};

		privateColorListArray = new List<Color>[3];
		for (int i = 0; i < 3; i++)
			privateColorListArray[i] = new List<Color>() { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		protectedColorArray = new Color[3] { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		protectedColorMatrix = new Color[1, 3] { { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) } };

		protectedColorList = new List<Color>() { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		protectedColorStack = new Stack<Color>();
		protectedColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		protectedColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		protectedColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

		protectedColorQueue = new Queue<Color>();
		protectedColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		protectedColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		protectedColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

		protectedColorDict = new Dictionary<string, Color>()
		{
			{ "key1", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) },
			{ "key2", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) },
			{ "key3", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) }
		};

		protectedColorListArray = new List<Color>[3];
		for (int i = 0; i < 3; i++)
			protectedColorListArray[i] = new List<Color>() { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		internalColorArray = new Color[3] { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		internalColorMatrix = new Color[1, 3] { { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) } };

		internalColorList = new List<Color>() { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
			new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };

		internalColorStack = new Stack<Color>();
		internalColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		internalColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		internalColorStack.Push(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

		internalColorQueue = new Queue<Color>();
		internalColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		internalColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
		internalColorQueue.Enqueue(new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

		internalColorDict = new Dictionary<string, Color>()
		{
			{ "key1", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) },
			{ "key2", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) },
			{ "key3", new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) }
		};

		internalColorListArray = new List<Color>[3];
		for (int i = 0; i < 3; i++)
			internalColorListArray[i] = new List<Color>() { new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()),
				new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()) };
		#endregion

		#region BoundsCollections
		publicBoundsArray = new Bounds[3] { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		publicBoundsMatrix = new Bounds[1, 3] { { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) } };

		publicBoundsList = new List<Bounds>() { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		publicBoundsStack = new Stack<Bounds>();
		publicBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		publicBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		publicBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));

		publicBoundsQueue = new Queue<Bounds>();
		publicBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		publicBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		publicBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));

		publicBoundsDict = new Dictionary<string, Bounds>()
		{
			{ "key1", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) },
			{ "key2", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) },
			{ "key3", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) }
		};

		publicBoundsListArray = new List<Bounds>[3];
		for (int i = 0; i < 3; i++)
			publicBoundsListArray[i] = new List<Bounds>() { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		privateBoundsArray = new Bounds[3] { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		privateBoundsMatrix = new Bounds[1, 3] { { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) } };

		privateBoundsList = new List<Bounds>() { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		privateBoundsStack = new Stack<Bounds>();
		privateBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		privateBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		privateBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));

		privateBoundsQueue = new Queue<Bounds>();
		privateBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		privateBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		privateBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));

		privateBoundsDict = new Dictionary<string, Bounds>()
		{
			{ "key1", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) },
			{ "key2", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) },
			{ "key3", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) }
		};

		privateBoundsListArray = new List<Bounds>[3];
		for (int i = 0; i < 3; i++)
			privateBoundsListArray[i] = new List<Bounds>() { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		protectedBoundsArray = new Bounds[3] { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		protectedBoundsMatrix = new Bounds[1, 3] { { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) } };

		protectedBoundsList = new List<Bounds>() { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		protectedBoundsStack = new Stack<Bounds>();
		protectedBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		protectedBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		protectedBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));

		protectedBoundsQueue = new Queue<Bounds>();
		protectedBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		protectedBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		protectedBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));

		protectedBoundsDict = new Dictionary<string, Bounds>()
		{
			{ "key1", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) },
			{ "key2", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) },
			{ "key3", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) }
		};

		protectedBoundsListArray = new List<Bounds>[3];
		for (int i = 0; i < 3; i++)
			protectedBoundsListArray[i] = new List<Bounds>() { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		internalBoundsArray = new Bounds[3] { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		internalBoundsMatrix = new Bounds[1, 3] { { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) } };

		internalBoundsList = new List<Bounds>() { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
			new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };

		internalBoundsStack = new Stack<Bounds>();
		internalBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		internalBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		internalBoundsStack.Push(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));

		internalBoundsQueue = new Queue<Bounds>();
		internalBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		internalBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));
		internalBoundsQueue.Enqueue(new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))));

		internalBoundsDict = new Dictionary<string, Bounds>()
		{
			{ "key1", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) },
			{ "key2", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) },
			{ "key3", new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) }
		};

		internalBoundsListArray = new List<Bounds>[3];
		for (int i = 0; i < 3; i++)
			internalBoundsListArray[i] = new List<Bounds>() { new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))),
				new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000))) };
		#endregion

		#region BoundsIntCollections
		publicBoundsIntArray = new BoundsInt[3] { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		publicBoundsIntMatrix = new BoundsInt[1, 3] { { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) } };

		publicBoundsIntList = new List<BoundsInt>() { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		publicBoundsIntStack = new Stack<BoundsInt>();
		publicBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		publicBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		publicBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));

		publicBoundsIntQueue = new Queue<BoundsInt>();
		publicBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		publicBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		publicBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));

		publicBoundsIntDict = new Dictionary<string, BoundsInt>()
		{
			{ "key1", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) },
			{ "key2", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) },
			{ "key3", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) }
		};

		publicBoundsIntListArray = new List<BoundsInt>[3];
		for (int i = 0; i < 3; i++)
			publicBoundsIntListArray[i] = new List<BoundsInt>() { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		privateBoundsIntArray = new BoundsInt[3] { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		privateBoundsIntMatrix = new BoundsInt[1, 3] { { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) } };

		privateBoundsIntList = new List<BoundsInt>() { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		privateBoundsIntStack = new Stack<BoundsInt>();
		privateBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		privateBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		privateBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));

		privateBoundsIntQueue = new Queue<BoundsInt>();
		privateBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		privateBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		privateBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));

		privateBoundsIntDict = new Dictionary<string, BoundsInt>()
		{
			{ "key1", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) },
			{ "key2", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) },
			{ "key3", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) }
		};

		privateBoundsIntListArray = new List<BoundsInt>[3];
		for (int i = 0; i < 3; i++)
			privateBoundsIntListArray[i] = new List<BoundsInt>() { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		protectedBoundsIntArray = new BoundsInt[3] { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		protectedBoundsIntMatrix = new BoundsInt[1, 3] { { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) } };

		protectedBoundsIntList = new List<BoundsInt>() { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		protectedBoundsIntStack = new Stack<BoundsInt>();
		protectedBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		protectedBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		protectedBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));

		protectedBoundsIntQueue = new Queue<BoundsInt>();
		protectedBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		protectedBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		protectedBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));

		protectedBoundsIntDict = new Dictionary<string, BoundsInt>()
		{
			{ "key1", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) },
			{ "key2", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) },
			{ "key3", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) }
		};

		protectedBoundsIntListArray = new List<BoundsInt>[3];
		for (int i = 0; i < 3; i++)
			protectedBoundsIntListArray[i] = new List<BoundsInt>() { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		internalBoundsIntArray = new BoundsInt[3] { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		internalBoundsIntMatrix = new BoundsInt[1, 3] { { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) } };

		internalBoundsIntList = new List<BoundsInt>() { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
			new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };

		internalBoundsIntStack = new Stack<BoundsInt>();
		internalBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		internalBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		internalBoundsIntStack.Push(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));

		internalBoundsIntQueue = new Queue<BoundsInt>();
		internalBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		internalBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));
		internalBoundsIntQueue.Enqueue(new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())));

		internalBoundsIntDict = new Dictionary<string, BoundsInt>()
		{
			{ "key1", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) },
			{ "key2", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) },
			{ "key3", new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) }
		};

		internalBoundsIntListArray = new List<BoundsInt>[3];
		for (int i = 0; i < 3; i++)
			internalBoundsIntListArray[i] = new List<BoundsInt>() { new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())),
				new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next())) };
		#endregion
		#endregion

		#region Enums
		publicShortEnum = GetRandomEnumValue<ShortEnum>(rnd);
		privateShortEnum = GetRandomEnumValue<ShortEnum>(rnd);
		protectedShortEnum = GetRandomEnumValue<ShortEnum>(rnd);
		internalShortEnum = GetRandomEnumValue<ShortEnum>(rnd);

		publicUShortEnum = GetRandomEnumValue<UShortEnum>(rnd);
		privateUShortEnum = GetRandomEnumValue<UShortEnum>(rnd);
		protectedUShortEnum = GetRandomEnumValue<UShortEnum>(rnd);
		internalUShortEnum = GetRandomEnumValue<UShortEnum>(rnd);

		publicIntEnum = GetRandomEnumValue<IntEnum>(rnd);
		privateIntEnum = GetRandomEnumValue<IntEnum>(rnd);
		protectedIntEnum = GetRandomEnumValue<IntEnum>(rnd);
		internalIntEnum = GetRandomEnumValue<IntEnum>(rnd);

		publicUIntEnum = GetRandomEnumValue<UIntEnum>(rnd);
		privateUIntEnum = GetRandomEnumValue<UIntEnum>(rnd);
		protectedUIntEnum = GetRandomEnumValue<UIntEnum>(rnd);
		internalUIntEnum = GetRandomEnumValue<UIntEnum>(rnd);

		publicLongEnum = GetRandomEnumValue<LongEnum>(rnd);
		privateLongEnum = GetRandomEnumValue<LongEnum>(rnd);
		protectedLongEnum = GetRandomEnumValue<LongEnum>(rnd);
		internalLongEnum = GetRandomEnumValue<LongEnum>(rnd);

		publicULongEnum = GetRandomEnumValue<ULongEnum>(rnd);
		privateULongEnum = GetRandomEnumValue<ULongEnum>(rnd);
		protectedULongEnum = GetRandomEnumValue<ULongEnum>(rnd);
		internalULongEnum = GetRandomEnumValue<ULongEnum>(rnd);
		#endregion

		#region EnumsCollections
		#region ShortEnumCollections
		publicShortEnumArray = new ShortEnum[3] { GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd) };

		publicShortEnumMatrix = new ShortEnum[1, 3] { { GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd) } };

		publicShortEnumList = new List<ShortEnum>() { GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd) };

		publicShortEnumStack = new Stack<ShortEnum>();
		publicShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));
		publicShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));
		publicShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));

		publicShortEnumQueue = new Queue<ShortEnum>();
		publicShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));
		publicShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));
		publicShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));

		publicShortEnumDict = new Dictionary<string, ShortEnum>()
		{
			{ "key1", GetRandomEnumValue<ShortEnum>(rnd)},
			{ "key2", GetRandomEnumValue<ShortEnum>(rnd)},
			{ "key3", GetRandomEnumValue<ShortEnum>(rnd)}
		};

		publicShortEnumListArray = new List<ShortEnum>[3];
		for (int i = 0; i < 3; i++)
			publicShortEnumListArray[i] = new List<ShortEnum>() { GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd) };

		privateShortEnumArray = new ShortEnum[3] { GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd) };

		privateShortEnumMatrix = new ShortEnum[1, 3] { { GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd) } };

		privateShortEnumList = new List<ShortEnum>() { GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd) };

		privateShortEnumStack = new Stack<ShortEnum>();
		privateShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));
		privateShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));
		privateShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));

		privateShortEnumQueue = new Queue<ShortEnum>();
		privateShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));
		privateShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));
		privateShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));

		privateShortEnumDict = new Dictionary<string, ShortEnum>()
		{
			{ "key1", GetRandomEnumValue<ShortEnum>(rnd) },
			{ "key2", GetRandomEnumValue<ShortEnum>(rnd) },
			{ "key3", GetRandomEnumValue<ShortEnum>(rnd) }
		};

		privateShortEnumListArray = new List<ShortEnum>[3];
		for (int i = 0; i < 3; i++)
			privateShortEnumListArray[i] = new List<ShortEnum>() { GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd) };

		protectedShortEnumArray = new ShortEnum[3] { GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd) };

		protectedShortEnumMatrix = new ShortEnum[1, 3] { { GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd) } };

		protectedShortEnumList = new List<ShortEnum>() {GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd) };

		protectedShortEnumStack = new Stack<ShortEnum>();
		protectedShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));
		protectedShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));
		protectedShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));

		protectedShortEnumQueue = new Queue<ShortEnum>();
		protectedShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));
		protectedShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));
		protectedShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));

		protectedShortEnumDict = new Dictionary<string, ShortEnum>()
		{
			{ "key1", GetRandomEnumValue<ShortEnum>(rnd) },
			{ "key2", GetRandomEnumValue<ShortEnum>(rnd) },
			{ "key3", GetRandomEnumValue<ShortEnum>(rnd) }
		};

		protectedShortEnumListArray = new List<ShortEnum>[3];
		for (int i = 0; i < 3; i++)
			protectedShortEnumListArray[i] = new List<ShortEnum>() { GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd)};

		internalShortEnumArray = new ShortEnum[3] {GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd) };

		internalShortEnumMatrix = new ShortEnum[1, 3] { { GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd) } };

		internalShortEnumList = new List<ShortEnum>() { GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd),
			GetRandomEnumValue<ShortEnum>(rnd)};

		internalShortEnumStack = new Stack<ShortEnum>();
		internalShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));
		internalShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));
		internalShortEnumStack.Push(GetRandomEnumValue<ShortEnum>(rnd));

		internalShortEnumQueue = new Queue<ShortEnum>();
		internalShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));
		internalShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));
		internalShortEnumQueue.Enqueue(GetRandomEnumValue<ShortEnum>(rnd));

		internalShortEnumDict = new Dictionary<string, ShortEnum>()
		{
			{ "key1", GetRandomEnumValue<ShortEnum>(rnd)},
			{ "key2", GetRandomEnumValue<ShortEnum>(rnd)},
			{ "key3", GetRandomEnumValue<ShortEnum>(rnd)}
		};

		internalShortEnumListArray = new List<ShortEnum>[3];
		for (int i = 0; i < 3; i++)
			internalShortEnumListArray[i] = new List<ShortEnum>() { GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd),
				GetRandomEnumValue<ShortEnum>(rnd)};
		#endregion

		#region UShortEnumCollections
		publicUShortEnumArray = new UShortEnum[3] { GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd) };

		publicUShortEnumMatrix = new UShortEnum[1, 3] { { GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd) } };

		publicUShortEnumList = new List<UShortEnum>() { GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd) };

		publicUShortEnumStack = new Stack<UShortEnum>();
		publicUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));
		publicUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));
		publicUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));

		publicUShortEnumQueue = new Queue<UShortEnum>();
		publicUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));
		publicUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));
		publicUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));

		publicUShortEnumDict = new Dictionary<string, UShortEnum>()
		{
			{ "key1", GetRandomEnumValue<UShortEnum>(rnd)},
			{ "key2", GetRandomEnumValue<UShortEnum>(rnd)},
			{ "key3", GetRandomEnumValue<UShortEnum>(rnd)}
		};

		publicUShortEnumListArray = new List<UShortEnum>[3];
		for (int i = 0; i < 3; i++)
			publicUShortEnumListArray[i] = new List<UShortEnum>() { GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd) };

		privateUShortEnumArray = new UShortEnum[3] { GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd) };

		privateUShortEnumMatrix = new UShortEnum[1, 3] { { GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd) } };

		privateUShortEnumList = new List<UShortEnum>() { GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd) };

		privateUShortEnumStack = new Stack<UShortEnum>();
		privateUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));
		privateUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));
		privateUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));

		privateUShortEnumQueue = new Queue<UShortEnum>();
		privateUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));
		privateUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));
		privateUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));

		privateUShortEnumDict = new Dictionary<string, UShortEnum>()
		{
			{ "key1", GetRandomEnumValue<UShortEnum>(rnd) },
			{ "key2", GetRandomEnumValue<UShortEnum>(rnd) },
			{ "key3", GetRandomEnumValue<UShortEnum>(rnd) }
		};

		privateUShortEnumListArray = new List<UShortEnum>[3];
		for (int i = 0; i < 3; i++)
			privateUShortEnumListArray[i] = new List<UShortEnum>() { GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd) };

		protectedUShortEnumArray = new UShortEnum[3] { GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd) };

		protectedUShortEnumMatrix = new UShortEnum[1, 3] { { GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd) } };

		protectedUShortEnumList = new List<UShortEnum>() {GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd) };

		protectedUShortEnumStack = new Stack<UShortEnum>();
		protectedUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));
		protectedUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));
		protectedUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));

		protectedUShortEnumQueue = new Queue<UShortEnum>();
		protectedUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));
		protectedUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));
		protectedUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));

		protectedUShortEnumDict = new Dictionary<string, UShortEnum>()
		{
			{ "key1", GetRandomEnumValue<UShortEnum>(rnd) },
			{ "key2", GetRandomEnumValue<UShortEnum>(rnd) },
			{ "key3", GetRandomEnumValue<UShortEnum>(rnd) }
		};

		protectedUShortEnumListArray = new List<UShortEnum>[3];
		for (int i = 0; i < 3; i++)
			protectedUShortEnumListArray[i] = new List<UShortEnum>() { GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd)};

		internalUShortEnumArray = new UShortEnum[3] {GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd) };

		internalUShortEnumMatrix = new UShortEnum[1, 3] { { GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd) } };

		internalUShortEnumList = new List<UShortEnum>() { GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd),
			GetRandomEnumValue<UShortEnum>(rnd)};

		internalUShortEnumStack = new Stack<UShortEnum>();
		internalUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));
		internalUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));
		internalUShortEnumStack.Push(GetRandomEnumValue<UShortEnum>(rnd));

		internalUShortEnumQueue = new Queue<UShortEnum>();
		internalUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));
		internalUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));
		internalUShortEnumQueue.Enqueue(GetRandomEnumValue<UShortEnum>(rnd));

		internalUShortEnumDict = new Dictionary<string, UShortEnum>()
		{
			{ "key1", GetRandomEnumValue<UShortEnum>(rnd)},
			{ "key2", GetRandomEnumValue<UShortEnum>(rnd)},
			{ "key3", GetRandomEnumValue<UShortEnum>(rnd)}
		};

		internalUShortEnumListArray = new List<UShortEnum>[3];
		for (int i = 0; i < 3; i++)
			internalUShortEnumListArray[i] = new List<UShortEnum>() { GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd),
				GetRandomEnumValue<UShortEnum>(rnd)};
		#endregion

		#region IntEnumCollections
		publicIntEnumArray = new IntEnum[3] { GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd) };

		publicIntEnumMatrix = new IntEnum[1, 3] { { GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd) } };

		publicIntEnumList = new List<IntEnum>() { GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd) };

		publicIntEnumStack = new Stack<IntEnum>();
		publicIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));
		publicIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));
		publicIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));

		publicIntEnumQueue = new Queue<IntEnum>();
		publicIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));
		publicIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));
		publicIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));

		publicIntEnumDict = new Dictionary<string, IntEnum>()
		{
			{ "key1", GetRandomEnumValue<IntEnum>(rnd)},
			{ "key2", GetRandomEnumValue<IntEnum>(rnd)},
			{ "key3", GetRandomEnumValue<IntEnum>(rnd)}
		};

		publicIntEnumListArray = new List<IntEnum>[3];
		for (int i = 0; i < 3; i++)
			publicIntEnumListArray[i] = new List<IntEnum>() { GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd) };

		privateIntEnumArray = new IntEnum[3] { GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd) };

		privateIntEnumMatrix = new IntEnum[1, 3] { { GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd) } };

		privateIntEnumList = new List<IntEnum>() { GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd) };

		privateIntEnumStack = new Stack<IntEnum>();
		privateIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));
		privateIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));
		privateIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));

		privateIntEnumQueue = new Queue<IntEnum>();
		privateIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));
		privateIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));
		privateIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));

		privateIntEnumDict = new Dictionary<string, IntEnum>()
		{
			{ "key1", GetRandomEnumValue<IntEnum>(rnd) },
			{ "key2", GetRandomEnumValue<IntEnum>(rnd) },
			{ "key3", GetRandomEnumValue<IntEnum>(rnd) }
		};

		privateIntEnumListArray = new List<IntEnum>[3];
		for (int i = 0; i < 3; i++)
			privateIntEnumListArray[i] = new List<IntEnum>() { GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd) };

		protectedIntEnumArray = new IntEnum[3] { GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd) };

		protectedIntEnumMatrix = new IntEnum[1, 3] { { GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd) } };

		protectedIntEnumList = new List<IntEnum>() {GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd) };

		protectedIntEnumStack = new Stack<IntEnum>();
		protectedIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));
		protectedIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));
		protectedIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));

		protectedIntEnumQueue = new Queue<IntEnum>();
		protectedIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));
		protectedIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));
		protectedIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));

		protectedIntEnumDict = new Dictionary<string, IntEnum>()
		{
			{ "key1", GetRandomEnumValue<IntEnum>(rnd) },
			{ "key2", GetRandomEnumValue<IntEnum>(rnd) },
			{ "key3", GetRandomEnumValue<IntEnum>(rnd) }
		};

		protectedIntEnumListArray = new List<IntEnum>[3];
		for (int i = 0; i < 3; i++)
			protectedIntEnumListArray[i] = new List<IntEnum>() { GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd)};

		internalIntEnumArray = new IntEnum[3] {GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd) };

		internalIntEnumMatrix = new IntEnum[1, 3] { { GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd) } };

		internalIntEnumList = new List<IntEnum>() { GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd),
			GetRandomEnumValue<IntEnum>(rnd)};

		internalIntEnumStack = new Stack<IntEnum>();
		internalIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));
		internalIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));
		internalIntEnumStack.Push(GetRandomEnumValue<IntEnum>(rnd));

		internalIntEnumQueue = new Queue<IntEnum>();
		internalIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));
		internalIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));
		internalIntEnumQueue.Enqueue(GetRandomEnumValue<IntEnum>(rnd));

		internalIntEnumDict = new Dictionary<string, IntEnum>()
		{
			{ "key1", GetRandomEnumValue<IntEnum>(rnd)},
			{ "key2", GetRandomEnumValue<IntEnum>(rnd)},
			{ "key3", GetRandomEnumValue<IntEnum>(rnd)}
		};

		internalIntEnumListArray = new List<IntEnum>[3];
		for (int i = 0; i < 3; i++)
			internalIntEnumListArray[i] = new List<IntEnum>() { GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd),
				GetRandomEnumValue<IntEnum>(rnd)};
		#endregion

		#region UIntEnumCollections
		publicUIntEnumArray = new UIntEnum[3] { GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd) };

		publicUIntEnumMatrix = new UIntEnum[1, 3] { { GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd) } };

		publicUIntEnumList = new List<UIntEnum>() { GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd) };

		publicUIntEnumStack = new Stack<UIntEnum>();
		publicUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));
		publicUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));
		publicUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));

		publicUIntEnumQueue = new Queue<UIntEnum>();
		publicUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));
		publicUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));
		publicUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));

		publicUIntEnumDict = new Dictionary<string, UIntEnum>()
		{
			{ "key1", GetRandomEnumValue<UIntEnum>(rnd)},
			{ "key2", GetRandomEnumValue<UIntEnum>(rnd)},
			{ "key3", GetRandomEnumValue<UIntEnum>(rnd)}
		};

		publicUIntEnumListArray = new List<UIntEnum>[3];
		for (int i = 0; i < 3; i++)
			publicUIntEnumListArray[i] = new List<UIntEnum>() { GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd) };

		privateUIntEnumArray = new UIntEnum[3] { GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd) };

		privateUIntEnumMatrix = new UIntEnum[1, 3] { { GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd) } };

		privateUIntEnumList = new List<UIntEnum>() { GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd) };

		privateUIntEnumStack = new Stack<UIntEnum>();
		privateUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));
		privateUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));
		privateUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));

		privateUIntEnumQueue = new Queue<UIntEnum>();
		privateUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));
		privateUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));
		privateUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));

		privateUIntEnumDict = new Dictionary<string, UIntEnum>()
		{
			{ "key1", GetRandomEnumValue<UIntEnum>(rnd) },
			{ "key2", GetRandomEnumValue<UIntEnum>(rnd) },
			{ "key3", GetRandomEnumValue<UIntEnum>(rnd) }
		};

		privateUIntEnumListArray = new List<UIntEnum>[3];
		for (int i = 0; i < 3; i++)
			privateUIntEnumListArray[i] = new List<UIntEnum>() { GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd) };

		protectedUIntEnumArray = new UIntEnum[3] { GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd) };

		protectedUIntEnumMatrix = new UIntEnum[1, 3] { { GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd) } };

		protectedUIntEnumList = new List<UIntEnum>() {GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd) };

		protectedUIntEnumStack = new Stack<UIntEnum>();
		protectedUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));
		protectedUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));
		protectedUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));

		protectedUIntEnumQueue = new Queue<UIntEnum>();
		protectedUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));
		protectedUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));
		protectedUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));

		protectedUIntEnumDict = new Dictionary<string, UIntEnum>()
		{
			{ "key1", GetRandomEnumValue<UIntEnum>(rnd) },
			{ "key2", GetRandomEnumValue<UIntEnum>(rnd) },
			{ "key3", GetRandomEnumValue<UIntEnum>(rnd) }
		};

		protectedUIntEnumListArray = new List<UIntEnum>[3];
		for (int i = 0; i < 3; i++)
			protectedUIntEnumListArray[i] = new List<UIntEnum>() { GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd)};

		internalUIntEnumArray = new UIntEnum[3] {GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd) };

		internalUIntEnumMatrix = new UIntEnum[1, 3] { { GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd) } };

		internalUIntEnumList = new List<UIntEnum>() { GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd),
			GetRandomEnumValue<UIntEnum>(rnd)};

		internalUIntEnumStack = new Stack<UIntEnum>();
		internalUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));
		internalUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));
		internalUIntEnumStack.Push(GetRandomEnumValue<UIntEnum>(rnd));

		internalUIntEnumQueue = new Queue<UIntEnum>();
		internalUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));
		internalUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));
		internalUIntEnumQueue.Enqueue(GetRandomEnumValue<UIntEnum>(rnd));

		internalUIntEnumDict = new Dictionary<string, UIntEnum>()
		{
			{ "key1", GetRandomEnumValue<UIntEnum>(rnd)},
			{ "key2", GetRandomEnumValue<UIntEnum>(rnd)},
			{ "key3", GetRandomEnumValue<UIntEnum>(rnd)}
		};

		internalUIntEnumListArray = new List<UIntEnum>[3];
		for (int i = 0; i < 3; i++)
			internalUIntEnumListArray[i] = new List<UIntEnum>() { GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd),
				GetRandomEnumValue<UIntEnum>(rnd)};
		#endregion

		#region LongEnumCollections
		publicLongEnumArray = new LongEnum[3] { GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd) };

		publicLongEnumMatrix = new LongEnum[1, 3] { { GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd) } };

		publicLongEnumList = new List<LongEnum>() { GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd) };

		publicLongEnumStack = new Stack<LongEnum>();
		publicLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));
		publicLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));
		publicLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));

		publicLongEnumQueue = new Queue<LongEnum>();
		publicLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));
		publicLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));
		publicLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));

		publicLongEnumDict = new Dictionary<string, LongEnum>()
		{
			{ "key1", GetRandomEnumValue<LongEnum>(rnd)},
			{ "key2", GetRandomEnumValue<LongEnum>(rnd)},
			{ "key3", GetRandomEnumValue<LongEnum>(rnd)}
		};

		publicLongEnumListArray = new List<LongEnum>[3];
		for (int i = 0; i < 3; i++)
			publicLongEnumListArray[i] = new List<LongEnum>() { GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd) };

		privateLongEnumArray = new LongEnum[3] { GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd) };

		privateLongEnumMatrix = new LongEnum[1, 3] { { GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd) } };

		privateLongEnumList = new List<LongEnum>() { GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd) };

		privateLongEnumStack = new Stack<LongEnum>();
		privateLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));
		privateLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));
		privateLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));

		privateLongEnumQueue = new Queue<LongEnum>();
		privateLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));
		privateLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));
		privateLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));

		privateLongEnumDict = new Dictionary<string, LongEnum>()
		{
			{ "key1", GetRandomEnumValue<LongEnum>(rnd) },
			{ "key2", GetRandomEnumValue<LongEnum>(rnd) },
			{ "key3", GetRandomEnumValue<LongEnum>(rnd) }
		};

		privateLongEnumListArray = new List<LongEnum>[3];
		for (int i = 0; i < 3; i++)
			privateLongEnumListArray[i] = new List<LongEnum>() { GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd) };

		protectedLongEnumArray = new LongEnum[3] { GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd) };

		protectedLongEnumMatrix = new LongEnum[1, 3] { { GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd) } };

		protectedLongEnumList = new List<LongEnum>() {GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd) };

		protectedLongEnumStack = new Stack<LongEnum>();
		protectedLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));
		protectedLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));
		protectedLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));

		protectedLongEnumQueue = new Queue<LongEnum>();
		protectedLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));
		protectedLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));
		protectedLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));

		protectedLongEnumDict = new Dictionary<string, LongEnum>()
		{
			{ "key1", GetRandomEnumValue<LongEnum>(rnd) },
			{ "key2", GetRandomEnumValue<LongEnum>(rnd) },
			{ "key3", GetRandomEnumValue<LongEnum>(rnd) }
		};

		protectedLongEnumListArray = new List<LongEnum>[3];
		for (int i = 0; i < 3; i++)
			protectedLongEnumListArray[i] = new List<LongEnum>() { GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd)};

		internalLongEnumArray = new LongEnum[3] {GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd) };

		internalLongEnumMatrix = new LongEnum[1, 3] { { GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd) } };

		internalLongEnumList = new List<LongEnum>() { GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd),
			GetRandomEnumValue<LongEnum>(rnd)};

		internalLongEnumStack = new Stack<LongEnum>();
		internalLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));
		internalLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));
		internalLongEnumStack.Push(GetRandomEnumValue<LongEnum>(rnd));

		internalLongEnumQueue = new Queue<LongEnum>();
		internalLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));
		internalLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));
		internalLongEnumQueue.Enqueue(GetRandomEnumValue<LongEnum>(rnd));

		internalLongEnumDict = new Dictionary<string, LongEnum>()
		{
			{ "key1", GetRandomEnumValue<LongEnum>(rnd)},
			{ "key2", GetRandomEnumValue<LongEnum>(rnd)},
			{ "key3", GetRandomEnumValue<LongEnum>(rnd)}
		};

		internalLongEnumListArray = new List<LongEnum>[3];
		for (int i = 0; i < 3; i++)
			internalLongEnumListArray[i] = new List<LongEnum>() { GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd),
				GetRandomEnumValue<LongEnum>(rnd)};
		#endregion

		#region ULongEnumCollections
		publicULongEnumArray = new ULongEnum[3] { GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd) };

		publicULongEnumMatrix = new ULongEnum[1, 3] { { GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd) } };

		publicULongEnumList = new List<ULongEnum>() { GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd) };

		publicULongEnumStack = new Stack<ULongEnum>();
		publicULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));
		publicULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));
		publicULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));

		publicULongEnumQueue = new Queue<ULongEnum>();
		publicULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));
		publicULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));
		publicULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));

		publicULongEnumDict = new Dictionary<string, ULongEnum>()
		{
			{ "key1", GetRandomEnumValue<ULongEnum>(rnd)},
			{ "key2", GetRandomEnumValue<ULongEnum>(rnd)},
			{ "key3", GetRandomEnumValue<ULongEnum>(rnd)}
		};

		publicULongEnumListArray = new List<ULongEnum>[3];
		for (int i = 0; i < 3; i++)
			publicULongEnumListArray[i] = new List<ULongEnum>() { GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd) };

		privateULongEnumArray = new ULongEnum[3] { GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd) };

		privateULongEnumMatrix = new ULongEnum[1, 3] { { GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd) } };

		privateULongEnumList = new List<ULongEnum>() { GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd) };

		privateULongEnumStack = new Stack<ULongEnum>();
		privateULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));
		privateULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));
		privateULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));

		privateULongEnumQueue = new Queue<ULongEnum>();
		privateULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));
		privateULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));
		privateULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));

		privateULongEnumDict = new Dictionary<string, ULongEnum>()
		{
			{ "key1", GetRandomEnumValue<ULongEnum>(rnd) },
			{ "key2", GetRandomEnumValue<ULongEnum>(rnd) },
			{ "key3", GetRandomEnumValue<ULongEnum>(rnd) }
		};

		privateULongEnumListArray = new List<ULongEnum>[3];
		for (int i = 0; i < 3; i++)
			privateULongEnumListArray[i] = new List<ULongEnum>() { GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd) };

		protectedULongEnumArray = new ULongEnum[3] { GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd) };

		protectedULongEnumMatrix = new ULongEnum[1, 3] { { GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd) } };

		protectedULongEnumList = new List<ULongEnum>() {GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd) };

		protectedULongEnumStack = new Stack<ULongEnum>();
		protectedULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));
		protectedULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));
		protectedULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));

		protectedULongEnumQueue = new Queue<ULongEnum>();
		protectedULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));
		protectedULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));
		protectedULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));

		protectedULongEnumDict = new Dictionary<string, ULongEnum>()
		{
			{ "key1", GetRandomEnumValue<ULongEnum>(rnd) },
			{ "key2", GetRandomEnumValue<ULongEnum>(rnd) },
			{ "key3", GetRandomEnumValue<ULongEnum>(rnd) }
		};

		protectedULongEnumListArray = new List<ULongEnum>[3];
		for (int i = 0; i < 3; i++)
			protectedULongEnumListArray[i] = new List<ULongEnum>() { GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd)};

		internalULongEnumArray = new ULongEnum[3] {GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd) };

		internalULongEnumMatrix = new ULongEnum[1, 3] { { GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd) } };

		internalULongEnumList = new List<ULongEnum>() { GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd),
			GetRandomEnumValue<ULongEnum>(rnd)};

		internalULongEnumStack = new Stack<ULongEnum>();
		internalULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));
		internalULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));
		internalULongEnumStack.Push(GetRandomEnumValue<ULongEnum>(rnd));

		internalULongEnumQueue = new Queue<ULongEnum>();
		internalULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));
		internalULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));
		internalULongEnumQueue.Enqueue(GetRandomEnumValue<ULongEnum>(rnd));

		internalULongEnumDict = new Dictionary<string, ULongEnum>()
		{
			{ "key1", GetRandomEnumValue<ULongEnum>(rnd)},
			{ "key2", GetRandomEnumValue<ULongEnum>(rnd)},
			{ "key3", GetRandomEnumValue<ULongEnum>(rnd)}
		};

		internalULongEnumListArray = new List<ULongEnum>[3];
		for (int i = 0; i < 3; i++)
			internalULongEnumListArray[i] = new List<ULongEnum>() { GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd),
				GetRandomEnumValue<ULongEnum>(rnd)};
		#endregion
		#endregion

		#region CustomStruct
		publicTestStruct = new TestStruct(rnd);
		privateTestStruct = new TestStruct(rnd);
		protectedTestStruct = new TestStruct(rnd);
		internalTestStruct = new TestStruct(rnd);
		#endregion

		#region CustomStructCollections
		publicTestStructArray = new TestStruct[3] { new TestStruct(rnd),
			new TestStruct(rnd),
			new TestStruct(rnd) };

		publicTestStructMatrix = new TestStruct[1, 3] { { new TestStruct(rnd),
				new TestStruct(rnd),
				new TestStruct(rnd) } };

		publicTestStructList = new List<TestStruct>() { new TestStruct(rnd),
			new TestStruct(rnd),
			new TestStruct(rnd) };

		publicTestStructStack = new Stack<TestStruct>();
		publicTestStructStack.Push(new TestStruct(rnd));
		publicTestStructStack.Push(new TestStruct(rnd));
		publicTestStructStack.Push(new TestStruct(rnd));

		publicTestStructQueue = new Queue<TestStruct>();
		publicTestStructQueue.Enqueue(new TestStruct(rnd));
		publicTestStructQueue.Enqueue(new TestStruct(rnd));
		publicTestStructQueue.Enqueue(new TestStruct(rnd));

		publicTestStructDict = new Dictionary<string, TestStruct>()
		{
			{ "key1", new TestStruct(rnd) },
			{ "key2", new TestStruct(rnd) },
			{ "key3", new TestStruct(rnd) }
		};

		publicTestStructListArray = new List<TestStruct>[3];
		for (int i = 0; i < 3; i++)
			publicTestStructListArray[i] = new List<TestStruct>() { new TestStruct(rnd),
				new TestStruct(rnd),
				new TestStruct(rnd) };

		privateTestStructArray = new TestStruct[3] { new TestStruct(rnd),
			new TestStruct(rnd),
			new TestStruct(rnd) };

		privateTestStructMatrix = new TestStruct[1, 3] { { new TestStruct(rnd),
				new TestStruct(rnd),
				new TestStruct(rnd) } };

		privateTestStructList = new List<TestStruct>() { new TestStruct(rnd),
			new TestStruct(rnd),
			new TestStruct(rnd) };

		privateTestStructStack = new Stack<TestStruct>();
		privateTestStructStack.Push(new TestStruct(rnd));
		privateTestStructStack.Push(new TestStruct(rnd));
		privateTestStructStack.Push(new TestStruct(rnd));

		privateTestStructQueue = new Queue<TestStruct>();
		privateTestStructQueue.Enqueue(new TestStruct(rnd));
		privateTestStructQueue.Enqueue(new TestStruct(rnd));
		privateTestStructQueue.Enqueue(new TestStruct(rnd));

		privateTestStructDict = new Dictionary<string, TestStruct>()
		{
			{ "key1", new TestStruct(rnd) },
			{ "key2", new TestStruct(rnd) },
			{ "key3", new TestStruct(rnd) }
		};

		privateTestStructListArray = new List<TestStruct>[3];
		for (int i = 0; i < 3; i++)
			privateTestStructListArray[i] = new List<TestStruct>() { new TestStruct(rnd),
				new TestStruct(rnd),
				new TestStruct(rnd) };

		protectedTestStructArray = new TestStruct[3] { new TestStruct(rnd),
			new TestStruct(rnd),
			new TestStruct(rnd) };

		protectedTestStructMatrix = new TestStruct[1, 3] { { new TestStruct(rnd),
				new TestStruct(rnd),
				new TestStruct(rnd) } };

		protectedTestStructList = new List<TestStruct>() { new TestStruct(rnd),
			new TestStruct(rnd),
			new TestStruct(rnd) };

		protectedTestStructStack = new Stack<TestStruct>();
		protectedTestStructStack.Push(new TestStruct(rnd));
		protectedTestStructStack.Push(new TestStruct(rnd));
		protectedTestStructStack.Push(new TestStruct(rnd));

		protectedTestStructQueue = new Queue<TestStruct>();
		protectedTestStructQueue.Enqueue(new TestStruct(rnd));
		protectedTestStructQueue.Enqueue(new TestStruct(rnd));
		protectedTestStructQueue.Enqueue(new TestStruct(rnd));

		protectedTestStructDict = new Dictionary<string, TestStruct>()
		{
			{ "key1", new TestStruct(rnd) },
			{ "key2", new TestStruct(rnd) },
			{ "key3", new TestStruct(rnd) }
		};

		protectedTestStructListArray = new List<TestStruct>[3];
		for (int i = 0; i < 3; i++)
			protectedTestStructListArray[i] = new List<TestStruct>() { new TestStruct(rnd),
				new TestStruct(rnd),
				new TestStruct(rnd) };

		internalTestStructArray = new TestStruct[3] { new TestStruct(rnd),
			new TestStruct(rnd),
			new TestStruct(rnd) };

		internalTestStructMatrix = new TestStruct[1, 3] { { new TestStruct(rnd),
				new TestStruct(rnd),
				new TestStruct(rnd) } };

		internalTestStructList = new List<TestStruct>() { new TestStruct(rnd),
			new TestStruct(rnd),
			new TestStruct(rnd) };

		internalTestStructStack = new Stack<TestStruct>();
		internalTestStructStack.Push(new TestStruct(rnd));
		internalTestStructStack.Push(new TestStruct(rnd));
		internalTestStructStack.Push(new TestStruct(rnd));

		internalTestStructQueue = new Queue<TestStruct>();
		internalTestStructQueue.Enqueue(new TestStruct(rnd));
		internalTestStructQueue.Enqueue(new TestStruct(rnd));
		internalTestStructQueue.Enqueue(new TestStruct(rnd));

		internalTestStructDict = new Dictionary<string, TestStruct>()
		{
			{ "key1", new TestStruct(rnd) },
			{ "key2", new TestStruct(rnd) },
			{ "key3", new TestStruct(rnd) }
		};

		internalTestStructListArray = new List<TestStruct>[3];
		for (int i = 0; i < 3; i++)
			internalTestStructListArray[i] = new List<TestStruct>() { new TestStruct(rnd),
				new TestStruct(rnd),
				new TestStruct(rnd) };
		#endregion
	}
}

public abstract class BaseTestClass
{
	#region C#Fields
	public bool basePublicBool;
	private bool basePrivateBool;
	protected bool baseProtectedBool;
	internal bool baseInternalBool;

	public byte basePublicByte;
	private byte basePrivateByte;
	protected byte baseProtectedByte;
	internal byte baseInternalByte;

	public sbyte basePublicSByte;
	private sbyte basePrivateSByte;
	protected sbyte baseProtectedSByte;
	internal sbyte baseInternalSByte;

	public short basePublicShort;
	private short basePrivateShort;
	protected short baseProtectedShort;
	internal short baseInternalShort;

	public ushort basePublicUShort;
	private ushort basePrivateUShort;
	protected ushort baseProtectedUShort;
	internal ushort baseInternalUShort;

	public int basePublicInt;
	private int basePrivateInt;
	protected int baseProtectedInt;
	internal int baseInternalInt;

	public uint basePublicUInt;
	private uint basePrivateUInt;
	protected uint baseProtectedUInt;
	internal uint baseInternalUInt;

	public long basePublicLong;
	private long basePrivateLong;
	protected long baseProtectedLong;
	internal long baseInternalLong;

	public ulong basePublicULong;
	private ulong basePrivateULong;
	protected ulong baseProtectedULong;
	internal ulong baseInternalULong;

	public float basePublicFloat;
	private float basePrivateFloat;
	protected float baseProtectedFloat;
	internal float baseInternalFloat;

	public double basePublicDouble;
	private double basePrivateDouble;
	protected double baseProtectedDouble;
	internal double baseInternalDouble;

	public decimal basePublicDecimal;
	private decimal basePrivateDecimal;
	protected decimal baseProtectedDecimal;
	internal decimal baseInternalDecimal;

	public char basePublicChar;
	private char basePrivateChar;
	protected char baseProtectedChar;
	internal char baseInternalChar;

	public string basePublicString;
	private string basePrivateString;
	protected string baseProtectedString;
	internal string baseInternalString;
	#endregion

	#region UnityFields
	public Vector2 basePublicVector2;
	private Vector2 basePrivateVector2;
	protected Vector2 baseProtectedVector2;
	internal Vector2 baseInternalVector2;

	public Vector3 basePublicVector3;
	private Vector3 basePrivateVector3;
	protected Vector3 baseProtectedVector3;
	internal Vector3 baseInternalVector3;

	public Vector2Int basePublicVector2Int;
	private Vector2Int basePrivateVector2Int;
	protected Vector2Int baseProtectedVector2Int;
	internal Vector2Int baseInternalVector2Int;

	public Vector3Int basePublicVector3Int;
	private Vector3Int basePrivateVector3Int;
	protected Vector3Int baseProtectedVector3Int;
	internal Vector3Int baseInternalVector3Int;

	public Color basePublicColor;
	private Color basePrivateColor;
	protected Color baseProtectedColor;
	internal Color baseInternalColor;

	public Bounds basePublicBounds;
	private Bounds basePrivateBounds;
	protected Bounds baseProtectedBounds;
	internal Bounds baseInternalBounds;

	public BoundsInt basePublicBoundsInt;
	private BoundsInt basePrivateBoundsInt;
	protected BoundsInt baseProtectedBoundsInt;
	internal BoundsInt baseInternalBoundsInt;
	#endregion

	#region Enums
	public ShortEnum basePublicShortEnum;
	private ShortEnum basePrivateShortEnum;
	protected ShortEnum baseProtectedShortEnum;
	internal ShortEnum baseInternalShortEnum;

	public UShortEnum basePublicUShortEnum;
	private UShortEnum basePrivateUShortEnum;
	protected UShortEnum baseProtectedUShortEnum;
	internal UShortEnum baseInternalUShortEnum;

	public IntEnum basePublicIntEnum;
	private IntEnum basePrivateIntEnum;
	protected IntEnum baseProtectedIntEnum;
	internal IntEnum baseInternalIntEnum;

	public UIntEnum basePublicUIntEnum;
	private UIntEnum basePrivateUIntEnum;
	protected UIntEnum baseProtectedUIntEnum;
	internal UIntEnum baseInternalUIntEnum;

	public LongEnum basePublicLongEnum;
	private LongEnum basePrivateLongEnum;
	protected LongEnum baseProtectedLongEnum;
	internal LongEnum baseInternalLongEnum;

	public ULongEnum basePublicULongEnum;
	private ULongEnum basePrivateULongEnum;
	protected ULongEnum baseProtectedULongEnum;
	internal ULongEnum baseInternalULongEnum;
	#endregion

	#region CustomStruct
	public TestStruct basePublicTestStruct;
	private TestStruct basePrivateTestStruct;
	protected TestStruct baseProtectedTestStruct;
	internal TestStruct baseInternalTestStruct;
	#endregion

	public BaseTestClass()
	{
		Random rnd = new Random();

		#region C#Fields
		basePublicBool = rnd.Next(2) == 0;
		basePrivateBool = rnd.Next(2) == 0;
		baseProtectedBool = rnd.Next(2) == 0;
		baseInternalBool = rnd.Next(2) == 0;

		basePublicByte = (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1);
		basePrivateByte = (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1);
		baseProtectedByte = (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1);
		baseInternalByte = (byte)rnd.Next(byte.MinValue, byte.MaxValue + 1);

		basePublicSByte = (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1);
		basePrivateSByte = (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1);
		baseProtectedSByte = (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1);
		baseInternalSByte = (sbyte)rnd.Next(sbyte.MinValue, sbyte.MaxValue + 1);

		basePublicShort = (short)rnd.Next(short.MinValue, short.MaxValue + 1);
		basePrivateShort = (short)rnd.Next(short.MinValue, short.MaxValue + 1);
		baseProtectedShort = (short)rnd.Next(short.MinValue, short.MaxValue + 1);
		baseInternalShort = (short)rnd.Next(short.MinValue, short.MaxValue + 1);

		basePublicUShort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1);
		basePrivateUShort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1);
		baseProtectedUShort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1);
		baseInternalUShort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue + 1);

		basePublicInt = rnd.Next();
		basePrivateInt = rnd.Next();
		baseProtectedInt = rnd.Next();
		baseInternalInt = rnd.Next();

		basePublicUInt = (uint)rnd.Next();
		basePrivateUInt = (uint)rnd.Next();
		baseProtectedUInt = (uint)rnd.Next();
		baseInternalUInt = (uint)rnd.Next();

		basePublicLong = ((long)rnd.Next() << 32) | (uint)rnd.Next();
		basePrivateLong = ((long)rnd.Next() << 32) | (uint)rnd.Next();
		baseProtectedLong = ((long)rnd.Next() << 32) | (uint)rnd.Next();
		baseInternalLong = ((long)rnd.Next() << 32) | (uint)rnd.Next();

		basePublicULong = (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next());
		basePrivateULong = (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next());
		baseProtectedULong = (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next());
		baseInternalULong = (ulong)(((long)rnd.Next() << 32) | (uint)rnd.Next());

		basePublicFloat = (float)(rnd.NextDouble() * 1000);
		basePrivateFloat = (float)(rnd.NextDouble() * 1000);
		baseProtectedFloat = (float)(rnd.NextDouble() * 1000);
		baseInternalFloat = (float)(rnd.NextDouble() * 1000);

		basePublicDouble = rnd.NextDouble() * 1000;
		basePrivateDouble = rnd.NextDouble() * 1000;
		baseProtectedDouble = rnd.NextDouble() * 1000;
		baseInternalDouble = rnd.NextDouble() * 1000;

		basePublicDecimal = (decimal)(rnd.NextDouble() * 1000);
		basePrivateDecimal = (decimal)(rnd.NextDouble() * 1000);
		baseProtectedDecimal = (decimal)(rnd.NextDouble() * 1000);
		baseInternalDecimal = (decimal)(rnd.NextDouble() * 1000);

		basePublicChar = (char)rnd.Next(65, 91);
		basePrivateChar = (char)rnd.Next(65, 91);
		baseProtectedChar = (char)rnd.Next(65, 91);
		baseInternalChar = (char)rnd.Next(65, 91);

		basePublicString = RandomString(rnd, 8);
		basePrivateString = RandomString(rnd, 8);
		baseProtectedString = RandomString(rnd, 8);
		baseInternalString = RandomString(rnd, 8);
		#endregion

		#region UnityFields
		basePublicVector2 = new Vector2((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		basePrivateVector2 = new Vector2((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		baseProtectedVector2 = new Vector2((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		baseInternalVector2 = new Vector2((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));

		basePublicVector3 = new Vector3((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		basePrivateVector3 = new Vector3((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		baseProtectedVector3 = new Vector3((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));
		baseInternalVector3 = new Vector3((float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000),
			(float)(rnd.NextDouble() * 1000));

		basePublicVector2Int = new Vector2Int(rnd.Next(), rnd.Next());
		basePrivateVector2Int = new Vector2Int(rnd.Next(), rnd.Next());
		baseProtectedVector2Int = new Vector2Int(rnd.Next(), rnd.Next());
		baseInternalVector2Int = new Vector2Int(rnd.Next(), rnd.Next());

		basePublicVector3Int = new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next());
		basePrivateVector3Int = new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next());
		baseProtectedVector3Int = new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next());
		baseInternalVector3Int = new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next());

		basePublicColor = new Color((float)rnd.NextDouble(),
								(float)rnd.NextDouble(),
								(float)rnd.NextDouble(),
								(float)rnd.NextDouble());
		basePrivateColor = new Color((float)rnd.NextDouble(),
								 (float)rnd.NextDouble(),
								 (float)rnd.NextDouble(),
								 (float)rnd.NextDouble());
		baseProtectedColor = new Color((float)rnd.NextDouble(),
								   (float)rnd.NextDouble(),
								   (float)rnd.NextDouble(),
								   (float)rnd.NextDouble());
		baseInternalColor = new Color((float)rnd.NextDouble(),
								  (float)rnd.NextDouble(),
								  (float)rnd.NextDouble(),
								  (float)rnd.NextDouble());

		basePublicBounds = new Bounds(
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)));
		basePrivateBounds = new Bounds(
			new Vector3((float)(rnd.NextDouble() * 1000),
						   (float)(rnd.NextDouble() * 1000),
						   (float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)));
		baseProtectedBounds = new Bounds(
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)),
			new Vector3((float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000),
						(float)(rnd.NextDouble() * 1000)));
		baseInternalBounds = new Bounds(new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)), new Vector3((float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000), (float)(rnd.NextDouble() * 1000)));

		basePublicBoundsInt = new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()));
		basePrivateBoundsInt = new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()));
		baseProtectedBoundsInt = new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()));
		baseInternalBoundsInt = new BoundsInt(new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()), new Vector3Int(rnd.Next(), rnd.Next(), rnd.Next()));
		#endregion

		#region Enums
		basePublicShortEnum = GetRandomEnumValue<ShortEnum>(rnd);
		basePrivateShortEnum = GetRandomEnumValue<ShortEnum>(rnd);
		baseProtectedShortEnum = GetRandomEnumValue<ShortEnum>(rnd);
		baseInternalShortEnum = GetRandomEnumValue<ShortEnum>(rnd);

		basePublicUShortEnum = GetRandomEnumValue<UShortEnum>(rnd);
		basePrivateUShortEnum = GetRandomEnumValue<UShortEnum>(rnd);
		baseProtectedUShortEnum = GetRandomEnumValue<UShortEnum>(rnd);
		baseInternalUShortEnum = GetRandomEnumValue<UShortEnum>(rnd);

		basePublicIntEnum = GetRandomEnumValue<IntEnum>(rnd);
		basePrivateIntEnum = GetRandomEnumValue<IntEnum>(rnd);
		baseProtectedIntEnum = GetRandomEnumValue<IntEnum>(rnd);
		baseInternalIntEnum = GetRandomEnumValue<IntEnum>(rnd);

		basePublicUIntEnum = GetRandomEnumValue<UIntEnum>(rnd);
		basePrivateUIntEnum = GetRandomEnumValue<UIntEnum>(rnd);
		baseProtectedUIntEnum = GetRandomEnumValue<UIntEnum>(rnd);
		baseInternalUIntEnum = GetRandomEnumValue<UIntEnum>(rnd);

		basePublicLongEnum = GetRandomEnumValue<LongEnum>(rnd);
		basePrivateLongEnum = GetRandomEnumValue<LongEnum>(rnd);
		baseProtectedLongEnum = GetRandomEnumValue<LongEnum>(rnd);
		baseInternalLongEnum = GetRandomEnumValue<LongEnum>(rnd);

		basePublicULongEnum = GetRandomEnumValue<ULongEnum>(rnd);
		basePrivateULongEnum = GetRandomEnumValue<ULongEnum>(rnd);
		baseProtectedULongEnum = GetRandomEnumValue<ULongEnum>(rnd);
		baseInternalULongEnum = GetRandomEnumValue<ULongEnum>(rnd);
		#endregion

		#region CustomStruct
		basePublicTestStruct = new TestStruct(rnd);
		basePrivateTestStruct = new TestStruct(rnd);
		baseProtectedTestStruct = new TestStruct(rnd);
		baseInternalTestStruct = new TestStruct(rnd);
		#endregion
	}

	protected string RandomString(Random rnd, int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		char[] str = new char[length];
		for (int i = 0; i < length; i++)
			str[i] = chars[rnd.Next(chars.Length)];
		return new string(str);
	}

	protected EnumType GetRandomEnumValue<EnumType>(Random rnd) where EnumType : Enum
	{
		Array values = Enum.GetValues(typeof(EnumType));
		return (EnumType)values.GetValue(rnd.Next(values.Length));
	}
}

public enum ShortEnum : Int16
{
	a, b, c
}

public enum UShortEnum : UInt16
{
	a, b, c
}

public enum IntEnum : Int32
{
	a, b, c
}

public enum UIntEnum : UInt32
{
	a, b, c
}

public enum LongEnum : Int64
{
	a, b, c
}

public enum ULongEnum : UInt64
{
	a, b, c
}

public struct TestStruct
{
	public int a;
	public bool b;
	public TestStruct2 testStruct2;
	public List<TestClass2> testClass2s;

	public TestStruct(Random rnd)
	{
		this.a = rnd.Next();
		this.b = rnd.Next(2) == 0;
		this.testStruct2 = new TestStruct2(rnd);
		testClass2s = new List<TestClass2>();
		testClass2s.Add(new TestClass2(rnd));
		testClass2s.Add(new TestClass2(rnd));
		testClass2s.Add(new TestClass2(rnd));
	}
}

public struct TestStruct2
{
	public int a;
	public bool b;

	public TestStruct2(Random rnd)
	{
		this.a = rnd.Next();
		this.b = rnd.Next(2) == 0;
	}
}

public class TestClass2
{
	public int a;
	private bool b;

	public TestClass2(Random rnd)
	{
		this.a = rnd.Next();
		this.b = rnd.Next(2) == 0;
	}
}