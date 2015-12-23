using System;
using Sqo;

namespace DeveloperLogicAssessment.DAO
{
    public abstract class BaseDAO<T>
    {
        protected Siaqodb DB()
        {
            return SiaqodbFactory.GetInstance();
        }

        protected void save(T obj)
        {
            DB().StoreObject(obj);
        }
    }

    class SiaqodbFactory
    {
        private static Siaqodb instance;

        public static Siaqodb GetInstance()
        {
            if (instance == null)
            {
                instance = new Siaqodb(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }

            return instance;
        }
    }

}
