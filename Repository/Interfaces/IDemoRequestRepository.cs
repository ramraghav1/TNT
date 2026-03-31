namespace Repository.Interfaces
{
    using Repository.DataModels;

    public interface IDemoRequestRepository
    {
        Task<long> InsertAsync(DemoRequestDTO entity);
        Task<IEnumerable<DemoRequestDTO>> ListAsync();
        Task<bool> ExistsByEmailAndProductAsync(string email, string productInterest);
    }
}
