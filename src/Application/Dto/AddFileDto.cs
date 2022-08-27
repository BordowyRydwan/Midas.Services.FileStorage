using Microsoft.AspNetCore.Http;

namespace Application.Dto;

public class AddFileDto
{
    public IFormFile Content { get; set; }
    public string Type { get; set; }
}