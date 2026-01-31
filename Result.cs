namespace Hospital_API.Application.DTOs
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? Error { get; private set; }
        public Result Success => new Result { IsSuccess = true };
        public Result Failure(string error) => new Result { IsSuccess = false, Error = error };
    }
}
