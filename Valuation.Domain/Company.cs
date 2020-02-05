using LanguageExt;
using static LanguageExt.Prelude;

namespace Valuation.Domain
{
    public class Company
    {
        private Company(int id, string name, string additionalInformation)
        {
            Id = id;
            Name = name;
            AdditionalInformation = additionalInformation;
        }

        public static Option<Company> Create(int id, string name, string additionalInformation)
        {
            if (string.IsNullOrWhiteSpace(name))
                return None;
            else
                return new Company(id, name, additionalInformation);
        }

        public int Id { get; }
        public string Name { get; }
        public string AdditionalInformation { get; }
    }
}
