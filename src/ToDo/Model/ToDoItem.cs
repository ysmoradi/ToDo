﻿using Prism.Mvvm;
using System;

namespace ToDo.Model
{
    public class ToDoItem : BindableBase
    {
        private int _Id;
        public virtual int Id
        {
            get => _Id;
            set => SetProperty(ref _Id, value);
        }

        private DateTimeOffset _CreatedDateTime;

        public virtual DateTimeOffset CreatedDateTime
        {
            get => _CreatedDateTime;
            set => SetProperty(ref _CreatedDateTime, value);
        }

        private string _Text;
        public virtual string Text
        {
            get => _Text;
            set => SetProperty(ref _Text, value);
        }

        private bool _IsFinished;
        public virtual bool IsFinished
        {
            get => _IsFinished;
            set => SetProperty(ref _IsFinished, value);
        }

        private int? _GroupId;
        public virtual int? GroupId
        {
            get => _GroupId;
            set => SetProperty(ref _GroupId, value);
        }

        private ToDoGroup _Group;
        public virtual ToDoGroup Group
        {
            get => _Group;
            set => SetProperty(ref _Group, value);
        }
    }
}
