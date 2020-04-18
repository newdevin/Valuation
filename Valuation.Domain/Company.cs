
using System;

namespace Valuation.Domain
{
    public class Company
    {
        public Company(int id, string name, string additionalInformation)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} is required");

            Id = id;
            Name = name;
            AdditionalInformation = additionalInformation;
        }

        public int Id { get; }
        public string Name { get; }
        public string AdditionalInformation { get; }
    }
}
