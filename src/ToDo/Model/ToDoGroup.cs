﻿using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace ToDo.Model
{
    public class ToDoGroup : BindableBase
    {
        private int _Id;
        public virtual int Id
        {
            get { return _Id; }
            set { SetProperty(ref _Id, value); }
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
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        private ObservableCollection<ToDoItem> _ToDoItems;

        public virtual ObservableCollection<ToDoItem> ToDoItems
        {
            get { return _ToDoItems; }
            set { SetProperty(ref _ToDoItems, value); }
        }
    }
}
