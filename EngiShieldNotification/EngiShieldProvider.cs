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
        private List<EngiShieldNotificationControllerDeltaTime> gameObjects;

        public EngiShieldProvider()
        { 
            gameObjects = new List<GameObject>();            
        }

        public void Add(GameObject obj)
        {
            if (gameObjects.Contains(obj))
            {
                gameObjects.Add(obj);
            }
        }

        public void Remove(GameObject obj)
        {
            if (gameObjects.Contains(obj))
            {
                gameObjects.Remove(obj);
            }
        }

        public void Update (float time)
        {
            foreach(var go in gameObjects)
            {
                
            }
        }
    }
}
