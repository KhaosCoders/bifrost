using Bifrost.Commands.Identity;
using Bifrost.Serialization;
using System.Text;

namespace Bifrost.Tests.Serialization;

[TestClass]
public class SerializerTests
{
    const string serializedCommand = @"{""username"":""user"",""password"":""pwd"",""useCookie"":true,""useSession"":true,""twoFactorCode"":null,""twoFactorRecoveryCode"":null,""$type"":""Bifrost.Commands.Identity.LoginCommand, Bifrost.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null""}";

    [TestMethod]
    public async Task Serialize_ReturnJsonWithObjectType()
    {
        // Arrange
        LoginCommand command = new("user", "pwd", true, true);

        // Act
        Stream data = await Serializer.SerializeAsync(command);

        // Assert
        data.Should()
            .NotBeNull()
            .And.HavePosition(0);

        using StreamReader reader = new(data);
        string json = await reader.ReadToEndAsync();

        json.Should()
            .NotBeNullOrEmpty()
            .And.Be(serializedCommand);
    }

    [TestMethod]
    public async Task Deserialize_ReturnsCorrectTypedObject()
    {
        // Arrange
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(serializedCommand));
        LoginCommand command = new("user", "pwd", true, true);

        // Act
        object? result = await Serializer.TypedDeserializeAsync(stream);

        // Assert
        result.Should()
            .NotBeNull()
            .And.BeOfType<LoginCommand>()
                .Which.Should().BeEquivalentTo(command);
    }
}
