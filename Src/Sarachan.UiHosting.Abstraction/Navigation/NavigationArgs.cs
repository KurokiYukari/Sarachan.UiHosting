namespace Sarachan.UiHosting.Navigation
{
    public static class NavigationArgs
    {
        public abstract class Base : ContextArgs
        {
            private readonly List<Task> _eventTasks = new();
            public IReadOnlyList<Task> EventTasks => _eventTasks;

            public CancellationToken EventCancellationToken { get; init; }

            public void AddEventTask(Task task)
            {
                _eventTasks.Add(task);
            }
        }

        public class Navigate : Base { }

        public class Close : Base 
        {
            public bool? Result { get; }

            public Close(bool? result)
            {
                Result = result;
            }
        }
    }
}
