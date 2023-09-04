using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace ServerConnection
{
    public class ApiData
    {
        public string? Name { get; set; }
        public string? Id { get; set; }

        public override bool Equals(object? other)
        {
            if(other is ApiData)
            {
                return this.Name == ((ApiData)other).Name && this.Id == ((ApiData)other).Id;
            }
            else
            {
                return false;
            }
            
        }
        public override string ToString()
        {
            return $"{Name}: {Id}";
        }

        public override int GetHashCode()
        {
            int nameHash, idHash;
            if (Name is null)
                nameHash = "".GetHashCode();
            else
                nameHash = Name.GetHashCode();
            if (Id is null) 
                idHash = "".GetHashCode();
            else
                idHash = Id.GetHashCode();
            return nameHash ^ idHash;
        }
    }
}