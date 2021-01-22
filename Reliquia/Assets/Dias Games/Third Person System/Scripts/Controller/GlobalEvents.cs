using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class GlobalEvents : MonoBehaviour
    {
        public static List<GameEvent> gameEvents = new List<GameEvent>();

        private void Awake()
        {
            gameEvents = new List<GameEvent>();
        }



        public static void ExecuteEvent(string EventName, GameObject target, object parameter)
        {
            foreach (GameEvent gameEvent in gameEvents)
            {
                if (gameEvent.EventName == EventName)
                    gameEvent.ExecuteEvent(target, parameter);
            }
        }



        public static void AddEvent(string EventName, GameAction action)
        {
            foreach (GameEvent gameEvent in gameEvents)
            {
                if (gameEvent.EventName == EventName)
                {
                    gameEvent.ExecuteEvent += action;
                    return;
                }
            }

            gameEvents.Add(new GameEvent(EventName, action));
        }

    }

    public delegate void GameAction(GameObject target, object parameter = null);

    public class GameEvent
    {
        public string EventName;
        public GameAction ExecuteEvent;

        public GameEvent(string eventName, GameAction newEvent)
        {
            EventName = eventName;
            ExecuteEvent = newEvent;
        }
    }
}
