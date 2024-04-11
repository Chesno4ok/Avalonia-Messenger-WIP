namespace ChesnokMessengerAPI.Responses
{
    public class Response
    {

        public string? status;
        public string? message;

        public Response(string status, string message)
        {
            this.status = status;
            this.message = message;
        }

    }

    public class InvalidParametersResponse
    {
        public string? status;
        public string? message;
        public string?[]? parameters;

        public InvalidParametersResponse(string status, string message, string[] parameters)
        {
            this.status = status;
            this.message = message;
            this.parameters = parameters;
        }
    }
}
