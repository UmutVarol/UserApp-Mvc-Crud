namespace UserApp.Services
{
    public class ServiceResult
    {
        public bool Success { get; init; }
        public List<string> Errors { get; init; } = new();

        public static ServiceResult Ok() => new() { Success = true };
        public static ServiceResult Fail(IEnumerable<string> errors) => new() { Success = false, Errors = errors.ToList() };
    }
}
