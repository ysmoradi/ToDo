using Prism.Mvvm;
using System;

namespace ToDo.Model
{
    public class ToDoItem : BindableBase
    {
        public virtual int Id { get; set; }

        public virtual DateTimeOffset CreatedDateTime { get; set; }

        public virtual DateTimeOffset EndedDateTime { get; set; }

        public virtual string Text { get; set; }

        public virtual bool IsFinished { get; set; }

        public virtual int? GroupId { get; set; }

        public virtual bool ShowInMyDay { get; set; }

        public virtual ToDoGroup Group { get; set; }
    }
}
