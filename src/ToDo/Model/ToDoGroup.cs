using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.Model
{
    public class ToDoGroup : BindableBase
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

        private string _Name;
        public virtual string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }

        private string _Icon;
        public virtual string Icon
        {
            get => _Icon;
            set => SetProperty(ref _Icon, value);
        }

        private ObservableCollection<ToDoItem> _ToDoItems;

        public virtual ObservableCollection<ToDoItem> ToDoItems
        {
            get => _ToDoItems;
            set => SetProperty(ref _ToDoItems, value);
        }

        private int _ToDoItemsCount;

        [NotMapped]
        public virtual int ActiveToDoItemsCount
        {
            get => _ToDoItemsCount;
            set => SetProperty(ref _ToDoItemsCount, value);
        }
    }
}
