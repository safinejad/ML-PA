using System.Collections.Concurrent;

namespace FakeRepository
{
     public class FakeRepo<T> // Is Just fake!
     {
         private HashSet<T> bag = new HashSet<T>();
        public IEnumerable<T> GetAll()
         {
             return bag.AsEnumerable();
         }

        public int Create(T entity)
        {
            if (bag.Contains(entity))
            {
                throw new InvalidDataException("Item Already Exists");
            }

            bag.Add(entity);
            return bag.Count;
        }


        public void Update(T entity)
         {
             if (!bag.Contains(entity))
             {
                 throw new InvalidDataException("Cannot find the entity to update");
             }

             bag.Add(entity);
         }

        public void Delete(T book)
        {
            if (!bag.Contains(book))
            {
                throw new InvalidDataException("Cannot find the entity to delete");
            }
            bag.Remove(book);
        }
     }
}