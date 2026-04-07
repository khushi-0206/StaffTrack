using AuthService.Infrastructure.Services;

namespace AuthService.Tests.Infrastructure;

[TestFixture]
public class PasswordHasherTests
{
    [Test]
    public void Hash_And_Verify_RoundTrip()
    {
        var h = new PasswordHasher();
        var hash = h.Hash("MySecret1!");
        Assert.That(h.Verify("MySecret1!", hash), Is.True);
        Assert.That(h.Verify("Other1!", hash), Is.False);
    }
}
