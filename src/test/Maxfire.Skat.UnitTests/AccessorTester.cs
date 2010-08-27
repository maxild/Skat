using System;
using System.Reflection;
using System.Security.Permissions;
using Maxfire.Core.Reflection;
using Maxfire.Skat.Reflection;
using Maxfire.TestCommons;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class AccessorTester
	{
		class Point
		{
			public int X { get; set; }
			public int Y { get; set; }
		}

		[Fact]
		public void PropertyAccessorWorksOnMutableType()
		{
			var p = new Point { X = 10, Y = 20 };

			var propertyX = IntrospectionOf<Point>.GetAccessorFor(x => x.X);
			var propertyY = IntrospectionOf<Point>.GetAccessorFor(x => x.Y);
			
			propertyX.GetValue(p).ShouldEqual(10);
			propertyY.GetValue(p).ShouldEqual(20);

			propertyX.SetValue(p, 12);
			propertyY.SetValue(p, 24);

			p.X.ShouldEqual(12);
			p.Y.ShouldEqual(24);
		}

		/// <summary>
		/// Compiler generated backing field, and private setter methods
		/// </summary>
		public class ImmutablePointWithPrivateSetter
		{
			public ImmutablePointWithPrivateSetter(int x, int y)
			{
				X = x;
				Y = y;
			}

			public int X { get; private set; }
			public int Y { get; private set; }
		}

		[Fact]
		public void PropertyAccessorWorksOnImmutableTypeWithPrivateSetterWhenRunningInFullTrust()
		{
			var p = new ImmutablePointWithPrivateSetter(10, 20);

			var propertyX = IntrospectionOf<ImmutablePointWithPrivateSetter>.GetAccessorFor(x => x.X);
			var propertyY = IntrospectionOf<ImmutablePointWithPrivateSetter>.GetAccessorFor(x => x.Y);

			propertyX.GetValue(p).ShouldEqual(10);
			propertyY.GetValue(p).ShouldEqual(20);

			propertyX.SetValue(p, 12);
			propertyY.SetValue(p, 24);

			p.X.ShouldEqual(12);
			p.Y.ShouldEqual(24);
		}

		public class CallingPrivateSetter : MarshalByRefObject
		{
			public int SetAndGetX(int value)
			{
				var p = new ImmutablePointWithPrivateSetter(0, 0);

				var propertyX = IntrospectionOf<ImmutablePointWithPrivateSetter>.GetAccessorFor(x => x.X);

				propertyX.SetValue(p, value);

				return propertyX.GetValue(p);
			}
		}

		// NOTE: Since .NET 2 SP1, reflection is available in partial trust, and reflection will demand MemberAccess in this case!

		[Fact, NoCoverage]
		public void PropertyAccessorThrowsMethodAccessExceptionWhenCallingPrivateSetterWhenRunningInPartialTrustWithoutMemberAccess()
		{
			Console.WriteLine("ImageRuntimeVersion: " + Assembly.GetExecutingAssembly().ImageRuntimeVersion);
			PartialTrustContext.RunTest<CallingPrivateSetter>(
				runner => Assert.Throws<MethodAccessException>(() => runner.SetAndGetX(12)),
				permissions => permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.NoFlags)));
		}

		[Fact, NoCoverage]
		public void PropertyAccessorWorksOnImmutableTypeWithPrivateSetterWhenRunningInPartialTrustWithReflectionPermissionFlagRestrictedMemberAccess()
		{
			Console.WriteLine("ImageRuntimeVersion: " + Assembly.GetExecutingAssembly().ImageRuntimeVersion);
			PartialTrustContext.RunTest<CallingPrivateSetter>(
				runner => runner.SetAndGetX(12).ShouldEqual(12),
				permissions => permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess)));
		}

		[Fact, NoCoverage]
		public void PropertyAccessorWorksOnImmutableTypeWithPrivateSetterWhenRunningInPartialTrustWithReflectionPermissionFlagMemberAccess()
		{
			Console.WriteLine("ImageRuntimeVersion: " + Assembly.GetExecutingAssembly().ImageRuntimeVersion);
			PartialTrustContext.RunTest<CallingPrivateSetter>(
				runner => runner.SetAndGetX(12).ShouldEqual(12),
				permissions => permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess)));
		}

		/// <summary>
		/// No setter methods
		/// </summary>
		public class ImmutablePointWithNoSetter
		{
			private readonly int _x;
			private readonly int _y;
			
			public ImmutablePointWithNoSetter(int x, int y)
			{
				_x = x;
				_y = y;
			}

			public int X { get { return _x; } }
			public int Y { get { return _y; } }
		}

		[Fact]
		public void UsingPropertyAccessorOnImmutableTypeWithNoSetter()
		{
			var p = new ImmutablePointWithNoSetter(10, 20);

			var propertyX = IntrospectionOf<ImmutablePointWithNoSetter>.GetAccessorFor(x => x.X);
			
			var ex = Assert.Throws<ArgumentException>(() => propertyX.SetValue(p, 12));
			ex.Message.ShouldEqual("Property set method not found.");
		}
	}
}