using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.Model
{
    public class ToDoGroup : BindableBase
    {
        public virtual int Id { get; set; }

        public virtual DateTimeOffset CreatedDateTime { get; set; }

        public virtual string Name { get; set; }

        public virtual string Icon { get; set; }

        public virtual ObservableCollection<ToDoItem> ToDoItems { get; set; }

        [NotMapped]
        public virtual int ActiveToDoItemsCount { get; set; }
    }
}
