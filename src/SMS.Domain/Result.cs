namespace SMS.Domain
{
    public class Result
    {
        public bool IsFailure { get; }
        
        public bool IsSuccess => !IsFailure;
        
        public string Message { get; }

        private Result(bool isFailure, string message)
        {
            IsFailure = isFailure;
            Message = message;
        }

        public static Result Success() => new Result(false, "Ok");

        public static Result Fail(string error) => new Result(true, error);
    }
}