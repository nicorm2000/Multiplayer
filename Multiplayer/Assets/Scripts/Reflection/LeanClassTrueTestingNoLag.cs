using Net;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class TesterBase
{
	System.Random rnd = new System.Random();
	public TesterBase()
    {
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
	}

	[SerializeField, NetVariable(1, NETAUTHORITY.CLIENT)] public bool basePublicBool;
	/*[SerializeField, NetVariable(2, NETAUTHORITY.CLIENT)]*/
	private bool basePrivateBool;
	/*[SerializeField, NetVariable(3, NETAUTHORITY.CLIENT)]*/
	protected bool baseProtectedBool;
	/*[SerializeField, NetVariable(4, NETAUTHORITY.CLIENT)]*/
	internal bool baseInternalBool;
	/*[SerializeField, NetVariable(5, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public byte basePublicByte;
	[SerializeField, NetVariable(6, NETAUTHORITY.CLIENT)] private byte basePrivateByte;
	/*[SerializeField, NetVariable(7, NETAUTHORITY.CLIENT)]*/
	protected byte baseProtectedByte;
	/*[SerializeField, NetVariable(8, NETAUTHORITY.CLIENT)]*/
	internal byte baseInternalByte;
	/*[SerializeField, NetVariable(9, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public sbyte basePublicSByte;
	/*[SerializeField, NetVariable(10, NETAUTHORITY.CLIENT)]*/
	private sbyte basePrivateSByte;
	[SerializeField, NetVariable(11, NETAUTHORITY.CLIENT)] protected sbyte baseProtectedSByte;
	/*[SerializeField, NetVariable(12, NETAUTHORITY.CLIENT)]*/
	internal sbyte baseInternalSByte;
	/*[SerializeField, NetVariable(13, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public short basePublicShort;
	/*[SerializeField, NetVariable(14, NETAUTHORITY.CLIENT)]*/
	private short basePrivateShort;
	/*[SerializeField, NetVariable(15, NETAUTHORITY.CLIENT)]*/
	protected short baseProtectedShort;
	[SerializeField, NetVariable(16, NETAUTHORITY.CLIENT)] internal short baseInternalShort;
	[SerializeField, NetVariable(17, NETAUTHORITY.CLIENT)] public ushort basePublicUShort;
	/*[SerializeField, NetVariable(18, NETAUTHORITY.CLIENT)]*/
	private ushort basePrivateUShort;
	/*[SerializeField, NetVariable(19, NETAUTHORITY.CLIENT)]*/
	protected ushort baseProtectedUShort;
	/*[SerializeField, NetVariable(20, NETAUTHORITY.CLIENT)]*/
	internal ushort baseInternalUShort;
	/*[SerializeField, NetVariable(21, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public int basePublicInt;
	[SerializeField, NetVariable(22, NETAUTHORITY.CLIENT)] private int basePrivateInt;
	/*[SerializeField, NetVariable(23, NETAUTHORITY.CLIENT)]*/
	protected int baseProtectedInt;
	/*[SerializeField, NetVariable(24, NETAUTHORITY.CLIENT)]*/
	internal int baseInternalInt;
	/*[SerializeField, NetVariable(25, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public uint basePublicUInt;
	/*[SerializeField, NetVariable(26, NETAUTHORITY.CLIENT)]*/
	private uint basePrivateUInt;
	[SerializeField, NetVariable(27, NETAUTHORITY.CLIENT)] protected uint baseProtectedUInt;
	/*[SerializeField, NetVariable(28, NETAUTHORITY.CLIENT)]*/
	internal uint baseInternalUInt;
	/*[SerializeField, NetVariable(29, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public long basePublicLong;
	/*[SerializeField, NetVariable(30, NETAUTHORITY.CLIENT)]*/
	private long basePrivateLong;
	/*[SerializeField, NetVariable(31, NETAUTHORITY.CLIENT)]*/
	protected long baseProtectedLong;
	[SerializeField, NetVariable(32, NETAUTHORITY.CLIENT)] internal long baseInternalLong;
	/*[SerializeField, NetVariable(33, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public ulong basePublicULong;
	/*[SerializeField, NetVariable(34, NETAUTHORITY.CLIENT)]*/
	private ulong basePrivateULong;
	/*[SerializeField, NetVariable(35, NETAUTHORITY.CLIENT)]*/
	protected ulong baseProtectedULong;
	[SerializeField, NetVariable(36, NETAUTHORITY.CLIENT)]
	internal ulong baseInternalULong;
	[SerializeField, NetVariable(37, NETAUTHORITY.CLIENT)]
	public float basePublicFloat;
	/*[SerializeField, NetVariable(38, NETAUTHORITY.CLIENT)]*/
	private float basePrivateFloat;
	/*[SerializeField, NetVariable(39, NETAUTHORITY.CLIENT)]*/
	protected float baseProtectedFloat;
	/*[SerializeField, NetVariable(40, NETAUTHORITY.CLIENT)]*/
	internal float baseInternalFloat;
	/*[SerializeField, NetVariable(41, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public double basePublicDouble;
	[SerializeField, NetVariable(42, NETAUTHORITY.CLIENT)]
	private double basePrivateDouble;
	/*[SerializeField, NetVariable(43, NETAUTHORITY.CLIENT)]*/
	protected double baseProtectedDouble;
	/*[SerializeField, NetVariable(44, NETAUTHORITY.CLIENT)]*/
	internal double baseInternalDouble;
	/*[SerializeField, NetVariable(45, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public decimal basePublicDecimal;
	/*[SerializeField, NetVariable(46, NETAUTHORITY.CLIENT)]*/
	private decimal basePrivateDecimal;
	[SerializeField, NetVariable(47, NETAUTHORITY.CLIENT)]
	protected decimal baseProtectedDecimal;
	/*[SerializeField, NetVariable(48, NETAUTHORITY.CLIENT)]*/
	internal decimal baseInternalDecimal;
	/*[SerializeField, NetVariable(49, NETAUTHORITY.CLIENT)]*/
	[HideInInspector] public char basePublicChar;
	/*[SerializeField, NetVariable(50, NETAUTHORITY.CLIENT)]*/
	private char basePrivateChar;
	/*[SerializeField, NetVariable(51, NETAUTHORITY.CLIENT)]*/
	protected char baseProtectedChar;
	[SerializeField, NetVariable(52, NETAUTHORITY.CLIENT)]
	internal char baseInternalChar;
	[SerializeField, NetVariable(53, NETAUTHORITY.CLIENT)]
	public string basePublicString;
	/*[SerializeField, NetVariable(54, NETAUTHORITY.CLIENT)]*/
	private string basePrivateString;
	/*[SerializeField, NetVariable(55, NETAUTHORITY.CLIENT)]*/
	protected string baseProtectedString;
	/*[SerializeField, NetVariable(56, NETAUTHORITY.CLIENT)]*/
	internal string baseInternalString;


	protected string RandomString(System.Random rnd, int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		char[] str = new char[length];
		for (int i = 0; i < length; i++)
			str[i] = chars[rnd.Next(chars.Length)];
		return new string(str);
	}
}

[System.Serializable]
public class LeanClassTrueTestingNoLag
{
	System.Random rnd = new System.Random();
	protected string RandomString(System.Random rnd, int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		char[] str = new char[length];
		for (int i = 0; i < length; i++)
			str[i] = chars[rnd.Next(chars.Length)];
		return new string(str);
	}

	public LeanClassTrueTestingNoLag()
	{
		basePublicTestStruct = new TestStruct(rnd);
		basePrivateTestStruct = new TestStruct(rnd);
		baseProtectedTestStruct = new TestStruct(rnd);
		baseInternalTestStruct = new TestStruct(rnd);
	}
	[SerializeField, NetVariable(0, NETAUTHORITY.CLIENT)] public TestStruct basePublicTestStruct;
	[SerializeField, NetVariable(1, NETAUTHORITY.CLIENT)] private TestStruct basePrivateTestStruct;
	[SerializeField, NetVariable(2, NETAUTHORITY.CLIENT)] protected TestStruct baseProtectedTestStruct;
	[SerializeField, NetVariable(3, NETAUTHORITY.CLIENT)] internal TestStruct baseInternalTestStruct;
}