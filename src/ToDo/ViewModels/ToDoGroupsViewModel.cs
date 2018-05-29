using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ToDo.DataAccess;
using ToDo.Events;
using ToDo.Model;

namespace ToDo.ViewModels
{
    public class ToDoGroupsViewModel : BindableBase, IDestructible
    {
        private readonly ToDoDbContext _dbContext;
        private readonly INavigationService _navigationService;
        private readonly IDisposable _toDoItemAddedDisposal;
        private readonly IDisposable _toDoItemRemovedDisposal;

        public DelegateCommand LoadToDoGroups { get; set; }
        public DelegateCommand AddToDoGroup { get; set; }
        public DelegateCommand<ToDoGroup> DeleteToDoGroup { get; set; }
        public DelegateCommand OpenSearch { get; set; }
        public DelegateCommand<ToDoGroup> OpenToDoItems { get; set; }
        public DelegateCommand OpenMyDayItems { get; set; }
        public ObservableCollection<ToDoGroup> ToDoGroups { get; set; }

        public bool IsBusy { get; set; }

        public string NewToDoGroupName { get; set; }

        public ToDoGroupsViewModel(INavigationService navigationService,
            ToDoDbContext dbContext,
            IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;

            _dbContext = dbContext;

            _toDoItemAddedDisposal = eventAggregator.GetEvent<ToDoItemAddedEvent>()
                .Subscribe(addedToDoItemEvent =>
                {
                    if (addedToDoItemEvent.AddedToDoItem.GroupId != null)
                        ToDoGroups.Single(toDoGroup => toDoGroup.Id == addedToDoItemEvent.AddedToDoItem.GroupId).ActiveToDoItemsCount += 1;
                }, ThreadOption.UIThread, keepSubscriberReferenceAlive: true);

            _toDoItemRemovedDisposal = eventAggregator.GetEvent<ToDoItemRemovedEvent>()
                .Subscribe(removedToDoItemEvent =>
                {
                    if (removedToDoItemEvent.RemovedToDoItem.GroupId != null)
                        ToDoGroups.Single(toDoGroup => toDoGroup.Id == removedToDoItemEvent.RemovedToDoItem.GroupId).ActiveToDoItemsCount -= 1;
                }, ThreadOption.UIThread, keepSubscriberReferenceAlive: true);

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
                await navigationService.NavigateAsync("Nav/ToDoItems", new NavigationParameters
                {
                    { "toDoGroupId", toDoGroup.Id }
                });
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
            _toDoItemAddedDisposal?.Dispose();
            _toDoItemRemovedDisposal?.Dispose();
            _dbContext.Dispose();
        }
    }
}
