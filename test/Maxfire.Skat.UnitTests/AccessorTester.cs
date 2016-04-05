using System;
using System.Reflection;
using System.Security.Permissions;
using Maxfire.Skat.Reflection;
using Shouldly;
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

			propertyX.GetValue(p).ShouldBe(10);
			propertyY.GetValue(p).ShouldBe(20);

			propertyX.SetValue(p, 12);
			propertyY.SetValue(p, 24);

			p.X.ShouldBe(12);
			p.Y.ShouldBe(24);
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

			propertyX.GetValue(p).ShouldBe(10);
			propertyY.GetValue(p).ShouldBe(20);

			propertyX.SetValue(p, 12);
			propertyY.SetValue(p, 24);

			p.X.ShouldBe(12);
			p.Y.ShouldBe(24);
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

		[Fact]
		public void PropertyAccessorThrowsMethodAccessExceptionWhenCallingPrivateSetterWhenRunningInPartialTrustWithoutMemberAccess()
		{
			Console.WriteLine("ImageRuntimeVersion: " + Assembly.GetExecutingAssembly().ImageRuntimeVersion);
			PartialTrustContext.RunTest<CallingPrivateSetter>(
				runner => Assert.Throws<MethodAccessException>(() => runner.SetAndGetX(12)),
				permissions => permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.NoFlags)));
		}

		[Fact]
		public void PropertyAccessorWorksOnImmutableTypeWithPrivateSetterWhenRunningInPartialTrustWithReflectionPermissionFlagRestrictedMemberAccess()
		{
			Console.WriteLine("ImageRuntimeVersion: " + Assembly.GetExecutingAssembly().ImageRuntimeVersion);
			PartialTrustContext.RunTest<CallingPrivateSetter>(
				runner => runner.SetAndGetX(12).ShouldBe(12),
				permissions => permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess)));
		}

		[Fact]
		public void PropertyAccessorWorksOnImmutableTypeWithPrivateSetterWhenRunningInPartialTrustWithReflectionPermissionFlagMemberAccess()
		{
			Console.WriteLine("ImageRuntimeVersion: " + Assembly.GetExecutingAssembly().ImageRuntimeVersion);
			PartialTrustContext.RunTest<CallingPrivateSetter>(
				runner => runner.SetAndGetX(12).ShouldBe(12),
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
			ex.Message.ShouldBe("Property set method not found.");
		}
	}
}
