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
    public class ToDoItemsViewModel : BindableBase, IDestructible, INavigatedAware
    {
        private readonly ToDoDbContext _dbContext;

        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        public readonly IDisposable _toDoItemRemovedDisposal;

        private int? _toDoGroupId;

        public virtual DelegateCommand LoadToDoItems { get; set; }

        public virtual DelegateCommand<ToDoItem> ToggleToDoItemIsFinished { get; set; }

        public virtual DelegateCommand AddToDoItem { get; set; }

        public virtual DelegateCommand<ToDoItem> NavigateToDetail { get; set; }

        public virtual DelegateCommand<ToDoItem> DeleteToDoItem { get; set; }

        public virtual ObservableCollection<ToDoItem> ToDoItems { get; set; }

        public virtual bool IsBusy { get; set; }

        public virtual string NewToDoText { get; set; }

        public virtual string GroupName { get; set; }

        public virtual bool LoadAll { get; set; }

        private IQueryable<ToDoItem> GetToDoItemsQuery(IQueryable<ToDoItem> toDoItemsBaseQuery)
        {
            toDoItemsBaseQuery = _toDoGroupId.HasValue ? toDoItemsBaseQuery.Where(toDo => toDo.GroupId == _toDoGroupId) : toDoItemsBaseQuery.Where(toDo => toDo.ShowInMyDay == true || toDo.GroupId == null);

            if (LoadAll == false)
                toDoItemsBaseQuery = toDoItemsBaseQuery.Where(toDo => toDo.IsFinished == false);
            else
                toDoItemsBaseQuery = toDoItemsBaseQuery.OrderBy(toDo => toDo.IsFinished == true);

            return toDoItemsBaseQuery;
        }

        public virtual void Destroy()
        {
            _toDoItemRemovedDisposal.Dispose();
            _dbContext.Dispose();
        }

        public virtual async void OnNavigatedTo(NavigationParameters navigationParams)
        {
            if (navigationParams.GetNavigationMode() == NavigationMode.Back)
                return;

            navigationParams.TryGetValue("toDoGroupId", out int? toDoGroupId);

            _toDoGroupId = toDoGroupId;

            LoadToDoItems.Execute();

            GroupName = _toDoGroupId.HasValue ? (await _dbContext.ToDoGroups.FindAsync(_toDoGroupId))?.Name : "My day";
        }

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public ToDoItemsViewModel(INavigationService navigationService, ToDoDbContext dbContext, IEventAggregator eventAggregator)
        {
            _dbContext = dbContext;
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            _toDoItemRemovedDisposal = _eventAggregator.GetEvent<ToDoItemRemovedEvent>()
                .Subscribe(async toDoItemRemovedEvent =>
                {
                    DeleteToDoItem.Execute(await _dbContext.ToDoItems.FindAsync(toDoItemRemovedEvent.RemovedToDoItem.Id));
                }, ThreadOption.UIThread, keepSubscriberReferenceAlive: true);

            LoadToDoItems = new DelegateCommand(async () =>
            {
                try
                {
                    IsBusy = true;

                    await _dbContext.Database.EnsureCreatedAsync();

                    await GetToDoItemsQuery(_dbContext.ToDoItems).LoadAsync();

                    ToDoItems = new ObservableCollection<ToDoItem>(GetToDoItemsQuery(_dbContext.ToDoItems.Local.AsQueryable()));
                }
                finally
                {
                    IsBusy = false;
                }
            }, () => !IsBusy);

            LoadToDoItems.ObservesProperty(() => IsBusy);

            ToggleToDoItemIsFinished = new DelegateCommand<ToDoItem>(async (toDoItem) =>
            {

                try
                {
                    IsBusy = true;

                    toDoItem.IsFinished = !toDoItem.IsFinished;

                    if (toDoItem.IsFinished == true)
                    {
                        toDoItem.EndedDateTime = DateTimeOffset.UtcNow;
                    }

                    await _dbContext.SaveChangesAsync();

                    if (LoadAll == false && toDoItem.IsFinished == true)
                    {
                        ToDoItems.Remove(toDoItem);
                    }
                }
                catch
                {
                    await _dbContext.Entry(toDoItem).ReloadAsync();
                    throw;
                }
                finally
                {
                    IsBusy = false;
                }

            }, (toDoItem) => !IsBusy);

            ToggleToDoItemIsFinished.ObservesProperty(() => IsBusy);

            AddToDoItem = new DelegateCommand(async () =>
            {
                try
                {
                    IsBusy = true;

                    ToDoItem toDoItem = new ToDoItem { IsFinished = false, Text = NewToDoText, GroupId = _toDoGroupId, CreatedDateTime = DateTimeOffset.UtcNow };

                    await _dbContext.ToDoItems.AddAsync(toDoItem);

                    await _dbContext.SaveChangesAsync();

                    ToDoItems.Add(toDoItem);

                    _eventAggregator.GetEvent<ToDoItemAddedEvent>()
                        .Publish(new ToDoItemAddedEvent { AddedToDoItem = toDoItem });

                    NewToDoText = "";
                }
                finally
                {
                    IsBusy = false;
                }
            }, () => !IsBusy && !string.IsNullOrEmpty(NewToDoText));

            AddToDoItem.ObservesProperty(() => IsBusy);
            AddToDoItem.ObservesProperty(() => NewToDoText);

            DeleteToDoItem = new DelegateCommand<ToDoItem>(async (toDoItem) =>
            {
                try
                {
                    IsBusy = true;
                    _dbContext.Remove(toDoItem);
                    await _dbContext.SaveChangesAsync();
                    _eventAggregator.GetEvent<ToDoItemRemovedEvent>()
                        .Publish(new ToDoItemRemovedEvent { RemovedToDoItem = toDoItem });
                    ToDoItems.Remove(toDoItem);
                }
                finally
                {
                    IsBusy = false;
                }

            }, (toDoItem) => !IsBusy);

            DeleteToDoItem.ObservesProperty(() => IsBusy);

            NavigateToDetail = new DelegateCommand<ToDoItem>(async (toDoItem) =>
            {
                await navigationService.NavigateAsync("ToDoItemDetail", new NavigationParameters
                {
                    { "toDoItemId", toDoItem.Id }
                });
            });
        }
    }
}
