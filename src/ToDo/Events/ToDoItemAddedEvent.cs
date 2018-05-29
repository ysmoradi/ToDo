using Prism.Events;
using ToDo.Model;

namespace ToDo.Events
{
    public class ToDoItemAddedEvent : PubSubEvent<ToDoItemAddedEvent>
    {
        public ToDoItem AddedToDoItem { get; set; }
    }
}
