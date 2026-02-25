using System;

namespace Repository.DataModels.Remittance
{
    public class AdministrativeDivisionDTO
    {
        public class AdministrativeDivisionResponse
        {
            public long Id { get; set; }
            public long CountryId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Code { get; set; }
            public int DivisionLevel { get; set; }
            public long? ParentId { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
