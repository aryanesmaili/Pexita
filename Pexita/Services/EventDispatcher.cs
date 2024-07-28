namespace Pexita.Services
{
    public class EventDispatcher
    {
        /// <summary>
        /// This is the singleton dictionary that keeps the info saying what synchronous handler should be called to handle each event type:
        /// </summary>
        private readonly Dictionary<Type, List<Action<object>>> _syncHandlers = new Dictionary<Type, List<Action<object>>>();

        /// <summary>
        /// This is another Singleton dictionary that keeps the info saying what asynchronous handler should be called to handle each event type:
        /// </summary>
        private readonly Dictionary<Type, List<Func<object, Task>>> _asyncHandlers = new Dictionary<Type, List<Func<object, Task>>>();

        /// <summary>
        /// Registers a handler to the singleton dictionary so that the handler would be later called in case the respective event is triggered.
        /// </summary>
        /// <typeparam name="TEvent"> The event we're going to handle using this handler.</typeparam>
        /// <param name="handler"> the event handler to be assigned to <typeparamref name="TEvent"/>.</param>
        public void RegisterHandler<TEvent>(Action<TEvent> handler)
        {
            // Check if we do not already have a handler for that type of event
            if (!_syncHandlers.ContainsKey(typeof(TEvent)))
            {
                // Create a list of handler functions for the specific event type
                _syncHandlers[typeof(TEvent)] = new List<Action<object>>();
            }

            // Add the handler to the list, casting the input parameter to TEvent
            _syncHandlers[typeof(TEvent)].Add(e => handler((TEvent)e));
        }

        /// <summary>
        /// Registers an asynchronous handler to the dictionary so that the handler would be later called in case the respective event is triggered.
        /// </summary>
        /// <typeparam name="TEvent">The event we're going to handle using this handler.</typeparam>
        /// <param name="handler">The event handler to be assigned to <typeparamref name="TEvent"/>.</param>
        public void RegisterHandlerAsync<TEvent>(Func<TEvent, Task> handler)
        {
            // Check if we do not already have a handler for that type of event
            if (!_asyncHandlers.ContainsKey(typeof(TEvent)))
            {
                // Create a list of handler functions for the specific event type
                _asyncHandlers[typeof(TEvent)] = new List<Func<object, Task>>();
            }

            // Add the handler to the list, casting the input parameter to TEvent
            _asyncHandlers[typeof(TEvent)].Add(async e => await handler((TEvent)e));
        }

        /// <summary>
        /// This function will dispatch the event to all of its subscribers and runs the respective handler on the event itself.
        /// </summary>
        /// <typeparam name="TEvent"> The event type to be handled.</typeparam>
        /// <param name="eventToDispatch"> The event to be handled</param>
        public void Dispatch<TEvent>(TEvent eventToDispatch)
        {
            var eventType = typeof(TEvent);

            if (_syncHandlers.TryGetValue(eventType, out var eventHandlers)) // Check if we have a handler for this type of event
            {
                foreach (var handler in eventHandlers) // Go through the list of functions to be executed
                {
                    try
                    {
                        handler(eventToDispatch); // Execute those actions on the event based on its info
                    }
                    catch (Exception ex)
                    {
                        // Handle or log the exception as needed
                        Console.WriteLine($"Error executing handler for {eventType.Name}: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Triggers the event and executes all registered asynchronous handlers for that event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to trigger.</typeparam>
        /// <param name="eventData">The event data to pass to the handlers.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DispatchAsync<TEvent>(TEvent eventData)
        {
            var eventType = typeof(TEvent);

            if (_asyncHandlers.TryGetValue(eventType, out var eventHandlers)) // Check if there are handlers registered for the event type
            {
                foreach (var handler in eventHandlers) // Execute each handler asynchronously
                {
                    try
                    {
                        await handler(eventData);
                    }
                    catch (Exception ex)
                    {
                        // Handle or log the exception as needed
                        Console.WriteLine($"Error executing async handler for {eventType.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}
