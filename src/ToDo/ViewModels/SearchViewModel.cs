using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ToDo.DataAccess;
using ToDo.Model;

namespace ToDo.ViewModels
{
    public class SearchViewModel : BindableBase, IDestructible
    {
        private readonly ToDoDbContext _dbContext;

        public string SearchText { get; set; }

        public void OnSearchTextChanged()
        {
            _SearchTextSubject.OnNext(SearchText);
        }

        private Subject<string> _SearchTextSubject = new Subject<string>();

        public ObservableCollection<ToDoItem> ToDoItems { get; set; }

        public SearchViewModel(ToDoDbContext dbContext)
        {
            _dbContext = dbContext;

            _SearchTextSubject
                .Throttle(TimeSpan.FromMilliseconds(300))
                .DistinctUntilChanged()
                .Subscribe(async txt =>
                {
                    ToDoItems = string.IsNullOrEmpty(txt) ? new ObservableCollection<ToDoItem>() : new ObservableCollection<ToDoItem>(await dbContext.ToDoItems.Where(toDoItem => toDoItem.Text.Contains(txt)).ToArrayAsync());
                });
        }

        public virtual void Destroy()
        {
            _SearchTextSubject.Dispose();
            _dbContext.Dispose();
        }
    }
}
