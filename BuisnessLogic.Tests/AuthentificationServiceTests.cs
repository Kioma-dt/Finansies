using BuisnessLogic.AuthentificationService;
using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Moq;

namespace BuisnessLogic.Tests;

using FluentAssertions;
using System;
using System.Linq;
using Xunit;
public class AuthentificationBCryptServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthentificationBCryptService _service;

    public AuthentificationBCryptServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _service = new AuthentificationBCryptService(
            _userRepositoryMock.Object);
    }

    [Fact]
    public async Task LogIn_WhenUserExistsAndPasswordIsCorrect_ShouldReturnUser()
    {
        // Arrange
        const string name = "Roman";
        const string password = "123456";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _userRepositoryMock
            .Setup(x => x.GetByName(name))
            .ReturnsAsync(user);

        // Act
        var result = await _service.LogIn(name, password);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(user);

        _userRepositoryMock.Verify(
            x => x.GetByName(name),
            Times.Once);
    }

    [Fact]
    public async Task LogIn_WhenUserDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange
        const string name = "Roman";
        const string password = "123456";

        _userRepositoryMock
            .Setup(x => x.GetByName(name))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _service.LogIn(name, password);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage($"No Such User({name})");

        _userRepositoryMock.Verify(
            x => x.GetByName(name),
            Times.Once);
    }

    [Fact]
    public async Task LogIn_WhenPasswordIsIncorrect_ShouldThrowArgumentException()
    {
        // Arrange
        const string name = "Roman";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password")
        };

        _userRepositoryMock
            .Setup(x => x.GetByName(name))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = () => _service.LogIn(name, "wrong-password");

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("Wrong Password");

        _userRepositoryMock.Verify(
            x => x.GetByName(name),
            Times.Once);
    }

    [Fact]
    public async Task Register_WhenUserAlreadyExists_ShouldThrowArgumentException()
    {
        // Arrange
        const string name = "Roman";
        const string password = "123456";

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            PasswordHash = "hash"
        };

        _userRepositoryMock
            .Setup(x => x.GetByName(name))
            .ReturnsAsync(existingUser);

        // Act
        Func<Task> act = () => _service.Register(name, password);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage($"Such user ({name}) already exists");

        _userRepositoryMock.Verify(
            x => x.GetByName(name),
            Times.Once);
    }

    [Fact]
    public async Task Register_WhenUserDoesNotExist_ShouldCreateUser()
    {
        // Arrange
        const string name = "Roman";
        const string password = "123456";

        _userRepositoryMock
            .Setup(x => x.GetByName(name))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.Register(name, password);

        // Assert
        result.Should().NotBeNull();

        result!.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(name);

        result.PasswordHash.Should().NotBeNullOrWhiteSpace();
        result.PasswordHash.Should().NotBe(password);

        BCrypt.Net.BCrypt.Verify(password, result.PasswordHash)
            .Should().BeTrue();

        _userRepositoryMock.Verify(
            x => x.GetByName(name),
            Times.Once);
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("password")]
    [InlineData("qwerty123")]
    public async Task Register_ShouldHashPassword(string password)
    {
        // Arrange
        const string name = "Roman";

        _userRepositoryMock
            .Setup(x => x.GetByName(name))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.Register(name, password);

        // Assert
        BCrypt.Net.BCrypt.Verify(
            password,
            result!.PasswordHash)
            .Should()
            .BeTrue();
    }
}