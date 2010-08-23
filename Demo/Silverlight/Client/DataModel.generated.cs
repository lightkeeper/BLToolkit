﻿//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by BLToolkit template for T4.
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.ServiceModel;

using BLToolkit.Data;
using BLToolkit.Data.Linq;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;
using BLToolkit.ServiceModel;

namespace Client
{
	public partial class DataModel : ServiceModelDataContext
	{
		public Table<BinaryData>    BinaryData    { get { return this.GetTable<BinaryData>();    } }
		public Table<Child>         Child         { get { return this.GetTable<Child>();         } }
		public Table<DataTypes>     DataTypes     { get { return this.GetTable<DataTypes>();     } }
		public Table<DataTypeTest>  DataTypeTest  { get { return this.GetTable<DataTypeTest>();  } }
		public Table<Doctor>        Doctor        { get { return this.GetTable<Doctor>();        } }
		public Table<GrandChild>    GrandChild    { get { return this.GetTable<GrandChild>();    } }
		public Table<LinqDataTypes> LinqDataTypes { get { return this.GetTable<LinqDataTypes>(); } }
		public Table<Parent>        Parent        { get { return this.GetTable<Parent>();        } }
		public Table<Patient>       Patient       { get { return this.GetTable<Patient>();       } }
		public Table<Person>        Person        { get { return this.GetTable<Person>();        } }
	}

	[TableName(Name="BinaryData")]
	public partial class BinaryData
	{
		[Identity, PrimaryKey(1)] public int    BinaryDataID { get; set; }
		                          public byte[] Stamp        { get; set; }
		                          public byte[] Data         { get; set; }
	}

	[TableName(Name="Child")]
	public partial class Child
	{
		[Nullable] public int? ParentID { get; set; }
		[Nullable] public int? ChildID  { get; set; }
	}

	[TableName(Name="DataTypes")]
	public partial class DataTypes
	{
		[Nullable] public int?     ID         { get; set; }
		[Nullable] public decimal? MoneyValue { get; set; }
	}

	[TableName(Name="DataTypeTest")]
	public partial class DataTypeTest
	{
		[Identity, PrimaryKey(1)] public int       DataTypeID { get; set; }
		[Nullable               ] public byte[]    Binary_    { get; set; }
		[Nullable               ] public bool?     Boolean_   { get; set; }
		[Nullable               ] public byte?     Byte_      { get; set; }
		[Nullable               ] public byte[]    Bytes_     { get; set; }
		[Nullable               ] public char?     Char_      { get; set; }
		[Nullable               ] public DateTime? DateTime_  { get; set; }
		[Nullable               ] public decimal?  Decimal_   { get; set; }
		[Nullable               ] public double?   Double_    { get; set; }
		[Nullable               ] public Guid?     Guid_      { get; set; }
		[Nullable               ] public short?    Int16_     { get; set; }
		[Nullable               ] public int?      Int32_     { get; set; }
		[Nullable               ] public long?     Int64_     { get; set; }
		[Nullable               ] public decimal?  Money_     { get; set; }
		[Nullable               ] public byte?     SByte_     { get; set; }
		[Nullable               ] public float?    Single_    { get; set; }
		[Nullable               ] public byte[]    Stream_    { get; set; }
		[Nullable               ] public string    String_    { get; set; }
		[Nullable               ] public short?    UInt16_    { get; set; }
		[Nullable               ] public int?      UInt32_    { get; set; }
		[Nullable               ] public long?     UInt64_    { get; set; }
		[Nullable               ] public string    Xml_       { get; set; }
	}

	[TableName(Name="Doctor")]
	public partial class Doctor
	{
		[PrimaryKey(1)] public int    PersonID { get; set; }
		                public string Taxonomy { get; set; }

		// FK_Doctor_Person
		[Association(ThisKey="PersonID", OtherKey="PersonID")]
		public Person Person { get; set; }
	}

	[TableName(Name="GrandChild")]
	public partial class GrandChild
	{
		[Nullable] public int? ParentID     { get; set; }
		[Nullable] public int? ChildID      { get; set; }
		[Nullable] public int? GrandChildID { get; set; }
	}

	[TableName(Name="LinqDataTypes")]
	public partial class LinqDataTypes
	{
		[Nullable] public int?      ID            { get; set; }
		[Nullable] public decimal?  MoneyValue    { get; set; }
		[Nullable] public DateTime? DateTimeValue { get; set; }
		[Nullable] public bool?     BoolValue     { get; set; }
		[Nullable] public Guid?     GuidValue     { get; set; }
		[Nullable] public byte[]    BinaryValue   { get; set; }
		[Nullable] public short?    SmallIntValue { get; set; }
	}

	[TableName(Name="Parent")]
	public partial class Parent
	{
		[Nullable] public int? ParentID { get; set; }
		[Nullable] public int? Value1   { get; set; }
	}

	[TableName(Name="Patient")]
	public partial class Patient
	{
		[PrimaryKey(1)] public int    PersonID  { get; set; }
		                public string Diagnosis { get; set; }

		// FK_Patient_Person
		[Association(ThisKey="PersonID", OtherKey="PersonID")]
		public Person Person { get; set; }
	}

	[TableName(Name="Person")]
	public partial class Person
	{
		[Identity, PrimaryKey(1)] public int    PersonID   { get; set; }
		                          public string FirstName  { get; set; }
		                          public string LastName   { get; set; }
		[Nullable               ] public string MiddleName { get; set; }
		                          public char   Gender     { get; set; }

		// FK_Doctor_Person_BackReference
		[Association(ThisKey="PersonID", OtherKey="PersonID")]
		public Doctor Doctor { get; set; }

		// FK_Patient_Person_BackReference
		[Association(ThisKey="PersonID", OtherKey="PersonID")]
		public Patient Patient { get; set; }
	}
}

namespace Client
{
	public partial class DataModel
	{
		public DataModel() : base(
			new BasicHttpBinding(BasicHttpSecurityMode.None)
			{
				MaxReceivedMessageSize = 10000000,
				//MaxBufferPoolSize      = 10000000,
				MaxBufferSize          = 10000000,
				CloseTimeout           = new TimeSpan(00, 01, 00),
				OpenTimeout            = new TimeSpan(00, 01, 00),
				ReceiveTimeout         = new TimeSpan(00, 10, 00),
				SendTimeout            = new TimeSpan(00, 10, 00),
			},
			new EndpointAddress("http://localhost:31020/TestLinqService.svc"))
		{
		}
	}
}
