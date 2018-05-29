using Prism.Mvvm;
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

        private DateTimeOffset _EndedDateTime;

        public virtual DateTimeOffset EndedDateTime
        {
            get => _EndedDateTime;
            set => SetProperty(ref _EndedDateTime, value);
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

        private bool _ShowInMyDay;
        public virtual bool ShowInMyDay
        {
            get => _ShowInMyDay;
            set => SetProperty(ref _ShowInMyDay, value);
        }

        private ToDoGroup _Group;
        public virtual ToDoGroup Group
        {
            get => _Group;
            set => SetProperty(ref _Group, value);
        }
    }
}
