using Application.Interfaces;

namespace Application.Dto;

public class MessageListDto : IListDto<MessageDto>
{
    public int Count { get; set; }
    public ICollection<MessageDto> Items { get; set; }
}