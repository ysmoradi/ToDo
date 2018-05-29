using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ToDo.DataAccess;
using ToDo.Model;
using Xamarin.Forms;

namespace ToDo.ViewModels
{
    public class ToDoGroupsViewModel : BindableBase, IDestructible
    {
        private readonly ToDoDbContext _dbContext;
        private readonly INavigationService _navigationService;

        public DelegateCommand LoadToDoGroups { get; set; }
        public DelegateCommand AddToDoGroup { get; set; }
        public DelegateCommand<ToDoGroup> DeleteToDoGroup { get; set; }
        public DelegateCommand OpenSearch { get; set; }
        public DelegateCommand<ToDoGroup> OpenToDoItems { get; set; }
        public DelegateCommand OpenMyDayItems { get; set; }

        private ObservableCollection<ToDoGroup> _ToDoGroups;
        public ObservableCollection<ToDoGroup> ToDoGroups
        {
            get => _ToDoGroups;
            set => SetProperty(ref _ToDoGroups, value);
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set => SetProperty(ref _IsBusy, value);
        }

        private string _NewToDoGroupName;
        public string NewToDoGroupName
        {
            get => _NewToDoGroupName;
            set => SetProperty(ref _NewToDoGroupName, value);
        }

        public ToDoGroupsViewModel(INavigationService navigationService, ToDoDbContext dbContext)
        {
            _navigationService = navigationService;

            _dbContext = dbContext;

            MessagingCenter.Subscribe<ToDoItem>(this, "ToDoItemAdded", (toDoItem) =>
            {
                if (toDoItem.GroupId != null)
                    ToDoGroups.Single(toDoGroup => toDoGroup.Id == toDoItem.GroupId).ActiveToDoItemsCount += 1;
            });
            MessagingCenter.Subscribe<ToDoItem>(this, "ToDoItemDeleted", (toDoItem) =>
            {
                if (toDoItem.GroupId != null)
                    ToDoGroups.Single(toDoGroup => toDoGroup.Id == toDoItem.GroupId).ActiveToDoItemsCount -= 1;
            });

            LoadToDoGroups = new DelegateCommand(async () =>
            {
                try
                {
                    IsBusy = true;

                    await _dbContext.Database.EnsureCreatedAsync();

                    ToDoGroup[] toDoGroups = await _dbContext.ToDoGroups
                        .Select(tdG => new ToDoGroup
                        {
                            Id = tdG.Id,
                            Name = tdG.Name,
                            CreatedDateTime = tdG.CreatedDateTime,
                            ActiveToDoItemsCount = tdG.ToDoItems.Count(toDoItem => toDoItem.IsFinished == false)
                        })
                        .ToArrayAsync();

                    foreach (ToDoGroup toDoGroup in toDoGroups)
                    {
                        _dbContext.Attach(toDoGroup);
                    }

                    ToDoGroups = _dbContext.ToDoGroups.Local.ToObservableCollection();
                }
                finally
                {
                    IsBusy = false;
                }
            });

            AddToDoGroup = new DelegateCommand(async () =>
            {
                try
                {
                    IsBusy = true;

                    ToDoGroup toDoGroup = new ToDoGroup { Name = NewToDoGroupName, CreatedDateTime = DateTimeOffset.UtcNow };

                    await _dbContext.ToDoGroups.AddAsync(toDoGroup);

                    await _dbContext.SaveChangesAsync();

                    NewToDoGroupName = "";
                }
                finally
                {
                    IsBusy = false;
                }
            }, () => !IsBusy && !string.IsNullOrEmpty(NewToDoGroupName));

            AddToDoGroup.ObservesProperty(() => IsBusy);
            AddToDoGroup.ObservesProperty(() => NewToDoGroupName);

            DeleteToDoGroup = new DelegateCommand<ToDoGroup>(async toDoGroup =>
            {
                try
                {
                    IsBusy = true;
                    _dbContext.Remove(toDoGroup);
                    await _dbContext.SaveChangesAsync();
                }
                finally
                {
                    IsBusy = false;
                }
            });

            OpenToDoItems = new DelegateCommand<ToDoGroup>(async (toDoGroup) =>
            {
                    await navigationService.NavigateAsync("Nav/ToDoItems", new Dictionary<string, object>
                {
                    { "toDoGroupId", toDoGroup.Id }
                }.ToNavParams());               
            });

            OpenSearch = new DelegateCommand(async () =>
            {
                await navigationService.NavigateAsync("Nav/Search");
            });

            OpenMyDayItems = new DelegateCommand(async () =>
            {
                await navigationService.NavigateAsync("Nav/ToDoItems");
            });
        }
        public virtual void Destroy()
        {
            MessagingCenter.Unsubscribe<ToDoItem>(this, "ToDoItemAdded");
            MessagingCenter.Unsubscribe<ToDoItem>(this, "ToDoItemDeleted");
            _dbContext.Dispose();
        }
    }
}
