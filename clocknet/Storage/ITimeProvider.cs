﻿namespace clocknet.Storage;

public interface ITimeProvider
{
    DateTime Now { get; }
}

public class TimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now; 
}

