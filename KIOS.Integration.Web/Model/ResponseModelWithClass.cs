namespace KIOS.Integration.Web.Model
{
    public class ResponseModelWithClass
    {

        public IList<string> Errors;

        public int MessageType { get; set; }

        public string Message { get; set; }

        public int HttpStatusCode { get; set; }
    }
}
