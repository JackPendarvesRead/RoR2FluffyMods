using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngiShieldNotification
{
    public class EngiShieldNotificationProvider
    {
        private List<GameObject> gameObjects;
        private List<EngiShieldNotificationTimer> timeControllers;

        public EngiShieldNotificationProvider()
        { 
            gameObjects = new List<GameObject>();
            timeControllers = new List<EngiShieldNotificationTimer>();
        }

        public void Add(GameObject obj)
        {
            if (!gameObjects.Contains(obj))
            {
                gameObjects.Add(obj);
                timeControllers.Add(new EngiShieldNotificationTimer(obj));
            }
        }

        public void Remove(GameObject obj)
        {
            if (gameObjects.Contains(obj))
            {
                gameObjects.Remove(obj);
                timeControllers.Remove(timeControllers.Where(t => t.gameObject == obj).First());
            }
        }

        public void Update (float time)
        {
            foreach(var tc in timeControllers)
            {
                tc.Update(time);
            }
        }
    }
}
