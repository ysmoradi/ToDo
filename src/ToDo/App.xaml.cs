﻿using Prism.Autofac;
using Prism.Autofac.Forms;
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

            NavigationService.NavigateAsync("ToDoGroups/ToDoItems");
        }
        
        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<NavigationPage>("Nav");
            Container.RegisterTypeForNavigation<ToDoItemsView, ToDoItemsViewModel>("ToDoItems");
            Container.RegisterTypeForNavigation<ToDoGroupsView, ToDoGroupsViewModel>("ToDoGroups");
            Container.RegisterTypeForNavigation<ToDoItemDetailView, ToDoItemDetailViewModel>("ToDoItemDetail");
        }
    }
}
