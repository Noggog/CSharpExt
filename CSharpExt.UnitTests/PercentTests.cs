using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests;

public class PercentTests
{
    [Fact]
    public void Typical()
    {
        var p = new Percent(0.5);
        p.Value.ShouldBe(0.5);
    }

    [Fact]
    public void OutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            new Percent(-0.1);
        });
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            new Percent(1.1);
        });
    }

    [Fact]
    public void Inverse()
    {
        var p = new Percent(0.7);
        p.Inverse.Value.EqualsWithin(0.3).ShouldBeTrue();
        p.Inverse.Inverse.Value.EqualsWithin(0.7).ShouldBeTrue();
    }

    [Fact]
    public void Add()
    {
        var p = new Percent(0.15d);
        var p2 = new Percent(0.25d);
        var p3 = p + p2;
        p3.Value.ShouldBe(0.4d);
        var p4 = new Percent(0.7);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var p5 = p3 + p4;
        });
    }

    [Fact]
    public void Subtract()
    {
        var p = new Percent(0.7d);
        var p2 = new Percent(0.25d);
        var p3 = p - p2;
        p3.Value.EqualsWithin(0.45d).ShouldBeTrue();
        var p4 = new Percent(0.7);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var p5 = p3 - p4;
        });
    }

    [Fact]
    public void Multiply()
    {
        var p = new Percent(0.5d);
        var p2 = new Percent(0.25d);
        var p3 = p * p2;
        p3.Value.ShouldBe(0.125d);
    }

    [Fact]
    public void Divide()
    {
        var p = new Percent(0.5d);
        var p2 = new Percent(0.25d);
        var p3 = p / p2;
        p3.ShouldBe(2d);
    }

    [Fact]
    public void PutInRange()
    {
        var p1 = Percent.FactoryPutInRange(1.1);
        p1.Value.ShouldBe(1);
        var p2 = Percent.FactoryPutInRange(-0.1);
        p2.Value.ShouldBe(0);
        var p3 = Percent.FactoryPutInRange(0.5);
        p3.Value.ShouldBe(0.5);
    }

    [Fact]
    public void PutInRangeWithMax()
    {
        var p1 = Percent.FactoryPutInRange(500, 255);
        p1.Value.ShouldBe(1);
        var p2 = Percent.FactoryPutInRange(-5, 255);
        p2.Value.ShouldBe(0);
        var p3 = Percent.FactoryPutInRange(50, 255);
        p3.Value.EqualsWithin(0.196078431372549).ShouldBeTrue();
    }

    [Fact]
    public void PutInRangeWithMaxLong()
    {
        var p1 = Percent.FactoryPutInRange(500L, 255L);
        p1.Value.ShouldBe(1);
        var p2 = Percent.FactoryPutInRange(-5L, 255L);
        p2.Value.ShouldBe(0);
        var p3 = Percent.FactoryPutInRange(50L, 255L);
        p3.Value.EqualsWithin(0.196078431372549).ShouldBeTrue();
    }

    [Fact]
    public void Equality()
    {
        var p1 = new Percent(0.123);
        var p2 = new Percent(0.123);
        p1.ShouldBe(p2);
        p1.GetHashCode().ShouldBe(p2.GetHashCode());
        var b = p1 == p2;
        b.ShouldBeTrue();
        var b2 = p1 != p2;
        b2.ShouldBeFalse();
    }

    [Fact]
    public void NonEquality()
    {
        var p1 = new Percent(0.123);
        var p2 = new Percent(0.124);
        p1.ShouldNotBe(p2);
        p1.GetHashCode().ShouldNotBe(p2.GetHashCode());
        var b = p1 == p2;
        b.ShouldBeFalse();
        var b2 = p1 != p2;
        b2.ShouldBeTrue();
    }

    [Fact]
    public void LessThanCompare()
    {
        var p1 = new Percent(0.1);
        var p2 = new Percent(0.2);
        var c = p1 < p2;
        c.ShouldBeTrue();
        c = p2 < p1;
        c.ShouldBeFalse();
        c = p1 < p1;
        c.ShouldBeFalse();
    }

    [Fact]
    public void LessThanOrEqualCompare()
    {
        var p1 = new Percent(0.1);
        var p2 = new Percent(0.2);
        var c = p1 <= p2;
        c.ShouldBeTrue();
        c = p2 <= p1;
        c.ShouldBeFalse();
        c = p1 <= p1;
        c.ShouldBeTrue();
    }

    [Fact]
    public void GreaterThanOrEqualCompare()
    {
        var p1 = new Percent(0.1);
        var p2 = new Percent(0.2);
        var c = p1 >= p2;
        c.ShouldBeFalse();
        c = p2 >= p1;
        c.ShouldBeTrue();
        c = p1 >= p1;
        c.ShouldBeTrue();
    }

    [Fact]
    public void GreaterThanCompare()
    {
        var p1 = new Percent(0.1);
        var p2 = new Percent(0.2);
        var c = p1 > p2;
        c.ShouldBeFalse();
        c = p2 > p1;
        c.ShouldBeTrue();
        c = p1 > p1;
        c.ShouldBeFalse();
    }
}