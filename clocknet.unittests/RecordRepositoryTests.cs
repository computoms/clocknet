﻿using clocknet.Storage;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using YamlDotNet.Serialization;

namespace clocknet.unittests;

public class RecordRepositoryTests
{
    private readonly AutoMocker _mocker = new();
    private readonly RecordRepository _repository;
    private readonly Mock<IStorage> _storage;

    public RecordRepositoryTests()
    {
        _repository = _mocker.CreateInstance<RecordRepository>();
        _storage = _mocker.GetMock<IStorage>();
    }

    [Theory]
    [InlineData("New Entry .123", false)]
    [InlineData("11:00 New Entry .123", true)]
    [InlineData("11:00 .123", true)]
    public void WhenAddingRawEntry_ThenEntryIsAddedToStorage(string entry, bool parseTime)
    {
        // Act
        _repository.AddRaw(entry, parseTime);

        // Assert
        _storage.Verify(x => x.AddEntryRaw(entry, parseTime), Times.Once);
    }

    [Fact]
    public void WithThrowingStorage_WhenAddingEntry_ThenDoesNotThrow()
    {
        // Arrange
        _storage.Setup(x => x.AddEntry(It.IsAny<Task>(), It.IsAny<Record>())).Throws<InvalidDataException>();

        // Act
        var act = () => _repository.AddRecord(new Task("Test", new string[0], ""), new Record(DateTime.Now, null));

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void WithThrowingStorage_WhenAddingEntryRaw_ThenDoesNotThrow()
    {
        // Arrange
        _storage.Setup(x => x.AddEntryRaw(It.IsAny<string>(), It.IsAny<bool>())).Throws<InvalidDataException>();

        // Act
        var act = () => _repository.AddRaw("test");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void WhenAddingEntry_ThenEntryIsAddedToStorage()
    {
        // Arrange
        var activity = new Task("Test", new string[0], "");
        var record = new Record(DateTime.Now, null);

        // Act
        _repository.AddRecord(activity, record);

        // Assert
        _storage.Verify(x => x.AddEntry(activity, record), Times.Once);
    }

    [Fact]
    public void WithOneEntryWithOneTag_WhenFilterByTag_ThenReturnsEntry()
    {
        // Arrange
        _storage.Setup(x => x.GetActivities()).Returns(new List<Activity>() { new Activity(new Task("Entry", new string[] { "tag" }, "123")) });

        // Act
        var result = _repository.FilterByTag(new List<string>() { "tag" });

        // Assert
        result.Should().HaveCount(1);
        result.First().Task.Title.Should().Be("Entry");
        result.First().Task.Id.Should().Be("123");
    }

    [Fact]
    public void WithOneEntryWithTwoTags_WhenFilterByTag_ThenReturnsEntry()
    {
        // Arrange
        _storage.Setup(x => x.GetActivities()).Returns(new List<Activity>() { new Activity(new Task("Entry", new string[] { "tag1", "tag2" }, "123")) });

        // Act
        var result = _repository.FilterByTag(new List<string>() { "tag1" });

        // Assert
        result.Should().HaveCount(1);
        result.First().Task.Title.Should().Be("Entry");
        result.First().Task.Id.Should().Be("123");
    }

    [Fact]
    public void WithTwoEntries_WithTwoTags_WhenFilterByTag_ThenReturnsCorrectEntry()
    {
        // Arrange
        _storage.Setup(x => x.GetActivities()).Returns(
	    new List<Activity>() 
	    { 
	        new Activity(new Task("Entry1", new string[] { "tag1", "tag2" }, "123")),
	        new Activity(new Task("Entry2", new string[] { "tag1", "tag3" }, "345")),
	    });

        // Act
        var result = _repository.FilterByTag(new List<string>() { "tag1", "tag2" });

        // Assert
        result.Should().HaveCount(1);
        result.First().Task.Title.Should().Be("Entry1");
        result.First().Task.Id.Should().Be("123");
    }

    [Fact]
    public void WithOneActivity_WithTwoRecords_WhenFilterByDate_ThenReturnsCorrectRecords()
    {
        // Arrange
        var activity = new Activity(new Task("Entry1", new string[] { }, ""));
        var startTime = new DateTime(2022, 10, 10, 10, 0, 0);
        activity.AddRecord(new Record(startTime, new DateTime(2022, 10, 10, 11, 0, 0)));
        activity.AddRecord(new Record(new DateTime(2022, 10, 11, 10, 0, 0), new DateTime(2022, 10, 11, 11, 0, 0)));
        _storage.Setup(x => x.GetActivities())
            .Returns(
                new List<Activity>()
                {
                    activity
                });

        // Act
        var result = _repository.FilterByDate(new DateTime(2022, 10, 10));

        // Assert
        result.First().Records.Should().HaveCount(1);
        result.First().Records.First().StartTime.Should().Be(startTime);
    }

    [Fact]
    public void WithTwoActivities_WhenFilter_ThenActivityNotConainingRecordsAreNotReturned()
    {
        // Arrange
        _storage.Setup(x => x.GetActivities())
            .Returns(new List<Activity>()
            {
                new Activity(new Task("Activtiy1", new string[0], ""), new List<Record>(){new Record(new DateTime(2022, 10, 10), new DateTime(2022, 10, 10))}),
                new Activity(new Task("Activtiy2", new string[0], ""), new List<Record>(){new Record(new DateTime(2022, 10, 11), new DateTime(2022, 10, 11))}),
            });

        // Act
        var result = _repository.FilterByDate(new DateTime(2022, 10, 10));

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void DisplayDuration()
    {
        TimeSpan ts = new TimeSpan(10, 11, 0);
        var str = ts.ToString(@"hh\:mm");
        str.Should().Be("10:11");
    }
}
