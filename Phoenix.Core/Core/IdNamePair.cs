using System;

namespace Phoenix.Core.Core
{
    [Serializable]
    public class IdNamePair 
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IdNamePair()
        {
        }

        public IdNamePair(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var right = obj as IdNamePair;

            return right != null
                && right.Id == Id
                && right.Name == Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
