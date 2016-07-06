using System;
using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
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

        /// <summary>
        /// No setter methods
        /// </summary>
        public class ImmutablePointWithNoSetter
        {
            public ImmutablePointWithNoSetter(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        [Fact]
        public void UsingPropertyAccessorOnImmutableTypeWithNoSetter()
        {
            var p = new ImmutablePointWithNoSetter(10, 20);

            var propertyX = IntrospectionOf<ImmutablePointWithNoSetter>.GetAccessorFor(x => x.X);

            var ex = Assert.Throws<ArgumentException>(() => propertyX.SetValue(p, 12));

            if (IsMonoCLR())
            {
                // Mono
                ex.Message.ShouldBe("Set Method not found for 'X'");
            }
            else
            {
                // CoreCLR, DesktopCLR
                ex.Message.ShouldBe("Property set method not found.");
            }
        }

        static bool IsMonoCLR()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        static bool IsMicrosoftCLR()
        {
            return Type.GetType("Mono.Runtime") == null;
        }
    }
}
