using System;
using System.Collections.Generic;

namespace GameFramework
{
    public static class GameFrameworkEntry
    {
        private const string GameFrameworkVersion = "3.1.3";

        private static readonly LinkedList<GameFrameworkModule> s_GameFrameworkModules = new LinkedList<GameFrameworkModule>();

        public static string Version
        {
            get
            {
                return "3.1.3";
            }
        }

        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (GameFrameworkModule current in GameFrameworkEntry.s_GameFrameworkModules)
            {
                current.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public static void Shutdown()
        {
            for (LinkedListNode<GameFrameworkModule> linkedListNode = GameFrameworkEntry.s_GameFrameworkModules.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
            {
                linkedListNode.Value.Shutdown();
            }
            GameFrameworkEntry.s_GameFrameworkModules.Clear();
        }

        public static T GetModule<T>() where T : class
        {
            Type typeFromHandle = typeof(T);
            if (!typeFromHandle.IsInterface)
            {
                throw new GameFrameworkException(string.Format("You must get module by interface, but '{0}' is not.", typeFromHandle.FullName));
            }
            if (!typeFromHandle.FullName.StartsWith("GameFramework."))
            {
                throw new GameFrameworkException(string.Format("You must get a Game Framework module, but '{0}' is not.", typeFromHandle.FullName));
            }
            string text = string.Format("{0}.{1}", typeFromHandle.Namespace, typeFromHandle.Name.Substring(1));
            Type type = Type.GetType(text);
            if (type == null)
            {
                throw new GameFrameworkException(string.Format("Can not find Game Framework module type '{0}'.", text));
            }
            return GameFrameworkEntry.GetModule(type) as T;
        }

        private static GameFrameworkModule GetModule(Type moduleType)
        {
            foreach (GameFrameworkModule current in GameFrameworkEntry.s_GameFrameworkModules)
            {
                if (current.GetType() == moduleType)
                {
                    return current;
                }
            }
            return GameFrameworkEntry.CreateModule(moduleType);
        }

        private static GameFrameworkModule CreateModule(Type moduleType)
        {
            GameFrameworkModule gameFrameworkModule = (GameFrameworkModule)Activator.CreateInstance(moduleType);
            if (gameFrameworkModule == null)
            {
                throw new GameFrameworkException(string.Format("Can not create module '{0}'.", gameFrameworkModule.GetType().FullName));
            }
            LinkedListNode<GameFrameworkModule> linkedListNode;
            for (linkedListNode = GameFrameworkEntry.s_GameFrameworkModules.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
            {
                if (gameFrameworkModule.Priority > linkedListNode.Value.Priority)
                {
                    break;
                }
            }
            if (linkedListNode != null)
            {
                GameFrameworkEntry.s_GameFrameworkModules.AddBefore(linkedListNode, gameFrameworkModule);
            }
            else
            {
                GameFrameworkEntry.s_GameFrameworkModules.AddLast(gameFrameworkModule);
            }
            return gameFrameworkModule;
        }
    }
}
