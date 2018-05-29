using Autofac;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using ToDo.DataAccess;
using ToDo.ViewModels;
using ToDo.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ToDo
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("ToDoGroups/Nav/ToDoItems");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>("Nav");
            containerRegistry.RegisterForNavigation<ToDoGroupsView, ToDoGroupsViewModel>("ToDoGroups");
            containerRegistry.RegisterForNavigation<ToDoItemsView, ToDoItemsViewModel>("ToDoItems");
            containerRegistry.RegisterForNavigation<ToDoItemDetailView, ToDoItemDetailViewModel>("ToDoItemDetail");
            containerRegistry.RegisterForNavigation<SearchView, SearchViewModel>("Search");
            containerRegistry.Register<ToDoDbContext>();
        }
    }
}
