namespace Application.Wrappers
{
	public class Response<T>
    {
		public bool Succeeded { get; set; }
		public string Message { get; set; }
		public int Count { get; set; }
		public List<string> Errors { get; set; }
		public T Data { get; set; }

		public Response()
        {
        }
        public Response(T data, bool succeeded)
        {
            Succeeded = succeeded;
            Data = data;
        }
		public Response(T data, bool succeeded, int count)
		{
			Succeeded = succeeded;
			Data = data;
            Count = count;
		}
		public Response(string message)
        {
            Succeeded = false;
            Message = message;
        }

       
    }
}