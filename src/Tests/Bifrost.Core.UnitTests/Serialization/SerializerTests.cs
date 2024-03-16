using Bifrost.Queries;
using Bifrost.Serialization;
using MediatR;
using System.Text;

namespace Bifrost.Tests.Serialization;

[TestClass]
public class SerializerTests
{
    [TestMethod]
    public async Task Serialize_ReturnJsonWithObjectType()
    {
        // Arrange
        const string serializedCommand = @"{""$type"":""Bifrost.Commands.DummyCommand, Bifrost.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null""}";
        DummyQuery command = new();

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
        const string serializedCommand = @"{""$type"":""Bifrost.Commands.DummyCommand, Bifrost.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null""}";
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(serializedCommand));

        // Act
        object? result = await Serializer.TypedDeserializeAsync(stream);

        // Assert
        result.Should()
            .NotBeNull()
            .And.BeOfType<DummyQuery>();
    }
}
