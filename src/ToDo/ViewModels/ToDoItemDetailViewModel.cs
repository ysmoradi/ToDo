using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ToDo.DataAccess;
using ToDo.Model;
using Xamarin.Forms;

namespace ToDo.ViewModels
{
    public class ToDoItemDetailViewModel : BindableBase, INavigatedAware, IDestructible
    {
        private readonly ToDoDbContext _dbContext;

        public DelegateCommand DeleteToDoItemDetail { get; set; }

        private int _toDoItemId;

        public ToDoItem ToDoItem { get; set; }

        public virtual async void OnNavigatedTo(NavigationParameters navigationParams)
        {
            _toDoItemId = navigationParams.GetValue<int>("toDoItemId");

            ToDoItem = await _dbContext.ToDoItems.FindAsync(_toDoItemId);
        }

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {
            _dbContext.Dispose();
        }

        public ToDoItemDetailViewModel(INavigationService navigationService, ToDoDbContext dbContext)
        {
            _dbContext = dbContext;

            DeleteToDoItemDetail = new DelegateCommand(async () =>
            {
                MessagingCenter.Send(ToDoItem, "ToDoItemRemoved");
                await navigationService.GoBackAsync();
            });
        }
    }
}
