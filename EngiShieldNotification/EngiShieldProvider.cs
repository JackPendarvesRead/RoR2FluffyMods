using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngiShieldNotification
{
    public class EngiShieldProvider
    {
        private List<GameObject> gameObjects;
        private List<EngiShieldNotificationControllerDeltaTime> timeControllers;

        public EngiShieldProvider()
        { 
            gameObjects = new List<GameObject>();            
        }

        public void Add(GameObject obj)
        {
            if (gameObjects.Contains(obj))
            {
                gameObjects.Add(obj);
                timeControllers.Add(new EngiShieldNotificationControllerDeltaTime(obj));
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
