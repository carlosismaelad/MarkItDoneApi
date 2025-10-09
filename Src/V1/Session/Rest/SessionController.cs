using MarkItDoneApi.Src.V1.Session.Rest.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MarkItDoneApi.Src.V1.Session.Rest
{
  [ApiController]
  [Route("api/v1")]
  public class SessionController : ControllerBase
  {

    [HttpPost("register")]
    public void Register(RegisterRequest request)
    {
      string jsonString = $$"""
                            {
                              "email": "{{request.Email}}",
                              "password": {{request.Password}},
                            }
                            """;
      Console.WriteLine(jsonString);
    }
  }
}