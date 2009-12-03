﻿using System;
using System.Linq;
using BLToolkit.Data.DataProvider;
using NUnit.Framework;

namespace Data.Linq
{
	using Model;

	[TestFixture]
	public class Association : TestBase
	{
		//[Test]
		public void Test1()
		{
			var expected = from p in Parent select p.Children;
			ForEachProvider(db => AreEqual(expected, from p in db.Parent select p.Children));
		}

		//[Test]
		public void Test2()
		{
			var expected = from p in Parent select p.Children.Select(c => c.ChildID);
			ForEachProvider(db => AreEqual(expected, from p in db.Parent select p.Children.Select(c => c.ChildID)));
		}

		[Test]
		public void Test3()
		{
			var expected = from ch in Child where ch.ParentID == 1 select new { ch, ch.Parent };
			ForEachProvider(db => AreEqual(expected, from ch in db.Child where ch.ParentID == 1 select new { ch, ch.Parent }));
		}

		//[Test]
		public void Test4()
		{
			var expected =
				from ch in Child
				orderby ch.ChildID
				select Parent.Where(p => p.ParentID == ch.Parent.ParentID).Select(p => p);

			ForEachProvider(db =>
			{
				var q =
					from ch in db.Child
					orderby ch.ChildID
					select db.Parent.Where(p => p.ParentID == ch.Parent.ParentID).Select(p => p);

				var list  = q.ToList();
				var elist = expected.ToList();

				Assert.AreEqual(elist.Count(), list.Count);

				for (var i = 0; i < list.Count; i++)
					AreEqual(elist[i], list[i]);
			});
		}

		[Test]
		public void Test5()
		{
			var expected =
				from p  in Parent
				from ch in p.Children
				where ch.ParentID < 4 || ch.ParentID >= 4
				select new { p.ParentID, ch.ChildID };

			ForEachProvider(db => AreEqual(expected,
				from p  in db.Parent
				from ch in p.Children
				where ch.ParentID < 4 || ch.ParentID >= 4
				select new { p.ParentID, ch.ChildID }));
		}

		[Test]
		public void Test6()
		{
			var expected =
				from p  in Parent
				from ch in p.Children
				where p.ParentID < 4 || p.ParentID >= 4
				select new { p.ParentID };

			ForEachProvider(db => AreEqual(expected,
				from p  in db.Parent
				from ch in p.Children
				where p.ParentID < 4 || p.ParentID >= 4
				select new { p.ParentID }));
		}

		[Test]
		public void Test7()
		{
			var expected =
				from p  in Parent
				from ch in p.Children
				where p.ParentID < 4 || p.ParentID >= 4
				select new { p.ParentID, ch.ChildID };

			ForEachProvider(db => AreEqual(expected,
				from p  in db.Parent
				from ch in p.Children
				where p.ParentID < 4 || p.ParentID >= 4
				select new { p.ParentID, ch.ChildID }));
		}

		[Test]
		public void SelectMany1()
		{
			var expected = Parent.SelectMany(p => p.Children.Select(ch => p));
			ForEachProvider(db => AreEqual(expected, db.Parent.SelectMany(p => p.Children.Select(ch => p))));
		}

		[Test]
		public void SelectMany2()
		{
			var expected = Parent.SelectMany(p => Child.Select(ch => p));
			ForEachProvider(db => AreEqual(expected, db.Parent.SelectMany(p => db.Child.Select(ch => p))));
		}

		//[Test]
		public void SelectMany3()
		{
			var expected =
				Child
					.GroupBy(ch => ch.Parent)
					.Where(g => g.Count() > 2)
					.SelectMany(
					g => 
						g.Select(ch => ch.Parent));

			ForEachProvider(db => AreEqual(expected,
				db.Child
					.GroupBy(ch => ch.Parent)
					.Where(g => g.Count() > 2)
					.SelectMany(g => g.Select(ch => ch.Parent))));
		}

		//[Test]
		public void SelectMany4()
		{
			var expected =
				Child
					.GroupBy(ch => ch.Parent)
					.Where(g => g.Count() > 2)
					.SelectMany(g => g.Select(ch => ch.Parent.ParentID));

			ForEachProvider(db => AreEqual(expected,
				db.Child
					.GroupBy(ch => ch.Parent)
					.Where(g => g.Count() > 2)
					.SelectMany(g => g.Select(ch => ch.Parent.ParentID))));
		}

		[Test]
		public void SelectMany5()
		{
			var expected = Parent.SelectMany(p => p.Children.Select(ch => p.ParentID));
			ForEachProvider(db => AreEqual(expected, db.Parent.SelectMany(p => p.Children.Select(ch => p.ParentID))));
		}

		[Test]
		public void GroupBy1()
		{
			var expected = from ch in Child group ch by ch.Parent into g select g.Key;
			ForEachProvider(db => AreEqual(expected, from ch in db.Child group ch by ch.Parent into g select g.Key));
		}

		[Test]
		public void GroupBy2()
		{
			var expected = (from ch in Child group ch by ch.Parent1).ToList().Select(g => g.Key);
			ForEachProvider(db => AreEqual(expected, (from ch in db.Child group ch by ch.Parent1).ToList().Select(g => g.Key)));
		}

		[Test]
		public void Count1()
		{
			var expected = from p in Parent where p.Children.Count > 2 select p;
			ForEachProvider(new[] { ProviderName.SqlCe }, db => AreEqual(expected, from p in db.Parent where p.Children.Count > 2 select p));
		}

		[Test]
		public void EqualsNull1()
		{
			var expected =
				from   employee in Employee
				where  employee.ReportsToEmployee != null
				select employee.EmployeeID;

			using (var db = new NorthwindDB()) AreEqual(expected, 
				from   employee in db.Employee
				where  employee.ReportsToEmployee != null
				select employee.EmployeeID);
		}

		[Test]
		public void EqualsNull2()
		{
			var expected =
				from   employee in Employee
				where  employee.ReportsToEmployee != null
				select employee;

			using (var db = new NorthwindDB()) AreEqual(expected, 
				from   employee in db.Employee
				where  employee.ReportsToEmployee != null
				select employee);
		}

		[Test]
		public void EqualsNull3()
		{
			var expected =
				from employee in Employee
				where employee.ReportsToEmployee != null
				select new { employee.ReportsToEmployee, employee };

			using (var db = new NorthwindDB()) AreEqual(expected, 
				from   employee in db.Employee
				where  employee.ReportsToEmployee != null
				select new { employee.ReportsToEmployee, employee });
		}
	}
}
