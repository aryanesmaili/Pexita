using Pexita.Data.Entities;
namespace Pexita.Services
{
    public class EventDispatcher
    {

        /// <summary>
        /// This is the singleton dictionary that keeps the info saying what handler should be called to handle each event type:
        /// </summary>
        private readonly Dictionary<Type, List<Action<object>>> _handlers = new Dictionary<Type, List<Action<object>>>();

        /// <summary>
        /// Registers a handler to the singleton dictionary so that the handler would be later called in case the respective event is triggered.
        /// </summary>
        /// <typeparam name="TEvent"> The event we're going to handle using this handler.</typeparam>
        /// <param name="handler"> the event handler to be assigned to <typeparamref name="TEvent"/></param>
        public void RegisterHandler<TEvent>(Action<TEvent> handler)
        {
            if (!_handlers.ContainsKey(typeof(TEvent))) // if we do not already have a handler for that type of event:
            {
                _handlers[typeof(TEvent)] = new List<Action<object>>(); // we make a list of handle functions at the key value of that specific event.
            }

            _handlers[typeof(TEvent)].Add(e => handler((TEvent)e));
        }

        /// <summary>
        /// This function will Dispatch the event to all of its subscribers and runs the respective handler on the event itself.
        /// </summary>
        /// <typeparam name="TEvent"> The event type to be handled.</typeparam>
        /// <param name="eventToDispatch"> The Event to be handled</param>
        public void Dispatch<TEvent>(TEvent eventToDispatch)
        {
            if (_handlers.ContainsKey(eventToDispatch.GetType())) // if we have a handler for this type of event:
            {
                foreach (var handler in _handlers[eventToDispatch.GetType()]) // go through the list of functions to be done defined in the handler:
                {
                    handler(eventToDispatch); // do those actions on the event based on its info.
                }
            }
        }
    }
}
