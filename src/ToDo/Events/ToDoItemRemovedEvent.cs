using Prism.Events;
using ToDo.Model;

namespace ToDo.Events
{
    public class ToDoItemRemovedEvent : PubSubEvent<ToDoItemRemovedEvent>
    {
        public ToDoItem RemovedToDoItem { get; set; }
    }
}
