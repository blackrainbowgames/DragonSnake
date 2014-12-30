using UnityEngine;

namespace Assets.Scripts
{
	public static class PrefabsHelper
	{
        public static GameObject InstantiateCell()
        {
            return Instantiate("Prefabs/Cell");
        }

        public static GameObject InstantiateSparks()
        {
            return Instantiate("Prefabs/Sparks");
        }

        private static GameObject Instantiate(string path)
        {
            try
            {
                var instance = (GameObject) Object.Instantiate(Resources.Load(path, typeof (GameObject)));

                instance.name = instance.name.Replace("(Clone)", "");

                return instance;
            }
            catch
            {
                Debug.Log(path);
                throw;
            }
        }
	}
}
