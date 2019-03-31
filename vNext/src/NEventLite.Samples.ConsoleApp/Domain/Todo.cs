﻿using System;

namespace NEventLite.Samples.ConsoleApp.Domain
{
    public class Todo
    {
        public Todo(Guid id, string text)
        {
            Id = id;
            Text = text;
        }

        public Guid Id { get; }
        public string Text { get; set; }
        public bool IsCompleted { get; set; }
    }
}
